using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth   = 100;
    public int currentHealth;
    public bool IsDead { get; private set; }

    [Header("Combat")]
    public int attackDamage = 10;


    void Start()
    {
        currentHealth = maxHealth;
        IsDead        = false;
    }

    public void TakeDamage(int dmg)
    {
        if (IsDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        Destroy(gameObject);
    }
}