using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;

    public int damage = 25;
    public float range = 1.5f;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Player.Attack.performed += OnAttack;
    }

    void OnDisable()
    {
        controls.Player.Attack.performed -= OnAttack;
        controls.Disable();
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            range,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}