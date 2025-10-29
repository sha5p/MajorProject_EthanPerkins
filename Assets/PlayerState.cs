[System.Serializable]
public class PlayerState
{
    // The unique ID or name for the saved vehicle/player
    public string CarName;

    // Player's or vehicle's stats
    public float Damage;
    public float Distance;
    public float FiringRate;

    // Constructor to easily create a new state
    public PlayerState(string carName, float damage, float distance, float firingRate)
    {
        CarName = carName;
        Damage = damage;
        Distance = distance;
        FiringRate = firingRate;
    }
}
