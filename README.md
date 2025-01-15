# Prototipo 2D
 
El prototipo desarrollado consiste en un juego de plataformas de scroll lateral en el que el personaje deberá superar obstáculos y conseguir una puntuación mínima (3000) para poder superar el nivel.

 [Enlace al video de la demo del prototipo, por problemas de tamaño no es posible colocar el vídeo.](https://drive.google.com/file/d/1owEnDYMG8PcNsq_macL9dt6cnYHlE92e/view?usp=sharing)

# Jugador

El personaje es capaz de moverse a los lados, saltar y tener sonidos de acuerdo a las diferentes animaciones. Para ello, se cuenta con el script **PlayerMovement.cs**. Además, el personaje controlado por el jugador posee a su vez el script **LifePlayer.cs** que gestiona las colisiones con el fin de aumentar la puntuación, reducir la salud, etc.

 ![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/1.%20Player.png)
 ![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/2.%20Player2.png)

**PlayerMovement.cs** esta comprobando continuamente si el jugador se encuentra en el suelo y de modificar la propiedad Velocity de su Rigidbody2D para mover al jugador.

```c#
private void Update()
    {
        movimientoHorizontal = Input.GetAxis("Horizontal") * velocidadDeMovimiento;
        
        animator.SetFloat("Horizontal",Mathf.Abs(movimientoHorizontal));
        animator.SetFloat("VelocidadY", rb2D.velocity.y);
        
        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
        }
    }

private void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0, suelo);
        animator.SetBool("enSuelo", enSuelo);

        if (sePuedeMover)
        {
            Mover(movimientoHorizontal * Time.fixedDeltaTime, salto);
        }
        
        if (!estabaEnElAire && !enSuelo)
        {
            estabaEnElAire = true;
        }
        else if (estabaEnElAire && enSuelo)
        {
            ReproducirSonidoAterrizaje(); 
            estabaEnElAire = false;
        }

        salto = false;
        
        if (enSuelo && Mathf.Abs(movimientoHorizontal) > 0.1f && !audioSource.isPlaying)
        {
            ReproducirSonidoPaso();
        }
    }

private void Mover(float mover, bool saltar)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rb2D.velocity.y);
        rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, velocidadObjetivo, ref velocidad, suavizadoDeMovimiento);
        
        if (mover > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Girar();
        }

        if (enSuelo && saltar)
        {
            Saltar();
        }
    }

private void Saltar()
    {
        enSuelo = false;
        rb2D.AddForce(new Vector2(0f, fuerzaDeSalto));
        AudioSource.PlayClipAtPoint(sonidoSalto, transform.position);
        OnJump?.Invoke(this, EventArgs.Empty);
    }

```

Además, el script **LifePlayer.cs** se centra en controlar aspectos como la puntuacion mediante recolección, la salud, los impactos (siendo tanto con enemigos como de proyectiles) y la reaparición del jugador en caso de que nos quedemos sin vida

```c#
public void Curar(float cantidad)
    {
        vida += cantidad;
        LifeBar.CambiarVidaActual(vida);
    }

public void Daño(float daño)
    {
        CinemachineShake.Instance.MoverCamara(4, 4, 0.5f);
        vida -= daño;
        LifeBar.CambiarVidaActual(vida);
        vida = Mathf.Clamp(vida, 0, vidaMax);
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
        OnTakeDamage?.Invoke(this, new OnTakeDamageEventArgs { cantidadVida = vida });

        if (vida <= 0)
        {
            Reaparecer();
        }
    }

public void Daño(float daño, Vector2 posicion)
    {
        CinemachineShake.Instance.MoverCamara(4, 4, 0.5f);
        vida -= daño;
        LifeBar.CambiarVidaActual(vida);
        vida = Mathf.Clamp(vida, 0, vidaMax);
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
        OnTakeDamage?.Invoke(this, new OnTakeDamageEventArgs { cantidadVida = vida });
        animator.SetTrigger("Golpe");
        StartCoroutine(PerderControl());
        StartCoroutine(DesactivarColision());
        playerMovement.GolpeRebote(posicion);

        if (vida <= 0)
        {
            Reaparecer();
        }
    }

public void DañoDisparo(float daño, Vector2 posicion)
    {
        CinemachineShake.Instance.MoverCamara(4, 4, 0.5f);
        vida -= daño;
        LifeBar.CambiarVidaActual(vida);
        vida = Mathf.Clamp(vida, 0, vidaMax);
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
        OnTakeDamage?.Invoke(this, new OnTakeDamageEventArgs { cantidadVida = vida });
        animator.SetTrigger("Golpe");
        StartCoroutine(PerderControl());
        StartCoroutine(DesactivarColision());

        if (vida <= 0)
        {
            Reaparecer();
        }
    }

private IEnumerator DesactivarColision()
    {
       Physics2D.IgnoreLayerCollision(9, 10, true);
       yield return new WaitForSeconds(tiempoSinControl);
       Physics2D.IgnoreLayerCollision(9, 10, false);
    }
private IEnumerator PerderControl()
    {
        playerMovement.sePuedeMover = false;
        yield return new WaitForSeconds(tiempoSinControl);
        playerMovement.sePuedeMover = true;
    }

private void Reaparecer()
    {
        transform.position = puntoReaparicion.position;
        vida = vidaMax;
        LifeBar.InicializarBarraDeVida(vida);
    }
```

# Recolección de objetos

En el nivel se encuentran 4 diferentes objetos a recoger, 3 que dan diferente cantidad de puntos (100, 200 y 500) y otro que da puntos de vida (20). Estos objetos de puntos tienen la etiqueta de *Puntos* y cuentan con un ```BoxCollider2D``` con la propiedad ```isTrigger``` activada. Lo mismo con el objeto de vida, con la diferencia de que la etiqueta asociada es *Life*. Para sumar la puntuación, cada objeto llevará un script sencillo que sumará la puntuación asociadada al objeto Score que tenemos en el canvas encargado de mostrar la puntuación hasta el momento. Para los objetos de vida también habrá asociado un script que cuando se recolecte se sume la vida del personaje. Además, mediante eventos, se ha hecho que cuando el personaje tenga menos de una cantidad de vida (40), aparezcan los objetos de vida en el mapa. Todos los objetos cuentan con una animación *Idle* y otra a la hora de recolectarse mediante un evento al ser recolectado. 

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/3.%20Ejecucion%20Recoleccion.gif)

