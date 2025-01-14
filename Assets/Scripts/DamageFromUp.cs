using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFromUp : MonoBehaviour
{
    [SerializeField] private GameObject efecto;
    private Animator animator;
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (other.GetContact(0).normal.y <= -0.9) {
                animator.SetTrigger("Golpe");
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
                other.gameObject.GetComponent<PlayerMovement>().Rebote();
            }
            else
            {
                other.gameObject.GetComponent<LifePlayer>().Daño(20, other.GetContact(0).normal);
            }
        }
    }

    private void Golpe()
    {
        Instantiate(efecto, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
