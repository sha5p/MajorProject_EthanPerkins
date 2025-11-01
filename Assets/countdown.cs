using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class countdown : MonoBehaviour
{
    public int countDownTime;
    public TextMeshProUGUI countdownDisplay;
    public GameObject countdownObject;

    public BattleScript battlescript;

    private int currentRound = 1;
    public int initialCountDownTime = 3;
    public void Start()
    {
        countDownTime = initialCountDownTime; 
        StartCoroutine(CountdownToStart());
    }
    IEnumerator CountdownToStart()
    {
        while(countDownTime > 0)
        {
            countdownDisplay.text=countDownTime.ToString();
            yield return new WaitForSeconds(1f);

            countDownTime--;
        }
        
        countdownDisplay.text = "Fight!";

        yield return new WaitForSeconds(0.3f);
        battlescript.BattleBegin();
        countdownObject.SetActive(false);
        

    }
    public void roundEnd()
    {
        StartCoroutine(RoundTransitionRoutine());
    }
    IEnumerator RoundTransitionRoutine()
    {
        if (currentRound != 4)
        {

            countdownDisplay.text = $"Round {currentRound} Over";
            yield return new WaitForSeconds(2.0f); // Wait 2 seconds
            currentRound++;

            countdownDisplay.text = $"Round {currentRound}";
            yield return new WaitForSeconds(1.0f); // Wait 1 second
            countDownTime = initialCountDownTime;
            StartCoroutine(CountdownToStart());
        }
        if (currentRound == 4)
        {
            gameover();
        }

    }
    public void gameover()
    {

    }
    public Image[] scoreMarkers;
    public Color emptyColor = Color.grey;
    public int GetCurrentRoundNumber()
    {
        int filledMarkers = 0;

        foreach (Image marker in scoreMarkers)
        {
            if (marker.color == emptyColor)
            {
                filledMarkers++;
            }
        }

        return filledMarkers + 1;
    }
    public void RoundTie()
    {
        StartCoroutine(RoundTieRoutine());
    }

    IEnumerator RoundTieRoutine()
    {
        countdownObject.SetActive(true);

        // Announce the Tie
        countdownDisplay.text = "TIE! Re-Fight!";
        yield return new WaitForSeconds(2.0f);

        countDownTime = initialCountDownTime;

        // Start the countdown for the CURRENT round again
        StartCoroutine(CountdownToStart());
    }
}
