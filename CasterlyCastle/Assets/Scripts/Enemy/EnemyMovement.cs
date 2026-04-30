using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Targets")]
    public Transform defaultTarget; 
    public Transform player;         

    private NavMeshAgent agent;
    private Animator animator;
    private Enemy enemy;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed    = 10f;
    [SerializeField] private float movementThreshold = 0.1f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange    = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    public enum EnemyState { Default, Chase, Attack }
    private EnemyState currentState = EnemyState.Default;

    private float lastAttackTime = -Mathf.Infinity;

    void Awake()
    {
        agent    = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        enemy    = GetComponent<Enemy>();

        agent.updateRotation = false;
    }



    void Update()
    {
        if (enemy == null || enemy.IsDead) return;

        UpdateState();
        ExecuteState();

        HandleRotation();
        HandleAnimation();
    }

    private void UpdateState()
    {
        float distToPlayer = player != null
            ? Vector3.Distance(transform.position, player.position)
            : Mathf.Infinity;

        switch (currentState)
        {
            case EnemyState.Default:
                if (distToPlayer <= detectionRange)
                    TransitionTo(EnemyState.Chase);
                break;

            case EnemyState.Chase:
                if (distToPlayer > detectionRange)
                    TransitionTo(EnemyState.Default);
                else if (distToPlayer <= attackRange)
                    TransitionTo(EnemyState.Attack);
                break;

            case EnemyState.Attack:
                if (distToPlayer > attackRange)
                    TransitionTo(EnemyState.Chase);
                break;

        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Default:
                if (defaultTarget != null)
                    agent.SetDestination(defaultTarget.position);
                else
                    agent.ResetPath();
                break;

            case EnemyState.Chase:
                if (player != null)
                    agent.SetDestination(player.position);
                break;

            case EnemyState.Attack:
                agent.ResetPath();
                FaceTarget(player);

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    animator?.SetTrigger("Attack");
                }
                break;

        }
    }

    private void TransitionTo(EnemyState newState)
    {
        currentState = newState;
    }

    public void OnAttackHit()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
        {
            var playerHealth = player.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(enemy.attackDamage);
        }
    }

    private void HandleRotation()
    {
        Vector3 direction = currentState == EnemyState.Attack && player != null
            ? (player.position - transform.position)
            : agent.velocity;

        if (direction.magnitude > 0.1f)
        {
            direction.y = 0f;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void FaceTarget(Transform t)
    {
        if (t == null) return;
        Vector3 dir = (t.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                rotationSpeed * Time.deltaTime
            );
    }

    private void HandleAnimation()
    {
        if (animator == null) return;

        float speed = agent.velocity.magnitude;
        animator.SetBool("isMoving",  speed > movementThreshold);
    }

}