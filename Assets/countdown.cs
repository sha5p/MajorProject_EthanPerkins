using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class countdown : MonoBehaviour
{
    public int countDownTime;
    public TextMeshProUGUI countdownDisplay;
    public GameObject countdownObject;

    public BattleScript battlescript;
    public void Start()
    {
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
    IEnumerator RoundTransitionRoutine()
    {
        int currentRound = GetCurrentRoundNumber(1);

        countdownObject.SetActive(true);
        countdownDisplay.text = $"Round {currentRound} Over";
        yield return new WaitForSeconds(2.0f); // Wait 2 seconds

        //battlescript.ResetBattleState(); 

        int nextRound = currentRound + 1;
        countdownDisplay.text = $"Round {nextRound}";
        yield return new WaitForSeconds(1.0f); // Wait 1 second

        // 4. Start the next countdown sequence (3, 2, 1, Fight!)
        countdownObject.SetActive(false); // Hide the Round text
        countDownTime = 3; // Reset the countdown timer
        StartCoroutine(CountdownToStart());
    }
    int GetCurrentRoundNumber(int z)
    {
        return 1;
    }
}
