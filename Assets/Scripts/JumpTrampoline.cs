using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTrampoline : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    private Animator animator;
    [SerializeField] private AudioClip audioClip;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.GetContact(0).normal.y <= -0.9)
            {
                animator.SetTrigger("Salto");
                AudioSource.PlayClipAtPoint(audioClip, transform.position);
                Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }
        }
    }
}
