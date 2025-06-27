using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelSelectCanvas;

    [SerializeField] private SoundFXManager soundFXManager;
    [SerializeField] private AudioClip buttonClick;

    public void StartCourse_1()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        SceneManager.LoadSceneAsync("Course2");
    }

    public void StartCourse_2()
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        SceneManager.LoadSceneAsync("Course3");
    }

    public void BackToMainMenu() 
    {
        soundFXManager.PlaySoundFXClip(buttonClick, this.transform, 1f);

        if (mainMenuCanvas != null && levelSelectCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
            levelSelectCanvas.SetActive(false);
        }
    }
}
