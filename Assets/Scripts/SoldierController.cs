using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SoldierController : MonoBehaviour
{
    public Transform[] waypoints;
    public float chaseRange = 8f; // distance to start chasing
    public float attackRange = 2f; // melee range
    public int health = 50;
    readonly int maxHealth = 50;
    [SerializeField] SoldierHealthBar healthbar;
    public int damage = 70;
    public float attackCooldown = 0.5f;
    NavMeshAgent agent;
    Transform player;
    int currentWP = 0;
    float lastAttackTime;
    enum State { Patrol, Chase, Attack }
    State state = State.Patrol;
    public GameObject runePrefab;
    private Animator animator;
    private bool isDead = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[0].position);
        }

        healthbar.UpdateHealthBar(maxHealth, health);
    }

    void Update()
    {
        if (isDead) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // State transitions
        if (distToPlayer <= attackRange)
            state = State.Attack;
        else if (distToPlayer <= chaseRange)
            state = State.Chase;
        else
            state = State.Patrol;

        // State behaviors
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
            case State.Attack:
                Attack();
                break;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;

        // If reached waypoint, go to next
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWP = (currentWP + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWP].position);
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (agent.velocity.magnitude > 0.1f) {
            agent.isStopped = false;
            return;
        }

        agent.isStopped = true;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attacking");

            // Face the player
            Vector3 dir = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(dir);

            // Atack player
            if (player.TryGetComponent<PlayerHealth>(out var ph))
                ph.TakeDamage(damage);

            lastAttackTime = Time.time;

            StartCoroutine(ResumeMovementAfterAttack(2.3f));
        }
    }

    IEnumerator ResumeMovementAfterAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.isStopped = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;
        animator.SetTrigger("Hit");
        StartCoroutine(HitReaction());
        healthbar.UpdateHealthBar(maxHealth, health);

        if (health <= 0)
        {
            Die();
        }
    }

    IEnumerator HitReaction()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(2f);
        agent.isStopped = false;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (runePrefab != null)
        {
            Instantiate(runePrefab, transform.position + Vector3.up * 0.05f, Quaternion.Euler(90f, 90f, 0f));
        }
        FindFirstObjectByType<PlayerUI>().AddLiberatedSoul();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Disable NavMeshAgent and Collider
        if (agent != null)
        {
            agent.enabled = false;
        }
        // Disable Collider
        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = false;
        }

        if (healthbar != null)
        {
            healthbar.Hide();
        }
    }
}
