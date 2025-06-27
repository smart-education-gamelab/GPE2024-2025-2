using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelSelectCanvas;
    [SerializeField] private GameObject optionsCanvas;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip buttonClick;

    public void SwitchToLevelSelect()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        if (mainMenuCanvas != null && levelSelectCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
            levelSelectCanvas.SetActive(true);
        }
    }

    public void SwitchToOptions()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        if (mainMenuCanvas != null && optionsCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
        }
    }
}
