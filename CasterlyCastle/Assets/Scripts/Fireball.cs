using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 25;
    public float lifeTime = 5f;
    public GameObject impactEffect;

    private Rigidbody rb;

    private Vector3 moveDirection;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.linearVelocity = moveDirection * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
{
    if (other.gameObject.layer ==
        LayerMask.NameToLayer("Enemy"))
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Instantiate(
            impactEffect,
            transform.position,
            Quaternion.identity
        );

        Destroy(gameObject);
    }
}
}