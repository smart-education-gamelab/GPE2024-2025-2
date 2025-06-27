using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TextMeshProUGUI tempCardNr; // Reference to the TextMeshProUGUI component
    public int cardValue; // The value of the card
    private Transform originalParent;
    private Canvas canvas;
    [NonSerialized] public CardManager cardManager; // Reference to the CardManager component

    private void Start()
    {
        originalParent = transform.parent;
        canvas = FindFirstObjectByType<Canvas>();
        tempCardNr.text = cardValue.ToString(); // Set the text to the card value
        transform.name = cardValue.ToString(); // Set the name of the card to its value
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(canvas.transform); // Move to the top level of the canvas
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; // Follow the mouse
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        Transform targetSlot = null;

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("CardSlot"))
            {
                targetSlot = result.gameObject.transform;
                break;
            }
        }

        if (targetSlot != null)
        {
            // Check if the slot is already filled
            if (targetSlot.childCount == 0)
            {
                transform.SetParent(targetSlot); // Snap to the slot
            }
            else
            {
                transform.SetParent(originalParent); // Return to the original parent
            }
        }
        else
        {
            transform.SetParent(originalParent); // Return to the original parent
            cardManager.SelectCard(this); // Notify CardManager when the card is selected
        }

        transform.localPosition = Vector3.zero; // Reset position
        FindFirstObjectByType<FunctionDisplay>().UpdateFunctionText(); // Update the function text
    }
}