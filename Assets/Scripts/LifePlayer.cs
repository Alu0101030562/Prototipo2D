using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LifePlayer : MonoBehaviour
{
    [SerializeField] private float vida;
    [SerializeField] private float vidaMax;
    private PlayerMovement playerMovement;
    [SerializeField] private float tiempoSinControl;
    private Animator animator;
    [SerializeField] private LifeBar LifeBar;
    [SerializeField] private Transform puntoReaparicion;
    [SerializeField] private AudioClip audioClip;
    
    public event EventHandler<OnTakeDamageEventArgs> OnTakeDamage;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        vida = vidaMax;
        LifeBar.InicializarBarraDeVida(vida);
    }
    
    public class OnTakeDamageEventArgs : EventArgs
    {
        public float cantidadVida;
    }

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

}
