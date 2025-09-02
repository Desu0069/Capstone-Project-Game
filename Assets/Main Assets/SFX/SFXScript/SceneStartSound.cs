using UnityEngine;

public class SceneStartSound : MonoBehaviour
{
    public AudioClip startSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = startSound;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.3f; // Lower volume
        audioSource.Play();

        // Stop playing after 3 seconds
        Invoke("StopSound", 3f);
    }

    void StopSound()
    {
        audioSource.Stop();
    }
}

