using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.GraphicsBuffer;

public class ShootingScript : MonoBehaviour
{
    public string bulletPrefabAddress = "Assets/BulletPrefab.prefab";
    private GameObject loadedBulletPrefab;
    private bool isPrefabLoaded = false;
    private AsyncOperationHandle<GameObject> opHandle;
    public float rotationSpeed = 5f; // Control how fast the turret turns
    public float fireRate = 1.0f; // Fire a bullet every 1 second
    private float nextFireTime = 0f;
    public float currentRepeatRate = 1f;
    public Transform currentTarget;
    void AssignTarget()
    {
        Transform ownerPlayerTransform = transform.parent?.parent;

        // Safety check to ensure the parent structure exists
        if (ownerPlayerTransform == null)
        {
            Debug.LogError("The Gun script is not parented to the Player correctly (Expected Gun -> Hand -> Player). Cannot determine owner player.");
            return;
        }

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        Debug.Log($"Found {allPlayers.Length} total players tagged 'Player'.");

        currentTarget = null;

        foreach (GameObject p in allPlayers)
        {
            if (p != null && p.transform != ownerPlayerTransform)
            {
                currentTarget = p.transform; // Set the target
                Debug.Log($"SUCCESS! Target set to: {currentTarget.name}");
                break; // Found the target, exit the loop
            }
            else if (p != null)
            {
                Debug.Log($"Skipped owner player: {p.name}");
            }
        }

        if (currentTarget == null)
        {
            Debug.LogWarning("No *other* player found in the scene to target!");
        }
    }
    void Start()
    {
        AssignTarget();

        Debug.Log(currentTarget+"Thisdowaidoaw");
        if (!string.IsNullOrEmpty(bulletPrefabAddress))
        {
            opHandle = Addressables.LoadAssetAsync<GameObject>(bulletPrefabAddress);

            opHandle.Completed += OnPrefabLoaded;
        }
        else
        {
            Debug.LogError("Bullet Prefab Address is not set!");
        }
        InvokeRepeating("Spawn", currentRepeatRate, currentRepeatRate);
    }
    void RotateToTargetYAxis()
    {
        Vector3 directionToTarget = currentTarget.position - transform.position;

        directionToTarget.y = 0;

        if (directionToTarget.sqrMagnitude == 0) return;

        Quaternion baseRotation = Quaternion.LookRotation(directionToTarget);
        Quaternion offsetRotation = Quaternion.Euler(0, 180, 0);
        Quaternion targetRotation = baseRotation * offsetRotation;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
    void Update()
    {
        if (currentTarget != null)
        {
            RotateToTargetYAxis();
        }
    }
    private void OnPrefabLoaded(AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            loadedBulletPrefab = obj.Result;
            isPrefabLoaded = true;
            Debug.Log("Bullet Prefab loaded successfully!");
        }
        else
        {
            Debug.LogError("Failed to load Bullet Prefab at address: " + bulletPrefabAddress);
        }
    }
    public void UpdateFireRate(float newRate)
    {
        Debug.Log("IDK this is happening yk");
        currentRepeatRate = newRate;
        CancelInvoke("Spawn");
        InvokeRepeating("Spawn", currentRepeatRate, currentRepeatRate);
    }
    private void Spawn()
    {
       nextFireTime = Time.time + fireRate;

       Debug.Log("Shooting");
       GameObject bullet = Instantiate(loadedBulletPrefab, transform.position, transform.rotation);

       Rigidbody rb = bullet.GetComponent<Rigidbody>();
       if (rb != null)
        {
            rb.AddForce(-bullet.transform.forward * 1000f);
        }
    }

    /*private void Update()
    {
        if (isPrefabLoaded && Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            Debug.Log("Shooting");
            GameObject bullet = Instantiate(loadedBulletPrefab, transform.position, transform.rotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(-bullet.transform.forward * 1000f);
            }
        }
    }
    */
    
    void OnDestroy()
    {
        if (opHandle.IsValid())
        {
            Addressables.Release(opHandle);
        }
    }
}
