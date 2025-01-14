using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
