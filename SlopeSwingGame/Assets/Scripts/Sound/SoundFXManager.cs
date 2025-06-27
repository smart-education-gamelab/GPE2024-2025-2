using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundFXObject;


    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //spawn audioSource
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign audioClip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get length of audioClip
        float clipLength = audioSource.clip.length;
        //destory the audioSource after clip is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //get random index
        int index = Random.Range(0, audioClip.Length);
        //spawn audioSource
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign audioClip
        audioSource.clip = audioClip[index];
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get length of audioClip
        float clipLength = audioSource.clip.length;
        //destory the audioSource after clip is done playing
        Destroy(audioSource.gameObject, clipLength);
    }
}