```c#
public class FruitPoints : MonoBehaviour
{
    [SerializeField] private GameObject efecto;
    [SerializeField] private float cantidadPuntos;
    [SerializeField] private Score score;
    [SerializeField] private AudioClip audioClip;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            score.SumarPuntos(cantidadPuntos);
            Instantiate(efecto, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
            Destroy(gameObject);
        }
    }
}

public class ExtraLife : MonoBehaviour
{
    [SerializeField] private GameObject efecto;
    [SerializeField] private float cantidadVida;
    private LifePlayer lifePlayer;
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        lifePlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<LifePlayer>();
        lifePlayer.OnTakeDamage += AparecerVida;
        
        gameObject.SetActive(false);
    }

    private void AparecerVida(object sender, LifePlayer.OnTakeDamageEventArgs e)
    {
        if (e.cantidadVida <= 40)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        
        if (other.CompareTag("Player"))
        {
            lifePlayer.Curar(cantidadVida);
            Instantiate(efecto, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(audioClip, transform.position);
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        if (lifePlayer != null)
        {
            lifePlayer.OnTakeDamage -= AparecerVida;
        }
    }
}
```

Para hacer la animación de desaparición del objeto una vez recolectado deberemos hacer un prefab de la animación y despues asociarla al script del objeto, ya sea de puntuación, vida o enemigos (este se explicara más adelante). También habra un script asociado al prefab para que pasado el tiempo de animación se destruya y no quede en bucle.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/4.%20Prefab.gif)

# Enemigos

El nivel cuenta con 4 tipos de enemigos, estos son las setas, los murciélagos, los slime y los arboles. Todos son derrotables menos los arboles, los cuales se encargaran de disparar un proyectil en caso de que estemos dentro de su rango de visión.

## Setas

Este enemigo cuenta con una animación de movimiento y una animación cuando los golpeamos desde arriba. También tiene asociado el script **EnemyMovement** para que se mueva por la plataforma de un lado a otro siempre que este tocando el suelo, en caso de que el controlador asociado no este tocando el terreno, se dará la vuelta y asi sucesivamente.

