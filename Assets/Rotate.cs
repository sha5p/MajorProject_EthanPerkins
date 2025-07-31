using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector3 rotatioin = new Vector3(0, 100, 0);
    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotatioin *1* Time.deltaTime);
    }
}
