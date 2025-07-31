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
    public void Start()
    {
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
        if(parentName.Contains("Body") && BodyPrefabs.Count > 0)
        {
            selectedAssetRef = BodyPrefabs[0];
            CarVechical = true;
            Debug.Log("TESTING");
        }
        else if (parentName.Contains("Wheels") && WheelPrefabs.Count > 0)
        {
            selectedAssetRef = WheelPrefabs[0];
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
            selectedAssetRef = BodyPrefabs[0];
            CarVechical = true;
            Debug.Log("TESTING");
        }
        else if (parentName.Contains("Wheels") && WheelPrefabs.Count > 0)
        {
            selectedAssetRef = WheelPrefabs[0];
        }
        else if (parentName.Contains("Weapon") && WeaponPrefabs.Count > 0)
        {
            selectedAssetRef = WeaponPrefabs[0];
        }
        this.prefabReference = selectedAssetRef;
        InstantiateFromAssetReference();
    }
    void InstantiateFromAssetReference()
    {
        if (prefabReference != null)
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
                        List<Transform> childrenToMove = new List<Transform>();
                        foreach (Transform child in Car.transform)
                        {
                            childrenToMove.Add(child);
                        }

                        foreach (Transform child in childrenToMove)
                        {
                            child.SetParent(instantiatedPrefab.transform, true); // true maintains local position/rotation/scale

                            Debug.Log($"Moved child '{child.name}' from Car to '{instantiatedPrefab.name}'");
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
        else
        {
            Debug.LogError("Prefab Reference is not assigned!");
        }
    }
}
