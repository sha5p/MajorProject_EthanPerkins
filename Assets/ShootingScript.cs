using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public GameObject bulletprefab;
    public Transform shootingPoint;

    void Start()
    {

        GameObject prefabToLoad = Resources.Load<GameObject>("BulletPrefab");

        if (prefabToLoad != null)
        {
            // Instantiate the prefab
            GameObject bulletprefab = prefabToLoad;

        }
        else
        {
            Debug.LogError("Prefab 'MyPrefab' not found in Resources folder. Make sure the name and path are correct.");
        }
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            GameObject bullet = Instantiate(bulletprefab, shootingPoint.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward); 
        }
    }
}
