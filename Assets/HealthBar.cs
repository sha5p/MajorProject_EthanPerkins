using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBarSlider;
    public Image fillImage;

    public Image[] scoreMarkers;
    public void GiveFullHealth(float health)
    {
        healthBarSlider.maxValue = health;
        healthBarSlider.value = health;
    }
    public void SetHeaHealth(float health)
    {
        healthBarSlider.value = health;
        if(healthBarSlider.value == 0)
        {
            PrintBattleResult();
        }
    }
    private readonly Color GreenColor = Color.green;
    private readonly Color RedColor = Color.red;
    private readonly Color EmptyColor = Color.white;
    private void PrintBattleResult()
    {
        // Get the color set in the SetHealth function (it should be red if health is zero)
        Color finalColor = fillImage.color;

        string resultMessage;

        if (finalColor.Equals(RedColor))
        {
            AwardWin(GreenColor);
        }
        else if (finalColor.Equals(GreenColor))
        {
            AwardWin(RedColor);
        }
        else
        {
            // If the color is in between (e.g., yellow/orange), just report the final color.
            resultMessage = $"Battle ended with an intermediate colour: {ColorUtility.ToHtmlStringRGB(finalColor)}";
        }

    }
    public void AwardWin(Color winColor)
    {
        // 1. Iterate through the score markers for THIS winning car
        for (int i = 0; i < scoreMarkers.Length; i++)
        {
            Image marker = scoreMarkers[i];

            if (ColorUtility.ToHtmlStringRGBA(marker.color) == ColorUtility.ToHtmlStringRGBA(EmptyColor))
            {

                marker.color = winColor;

                Debug.Log($"{gameObject.name} won! Filling Score Marker {i + 1}.");

                return;
            }
        }


    }


}
