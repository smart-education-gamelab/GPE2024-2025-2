using UnityEngine;

public class MainMenuOptionsManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject optionsCanvas;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip buttonClick;

    public void BackToMainMenu()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        if (mainMenuCanvas != null && optionsCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
        }
    }
}
