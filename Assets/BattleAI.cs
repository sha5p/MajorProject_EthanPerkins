using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BattleAI : Agent
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;

    [Header("Target")]
    public Transform target; // Assign target in Inspector

    [Header("Distance Settings")]
    public float minDistance = 3f;
    public float maxDistance = 5f;

    private Rigidbody rb;
    private const float maxEnvDistance = 10f; // For normalization and scaling

    // ✅ Called once when the agent is initialized
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        MaxStep = 1000;

        // Auto-find target if not assigned
        if (target == null)
        {
            Transform envParent = transform.parent;
            if (envParent != null)
            {
                foreach (Transform child in envParent)
                {
                    if (child.CompareTag("Player"))
                    {
                        target = child;
                        Debug.Log($"{name} found target: {target.name}");
                        break;
                    }
                }
            }

            if (target == null)
            {
                Debug.LogWarning($"{name} could not find a Player-tagged object in its environment.");
            }
        }
    }

    // ✅ Called every new training episode
    public override void OnEpisodeBegin()
    {
        // Randomize desired range every episode for generalization
        float baseDistance = Random.Range(2f, 6f);
        minDistance = baseDistance;
        maxDistance = baseDistance + 2f;

        ResetAgent();
    }

    private void FixedUpdate()
    {
        RequestDecision();
    }

    private void ResetAgent()
    {
        Transform envParent = transform.parent;
        Transform spawnPoint = envParent != null ? envParent.Find("SpawnPoint") : null;

        if (spawnPoint != null)
        {
            transform.SetParent(envParent);
            transform.localPosition = spawnPoint.localPosition;
            transform.localRotation = spawnPoint.localRotation;
        }
        else
        {
            transform.SetParent(envParent);
            transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
            transform.localRotation = Quaternion.identity;
        }
    }

    // ✅ Observations for the neural network
    public override void CollectObservations(VectorSensor sensor)
    {
        if (target == null)
        {
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            return;
        }

        // Normalize observations for better learning
        Vector3 relativePos = (transform.position - target.position) / maxEnvDistance;
        float distance = Vector3.Distance(transform.position, target.position) / maxEnvDistance;

        sensor.AddObservation(relativePos);     // 3 floats
        sensor.AddObservation(distance);        // 1 float
        sensor.AddObservation(minDistance / maxEnvDistance);
        sensor.AddObservation(maxDistance / maxEnvDistance);
    }

    // ✅ Main decision-making logic
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveZ = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveX = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        // Apply velocity-based movement
        Vector3 move = new Vector3(moveX, 0f, moveZ).normalized * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        // Rotate smoothly towards movement direction
        if (move != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(move, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }

        if (target == null)
        {
            AddReward(-0.001f); // Slight penalty if no target assigned
            return;
        }

        // ✅ Distance-based reward shaping
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float idealMid = (minDistance + maxDistance) / 2f;
        float deviation = Mathf.Abs(distanceToTarget - idealMid);

        // Continuous reward based on closeness to ideal distance
        AddReward(Mathf.Max(0f, 0.01f - 0.001f * deviation));

        // Bonus reward for staying inside the range
        if (distanceToTarget >= minDistance && distanceToTarget <= maxDistance)
        {
            AddReward(0.01f);
        }

        // Penalize and end if far outside desired range
        if (distanceToTarget < minDistance * 0.3f || distanceToTarget > maxDistance * 2f)
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        float moveZ = 0f;
        if (Input.GetKey(KeyCode.UpArrow)) moveZ = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveZ = -1f;

        float moveX = 0f;
        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;

        continuousActionsOut[0] = moveZ;
        continuousActionsOut[1] = moveX;
    }
}
