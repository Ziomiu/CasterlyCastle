using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;
    private Animator animator;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float movementThreshold = 0.1f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // we control rotation manually
        agent.updateRotation = false;
    }

    void Update()
    {
        if (target == null) return;

        // Move toward target
        agent.SetDestination(target.position);

        HandleRotation();
        HandleAnimation();
    }

    private void HandleRotation()
    {
        Vector3 direction = agent.velocity;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void HandleAnimation()
    {
        if (animator == null) return;

        float speed = agent.velocity.magnitude;


        // optional fallback if using bool:
        animator.SetBool("isMoving", speed > movementThreshold);
    }
}