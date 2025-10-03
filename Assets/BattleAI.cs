using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BattleAI : Agent
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Target")]
    public Transform target; // Assign target in Inspector

    public override void OnEpisodeBegin()
    {
        ResetAgent();
    }

    private void FixedUpdate()
    {
        // Optional raycast
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
            Debug.Log("Hit: " + hit.collider.name);

        //RequestDecision(); // forces Heuristic to be called every FixedUpdate
    }


    private void ResetAgent()
    {
        // Reset agent position and rotation
        transform.position = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
        transform.rotation = Quaternion.identity;

        // Reset target position
        if (target != null)
            target.position = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveX = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // World-relative movement
        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        if (movement != Vector3.zero)
        {
            transform.Translate(movement * speed * Time.deltaTime, Space.World);
        }

        // Reward for getting closer to the target
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            AddReward(0.01f * (1f / (distanceToTarget + 0.01f)));

            if (distanceToTarget < 1.0f)
            {
                AddReward(1.0f);
                EndEpisode();
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        // Arrow keys: world-relative movement
        continuousActionsOut[0] = Input.GetKey(KeyCode.UpArrow) ? 1f :
                                  Input.GetKey(KeyCode.DownArrow) ? -1f : 0f;

        continuousActionsOut[1] = Input.GetKey(KeyCode.RightArrow) ? 1f :
                                  Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f;

        Debug.Log($"Heuristic called: Forward={continuousActionsOut[0]}, Right={continuousActionsOut[1]}");
    }

}


/*   private Transform FindChildByNameRecursive(Transform parent, string name)
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

*/

/*
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
*/
