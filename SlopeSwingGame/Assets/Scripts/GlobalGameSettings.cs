
using UnityEngine;

public static class GlobalGameSettings
{
    public static bool SimplifiedValues => simplifiedValues;
    public static int[] SimplifiedTiers => simplifiedTiers;

    private static bool simplifiedValues = false;
    private static int[] simplifiedTiers = { 10000, 2499, 999, 0 };
    private static string[] tierColorHexes = { "#FFD36B", "#FBE9B1", "#F9F4D4", "#F7FFF7", "#E4E8EF", "#D0D1E7", "#BDBADF" };

    public static void SetSimplifiedValueText(bool simplified) { simplifiedValues = simplified; }

    public static string ConvertToSimplifiedText(int value)
    {
        if (value == 0)
        {
            return "0";
        }

        // Changes the displayed sign based on the true value's sign
        string valueSignText = value > 0 ? "+" : "-";

        int signCount = CalculateSignCount(value);

        string valueText = "";
        for (int i = 0; i < signCount; i++)
        {
            valueText += valueSignText;
        }

        return valueText;
    }

    public static float GetTierPercentage(int value)
    {
        float tempValue = Mathf.Abs(value);

        for (int i = 0; i < simplifiedTiers.Length; i++)
        {
            if (i == 0) { continue; }

            if (tempValue > simplifiedTiers[i])
            {
                // Bigger than our current minimum for the tier
                return tempValue / simplifiedTiers[i - 1];
            }
        }

        return 0;
    }

    public static Color GetTierColor(int value)
    {
        string targetHex = "#000000";

        if (value > simplifiedTiers[1])
        {
            targetHex = tierColorHexes[0];
        }
        else if (value > simplifiedTiers[2])
        {
            targetHex = tierColorHexes[1];
        }
        else if (value > simplifiedTiers[3])
        {
            targetHex = tierColorHexes[2];
        }
        else if (value == 0)
        {
            targetHex = tierColorHexes[3];
        }
        else if (-value > simplifiedTiers[1])
        {
            targetHex = tierColorHexes[6];
        }
        else if (-value > simplifiedTiers[2])
        {
            targetHex = tierColorHexes[5];
        }
        else if (-value > simplifiedTiers[3])
        {
            targetHex = tierColorHexes[4];
        }


        ColorUtility.TryParseHtmlString(targetHex, out Color colorFromHex);

        return colorFromHex;
    }

    private static int CalculateSignCount(int value)
    {
        int tempValue = Mathf.Abs(value);

        int signCount = simplifiedTiers.Length;

        for (int i = 0; i < simplifiedTiers.Length; i++)
        {
            if (tempValue > simplifiedTiers[i])
            {
                return signCount;
            }
            else
            {
                signCount--;
            }
        }

        return signCount;
    }
} 