```c#
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private float velocidad;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private float distancia;
    [SerializeField] private bool movimientoDerecha;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void FixedUpdate()
    {
        RaycastHit2D informacionSuelo = Physics2D.Raycast(controladorSuelo.position, Vector2.down, distancia);
        rb.velocity = new Vector2(velocidad, rb.velocity.y);

        if (informacionSuelo == false)
        {
            Girar();
        }
    }

    private void Girar()
    {
        movimientoDerecha = !movimientoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        velocidad *= -1;
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/5.%20Seta.gif)

## Murciélago

Al igual que la seta, cuenta con una animación de movimiento y de golpe. En esta caso, la forma de movimiento del murciélago sera mediante diferentes waypoints puestos en la zona, pero de forma en que vaya a los puntos de forma aleatoria, en vez de seguir el orden en el que se llaman o estan colocados. Para esto esta el script **EnemyRouteRandom.cs** en el cual añadiremos la lista de puntos para tener los waypoints, la velocidad a la que se desplazará y la distancia a la que pasará al siguiente punto.

```c#
public class EnemyRouteRandom : MonoBehaviour
{

    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private Transform[] puntosMovimiento;
    [SerializeField] private float distanciaMinima;
    private int numeroAleatorio;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        numeroAleatorio = Random.Range(0, puntosMovimiento.Length);
        spriteRenderer = GetComponent<SpriteRenderer>();
        Girar();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position,puntosMovimiento[numeroAleatorio].position, velocidadMovimiento * Time.deltaTime);

        if (Vector2.Distance(transform.position, puntosMovimiento[numeroAleatorio].position) < distanciaMinima)
        {
            numeroAleatorio = Random.Range(0, puntosMovimiento.Length);
            Girar();
        }
    }

    private void Girar()
    {
        if (transform.position.x < puntosMovimiento[numeroAleatorio].position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/6.%20Murcielago.gif)

## Slime

También con animacion de movimiento y golpe. Para este caso, también se haran uso de waypoints pero esta vez si que sera siguiendo la ruta que se le pone por defecto, es decir siguiendo el punto 1, después el punto 2... así hasta que llegue al final y vuelva. Todo esto se hace en el script **EnemyRouteFollow**.

```c#
public class EnemyRouteFollow : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private Transform[] puntosMovimiento;
    [SerializeField] private float distanciaMinima;
    private int siguientePaso = 0;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Girar();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position,puntosMovimiento[siguientePaso].position, velocidadMovimiento * Time.deltaTime);

        if (Vector2.Distance(transform.position, puntosMovimiento[siguientePaso].position) < distanciaMinima)
        {
            siguientePaso += 1;
            if (siguientePaso >= puntosMovimiento.Length)
            {
                siguientePaso = 0;
            }
            Girar();
        }
    }

    private void Girar()
    {
        if (transform.position.x < puntosMovimiento[siguientePaso].position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/7.%20Slime.gif)

## Elementos compartidos

A pesar de que los tres enemigos se mueven de diferente forma y tienen distinto comportamiento, todos comparten el script **DamageFromUp.cs**. Este script se encarga del comportamiento del enemigo cuando colisiona con el personaje, si es desde abajo o los laterales, hará daño al personaje, pero si le golpeamos desde arriba, hara la animación de golpe y morirá con la animación adicional del prefab, dándonos además un pequeño impulso como si fuera un rebote. Para hacer este efecto debemos hacer uso del metodo de ```Golpe()``` para ponerlo dentro de un evento al final de la animación de golpe.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/8.%20Evento%20animacion.png)

Una vez hecho esto, tenemos que crear un parametro en la animacion de tipo ```Trigger``` llamado Golpe y cuando pase de estado *Idle* a *Golpe* tenga la condicion del trigger del golpe

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/9.%20Trigger%20Golpe.png)

## Tronco

Este enemigo es diferente, puesto que no podemos golpearlo y disparará proyectiles al jugador si entra dentro de su rango de visión. Para esto se usa el script **EnemyShoot.cs** en el cual tendra un controlador que tiene una distancia cambiable por el usuario y en el caso de que toque al jugador, este va a disparar proyectiles, los cuales son el prefab de un proyectil. El enemigo tiene animacion cuando esta parado y cuando dispara.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/10.%20Tronco%20rango.png)

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/11.Troncos.gif)

# Objetos interactuables

Además de los enemigos y objetos recolectables, también hay algunos objetos que tienen impacto en el nivel.

## Pinchos

Este objeto sólo tiene el propósito de hacer daño al personaje en caso de que caigamos en ellos o los toquemos. Reducirá la vida del personaje y hara que se produzca su animación de golpe.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/12.%20Pinchos.gif)

## Trampolín

Este objeto nos dará un impulso para acceder a otras zonas o salir de unas a las que saltando no llegaríamos. Con el script **JumpTrampoline.cs** en caso de que caigamos sobre este objeto (no pasar al lado), nos impulsará en la direccion x según la fueza que le pongamos desde el inspector.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/13.%20Trampolin.gif)

```c#
public class JumpTrampoline : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    private Animator animator;
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetContact(0).normal.y <= -0.9)
            {
                animator.SetTrigger("Salto");
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
                Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
        }
    }
}
```

## Plataforma por evento

Estas plataformas se activan y desactivan conforme nuestro jugador hace un salto. Es por eso que haciendo uso del evento ```OnJump``` podremos saltar sobre las diferentes plataformas e iran apareciendo segun demos el salto. Algunas de las plataformas tienen un ```PlatformEffector2D``` para que al saltar debajo de ellas no haya una colision con el objeto y no podamos saltar más. En el script **Platforms.cs** se añade a un objeto la lista de plataforma a las cuales queremos que hagan el evento y desactivamos las plataformas continuas a las activas para que haya una diferencia al hacer el salto.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/14.%20Platform%20effector%202D.png)

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/15.%20plataforma%20evento.gif)

```c#
public class Platforms : MonoBehaviour
{
    public List<GameObject> plataformas;
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerMovement.OnJump += Activar;
    }

    private void Activar(object sender, EventArgs e)
    {
        foreach (GameObject item in plataformas)
        {
            item.SetActive(!item.activeSelf);
        }
    }
}
```


## Plataforma por tiempo

Estas plataformas son parecidas a las anteriores con la diferencia de que en vez de ser por un evento de salto, es por tiempo, es decir, cada cierto tiempo (1 seg) se cambian de estado y viceversa. Todas ellas también tienen un ```PlatformEffector2D``` para que el jugador no colisione con estas si pasa por debajo o por los lados.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/16.%20Plataforma%20Tiempo.gif)

```c#
public class TogglePlatforms : MonoBehaviour
{
    [SerializeField] private List<GameObject> plataformas; 
    [SerializeField] private float tiempoDeCambio = 1f;

