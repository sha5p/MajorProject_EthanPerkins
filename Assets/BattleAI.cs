using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class BattleAI : Agent
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float rotationSpeed = 10f;
    public float velocitySmooth = 0.1f;
    public float strafeMagnitude = 2f; // how far left/right the agent strafes
    public float strafeSpeed = 2f;     // how fast it oscillates left/right

    [Header("Target")]
    public Transform target;

    [Header("Distance Settings")]
    public float minDistance = 3f;
    public float maxDistance = 5f;

    private Rigidbody rb;
    private const float maxEnvDistance = 10f;
    private Vector3 previousMove;
    public HealthBar healthBar;

    public float TotalHealth = 100f;
    public void SaveCurrentDistance()
    {
        // 1. Determine file and car ID
        string objectName = this.gameObject.name;
        string baseName = objectName.Replace("(Clone)", "").Trim();
        string carNumber = baseName;
        string filename = GetStateFilename(carNumber);

        PlayerState loadedState = FileHandler.ReadFromJSON<PlayerState>(filename);

        loadedState.Distance = baseDistance;

        FileHandler.SaveToJSON(loadedState, filename);

        Debug.Log($"Saved new distance for Car {carNumber}: {baseDistance} to {filename}");
    }
    private string GetStateFilename(string carNumber)
    {
        return "PlayerState" + carNumber + ".json";
    }
    public float GetCarDistance(GameObject carObject)
    {
        string objectName = carObject.name;
        string baseName = objectName.Replace("(Clone)", "").Trim(); // Trim removes any extra spaces
        string carNumber = baseName;
        Debug.Log(carNumber+"This is the car number");
        string filename = GetStateFilename(carNumber);
        PlayerState loadedState = FileHandler.ReadFromJSON<PlayerState>(filename);

        if (loadedState != null)
        {
            float distance = loadedState.Distance;
            Debug.Log($"Loaded distance for Car {carNumber}: {distance}");
            return distance;
        }
        else
        {
            Debug.LogError($"Could not load data for car number: {carNumber}. Check if the file '{filename}' exists in the persistent data path.");
            return 0f; // Return 0 or another default/error value
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the colliding object is a bullet.
        //    (Assumes your bullet GameObject is T A G G E D as "Bullet")
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            float damageAmount = 10f; // Set a fixed damage value for simplicity
            if (bullet != null && bullet.owner == gameObject)
            {
                // Friendly fire detected.
                Debug.Log($"{gameObject.name} hit by own bullet. Ignoring damage. This is the other script comparer name"+bullet.owner);
                return;
            }
            else
            {
                TotalHealth -= damageAmount;
                healthBar.SetHeaHealth(TotalHealth);
            }

        }
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        MaxStep = 0;

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
                Debug.LogWarning($"{name} could not find a Player-tagged object in its environment.");
        }
    }
    public float baseDistance = 4f;
    public override void OnEpisodeBegin()
    {
        ResetAgent();
        //TotalHealth = 100f; // Reset health (if not done in ResetAgent)

        // 2. Load the saved distance from the JSON file

        // A. Use 'this.gameObject' because the BattleAI script is attached to the car itself
        float savedDistance = GetCarDistance(this.gameObject);

        // B. Use the loaded value to set the agent's desired distance range
        if (savedDistance > 0f)
        {
            // Use the loaded distance to set the base
            SetDistance(savedDistance);
            Debug.Log($"Loaded and applied base distance: {savedDistance}");
        }
        else
        {
            // Fallback to the default value if the file was not found
            SetDistance(baseDistance); // baseDistance is currently 4f in your script
            Debug.Log($"Using default base distance: {baseDistance}");
        }

    }
    public void SetDistance(float distance)
    {
        baseDistance = distance;
        minDistance = distance;
        maxDistance = distance + 2f;
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

        Vector3 relativePos = (transform.position - target.position) / maxEnvDistance;
        float distance = Vector3.Distance(transform.position, target.position) / maxEnvDistance;

        sensor.AddObservation(relativePos);
        sensor.AddObservation(distance);
        sensor.AddObservation(minDistance / maxEnvDistance);
        sensor.AddObservation(maxDistance / maxEnvDistance);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (target == null) return;

        float moveZ = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float moveX = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 move = new Vector3(moveX, 0f, moveZ);
        if (move.magnitude > 1f) move.Normalize();
        move *= speed;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        float distanceError = 0f;

        if (distanceToTarget < minDistance)
        {
            distanceError = minDistance - distanceToTarget; // push away if too close
            move -= dirToTarget * distanceError * speed;   // scale by speed
        }
        else if (distanceToTarget > maxDistance)
        {
            distanceError = distanceToTarget - maxDistance; // pull closer if too far
            move += dirToTarget * distanceError * speed;    // scale by speed
        }

        // Strafing inside the range
        if (distanceToTarget >= minDistance && distanceToTarget <= maxDistance)
        {
            Vector3 lateral = Vector3.Cross(Vector3.up, dirToTarget);
            float strafeDir = Mathf.Sin(Time.time * strafeSpeed);
            move += lateral * strafeDir * strafeMagnitude;
        }

        Vector3 targetVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, velocitySmooth);

        Vector3 smoothMove = Vector3.Lerp(previousMove, move, velocitySmooth);
        if (smoothMove.magnitude > 0.05f)
        {
            Quaternion targetRot = Quaternion.LookRotation(smoothMove, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
        previousMove = smoothMove;

        float idealMid = (minDistance + maxDistance) / 2f;
        float deviation = Mathf.Abs(distanceToTarget - idealMid);

        // Reward for staying near ideal distance
        AddReward(Mathf.Max(0f, 0.01f - 0.001f * deviation));
        if (distanceToTarget >= minDistance && distanceToTarget <= maxDistance)
            AddReward(0.01f);

        // Penalize idleness
        if (rb.linearVelocity.magnitude < 0.1f && (distanceToTarget < minDistance || distanceToTarget > maxDistance))
            AddReward(-0.005f);

        // === Circling reward: reward lateral motion around target ===
        if (distanceToTarget >= minDistance && distanceToTarget <= maxDistance)
        {
            Vector3 toTarget = (target.position - transform.position).normalized;
            Vector3 lateralDir = Vector3.Cross(Vector3.up, toTarget); // perpendicular
            float lateralVel = Vector3.Dot(rb.linearVelocity, lateralDir);  // velocity along lateral
            AddReward(lateralVel * 0.01f);                          // reward strafing sideways
        }

        // End episode if extremely far away
        if (distanceToTarget > maxDistance * 3f)
        {
            AddReward(-1f);
            //EndEpisode();
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
