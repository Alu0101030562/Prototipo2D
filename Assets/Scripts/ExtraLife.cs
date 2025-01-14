using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

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
        // Desuscribirse del evento al destruir este script
        if (lifePlayer != null)
        {
            lifePlayer.OnTakeDamage -= AparecerVida;
        }
    }
}
