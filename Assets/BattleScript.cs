using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleScript : MonoBehaviour
{
    [Tooltip("The spawn point for the first selected car (BattleCarVar).")]
    public Transform carSpawnPoint1;

    [Tooltip("The spawn point for the second selected car (BattleCarVar2).")]
    public Transform carSpawnPoint2;
    private float totalDamageTaken = 0f;
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the colliding object is a bullet.
        //    (Assumes your bullet GameObject is T A G G E D as "Bullet")
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float damageAmount = 10f; // Set a fixed damage value for simplicity

            // 2. Apply the damage and log the result
            totalDamageTaken += damageAmount;
            Debug.Log($"{gameObject.name} hit by a bullet! Damage taken: {damageAmount}. Total damage: {totalDamageTaken}");

            // 3. Destroy the bullet after impact
            Destroy(collision.gameObject);
        }
    }
    void Awake()
    {
        string carName1 = PlayerPrefs.GetString("BattleCarVar", "");
        string carName2 = PlayerPrefs.GetString("BattleCarVar2", "");

        if (string.IsNullOrEmpty(carName1) || string.IsNullOrEmpty(carName2))
        {
            if (string.IsNullOrEmpty(carName1)) Debug.LogError("Missing car for Player 1 (BattleCarVar).");
            if (string.IsNullOrEmpty(carName2)) Debug.LogError("Missing car for Player 2 (BattleCarVar2).");
            return;
        }

        Debug.Log($"Car 1: {carName1}. Car 2: {carName2}. Starting asynchronous asset loading.");

        Addressables.LoadAssetAsync<GameObject>(carName1).Completed += (handle) =>
        {
            OnCarPrefabLoaded(handle, carSpawnPoint1, carName1);
        };

        Addressables.LoadAssetAsync<GameObject>(carName2).Completed += (handle) =>
        {
            OnCarPrefabLoaded(handle, carSpawnPoint2, carName2);
        };
    }
    private void OnCarPrefabLoaded(AsyncOperationHandle<GameObject> handle, Transform spawnPoint, string carName)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject loadedCarPrefab = handle.Result;

            if (loadedCarPrefab == null)
            {
                Debug.LogError($"Loaded prefab for {carName} is null. Check Addressables group configuration.");
                return;
            }

            // Instantiate the car at the correct spawn point
            GameObject instantiatedCar = InstantiateCar(loadedCarPrefab, spawnPoint);

            // Apply the ML-Agents/Battle components setup (identical to your original logic)
            ConfigureInstantiatedCar(instantiatedCar);
        }
        else
        {
            Debug.LogError($"Failed to load car prefab ({carName}) from Addressables. Error: {handle.OperationException}");
        }
    }


    private GameObject InstantiateCar(GameObject prefab, Transform spawnPoint)
    {
        GameObject instantiatedCar;

        if (spawnPoint != null)
        {
            instantiatedCar = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation,
                spawnPoint.parent
            );
        }
        else
        {
            // Fallback for safety, though spawn points should be assigned in a battle scene
            instantiatedCar = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Debug.LogWarning($"Spawn Point was not assigned for {prefab.name}. Spawning at (0,0,0).");
        }

        return instantiatedCar;
    }


    private void ConfigureInstantiatedCar(GameObject car)
    {
        // 1. Destroy the existing generic Agent component (if it exists)
        Agent existingAgent = car.GetComponent<Agent>();
        if (existingAgent != null)
        {
            Destroy(existingAgent);
            Debug.Log($"Removed generic Agent from {car.name}.");
        }

        car.AddComponent<BattleAI>();

        Rotate rotateScript = car.GetComponent<Rotate>();
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
            Debug.Log($"Rotate script disabled on {car.name}.");
        }

        // 4. Find weapon mounts and add ShootingScript to blasters
        Transform weaponMount = car.transform.Find("WeaponMount");
        if (weaponMount != null)
        {
            foreach (Transform child in weaponMount)
            {
                if (child.name.Contains("blaster"))
                {
                    child.gameObject.AddComponent<ShootingScript>();
                    Debug.Log($"Added ShootingScript to {child.name} on {car.name}.");
                }
            }
        }
    }
}
