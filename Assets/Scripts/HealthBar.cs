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
        if (healthBarSlider.value == 0)
        {
            PrintBattleResult();
        }
    }
    private readonly Color GreenColor = Color.green;
    private readonly Color RedColor = Color.red;
    private readonly Color EmptyColor = Color.white;
    public GameObject self;
    public GameObject countdownGameobject;
    private void PrintBattleResult()
    {
        bool opponentIsZero = healthBar.healthBarSlider.value == 0;
        if (opponentIsZero && self.name == "HelathBar1")
        {
            // 🚨 TIE SCENARIO: Both players hit 0 simultaneously!
            Debug.Log("Round Tie! Both players were defeated.");



            // Notify the countdown script to advance the round
            countdownGameobject.SetActive(true);
            Countdown.RoundTie();

            return;
        }
        // Get the color set in the SetHealth function (it should be red if health is zero)
        Color finalColor = fillImage.color;
        DestroyAllBattleAIs();
        string resultMessage;

        if (finalColor.Equals(RedColor))
        {
            AwardWin(GreenColor);
            if (checker)
            {
                countdownGameobject.SetActive(true);
                checker = false;
            }

        }
        else if (finalColor.Equals(GreenColor))
        {
            AwardWin(RedColor);
            if (checker)
            {
                countdownGameobject.SetActive(true);
                checker = false;
            }

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
    public bool checker = false;
    public void AwardWin(Color winColor)
    {
        int currentWins = 0;
        int firstEmptyMarkerIndex = -1;

        // 1. Count existing wins and find the next empty spot
        for (int i = 0; i < scoreMarkers.Length; i++)
        {
            Image marker = scoreMarkers[i];

            // Check for existing wins of this color
            if (ColorUtility.ToHtmlStringRGBA(marker.color) == ColorUtility.ToHtmlStringRGBA(winColor))
            {
                currentWins++;
            }
            // Find the index of the first empty marker
            else if (ColorUtility.ToHtmlStringRGBA(marker.color) == ColorUtility.ToHtmlStringRGBA(EmptyColor) && firstEmptyMarkerIndex == -1)
            {
                firstEmptyMarkerIndex = i;
            }
        }

        if (firstEmptyMarkerIndex != -1)
        {
            Image marker = scoreMarkers[firstEmptyMarkerIndex];
            marker.color = winColor;
            currentWins++;
            healthBarSlider.value = 100;
            healthBar.healthBarSlider.value = 100;

            Debug.Log($"{gameObject.name} won! Filling Score Marker {firstEmptyMarkerIndex + 1}. Total wins: {currentWins}");

            // 3. Check for Game Over after the point is awarded
            if (currentWins >= 2)
            {
                Debug.Log($"*** MATCH WINNER ({winColor})! Game Over. ***");
                countdownGameobject.SetActive(true);
                string winner = ConvertWinColorToString(winColor);
                Countdown.gameover(winner); // Game Over
            }
            else
            {
                checker = true;
                countdownGameobject.SetActive(true);
                Countdown.roundEnd(); // Advance to next round
            }
        }
    }

    public string ConvertWinColorToString(Color winColor)
    {
        // Use ColorUtility.ToHtmlStringRGBA for reliable color comparison
        string winColorHtml = ColorUtility.ToHtmlStringRGBA(winColor);
        string redHtml = ColorUtility.ToHtmlStringRGBA(Color.red);
        string greenHtml = ColorUtility.ToHtmlStringRGBA(Color.green);

        if (winColorHtml.Equals(redHtml))
        {
            return "Red";
        }
        else if (winColorHtml.Equals(greenHtml))
        {
            return "Green";
        }
        else
        {
            return $"Unknown Color ({winColorHtml})";
        }
    }
}
