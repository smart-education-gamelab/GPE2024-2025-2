using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FunctionDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI functionText; // Reference to the TextMeshProUGUI component
    [SerializeField] private Transform aSlot; // Slot for 'a'
    [SerializeField] private Transform bSlot; // Slot for 'b'
    [SerializeField] private Transform cSlot; // Slot for 'c'

    private FunctionDrawer functionDrawer;

    void Start()
    {
        functionDrawer = FindFirstObjectByType<FunctionDrawer>(); // Get the FunctionDrawer component
        UpdateFunctionText();
    }

    public void UpdateFunctionText()
    {
        string functionString = "";

        string aValue = aSlot.childCount > 0 ? aSlot.GetChild(0).name : "?";
        string bValue = bSlot.childCount > 0 ? bSlot.GetChild(0).name : "?";
        string cValue = cSlot.childCount > 0 ? cSlot.GetChild(0).name : "?";
        
        // Check if the values are positive or negative
        // For aValue it does not matter because there is no plus or minus in front of it
        if (bValue != "?") bValue = int.Parse(bValue) > 0 ? "+ " + bValue : bValue.Insert(1, " ");
        if (cValue != "?") cValue = int.Parse(cValue) > 0 ? "+ " + cValue : cValue.Insert(1, " ");;
        
        // Update the function string based on the selected function type
        switch (functionDrawer.functionType)
        {
            case FunctionDrawer.FunctionType.Linear:
                functionString = $"y = {aValue}x {bValue}";
                break;
            case FunctionDrawer.FunctionType.Exponential:
                functionString = $"y = {aValue}x² {bValue}x {cValue}";
                break;
        }

        functionText.text = functionString;
        UpdateFunctionDisplayed(functionString);
    }

    private void UpdateFunctionDisplayed(string function)
    {
        functionDrawer.function = function; // Update the function in the FunctionDrawer
        functionDrawer.a = aSlot.childCount > 0 ? int.Parse(aSlot.GetChild(0).name) : 1;
        functionDrawer.b = bSlot.childCount > 0 ? int.Parse(bSlot.GetChild(0).name) : 1;
        functionDrawer.c = cSlot.childCount > 0 ? int.Parse(cSlot.GetChild(0).name) : 1;
        functionDrawer.DrawFunction(); // Draw the function
    }
}