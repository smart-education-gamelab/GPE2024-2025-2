using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button linearButton;
    [SerializeField] private Button exponentialButton;
    
    private FunctionDrawer functionDrawer;
    private FunctionDisplay functionDisplay;
    
    private void Awake()
    {
        functionDrawer = FindFirstObjectByType<FunctionDrawer>();
        functionDisplay = FindFirstObjectByType<FunctionDisplay>();
        
        linearButton.onClick.AddListener(OnLinearButtonClicked);
        exponentialButton.onClick.AddListener(OnExponentialButtonClicked);
    }

    private void OnDestroy()
    {
        linearButton.onClick.RemoveListener(OnLinearButtonClicked);
        exponentialButton.onClick.RemoveListener(OnExponentialButtonClicked);
    }

    private void OnLinearButtonClicked()
    {
        functionDrawer.functionType = FunctionDrawer.FunctionType.Linear;
        functionDrawer.DrawFunction();
        functionDisplay.UpdateFunctionText();
    }

    private void OnExponentialButtonClicked()
    {
        functionDrawer.functionType = FunctionDrawer.FunctionType.Exponential;
        functionDrawer.DrawFunction();
        functionDisplay.UpdateFunctionText();
    }
}
