using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class InitalTraining : MonoBehaviour
{
    public Transform carSpawnPoint;

    void Awake()
    {
        string carName = PlayerPrefs.GetString("SelectedCarName", "");

        if (string.IsNullOrEmpty(carName))
        {
            Debug.LogWarning("No car was selected in the menu. Cannot spawn a car.");
            return;
        }

        Debug.Log($"Selected car name found: {carName}. Loading asset from Addressables...");

        Addressables.LoadAssetAsync<GameObject>(carName).Completed += OnCarPrefabLoaded;
    }

    private void OnCarPrefabLoaded(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedCarPrefab = handle.Result;
            Debug.Log($"Successfully loaded {loadedCarPrefab.name}. Instantiating now.");

            GameObject instantiatedCar;

            if (carSpawnPoint != null)
            {
                instantiatedCar = Instantiate(loadedCarPrefab, carSpawnPoint.position, carSpawnPoint.rotation);
            }
            else
            {
                // Capture the instantiated object here as well
                instantiatedCar = Instantiate(loadedCarPrefab, Vector3.zero, Quaternion.identity);
                Debug.LogWarning("Car Spawn Point was not assigned. Spawning at (0,0,0).");
            }

            // Add the MeshCollider to the instantiated car
            instantiatedCar.AddComponent<Rigidbody>();
            instantiatedCar.AddComponent<BoxCollider>();
            Debug.Log("MeshCollider added to the new car.");

            Rotate rotateScript = instantiatedCar.GetComponent<Rotate>();
            BattleAI battleScript = instantiatedCar.AddComponent<BattleAI>();
            GameObject myObject = GameObject.Find("WeaponMount");
            foreach (Transform childTransform in myObject.transform)
            {
                Debug.Log($"IDK somthing to identify {childTransform.name}");
                if (childTransform.name.Contains("blaster"))
                {
                    ShootingScript ShootingScript = childTransform.AddComponent<ShootingScript>();
                }
            }


            if (rotateScript != null)
            {
                rotateScript.enabled = false;
                Debug.Log("Rotate script disabled on the new car.");
            }
            else
            {
                Debug.LogWarning("Rotate script not found on the instantiated car.");
            }
        }
        else
        {
            Debug.LogError($"Failed to load car prefab from Addressables. Error: {handle.OperationException}");
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
}
