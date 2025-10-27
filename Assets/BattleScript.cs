using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.GraphicsBuffer;

public class BattleScript : MonoBehaviour
{
    [Tooltip("The spawn point for the first selected car (BattleCarVar).")]
    public Transform carSpawnPoint1;

    [Tooltip("The spawn point for the second selected car (BattleCarVar2).")]
    public Transform carSpawnPoint2;

    [Header("HealthBar References")]
    public HealthBar healthBar1; 
    public HealthBar healthBar2;


    private GameObject instantiatedCar1;
    private GameObject instantiatedCar2;

    private AsyncOperationHandle<GameObject> carHandle1;
    private AsyncOperationHandle<GameObject> carHandle2;

    public void Awake()
    {
        // Retrieve the car names early
        string carName1 = PlayerPrefs.GetString("BattleCarVar", "");
        string carName2 = PlayerPrefs.GetString("BattleCarVar2", "");

        if (!string.IsNullOrEmpty(carName1))
        {
            // 1. START LOADING the assets as soon as the script awakens!
            // Do NOT use .Completed += here, just start the load operation.
            carHandle1 = Addressables.LoadAssetAsync<GameObject>(carName1);
        }
        if (!string.IsNullOrEmpty(carName2))
        {
            carHandle2 = Addressables.LoadAssetAsync<GameObject>(carName2);
        }
    }

    public void BattleBegin()
    {
        // The assets are already loaded (or nearly loaded) by the time this runs.

        // Check for success/completion of the handles and immediately process them.
        // The load operation has been running in the background since Awake().

        if (carHandle1.IsValid() && carHandle1.IsDone)
        {
            OnCarPrefabLoaded(carHandle1, carSpawnPoint1, PlayerPrefs.GetString("BattleCarVar", ""));
        }
        else
        {
            // Handle error or wait if somehow not done (unlikely if called after a countdown)
            Debug.LogError("Car 1 asset was not ready!");
        }

        if (carHandle2.IsValid() && carHandle2.IsDone)
        {
            OnCarPrefabLoaded(carHandle2, carSpawnPoint2, PlayerPrefs.GetString("BattleCarVar2", ""));
        }
        else
        {
            Debug.LogError("Car 2 asset was not ready!");
        }
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
            HealthBar assignedHealthBar = null;

            if (spawnPoint == carSpawnPoint1)
            {
                instantiatedCar1 = instantiatedCar;
                assignedHealthBar = healthBar1; // Get the reference for Car 1
            }
            else if (spawnPoint == carSpawnPoint2)
            {
                instantiatedCar2 = instantiatedCar;
                assignedHealthBar = healthBar2; // Get the reference for Car 2
            }
            ConfigureInstantiatedCar(instantiatedCar, assignedHealthBar);
            if (spawnPoint == carSpawnPoint1)
            {
                instantiatedCar1 = instantiatedCar;
            }
            else if (spawnPoint == carSpawnPoint2)
            {
                instantiatedCar2 = instantiatedCar;
            }

            if (instantiatedCar1 != null && instantiatedCar2 != null)
            {
                AssignTargets();
            }
        }
        else
        {
            Debug.LogError($"Failed to load car prefab ({carName}) from Addressables. Error: {handle.OperationException}");
        }
    }
    private void AssignTargets()
    {
        // Get the BattleAI components from the two instantiated cars
        BattleAI ai1 = instantiatedCar1.GetComponent<BattleAI>();
        BattleAI ai2 = instantiatedCar2.GetComponent<BattleAI>();

        if (ai1 != null && ai2 != null)
        {
            // Car 1's AI targets Car 2
            ai1.target = instantiatedCar2.transform;
            Debug.Log($"Car 1 ({instantiatedCar1.name}) target set to Car 2 ({instantiatedCar2.name}).");

            // Car 2's AI targets Car 1
            ai2.target = instantiatedCar1.transform;
            Debug.Log($"Car 2 ({instantiatedCar2.name}) target set to Car 1 ({instantiatedCar1.name}).");
            SetShooterTarget(instantiatedCar1, instantiatedCar2.transform);
            SetShooterTarget(instantiatedCar2, instantiatedCar1.transform);
        }
        else
        {
            Debug.LogError("Missing BattleAI component on one or both instantiated cars. Target assignment failed.");
        }
    }
    private void SetShooterTarget(GameObject car, Transform target)
    {
        Transform weaponMount = car.transform.Find("WeaponMount");
        if (weaponMount != null)
        {
            foreach (Transform child in weaponMount)
            {
                ShootingScript shooter = child.GetComponent<ShootingScript>();
                if (shooter != null)
                {
                    // Set the currentTarget for the weapon
                    shooter.currentTarget = target;
                    Debug.Log($"Shooter on {car.name} target set to {target.name}.");
                }
            }
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


    private void ConfigureInstantiatedCar(GameObject car, HealthBar healthBarRef)
    {
        // 1. Destroy the existing generic Agent component (if it exists)
        Agent existingAgent = car.GetComponent<Agent>();
        if (existingAgent != null)
        {
            Destroy(existingAgent);
            Debug.Log($"Removed generic Agent from {car.name}.");
        }

        BattleAI aiComponent = car.AddComponent<BattleAI>();

        if (healthBarRef != null)
        {
            aiComponent.healthBar = healthBarRef;
            Debug.Log($"Assigned HealthBar to BattleAI on {car.name}.");
        }
        else
        {
            Debug.LogWarning($"HealthBar reference is missing for {car.name}. Please assign it in the Inspector.");
        }
        Rotate rotateScript = car.GetComponent<Rotate>();
        if (rotateScript != null)
        {
            rotateScript.enabled = false;
            Debug.Log($"Rotate script disabled on {car.name}.");
        }

        Transform weaponMount = car.transform.Find("WeaponMount");
        if (weaponMount != null)
        {
            foreach (Transform child in weaponMount)
            {
                    child.gameObject.AddComponent<ShootingScript>();
                    Debug.Log($"Added ShootingScript to {child.name} on {car.name}.");
            }
        }
    }
}
