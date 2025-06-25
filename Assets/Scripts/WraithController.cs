using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AshWraithController : MonoBehaviour
{
    [Header("Health & Damage")]
    public int health = 50;
    readonly int maxHealth = 50;
    [SerializeField] WraithHealthBar healthbar;

    [Header("AI Ranges (meters)")]
    public float patrolRadius = 15f;  // beyond this it just patrols
    public float chaseRange = 12f;  // when player < this, start chasing
    public float stopDistance =  10f;  // when player < this, stop and attack
    public float retreatDistance =  2.5f;  // when player < this, back away
    [Header("Attack")]
    public float attackCooldown = 2f;
    public int projectileDamage = 10;
    public GameObject projectilePrefab;
    public GameObject runePrefab;
    public Transform firePoint;
    public AudioClip attackSound;
    [Header("Optional Patrol Points")]
    public Transform[] patrolPoints;
    NavMeshAgent agent;
    Transform player;
    AudioSource audioSource;
    readonly Renderer rend;
    float lastAttackTime;
    int currentPatrol = 0;
    enum State { Idle, Patrol, Chase, Attack, Retreat }
    State state = State.Patrol;
    State previousState = State.Patrol;
    Animator animator;
    private bool isDead = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        healthbar.UpdateHealthBar(maxHealth, health);

        if (patrolPoints != null && patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
        else
            state = State.Idle;
    }

    void Update()
    {
        if (isDead) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // Retreat if too close
        if (dist < retreatDistance)
            state = State.Retreat;

        // Attack if in attack range
        else if (dist <= stopDistance)
            state = State.Attack;

        // Chase if within chase range but outside stop distance
        else if (dist <= chaseRange)
            state = State.Chase;

        // Patrol if beyond chase range but have points
        else if (patrolPoints != null && patrolPoints.Length > 0)
            state = State.Patrol;

        // Idle otherwise
        else
            state = State.Idle;

        animator.SetFloat("Speed", agent.velocity.magnitude);

        // execute behavior
        switch (state)
        {
            case State.Idle:   Idle();   break;
            case State.Patrol: Patrol(); break;
            case State.Chase:  Chase();  break;
            case State.Attack: Attack(); break;
            case State.Retreat:Retreat();break;
        }

        if (state != previousState)
        {
            if (state == State.Attack)
                agent.isStopped = true;
            else
                agent.isStopped = false;

            previousState = state;
        }
    }

    void Idle()
    {
        agent.ResetPath();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrol = (currentPatrol + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrol].position);
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        // Only attack if almost stopped
        if (agent.velocity.magnitude > 0.1f)
        {
            return;
        }

        transform.LookAt(player);

        // Only shoot if in line of sight
        Vector3 dirToPlayer = player.position + Vector3.up * 0.8f - firePoint.position;
        float distToPlayer = dirToPlayer.magnitude;
        Ray ray = new(firePoint.position, dirToPlayer.normalized);

        int mask = LayerMask.GetMask("Default");

        if (Physics.Raycast(ray, out RaycastHit hit, distToPlayer, mask))
        {
            if (!hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Sword"))
            {
                return;
            }
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Cast");
            StartCoroutine(DelayedProjectile(1f));
            lastAttackTime = Time.time;
        }
    }

    IEnumerator DelayedProjectile(float delay)
    {
        yield return new WaitForSeconds(delay);

        var go = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (go.TryGetComponent<WraithProjectile>(out var proj))
            proj.Launch(firePoint.forward);

        audioSource.PlayOneShot(attackSound);
    }

    void Retreat()
    {
        // move directly away to maintain retreat distance
        Vector3 away = (transform.position - player.position).normalized;
        Vector3 target = transform.position + away * (retreatDistance - (retreatDistance - 0.1f));
        agent.SetDestination(target);
    }

    public void TakeDamage(int amt)
    {
        if (isDead) return;

        health -= amt;
        healthbar.UpdateHealthBar(maxHealth, health);

        if (health <= 0) Die();
        else ReactToHit();
    }

    void ReactToHit()
    {
        if (rend != null)
        {
            animator.SetTrigger("Hit");
            StopAllCoroutines();
        }
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

        // Trigger death animation
        if (animator != null)
            animator.SetTrigger("Die");

        // Disable NavMeshAgent
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
