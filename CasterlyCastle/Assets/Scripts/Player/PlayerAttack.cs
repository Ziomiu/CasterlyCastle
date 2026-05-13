using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Melee")]
    public Animator animator;

    public int damage = 25;
    public float range = 1.5f;
    public LayerMask enemyLayer;
    public Transform attackPoint;

    [Header("Fireball")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    public float fireRate = 0.5f;

    private float nextFireTime;

    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Attack.performed += OnAttack;
        controls.Player.Shoot.performed += OnShoot;
    }

    void OnDisable()
    {
        controls.Player.Attack.performed -= OnAttack;
        controls.Player.Shoot.performed -= OnShoot;

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


    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.time < nextFireTime)
            return;

        ShootFireball();

        nextFireTime = Time.time + fireRate;
    }

    private void ShootFireball()
{
    GameObject fireball = Instantiate(
        fireballPrefab,
        firePoint.position,
        Quaternion.identity
    );

    Fireball fireballScript =
        fireball.GetComponent<Fireball>();

    Vector3 mousePosition =
        Mouse.current.position.ReadValue();

    mousePosition.z = 10f;

    mousePosition =
        Camera.main.ScreenToWorldPoint(mousePosition);

    Vector3 direction =
        (mousePosition - firePoint.position).normalized;

    fireballScript.Initialize(direction);

    float angle =
        Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    fireball.transform.rotation =
        Quaternion.Euler(0, 0, angle);
}
}