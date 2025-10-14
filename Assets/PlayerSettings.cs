using UnityEngine;
using UnityEngine.UI; 
public class PlayerSettings : MonoBehaviour
{
    public Slider volumeSlider; 
    public float Distance = 0.5f;
    public BattleAI battleai;
    public float Speed = 0.5f;
    public ShootingScript shootingsScript;
    void Start()
    {
        if (volumeSlider != null)
        {
           
            volumeSlider.value = (Distance - 1f) / 9f;
            volumeSlider.onValueChanged.AddListener(UpdateDisVolume);
            volumeSlider.onValueChanged.AddListener(UpdateDisSpeed);
        }
    }

    // This method will be called whenever the slider's value changes
    public void UpdateDisVolume(float newSliderValue)
    {
        // Map the 0-1 slider value to the 1-10 Distance value
        Distance = 1f + (newSliderValue * 9f);
        Debug.Log("Distance " + Distance); // Optional: Log the new value
        BattleAI battleAI = GetComponent<BattleAI>();
        battleAI.minDistance = Distance;
        battleAI.maxDistance = Distance+2;
    }
    public void UpdateDisSpeed(float newSliderValue)
    {
        Speed = 1f + (newSliderValue * 9f);
        Debug.Log("Speed " + Speed);
        ShootingScript shootingsScript = GetComponent<ShootingScript>();
        shootingsScript.currentRepeatRate = Speed;
    }
}
