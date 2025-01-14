using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    [SerializeField] private float destroyTime;

    void Start()
    { 
        Destroy(gameObject, destroyTime);
    }
}
