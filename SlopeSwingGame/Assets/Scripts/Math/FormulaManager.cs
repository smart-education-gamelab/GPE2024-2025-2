using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FormulaManager : MonoBehaviour
{
    [SerializeField] private PileManager mPile;
    [SerializeField] private PileManager xyPile;
    private SwingManager swingManager;

    private FormulaConfiguration formulaConfiguration = FormulaConfiguration.Linear;
    private float mValue = 0;
    private float xyValue = 0;
    private bool locked = false;

    [Button("Debug Linear Formula")]
    public void SwitchToLinearFormula()
    {
        formulaConfiguration = FormulaConfiguration.Linear;
        ReCalculate();
    }

    [Button("Debug X Quadratic Formula")]
    public void SwitchToXQuadraticFormula()
    {
        formulaConfiguration = FormulaConfiguration.XSquared;
        ReCalculate();
    }

    [Button("Debug Y Quadratic Formula")]
    public void SwitchToYQuadraticFormula()
    {
        formulaConfiguration = FormulaConfiguration.YSquared;
        ReCalculate();
    }

    public void SetLocked(bool isLocked)
    {
        locked = isLocked;

        if (locked)
        {
            LockFormula();
        }
        else
        {
            ReCalculate();
        }
    }

    public void DeleteFormulaCards()
    {
        mPile.DeleteAllCards();
        xyPile.DeleteAllCards();

        mPile.CalculateCardTransforms();
        xyPile.CalculateCardTransforms();

        ReCalculate();
    }

    public void ReCalculate()
    {
        if (locked)
        {
            LockFormula();
            return;
        }

        if (mPile.Cards.Count < 1 || xyPile.Cards.Count < 1)
        {
            // Not all values are filled
            LockFormula();
            return;
        }

        if (!mPile.Cards[0].Valid || !xyPile.Cards[0].Valid)
        {
            // One of the cards is a substitute card
            LockFormula();
            return;
        }

        // All values are filled
        mValue = mPile.Cards[0].Value;
        xyValue = xyPile.Cards[0].Value;

        if (swingManager)
        {
            swingManager.SetFormula(formulaConfiguration, mValue, xyValue);
        }
    }

    private void LockFormula()
    {
        if (swingManager)
        {
            swingManager.SetFormula(formulaConfiguration, 0, 0);
        }
    }

    private float GetTrueResult()
    {
        float result = 0;

        switch (formulaConfiguration)
        {
            case (FormulaConfiguration.Linear):
                result = mValue * xyValue;
                break;
            case (FormulaConfiguration.XSquared):
                result = mValue * xyValue * xyValue;
                break;
            case (FormulaConfiguration.YSquared):
                result = mValue * xyValue * xyValue;
                break;
            default:
                break;
        }

        return result;
    }

    private void Awake()
    {
        swingManager = GetComponent<SwingManager>();
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
