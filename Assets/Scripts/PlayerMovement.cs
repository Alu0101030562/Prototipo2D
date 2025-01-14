using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    private Rigidbody2D rb2D;
    public bool sePuedeMover = true;
    [SerializeField] private Vector2 velocidadGolpe;
    
    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
    [SerializeField] private float velocidadDeMovimiento;
    [Range(0,0.3f)][SerializeField] private float suavizadoDeMovimiento;
    private Vector3 velocidad = Vector3.zero;
    private bool mirandoDerecha = true;
    
    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask suelo;
    [SerializeField] private Transform controladorSuelo;
    [SerializeField] private Vector3 dimensionesCaja;
    [SerializeField] private bool enSuelo;
    private bool salto = false;
    private bool estabaEnElAire = false;
    
    [Header("Rebote")]
    [SerializeField] private float velocidadRebote;
    
    [Header("Animacion")]
    private Animator animator;
    
    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip[] sonidoPasos;
    [SerializeField] private AudioClip sonidoSalto;
    [SerializeField] private AudioClip sonidoCaida;
    
    //Eventos
    public event EventHandler OnJump;
    

    
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

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
            estabaEnElAire = true; // El jugador está en el aire
        }
        else if (estabaEnElAire && enSuelo)
        {
            ReproducirSonidoAterrizaje(); // Reproducir sonido al aterrizar
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
    
    private void ReproducirSonidoPaso()
    {
        if (sonidoPasos.Length > 0)
        {
            AudioClip paso = sonidoPasos[UnityEngine.Random.Range(0, sonidoPasos.Length)];
            audioSource.PlayOneShot(paso);
        }
    }
    
    private void ReproducirSonidoAterrizaje()
    {
        audioSource.PlayOneShot(sonidoCaida); // Reproducir el sonido de aterrizaje
    }
    
    private void Saltar()
    {
        enSuelo = false;
        rb2D.AddForce(new Vector2(0f, fuerzaDeSalto));
        AudioSource.PlayClipAtPoint(sonidoSalto, transform.position);
        OnJump?.Invoke(this, EventArgs.Empty);
    }

    public void GolpeRebote(Vector2 puntoGolpe)
    {
        rb2D.velocity = new Vector2(-velocidadGolpe.x * puntoGolpe.x, velocidadGolpe.y);
    }
    
    public void Rebote()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, velocidadRebote);
    }
    
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(controladorSuelo.position, dimensionesCaja);
    }
}
