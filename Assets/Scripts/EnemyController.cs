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

    public float viewAngle = 120f; 
    public float viewDistance = 10f; 
    private bool isChasing = false;

    public float enemyHealth = 3f;

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        Debug.Log($"Враг получил {damage} урона. Осталось: {enemyHealth}.");

        if (enemyHealth <= 0f)
        {
            Die();
        }
    }

 

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        animator = GetComponent<Animator>();
        health = 3;
        prevHit = 0;
        damageWindow = 1.5f;
        attackWindow = 5f;

        
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

           
            PlayerController player = target.GetComponent<PlayerController>();
            if (player != null && player.vampirismHeal > 0)
            {
                player.HealPlayer(player.vampirismHeal);
                Debug.Log($"🧛 Вампиризм: Игрок отхилился на {player.vampirismHeal}");
            }

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









    private void Die()
    {
        Debug.Log("Враг убит!");
        animator.Play("Sword_Death_R");

        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        player.AddUpgradePoint();

        Destroy(gameObject, 3f);
    }


    void Update()
    {
        if (CanSeePlayer()) 
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

        if (angleToPlayer < viewAngle / 2) 
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, viewDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true; 
                }
            }
        }
        return false; 
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
            index = (index + 1) % patrolTargets.Length; 
        }
    }
}
