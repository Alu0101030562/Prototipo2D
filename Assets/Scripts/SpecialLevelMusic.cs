using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialLevelMusic : MonoBehaviour
{
    [SerializeField] private AudioSource ambientAudioSource;
    [SerializeField] private AudioClip specialLevelClip;
    [SerializeField] private AudioClip ambientClip;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ambientAudioSource.clip = ambientClip;
            ambientAudioSource.Stop();
            ambientAudioSource.clip = specialLevelClip;
            ambientAudioSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ambientAudioSource.clip = ambientClip;
            ambientAudioSource.Play();
        }
    }
}
