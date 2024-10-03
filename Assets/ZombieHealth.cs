using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log($"Zombie took {damage} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger death animation, destroy the zombie, or deactivate it
        Debug.Log("Zombie died.");
        Destroy(gameObject); // Or deactivate for object pooling
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Assuming bullets have a tag "Bullet" and deal damage
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage); // Apply bullet's damage
                Destroy(other.gameObject); // Destroy the bullet after impact
            }
        }
    }
}
