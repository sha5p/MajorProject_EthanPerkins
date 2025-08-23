using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Battle_Deisgn : MonoBehaviour
{
    [Header("Buttons Options")]
    public List<Button> ForwardButton = new List<Button>();
    public List<Button> BackWardsButton = new List<Button>();
    public GameObject Car;

    [Header("Design Option Prefabs")]
    public List<AssetReference> BodyPrefabs = new List<AssetReference>();
    public List<AssetReference> WheelPrefabs = new List<AssetReference>();
    public List<AssetReference> WeaponPrefabs = new List<AssetReference>();

    [SerializeField]
    private string prefabPath = "Assets/Design_Options/Body/1.prefab";

    [SerializeField]
    private AssetReference prefabReference;

    public bool CarVechical;
    public bool WheelVechical;

    public Dictionary<string, string> wheelToNewParentChildMap = new Dictionary<string, string>()
    {
        {"wheel-bl", "Wheel_RearLeft"}, 
        {"wheel-br", "Wheel_RearRight"},
        {"wheel-fl", "Wheel_FrontLeft"},
        {"wheel-fr", "Wheel_FrontRight"}
    };
    private const string Car_Number = "Car#";
    public void Start()
    {
        if (PlayerPrefs.HasKey(Car_Number))
        {
            PlayerPrefs.SetInt(Car_Number, 0);
            PlayerPrefs.Save();
        }
        int loadedNumber = PlayerPrefs.GetInt(Car_Number);


        foreach (Button button in ForwardButton)
        {
            button.onClick.AddListener(() => OnForwardButtonClicked(button));
        }
        foreach (Button button in BackWardsButton)
        {
            button.onClick.AddListener(() => OnBackWardsButton(button));
        }

    }
    void OnBackWardsButton(Button clickedButton)
    {
        string parentName = clickedButton.transform.parent.name;
        AssetReference selectedAssetRef = null;
        if (parentName.Contains("Body") && BodyPrefabs.Count > 0)
        {
            int loadedNumber = PlayerPrefs.GetInt(Car_Number);
            selectedAssetRef = BodyPrefabs[loadedNumber];
            PlayerPrefs.SetInt(Car_Number, loadedNumber - 1);
            PlayerPrefs.Save();
            CarVechical = true;
            Debug.Log("TESTING");
        }
        else if (parentName.Contains("Wheels") && WheelPrefabs.Count > 0)
        {
            int currentWheelIndex = PlayerPrefs.GetInt("CurrentWheelIndex", 0);
            currentWheelIndex = (currentWheelIndex - 1 + WheelPrefabs.Count) % WheelPrefabs.Count;
            PlayerPrefs.SetInt("CurrentWheelIndex", currentWheelIndex);
            PlayerPrefs.Save();
            WheelVechical = true;
            selectedAssetRef = WheelPrefabs[currentWheelIndex];
            this.prefabReference = selectedAssetRef;
            InstantiateFromAssetReference();
        }
        else if (parentName.Contains("Weapon") && WeaponPrefabs.Count > 0)
        {
            selectedAssetRef = WeaponPrefabs[0];
        }
        this.prefabReference = selectedAssetRef;
        InstantiateFromAssetReference();
    }
    void OnForwardButtonClicked(Button clickedButton)
    {
        string parentName = clickedButton.transform.parent.name;
        AssetReference selectedAssetRef = null;
        if (parentName.Contains("Body") && BodyPrefabs.Count > 0)
        {
            int loadedNumber = PlayerPrefs.GetInt(Car_Number);
            selectedAssetRef = BodyPrefabs[loadedNumber];
            PlayerPrefs.SetInt(Car_Number, loadedNumber+1);
            PlayerPrefs.Save();
            CarVechical = true;
            Debug.Log("TESTING");
            this.prefabReference = selectedAssetRef;
            InstantiateFromAssetReference();
        }
        else if (parentName.Contains("Wheels") && WheelPrefabs.Count > 0)
        {
            int currentWheelIndex = PlayerPrefs.GetInt("CurrentWheelIndex", 0);
            currentWheelIndex = (currentWheelIndex - 1 + WheelPrefabs.Count) % WheelPrefabs.Count;
            PlayerPrefs.SetInt("CurrentWheelIndex", currentWheelIndex);
            PlayerPrefs.Save();
            WheelVechical = true;
            selectedAssetRef = WheelPrefabs[currentWheelIndex];
            this.prefabReference = selectedAssetRef;
            InstantiateFromAssetReference();
        }
        else if (parentName.Contains("Weapon") && WeaponPrefabs.Count > 0)
        {
            selectedAssetRef = WeaponPrefabs[0];
        }
        
    }
    void InstantiateFromAssetReference()
    {
            if (CarVechical) 
            {
            Vector3 spawnPosition;
            Quaternion spawnRotation;
            if (Car != null)
            {
                spawnPosition = Car.transform.position;
                spawnRotation = Car.transform.rotation;
                Debug.Log($"Spawning at Car's position: {spawnPosition}");
            }
            else
            {
                spawnPosition = transform.position;
                spawnRotation = transform.rotation;
                Debug.Log($"Car is not yet assigned, spawning at script's GameObject position: {spawnPosition}");
            }
            AsyncOperationHandle<GameObject> opHandle = prefabReference.InstantiateAsync(spawnPosition, spawnRotation, null);

            opHandle.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject instantiatedPrefab = handle.Result;

                        Debug.Log($"Successfully instantiated prefab from AssetReference: {instantiatedPrefab.name}");
                        if (Car != null && CarVechical)
                        {
                            List<Transform> allChildrenOfOldCar = new List<Transform>();
                            int bob = 0;
                            foreach (Transform child in Car.transform)
                            {
                                allChildrenOfOldCar.Add(child);
                                Debug.Log("Child name: " + child.name);
                                if (child.name.Contains("Wheel_"))
                                {
                                    Transform grandChildTransform = child.GetChild(0);
                                    if (grandChildTransform.name.Contains("wheel-"))
                                    {
                                        if (instantiatedPrefab.transform.childCount > 0)
                                        {
                                            grandChildTransform.SetParent(instantiatedPrefab.transform.GetChild(bob));
                                            ResetLocalTransform(grandChildTransform);
                                            Debug.Log($"Reparented {grandChildTransform.name} to {instantiatedPrefab.transform.GetChild(bob).name}");
                                            bob += 1;
                                        }
                                    }
                                }
                                if (child.name.Contains("blaster"))
                                {
                                    child.SetParent(instantiatedPrefab.transform);
                                }
                            }

                            Destroy(Car);
                            Car = instantiatedPrefab;
                            CarVechical = false;
                            instantiatedPrefab.AddComponent<Rotate>();
                        }

                    }
                    else
                    {
                        Debug.LogError($"Failed to instantiate prefab from AssetReference: {handle.OperationException}");
                    }

                };
            }
            

            else if (WheelVechical)
            {
                if (Car == null)
                {
                    Debug.LogError("Cannot replace wheels, Car object is not assigned!");
                    WheelVechical = false;
                    return;
                }

                List<AsyncOperationHandle> wheelHandles = new List<AsyncOperationHandle>();

                foreach (Transform child in Car.transform)
                {
                    if (child.name.Contains("Wheel_"))
                    {
                        if (child.childCount > 0)
                        {
                            Destroy(child.GetChild(0).gameObject);
                        }

                        AsyncOperationHandle<GameObject> wheelHandle = prefabReference.InstantiateAsync(child);
                        wheelHandles.Add(wheelHandle);
                        Debug.Log("NO MORE THEN 4");
                    }
                }
                WheelVechical = false;
            }

    }

        
   
    private Transform FindChildByNameRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                return child;
            }
            Transform found = FindChildByNameRecursive(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
    private void ResetLocalTransform(Transform targetTransform)
    {
        if (targetTransform == null)
        {
            Debug.LogWarning("Attempted to reset a null transform.");
            return;
        }

        targetTransform.localPosition = Vector3.zero;
        targetTransform.localRotation = Quaternion.identity;
        targetTransform.localScale = Vector3.one;

    }
}

