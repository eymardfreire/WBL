using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Range(0, 1)]
    public float musicVolume = 0.5f; // Volume control, adjustable in the inspector

    public List<AudioClip> backgroundMusicClips; // Assign this list in the inspector
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the AudioManager across scenes
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // Set the audio source to loop
            audioSource.volume = musicVolume; // Set the initial volume
        }
        else
        {
            Destroy(gameObject); // Ensure there is only one AudioManager
        }
    }

    void Start()
    {
        PlayRandomMusic();
    }

    public void PlayRandomMusic()
    {
        if (backgroundMusicClips.Count > 0)
        {
            // Pick a random clip from the list
            int randomIndex = Random.Range(0, backgroundMusicClips.Count);
            AudioClip clip = backgroundMusicClips[randomIndex];

            // Assign the clip to the audio source and play it
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // Call this method to update the music volume
    public void SetVolume(float volume)
    {
        musicVolume = Mathf.Clamp(volume, 0, 1); // Ensuring the volume stays between 0 and 1
        audioSource.volume = musicVolume;
    }

    // Call this method to stop the music when needed
    public void StopMusic()
    {
        audioSource.Stop();
    }

    // Call this method if you need to change the music for a specific stage or scene
    public void ChangeMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
