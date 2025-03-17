using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public int health;
    private float prevHit, damageWindow;
    public Animator animator;

    private NavMeshAgent agent;
    private Transform target;
    public Transform[] patrolTargets;
    private int index;
    public bool isAttack = false;
    private float prevAttack, attackWindow;
    private bool isPlayerAttack;
    public event Action OnDeath;

    public float viewAngle = 120f; // Угол обзора
    public float viewDistance = 10f; // Дистанция обзора
    private bool isChasing = false; // Флаг преследования

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        health = 3;
        prevHit = 0;
        damageWindow = 1.5f;
        attackWindow = 5f;

        // Выбираем случайную стартовую точку патрулирования
        if (patrolTargets.Length > 0)
        {
            index = UnityEngine.Random.Range(0, patrolTargets.Length);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerAttack = target.GetComponent<PlayerController>().isAttack;
        if (other.gameObject.CompareTag("PlayerSword") && (Time.time > prevHit + damageWindow) && isPlayerAttack)
        {
            prevHit = Time.time;
            health--;
            if (health > 1)
            {
                animator.Play("KnockdownFront1");
            }
            else if (health == 1)
            {
                animator.Play("Sword_Defeat_1_Start");
            }
            else if (health <= 0)
            {
                Die();
            }
        }
    }





   

    // Method for enemy death
    public void Die()
    {
        animator.SetTrigger("isDead"); // Play death animation
        animator.Play("Sword_Death_R");
        
        OnDeath?.Invoke(); // Notify any subscribers that the enemy has died
        Destroy(gameObject, 3f); // Destroy the enemy object after 2 seconds
    }

    void Update()
    {
        if (CanSeePlayer()) // If the enemy can see the player, start chasing
        {
            isChasing = true;
        }

        if (isChasing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, target.position);
            if (distanceToPlayer < 2.5f)
            {
                Attack();
            }
            else
            {
                MoveToPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    private bool CanSeePlayer()
    {
        Vector3 directionToPlayer = (target.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        if (angleToPlayer < viewAngle / 2) // Within 120 degrees field of view
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; // Player is in the line of sight
                }
            }
        }
        return false; // Player is not in sight
    }

    private void MoveToPlayer()
    {
        animator.SetBool("isWalk", true);
        agent.destination = target.position;
    }

    private void Attack()
    {
        animator.SetBool("isWalk", false);
        agent.destination = transform.position;
        transform.LookAt(target.position);
        bool isKnockdown = animator.GetCurrentAnimatorStateInfo(0).IsName("KnockdownRight");
        if (Time.time > prevAttack + attackWindow && !isKnockdown)
        {
            prevAttack = Time.time;
            animator.Play("Sword_Attack_Ld3_quick");
        }
    }

    private void Patrol()
    {
        if (patrolTargets.Length > 0)
        {
            animator.SetBool("isWalk", true);
            agent.destination = patrolTargets[index].position;
            CheckNewPatrolTargets();
        }
    }

    private void CheckNewPatrolTargets()
    {
        Vector3 targetPos = patrolTargets[index].position;
        if (Vector3.Distance(transform.position, targetPos) < 3f)
        {
            index = (index + 1) % patrolTargets.Length; // Move cyclically through patrol targets
        }
    }
}
