using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBarSlider;
    public Image fillImage;
    public countdown Countdown;
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

    public GameObject countdownGameobject;
    private void PrintBattleResult()
    {
        // Get the color set in the SetHealth function (it should be red if health is zero)
        Color finalColor = fillImage.color;
        DestroyAllBattleAIs();
        string resultMessage;

        if (finalColor.Equals(RedColor))
        {
            AwardWin(GreenColor);
            countdownGameobject.SetActive(true);
            Countdown.roundEnd();
        }
        else if (finalColor.Equals(GreenColor))
        {
            AwardWin(RedColor);
            countdownGameobject.SetActive(true);
            Countdown.roundEnd();
        }
        else
        {
            // If the color is in between (e.g., yellow/orange), just report the final color.
            resultMessage = $"Battle ended with an intermediate colour: {ColorUtility.ToHtmlStringRGB(finalColor)}";
        }

    }
    private void DestroyAllBattleAIs()
    {
        // Find all active instances of the BattleAI script in the scene
        // Use the overload that takes a FindObjectsSortMode argument.
        BattleAI[] battleAIs = UnityEngine.Object.FindObjectsByType<BattleAI>(FindObjectsSortMode.None);

        // Loop through the array and destroy the GameObject each script is attached to
        foreach (BattleAI ai in battleAIs)
        {
            // Destroy the GameObject that the BattleAI component is attached to
            Destroy(ai.gameObject);
            Debug.Log($"Destroyed GameObject: {ai.gameObject.name} (with BattleAI script)");
        }
    }
    public HealthBar healthBar;
    public void AwardWin(Color winColor)
    {
        // 1. Iterate through the score markers for THIS winning car
        for (int i = 0; i < scoreMarkers.Length; i++)
        {
            Image marker = scoreMarkers[i];

            if (ColorUtility.ToHtmlStringRGBA(marker.color) == ColorUtility.ToHtmlStringRGBA(EmptyColor))
            {

                marker.color = winColor;
                healthBarSlider.value = 100;
                healthBar.healthBarSlider.value = 100;

                Debug.Log($"{gameObject.name} won! Filling Score Marker {i + 1}.");

                return;
            }
        }


    }


}