    private float temporizador;

    private void Start()
    {
        temporizador = tiempoDeCambio;
    }

    private void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            AlternarPlataformas();
            temporizador = tiempoDeCambio; // Reiniciar el temporizador
        }
    }

    private void AlternarPlataformas()
    {
        foreach (GameObject plataforma in plataformas)
        {
            plataforma.SetActive(!plataforma.activeSelf);
        }
    }
}
```

## Trofeo Victoria

Para interactuar con este objeto es necesario tener los puntos mínimos necesario para terminar la partida, en caso de ser así sonará una melodía de victoria y se pausará el juego como finalizado.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/30.%20Victoria.gif)

# Animaciones

Como hemos podido ver antes, hay animaciones para los objetos de puntos, los objetos de vida, los objetos interactuables y en los diferentes tipos de enemigos. Además de esto tenemos las animaciones del personaje, el cual tiene 5 estados diferentes:

- ```Idle```: animación a ejecutar cuando el jugador se encuentra parado.
- ```Correr```: animación de correr que se activa cuando el parámetro *Horizontal* sea superior a 0. Si *Horizontal* llega a ser menor a 0.1, hará la transicion a ```Idle```
- ```Salto```: animación de salto activada cuando el parámetro *VelocidadY* sea **MAYOR** a 0 y *enSuelo* sea **false**.
- ```Caida```: animación de salto activada cuando el parámetro *VelocidadY* sea **MENOR** a 0 y *enSuelo* sea **false**.
- ```GolpeJugador```: animación que se activa con el trigger *Golpe*.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/17.%20Animaciones%20personaje.png)

```Idle```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/18.%20Idle.gif)

```Correr```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/19.%20Correr.gif)

```Golpe```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/20.%20Golpe.gif)
![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/21.%20Golpe%20disparo.gif)

```Salto```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/23.%20Salto.gif)

```Caida```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/22.%20Caida.gif)



# Sonidos

Para la parte de sonidos se ha hecho uso de clips de audio que se añaden por el inspector y se reproducen en el momento, excepto el de ambiente y los de salas especiales, el cual el de ambiente se escucha en todo momento hasta que se entra en una sala especial, momento en el que cambia la melodía hasta que salgamos de la sala y vuelva la música de ambiente

Se han añadido los siguientes sonidos:

- ```Ambiente```: Melodía que se ejecuta al iniciar el nivel, esta asociado a un AudioSource para ejecutar el sonido nada mas empezar y que lo tenga en bucle.
- ```Pasos```: Mediante un array se introducen los diferentes sonidos de pasos del jugador y de forma aleatoria se van ejecutando cada vez que el jugador avanza.
- ```Salto```: Cuando el jugador salta reproduce un sonido.
- ```Caida```: Cuando el jugador aterriza sobre el suelo, ya sea después de un salto o al caer de otra plataforma o enemigo.
- ```Golpe```: Cuando el jugador es golpeado por un enemigo, proyectil o pinchos. Algo que afecte a la salud.
- ```Trampolin```: Al saltar sobre un trampolín.
- ```Enemigo Golpeo```: Al golpear sobre un enemigo y eliminarlo. Cada enemigo tiene un sonido diferente (Seta, Murcielago y Slime).
- ```Salud```: Al recoger objetos de vida.
- ```Puntos```: Al recoger objetos que dan puntuación.
- ```Proyectil```: Cuando el enemigo dispara un proyectil, este tiene sonido de disparo
- ```Salas especiales```: Cuando el jugador entra en una de las salas especiales, la melodia de ambiente cambia por una especial hasta que el jugador sale de la sala.
- ```Victoria```: Cuando el jugador consigue la puntuación mínima para superar el nivel y llega al trofeo, suena un sonido de victoria.

# Scrolling de fondo

El fondo cuenta con dos capas distintas que al combinarse forman unas montañas y el cielo

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/24.%20Fondo.png)

Luego, añadiremos un script a cada capa para darle la velocidad que queramos para aplicar el efecto **Parallax**

```c#
public class BackGroundParallax : MonoBehaviour
{
     [SerializeField] private Vector2 velocidadMovimiento;
     private Vector2 offset;
     private Material material;
     
