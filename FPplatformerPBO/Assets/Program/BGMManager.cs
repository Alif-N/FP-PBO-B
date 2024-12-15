using UnityEngine;

public class BGMController : MonoBehaviour
{
    private AudioSource audioSource; // Komponen AudioSource
    public AudioClip defaultBGM; // Musik default (BGM.mp3)
    public AudioClip powerUpBGM; // Musik Power Up (Power.ogg)

    [Range(0f, 1f)] public float volumeDefault = 0.5f; // Volume untuk musik default
    [Range(0f, 1f)] public float volumePowerUp = 0.7f; // Volume untuk musik Power Up

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null && defaultBGM != null)
        {
            audioSource.clip = defaultBGM;
            audioSource.loop = true;
            audioSource.volume = volumeDefault;
            audioSource.Play(); // Mainkan BGM default
        }
    }

    public void PlayPowerUpBGM()
    {
        if (audioSource != null && powerUpBGM != null)
        {
            audioSource.Stop();
            audioSource.clip = powerUpBGM;
            audioSource.volume = volumePowerUp; // Atur volume khusus Power Up
            audioSource.Play();
        }
    }

    public void PlayDefaultBGM()
    {
        if (audioSource != null && defaultBGM != null)
        {
            audioSource.Stop();
            audioSource.clip = defaultBGM;
            audioSource.volume = volumeDefault; // Kembalikan volume default
            audioSource.Play();
        }
    }
}