using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BetterGraphLine : MonoBehaviour
{
    [SerializeField] private GameObject[] visualElements;
    [SerializeField] private TextMeshPro positiveMarkTMP;
    [SerializeField] private TextMeshPro negativeMarkTMP;
    [SerializeField] private Transform lineTransform;
    [SerializeField] private Renderer lineRenderer;
    [SerializeField] private float lineWidth = 0.02f;
    [SerializeField] private float lineLength = 200f;

    public void SetColor(Color newColor)
    {
        // Use SetColor to set the main color shader property
        //lineRenderer.material.SetColor("_Color", newColor);
        // If your project uses URP, uncomment the following line and use it instead of the previous line
        lineRenderer.material.SetColor("_BaseColor", newColor);
    }

    public void SetMark(float value)
    {
        positiveMarkTMP.text = value.ToString();
        negativeMarkTMP.text = (-value).ToString();
    }

    public void SetMark(string value)
    {
        positiveMarkTMP.text = value;
        negativeMarkTMP.text = value;
    }

    public void SetMark(string positiveValue, string negativeValue)
    {
        positiveMarkTMP.text = positiveValue;
        negativeMarkTMP.text = negativeValue;
    }

    public void ToggleVisualElements(bool visiblity)
    {
        for (int i = 0; i < visualElements.Length; i++)
        {
            visualElements[i].SetActive(visiblity);
        }

        lineTransform.localScale = new Vector3(lineWidth, 1, lineLength);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
