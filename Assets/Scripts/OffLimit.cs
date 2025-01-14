using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffLimit : MonoBehaviour
{
    [SerializeField] private Transform puntoReaparicion; // Objeto que define la posición de reaparición

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = puntoReaparicion.position;
        }
    }
}
