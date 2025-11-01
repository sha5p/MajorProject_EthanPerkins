using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject owner;

    void Start()
    {
        // Get components in Start (or Awake) for efficiency

        // Original logic: destroy after 2 seconds
        Destroy(gameObject, 1.5f);
    }
}
