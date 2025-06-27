using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EndHoleScript : MonoBehaviour
{
    private BoxCollider _collider;
    private CourseScript _courseScript;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip holeClip;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<BoxCollider>();
        _courseScript = FindAnyObjectByType<CourseScript>();
        
        _collider.isTrigger = true;
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _courseScript.PlayerFinishedHole(other.gameObject);

            soundFXManager.PlaySoundFXClip(holeClip, _collider.transform, 1f);
        }
    }
}
