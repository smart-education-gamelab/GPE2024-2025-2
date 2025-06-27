using UnityEngine;

public class WorldButtonTabManager : MonoBehaviour
{
    [SerializeField] private WorldButton[] worldButtons;
    [SerializeField] private int defaultButton = 0;

    public void UpdateButtonDisplay(int buttonIndex)
    {
        for (int i = 0; i < worldButtons.Length; i++)
        {
            if (i == buttonIndex)
            {
                worldButtons[i].SetInteract(false);
                worldButtons[i].SetDisabled(true);
            }
            else
            {
                worldButtons[i].SetDisabled(false);
                worldButtons[i].SetInteract(true);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldButtons[defaultButton].Click();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
