using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CardCombiner;

public class CardCombiner : MonoBehaviour
{
    public enum CombineOperation { addition, subtration, multiplication, division }
    [SerializeField] private PileManager aPile;
    [SerializeField] private PileManager bPile;
    [SerializeField] private GameObject plusButtonGO;
    [SerializeField] private GameObject minusButtonGO;
    [SerializeField] private TextMeshPro resultTMP;
    private CombineOperation currentOperation = CombineOperation.addition;
    private PlayerCardManager playerCardManager;

    public void SetCurrentOperation(CombineOperation newOperation) { currentOperation = newOperation; }
    public void SetAddOperation() { SetCurrentOperation(CombineOperation.addition); }
    public void SetSubtractOperation() { SetCurrentOperation(CombineOperation.subtration); }

    public void EnableCurrentOperationButton()
    {
        switch (currentOperation)
        {
            case (CombineOperation.addition):
                plusButtonGO.SetActive(true);
                break;
            case (CombineOperation.subtration):
                minusButtonGO.SetActive(true);
                break;
        }
    }

    public void PerformCurrentOperation()
    {
        switch (currentOperation)
        {
            case (CombineOperation.addition):
                AddSelectedCards();
                break;
            case (CombineOperation.subtration):
                SubtractSelectedCards();
                break;
        }
    }

    public void AddSelectedCards() { CombineSelectedCards(CombineOperation.addition); }
    public void SubtractSelectedCards() { CombineSelectedCards(CombineOperation.subtration); }

    public void UpdateResult()
    {
        if (aPile.Cards.Count != 1 || bPile.Cards.Count != 1)
        {
            resultTMP.text = "Combine A and B";
            return;
        }

        float totalValue = GetTrueResult(currentOperation);

        totalValue = Mathf.Clamp(totalValue, -GlobalGameSettings.SimplifiedTiers[0], GlobalGameSettings.SimplifiedTiers[0]);

        totalValue /= 1000f;


        resultTMP.text = "New Card: " + totalValue.ToString();
    }

    private int GetTrueResult(CombineOperation combineOperation)
    {
        int totalValue = 0;
        int aValue = aPile.Cards[0].TrueValue;
        int bValue = bPile.Cards[0].TrueValue;

        switch (combineOperation)
        {
            case CombineOperation.addition:
                return totalValue = aValue + bValue;
            case CombineOperation.subtration:
                return totalValue = aValue - bValue;
            default:
                return 0;
        }
    }

    private void CombineSelectedCards(CombineOperation combineOperation)
    {
        if (aPile.Cards.Count != 1 || bPile.Cards.Count != 1)
        {
            return;
        }

        int totalValue = GetTrueResult(combineOperation);
        totalValue = Mathf.Clamp(totalValue, -GlobalGameSettings.SimplifiedTiers[0], GlobalGameSettings.SimplifiedTiers[0]);

        playerCardManager.MoveCardToHand(playerCardManager.CreateNewCard(totalValue));

        playerCardManager.DeleteFormulaCards();
    }

    private void Awake()
    {
        playerCardManager = GetComponent<PlayerCardManager>();
    }
}
