using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePlatforms : MonoBehaviour
{
    [SerializeField] private List<GameObject> plataformas; // Lista de plataformas a alternar
    [SerializeField] private float tiempoDeCambio = 1f; // Tiempo en segundos entre cambios

    private float temporizador;

    private void Start()
    {
        // Inicializar el temporizador con el tiempo de cambio
        temporizador = tiempoDeCambio;
    }

    private void Update()
    {
        // Reducir el temporizador
        temporizador -= Time.deltaTime;

        // Cuando el temporizador llega a 0, alternar todas las plataformas
        if (temporizador <= 0f)
        {
            AlternarPlataformas();
            temporizador = tiempoDeCambio; // Reiniciar el temporizador
        }
    }

    private void AlternarPlataformas()
    {
        // Alternar el estado activo de todas las plataformas
        foreach (GameObject plataforma in plataformas)
        {
            plataforma.SetActive(!plataforma.activeSelf);
        }
    }
}
