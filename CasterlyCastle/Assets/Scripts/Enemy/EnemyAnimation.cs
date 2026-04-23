using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    private EnemyMovement enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyMovement>();
    }

    public void OnAttackHit()
    {
        if (enemy != null)
            enemy.OnAttackHit();
    }
}