     private Rigidbody2D jugadorRB;

    private void Awake()
    {
      material = GetComponent<SpriteRenderer>().material;
      jugadorRB = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        offset = (jugadorRB.velocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/25.%20Parallax.gif)

# Tilemap

Para la parte del tilemap tenemos tres capas:

- ```Floor```: esta capa es la del mapa y tiene los componentes ```TilemapCollider2D```, ```Rigidbody2D``` y ```CompositeCollider2D```. Para que no haya problemas a la hora de las colisiones entre el mapa y el personaje, dentro del componente del ```TilemapCollider2D``` debemos seleccionar la opcion **Merge** en *Composite operation*.
- ```Background```: esta capa es para darle un adorno y que el jugador no colisione al pasar por zonas de decoración
- ```FloorSalaEspecial```: esta capa es para añadir estructuras en las salas especiales y, ademas de tener los mismo componentes que la capa *Floor*, se ha añadido el componente ```PlatformEffector2D``` para poder saltar a través de ellas y no tener problemas de colisión.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/26.%20Tilemap.png)

```Componentes de Floor```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/27.%20componentes%20floor.png)

```Componentes de FloorSalaEspecial```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/28.%20Componentes%20floorespecal.png)

# User Interface

Para la interfaz se ha creado un panel que contiene la puntuación del jugador y una barra de vida, la cual va cambiando dependiendo de la vida del personaje.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/29.%20Interfaz.gif)

Para la barra de vida se ha usado el script **LifeBar.cs** y se ha añadido al objeto padre dentro del canvas, dentro de este objeto padre estaran las imágenes de la barra de vida, tanto el borde como el relleno y en base a la vida del jugador y un componente ```Slider```, este ira actualizandose en base a la vida restante.

