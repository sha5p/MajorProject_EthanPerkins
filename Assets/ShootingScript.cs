using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShootingScript : MonoBehaviour
{
    public string bulletPrefabAddress = "Assets/BulletPrefab.prefab";
    private GameObject loadedBulletPrefab;
    private bool isPrefabLoaded = false;
    private AsyncOperationHandle<GameObject> opHandle;

    public float fireRate = 1.0f; // Fire a bullet every 1 second
    private float nextFireTime = 0f;

    void Start()
    {
        if (!string.IsNullOrEmpty(bulletPrefabAddress))
        {
            opHandle = Addressables.LoadAssetAsync<GameObject>(bulletPrefabAddress);

            opHandle.Completed += OnPrefabLoaded;
        }
        else
        {
            Debug.LogError("Bullet Prefab Address is not set!");
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

    private void Update()
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

    void OnDestroy()
    {
        if (opHandle.IsValid())
        {
            Addressables.Release(opHandle);
        }
    }
}
