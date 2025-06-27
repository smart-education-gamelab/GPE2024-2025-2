using UnityEngine;
using NaughtyAttributes;

public class BetterGraphManager : MonoBehaviour
{
    public enum GraphMode { none, cardinal, all }
    private Vector3 lastShotPosition;

    [SerializeField] private float[] yTiers = { 10f, 2.5f, 1f, 0.1f};
    [SerializeField] private string[] yTierString;
    [SerializeField] private BetterGraphLine[] cardinalAxis;
    [SerializeField] private BetterGraphLine[] tierLines;
    [SerializeField] private BetterGraphLine[] mirrorTierLines;
    [SerializeField] private Color cardinalColor;
    [SerializeField] private Color[] tierLineColors;
    private int graphModeIndex = 0;

    public void SetLastShotPosition(Vector3 position)
    {
        lastShotPosition = position;
        transform.position = lastShotPosition;
    }

    public void CycleGraphMode()
    {
        graphModeIndex++;

        if (graphModeIndex > 2)
        {
            graphModeIndex = 0;
        }

        switch(graphModeIndex)
        {
            case (0):
                SetGraphMode(GraphMode.none);
                break;
            case (1):
                SetGraphMode(GraphMode.cardinal);
                break;
            case (2):
                SetGraphMode(GraphMode.all);
                break;
        }
    }

    private void SetGraphMode(GraphMode graphMode)
    {
        switch (graphMode)
        {
            case (GraphMode.none):
                SetCardinalVisibility(false);
                SetTierVisibility(false);
                break;
            case (GraphMode.cardinal):
                SetCardinalVisibility(true);
                SetTierVisibility(false);
                break;
            case (GraphMode.all):
                SetCardinalVisibility(true);
                SetTierVisibility(true);
                break;
        }
    }

    private void SetCardinalVisibility(bool visible)
    {
        for (int i = 0; i < cardinalAxis.Length; i++)
        {
            cardinalAxis[i].ToggleVisualElements(visible);
        }
    }

    private void SetTierVisibility(bool visible)
    {
        for (int i = 0; i < tierLines.Length; i++)
        {
            tierLines[i].ToggleVisualElements(visible);
        }
        for (int i = 0; i < mirrorTierLines.Length; i++)
        {
            mirrorTierLines[i].ToggleVisualElements(visible);
        }
    }

    private void SetColors()
    {
        cardinalAxis[0].SetColor(cardinalColor);
        cardinalAxis[1].SetColor(cardinalColor);

        for (int i = 0; i < tierLines.Length; i++)
        {
            float yScaled = yTiers[i] * 1000f;
            tierLines[i].SetColor(tierLineColors[i]);
            mirrorTierLines[i].SetColor(tierLineColors[i]);
        }
    }

    private void Awake()
    {
        SetColors();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Button("Update Tier Lines")]
    private void UpdateTierLineAngles()
    {
        cardinalAxis[0].SetMark("Y", "-Y");
        cardinalAxis[1].SetMark("X", "-X");

        for (int i = 0; i < yTiers.Length; i++)
        {
            float angle = Mathf.Atan2(yTiers[i], 1) * Mathf.Rad2Deg;
            angle -= 90f;

            tierLines[i].transform.rotation = Quaternion.Euler(0, angle, 0);
            mirrorTierLines[i].transform.rotation = Quaternion.Euler(0, angle * -1, 0);
        }


        for (int i = 0; i < tierLines.Length; i++)
        {
            tierLines[i].SetMark("-" + yTierString[i] + "T" + ",-5F", "-" + yTierString[i] + "T" + ",5F");
            mirrorTierLines[i].SetMark(yTierString[i] + "T" + ",5F", yTierString[i] + "T" + ",-5F");
        }
    }
}
