using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private TextMeshProUGUI healthText;
    private int currentHealth;
    public bool IsDead { get; private set; }

    [Header("Invincibility Frames")]
    [SerializeField] private float invincibilityDuration = 0.5f;
    private float lastHitTime = -Mathf.Infinity;

    public int CurrentHealth => currentHealth;
    public int MaxHealth     => maxHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;

    void Start()
    {
        currentHealth = maxHealth;
        IsDead        = false;
        healthText.text = currentHealth.ToString();

    }

    public void TakeDamage(int dmg)
    {
        if (IsDead) return;
        if (dmg <= 0) return;

        if (Time.time < lastHitTime + invincibilityDuration) return;
        lastHitTime = Time.time;

        int finalDamage = dmg;

        currentHealth = Mathf.Max(currentHealth - finalDamage, 0);
        healthText.text = currentHealth.ToString();

        Debug.Log($"[PlayerHealth] Took {finalDamage} dmg → {currentHealth}/{maxHealth}");

        if (currentHealth == 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthText.text = currentHealth.ToString();


        Debug.Log($"[PlayerHealth] Healed {amount} → {currentHealth}/{maxHealth}");
    }

    public void HealToFull()  => Heal(maxHealth - currentHealth);


    private void Die()
    {
        if (IsDead) return;
        IsDead = true;


        Debug.Log("[PlayerHealth] Player died.");
    }
}