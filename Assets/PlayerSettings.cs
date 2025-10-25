using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            volumeSlider.onValueChanged.AddListener(UpdateDis);
            volumeSlider.onValueChanged.AddListener(UpdateDisSpeed);
        }
    }

    // This method will be called whenever the slider's value changes
    public void UpdateDis(float newSliderValue)
    {

        // Map the 0-1 slider value to the 1-10 Distance value
        Distance = 1f + (newSliderValue * 9f);
        BattleAI battleAI = Object.FindFirstObjectByType<BattleAI>();


        if (battleAI != null)
        {
            battleAI.SetDistance(Distance);
        }
        else
        {
            Debug.LogWarning("no BattleAI found in scene!");
        }


    }
    public void UpdateDisSpeed(float newSliderValue)
    {
        GameObject weaponMountParent = GameObject.Find("WeaponMount");

        // Get the Transform of the FIRST CHILD (index 0)
        Transform childTransform = weaponMountParent.transform.GetChild(0);
        GameObject targetGameObject = childTransform.gameObject;

        float Speed = 1f + (newSliderValue * 9f);

        Debug.Log("Speed " + Speed + " | Target: " + targetGameObject.name);

        ShootingScript shootingsScript = targetGameObject.GetComponent<ShootingScript>();

        shootingsScript.currentRepeatRate = Speed;
        shootingsScript.UpdateFireRate(shootingsScript.currentRepeatRate);
    }
}
