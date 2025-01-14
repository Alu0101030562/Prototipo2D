using UnityEngine;
using Cinemachine;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera1; // Cámara 1 (por defecto)
    [SerializeField] private CinemachineVirtualCamera camera2; // Cámara 2

    private bool isCamera1Active = true; // Indica si la Cámara 1 está activa

    void Start()
    { 
        if (camera1 != null)
        {
            camera1.Priority = 10;
        }

        if (camera2 != null)
        {
            camera2.Priority = 0;
        }
    }

    void Update()
    {
        // Detecta si se pulsa la tecla C
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        if (isCamera1Active)
        {
            // Cambiar a la Cámara 2
            if (camera2 is not null)
            {
                camera1.Priority = 0;
                camera2.Priority = 10;
            }
        }
        else
        {
            // Cambiar a la Cámara 1
            if (camera1 is not null)
            {
                camera1.Priority = 10;
                camera2.Priority = 0;
            }
        }

        // Alternar el estado de la cámara activa
        isCamera1Active = !isCamera1Active;
    }
}
