using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float velocidad;
    public int daño;

    private void Start()
    {
        Destroy(gameObject, 3);
    }
    private void Update()
    {
        transform.Translate(Time.deltaTime * velocidad * -Vector2.right);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out LifePlayer lifePlayer))
        {
            lifePlayer.DañoDisparo(daño, transform.position);
            Destroy(gameObject);
        }
    }
}
