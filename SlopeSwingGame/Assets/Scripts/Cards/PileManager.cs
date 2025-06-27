using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PileManager : MonoBehaviour
{
    public List<Card> Cards => cards;
    public float Width => width;
    public float Height => height;
    public bool CanSubstituteCard => canSubstituteCard;

    [SerializeField] private GameObject cardPrefab;
    [OnValueChanged("CalculateCardTransforms")][SerializeField] private int maxSize = 10;
    [OnValueChanged("CalculateCardTransforms")][Range(-10f, 10f)][SerializeField] private float curvature = 0f;
    [OnValueChanged("CalculateCardTransforms")][SerializeField] private float xOffset = 0;
    [OnValueChanged("CalculateCardTransforms")][SerializeField] private float yOffset = 0;
    [OnValueChanged("CalculateCardTransforms")][SerializeField] private float zOffset = 0;
    [OnValueChanged("CalculateCardTransforms")][Range(0f, 20)][SerializeField] private float spacing = 0.1f;
    [OnValueChanged("CalculateCardTransforms")][Range(-360f, 360f)][SerializeField] private float xPositionRotation;
    [OnValueChanged("CalculateCardTransforms")][Range(-360f, 360f)][SerializeField] private float rotationOffset;
    [SerializeField] private bool disableTextOnFull = true;
    [SerializeField] private bool canSubstituteCard = false;
    [SerializeField] private UnityEvent pileChanged;
    private TMPro.TextMeshPro formulaTMP;

    private List<Card> cards;
    private float lineWidth => spacing * cards.Count;
    private float cardDistance => lineWidth / (cards.Count - 1);

    private float width;
    private float height;

    public void SetPileText(string newText) { formulaTMP.text = newText; }

    public bool TryAddCard()
    {
        return cards.Count + 1 < maxSize;
    }

    public bool AcceptCard(Card newCard, PileManager newCardSourcePile)
    {
        if (cards.Contains(newCard))
        {
            return false; // We already have this card...
        }

        if (cards.Count >= maxSize)
        {
            // Too many cards

            // Do we accept substitutes?
            if (canSubstituteCard && newCardSourcePile)
            {
                // We do
                // Just replace the first card
                Card firstCard = cards[0];

                // Remove what we have
                RemoveCard(firstCard);

                // Add in the other one
                AddCard(newCard);

                // Add the old card to the old pile
                newCardSourcePile.AddCard(firstCard);
                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            AddCard(newCard);
            return true;
        }

    }

    public void AddCard(Card newCard)
    {
        cards.Add(newCard);
        newCard.transform.parent = transform;
        CalculateCardTransforms();
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        CalculateCardTransforms();
    }

    public void RemoveCard(int index)
    {
        GameObject cardToRemove = GetCardGO(index);
        cards.RemoveAt(index);
        CalculateCardTransforms();
    }

    public void RemoveAllCards()
    {
        cards.Clear();
    }

    public void DeleteAllCards()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i].gameObject);
        }

        RemoveAllCards();
    }

    public void SubstituteCard(Card oldCard, Card newCard)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == oldCard)
            {
                cards[i] = newCard;
                newCard.transform.parent = this.transform;
                CalculateCardTransforms();
                return;
            }
        }
    }

    public void SubstituteCard(int index, Card substituteCard)
    {
        cards[index] = substituteCard;
        cards[index].transform.parent = transform;
        CalculateCardTransforms();
    }
    public void CalculateCardTransforms()
    {
        SortPile();

        for (int i = 0; i < cards.Count; i++)
        {
            CalculateCardTransform(i);
        }

        CalculatePileWidth();
        CalculatePileHeight();
        EvaluateFormulaText();

        pileChanged.Invoke();
    }

    private int GetCardIndex(Card card) { return cards.FindIndex(searchedCard => searchedCard == card); }
    private GameObject GetCardGO(int index) { return cards[index].gameObject; }

    private void CalculateCardTransform(int cardIndex)
    {
        Vector3 totalOffset = new Vector3(xOffset, yOffset, zOffset);

        // Because card distance uses the total - 1 as a divisor
        // if we only have one card it would cause NaN errors.
        // To avoid this we have this catch:

        if (cardIndex == 0 && cards.Count == 1)
        {
            cards[cardIndex].SetTarget(transform, totalOffset);
            return;
        }

        // Positions
        //
        //                  0
        //                  1
        //                2   2
        //              3   3   3
        //            4   4   4   4
        //          5   5   5   5   5
        //
        // Calculate level of spacing from left to right
        // Position = -((total - 1) / 2) + spacing * index
        // Then do +spacing every time

        float cardXPosition = -(lineWidth / 2) + (cardDistance * cardIndex);

        // Creates essentially a bell curve as its a upside down paraballa (since X is negative)
        // Flipping the curvature sign because it makes more sense in my head that way
        float cardYPosition = ((curvature * -0.001f) * cardXPosition * cardXPosition);

        Vector2 cardPosition = new Vector2(cardXPosition, cardYPosition);
        cardPosition.x += totalOffset.x; // Probably a better way to figure out
        cardPosition.y += totalOffset.y; // the true position of the panel

        cards[cardIndex].SetTarget(transform, new Vector3(cardXPosition, 0, cardYPosition));
        //cards[cardIndex].SetTargetPosition(new Vector3(cardPosition.x, transform.position.y, cardPosition.y));

        // Rotation
        // The card rotates more the further it is from the center of the hand
        float cardAngle = cardXPosition * 0.1f * curvature;
        cardAngle += (cardXPosition * xPositionRotation * 0.01f); // Apply the offset
        cardAngle += rotationOffset * ((float)cardIndex / (float)cards.Count);
        cards[cardIndex].SetTargetYRotation(cardAngle);
    }

    private void CalculatePileWidth()
    {
        // Rigth minus left (end minus start)
        // Can't use actual positions as we don't know where the cards are technically on screen
        width = 0;
        
        if (cards.Count > 0)
        {
            width = (cards[cards.Count - 1].TargetPosition.x + cards[cards.Count - 1].GetColliderWidth() * 0.6f) - (cards[0].TargetPosition.x - cards[0].GetColliderWidth() * 0.6f);
        }
    }

    private void CalculatePileHeight()
    {
        height = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i].GetColliderHeight() > height)
            {
                height = cards[i].GetColliderHeight();
            }
        }

        height += curvature * 0.001f * (lineWidth / 2) * (lineWidth / 2) * 2;
    }

    // Uses Merge Sort
    private void SortPile()
    {
        cards = cards.OrderBy(c => c.TrueValue).ToList();
    }

    private void EvaluateFormulaText()
    {
        if (!formulaTMP || !disableTextOnFull)
        {
            return;
        }

        if (cards.Count > 0)
        {
            formulaTMP.enabled = false;
        }
        else
        {
            formulaTMP.enabled = true;
        }
    }

    private void Awake()
    {
        cards = new List<Card>();
        formulaTMP = GetComponent<TMPro.TextMeshPro>();
        
        if (pileChanged == null)
        {
            pileChanged = new UnityEvent();
        }
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
