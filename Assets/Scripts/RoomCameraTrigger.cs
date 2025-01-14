using UnityEngine;
using Cinemachine;

public class RoomCameraTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera roomCamera; // Asigna la cámara de la sala en el Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomCamera.Priority = 100;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            roomCamera.Priority = 0;
        }
    }
}