using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip buttonClick;

    [SerializeField] private GameObject[] disablePauseObjects;
    //[SerializeField] private GameObject[] disableGameObjects;
    private bool isPaused = false;
    private PlayerCardManager playerCardManager;
    
    private void Awake()
    {
        playerCardManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerCardManager>();
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        isPaused = !isPaused;
        playerCardManager.SetPaused(isPaused);
        foreach (GameObject canvasObject in disablePauseObjects)
        {
            canvasObject.SetActive(isPaused);
        }
        /*foreach (GameObject inGameObject in disableGameObjects)
        {
            inGameObject.SetActive(!isPaused);
        }*/
    }

    public void GoToMainMenu()
    {
        //soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        SceneManager.LoadSceneAsync("Main menu");
    }
}
