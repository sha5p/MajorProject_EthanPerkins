using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class BattleAI : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement += Vector3.back;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement += Vector3.right;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement += Vector3.left;
        }

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);
    }
    public void Start()
    {
        

      
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
