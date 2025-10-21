using UnityEngine;

public class Battle_BotDamageScript : MonoBehaviour
{
    private float totalDamageTaken = 0f;
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the colliding object is a bullet.
        //    (Assumes your bullet GameObject is T A G G E D as "Bullet")
        if (collision.gameObject.CompareTag("Bullet"))
        {
            float damageAmount = 10f; // Set a fixed damage value for simplicity

            // 2. Apply the damage and log the result
            totalDamageTaken += damageAmount;
            Debug.Log($"{gameObject.name} hit by a bullet! Damage taken: {damageAmount}. Total damage: {totalDamageTaken}");

            // 3. Destroy the bullet after impact
            Destroy(collision.gameObject);
        }
    }
}
