using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;

    void Start()
    {
        PlayRandomMusic();
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayRandomMusic();
        }
    }


    void PlayRandomMusic()
    {
        if (musicClips.Length == 0) return;

        int index = Random.Range(0, musicClips.Length); // Random index
        musicSource.clip = musicClips[index];
        musicSource.Play();
    }
}
