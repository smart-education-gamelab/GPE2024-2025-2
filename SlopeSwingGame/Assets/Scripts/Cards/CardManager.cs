using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab; // Prefab for the card
    [SerializeField] private List<Transform> cardSlots; // List of slots to place cards in
    [SerializeField] private int maxCards = 4; // Maximum number of cards to draw
    [SerializeField] private int maxCardValue = 10; // Maximum value for the cards
    private List<DraggableCard> cards = new();
    private List<DraggableCard> selectedCards = new(); // List to store selected cards
    public int currentCardCount;

    [SerializeField] private Button plusButton;
    [SerializeField] private Button minusButton;

    void Start()
    {
        DrawCards();

        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
    }

    private void DrawCards()
    {
        while (currentCardCount < maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, cardSlots[currentCardCount]);
            DraggableCard cardComponent = newCard.GetComponent<DraggableCard>();
            cardComponent.cardValue = Random.Range(-1 * maxCardValue, maxCardValue + 1);
            cardComponent.cardManager = this;
            cards.Add(cardComponent);
            currentCardCount++;
        }
    }

    public void SelectCard(DraggableCard card)
    {
        if (selectedCards.Contains(card)) return;

        if (selectedCards.Count < 2)
        {
            selectedCards.Add(card);
        }
        else
        {
            selectedCards.Clear();
            selectedCards.Add(card);
        }
    }

    private void OnPlusButtonClicked()
    {
        if (selectedCards.Count == 2)
        {
            int result = selectedCards[0].cardValue + selectedCards[1].cardValue;
            Destroy(selectedCards[0].gameObject);
            Destroy(selectedCards[1].gameObject);
            selectedCards.Clear();
            currentCardCount -= 2; // Decrease the card count by 2
            // Create a new card with the result
            CreateNewCard(result);
        }
    }

    private void OnMinusButtonClicked()
    {
        if (selectedCards.Count == 2)
        {
            int result = selectedCards[0].cardValue - selectedCards[1].cardValue;
            Destroy(selectedCards[0].gameObject);
            Destroy(selectedCards[1].gameObject);
            selectedCards.Clear();
            currentCardCount -= 2; // Decrease the card count by 2
            // Create a new card with the result
            CreateNewCard(result);
        }
    }

    private void CreateNewCard(int value)
    {
        if (currentCardCount < maxCards)
        {
            GameObject newCard = Instantiate(cardPrefab, cardSlots[currentCardCount]);
            DraggableCard cardComponent = newCard.GetComponent<DraggableCard>();
            cardComponent.cardValue = value;
            cardComponent.cardManager = this;
            cards.Add(cardComponent);
            currentCardCount++;
        }
    }
}