using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private int puntuacionNecesaria; // Puntuación requerida para finalizar el juego
    [SerializeField] private Score score; // Sistema de puntuación
    [SerializeField] private AudioClip audioClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verificar si la puntuación es suficiente
            if (score.GetPuntos() >= puntuacionNecesaria)
            {
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
                Finalizar(); // Finalizar el juego
            }
            else
            {
                Debug.Log("No tienes suficiente puntuación para ganar.");
            }
        }
    }

    private void Finalizar()
    {
        Time.timeScale = 0f;
    }
}
