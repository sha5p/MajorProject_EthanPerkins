using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Slider

public class PlayerSettings : MonoBehaviour
{
    public Slider volumeSlider; // Assign your UI Slider here in the Inspector
    public float Distance = 0.5f; 

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = Distance;
            volumeSlider.onValueChanged.AddListener(UpdateDisVolume);
        }
    }

    // This method will be called whenever the slider's value changes
    public void UpdateDisVolume(float newVolume)
    {
        Distance = newVolume;
        Debug.Log("Distance " + Distance); // Optional: Log the new value
        // You can add more logic here, e.g., set AudioListener.volume = masterVolume;
    }
}
