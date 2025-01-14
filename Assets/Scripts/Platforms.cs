using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

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
