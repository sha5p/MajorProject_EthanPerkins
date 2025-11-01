using System.Collections;
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
        // Start the Coroutine immediately. The rest of the logic will run inside it.
        StartCoroutine(InitializeSettingsAfterDelay());
    }

    private IEnumerator InitializeSettingsAfterDelay()
    {
        yield return new WaitForSeconds(1f);


        if (volumeSlider != null)
        {
            volumeSlider.value = (Distance - 1f) / 9f;

            BattleAI battleAI = Object.FindFirstObjectByType<BattleAI>();

            if (battleAI != null)
            {
                // The object was found, so it is safe to access .gameObject
                UnityEngine.GameObject aiGameObject = battleAI.gameObject;

                float damageing = GetShooterDamage(aiGameObject);
                float distance = GetCarDistance(aiGameObject);

                dis.valueChange(distance);
                speed.valueChange(damageing/10);
                damage.valueChange(damageing);
            }
            else
            {
                Debug.LogError("Failed to find BattleAI after 1-second delay. Check object timing/existence.");
            }
        }
    }

    // This method will be called whenever the slider's value changes
    public Stats_Show damage;
    public Stats_Show speed;
    public Stats_Show dis;
    public void UpdateDis(float newSliderValue)
    {

        // Map the 0-1 slider value to the 1-10 Distance value
        Distance = 1f + (newSliderValue * 9f);
        BattleAI battleAI = Object.FindFirstObjectByType<BattleAI>();
        dis.valueChange(Distance);
        

        if (battleAI != null)
        {
            battleAI.SetDistance(Distance);
            battleAI.SaveCurrentDistance(); 
        }
        else
        {
            Debug.LogWarning("no BattleAI found in scene!");
        }


    }
    
    public void UpdateDisSpeed(float newSliderValue)
    {
        BattleAI battleAI = Object.FindFirstObjectByType<BattleAI>();
        GameObject weaponMountParent = GameObject.Find("WeaponMount");

        // Get the Transform of the FIRST CHILD (index 0)
        Transform childTransform = weaponMountParent.transform.GetChild(0);
        GameObject targetGameObject = childTransform.gameObject;

        float Speed = 1f + (newSliderValue * 9f);

        Debug.Log("Speed " + Speed + " | Target: " + targetGameObject.name);

        ShootingScript shootingsScript = targetGameObject.GetComponent<ShootingScript>();

        shootingsScript.currentRepeatRate = Speed;
        shootingsScript.SaveCurrentRepeatRate();
        speed.valueChange(Speed);
        damage.valueChange(Speed * 10);
        shootingsScript.UpdateFireRate(shootingsScript.currentRepeatRate);
    }
    public float GetCarDistance(GameObject carObject)
    {
        string objectName = carObject.name;
        string baseName = objectName.Replace("(Clone)", "").Trim(); // Trim removes any extra spaces
        string carNumber = baseName;
        Debug.Log(carNumber + "This is the car number");
        string filename = GetStateFilename(carNumber);
        PlayerState loadedState = FileHandler.ReadFromJSON<PlayerState>(filename);

        if (loadedState != null)
        {
            float distance = loadedState.Distance;
            Debug.Log($"Loaded distance for Car {carNumber}: {distance}");
            return distance;
        }
        else
        {
            Debug.LogError($"Could not load data for car number: {carNumber}. Check if the file '{filename}' exists in the persistent data path.");
            return 0f; // Return 0 or another default/error value
        }
    }
    private string GetStateFilename(string carNumber)
    {
        return "PlayerState" + carNumber + ".json";
    }
    private float GetShooterDamage(GameObject shooterCar)
    {
        float defaultDamage = 10f; // A safe default value

        if (shooterCar == null)
        {
            Debug.LogError("Shooter object is null. Cannot load state data.");
            return defaultDamage;
        }

        // 1. Get and clean the shooting car's name (removing "(Clone)")
        string shooterCarName = shooterCar.name.Replace("(Clone)", "").Trim();

        string filename = GetStateFilename(shooterCarName);

        // 3. Load the state
        PlayerState loadedState = FileHandler.ReadFromJSON<PlayerState>(filename);

        if (loadedState != null)
        {
            // 4. Return the loaded damage
            Debug.Log($"Loaded Damage ({loadedState.Damage}) for shooter: {shooterCarName}");
            return loadedState.Damage;
        }
        else
        {
            Debug.LogWarning($"State file not found for shooter: {shooterCarName}. Using default damage: {defaultDamage}");
            return defaultDamage;
        }
    }
}