```c#
public class LifeBar : MonoBehaviour
{
    private Slider slider;
    

    public void CambiarVidaMaxima(float vidaMaxima)
    {
        slider.maxValue = vidaMaxima;
    }

    public void CambiarVidaActual(float cantidadVida)
    {
        slider.value = cantidadVida;
    }

    public void InicializarBarraDeVida(float cantidadVida)
    {
        slider = GetComponent<Slider>();
        CambiarVidaMaxima(cantidadVida);
        CambiarVidaActual(cantidadVida);
    }
}
```

# Cámaras

El nivel, que emplea cinemachine, emplea 5 cámaras diferentes.

- ```Camara Principal```: cámara que sigue al jugador constantemente y que está confinada por el componente
- ```Camara Secundaria```: esta cámara esta disponible pulsando la tecla ```C``` y tiene un rango más amplio que ofrece una vista mas alejada del jugador. Se emplea con el script **ChangeCamera.cs** en el cual se arrastran ambas cámaras virtuales al inspector, la camara 1 estará por defecto y en caso de pulsar la tecla correspondiente, esta camara secundaria tendrá un valor superior en prioridad y sera la principal, en caso de volver a pulsar la tecla, volverá a la cámara principal.
- ```Cámara Salas especiales```: Hay una cámara para cada sala especial, 3 en total y tienen el mismo comportamiento, el jugador al entrar en una de las salas, interactuará con un objeto vacio el cual tiene un ```BoxCollider2D``` con el *isTrigger* activado y un script con la cámara virtual asociado, al hacer esto la camara tendra una prioridad superior a la princial, lo que hará que el personaje vea el tamaño total de la sala, en lugar de ver solo una parte.

```Confiner```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/31.%20Cofinner.png)

```Script Cambio de camara```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/32.%20script%20camara.png)

```Script Cambio de Camara a sala especial```

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/33.%20Script%20sala%20especial.png)

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/34.%20Cambio%20camara.gif)

Además de emplear diferentes cámaras, se emplea el componente Cinemachine Impulse Listener para hacer temblar la cámara principal o la secundaria cuando el jugador recibe un golpe. El script **CinemachineShaker.cs** se encuentra en las camaras principales y secundarias, puesto que queremos que solo ocurra cuando este en el mapa normal y no en las salas especiales.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/35.%20shake%20camera.gif)

```c#
public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    private float tiempoMovimiento;
    private float tiempoMovimientoTotal;
    private float intensidadInicial;

    private void Awake()
    {
        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void MoverCamara(float intensidad, float frecuencia, float tiempo)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensidad;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frecuencia;
        intensidadInicial = intensidad;
        tiempoMovimientoTotal = tiempo;
        tiempoMovimiento = tiempo;
    }

    private void Update()
    {
        if (tiempoMovimiento > 0)
        {
            tiempoMovimiento -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(intensidadInicial, 0, 1 - (tiempoMovimiento / tiempoMovimientoTotal));
        }
    }

}
```

# Limites del mapa y vida

Se ha establecido un punto al principio del nivel para que, cuando el jugador caiga al vacio o se quede sin vida reaparezca en el punto indicado. Para el caso de caer se ha creado un objeto que actua como trigger y devuelve al jugador sin recibir daño y, para el caso de no tener vida, reaparece en el principio con la barra de vida al completo, sin reiniciar el nivel.

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/36.%20Offlimit.png)

![](https://github.com/Alu0101030562/Screenshots/blob/main/Screenshots/Prototipo2D/37.%20LimitesMapa.gif)

# Créditos

## Sprites

Todos los sprites se encuentran en la página de assets de Unity.

- Tilemap, jugador, recolectables, etc.: https://assetstore.unity.com/packages/2d/characters/pixel-adventure-1-155360
- Enemigos: https://assetstore.unity.com/packages/2d/characters/pixel-adventure-2-155418
- Fondo: https://assetstore.unity.com/packages/2d/environments/sunnyland-woods-129708

## Audio

Todos los audios han sido sacados de las siguientes páginas.

- https://pixabay.com/sound-effects/search/game/

- https://freesound.org/

- Melodia de fondo: Final Fantasy VII Rebirth OST - KAML Theme


