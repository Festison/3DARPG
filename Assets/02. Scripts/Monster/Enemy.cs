using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character, IAttackable, IHitable
{
    [SerializeField] float health = 3;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float aggroRange = 6f;

    public Transform player;
    NavMeshAgent agent;
    Animator animator;
    float timePassed;
    float newDestinationCD = 0.5f;
    public Vector3 startPosition;
    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = player.transform;
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        // 움직일 때
        animator.SetFloat("Move", agent.velocity.magnitude / agent.speed);

        if (player.Equals(null))
        {
            return;
        }

        // 공격 범위
        if (timePassed >= attackCD)
        {
            // 공격 딜레이
            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                animator.SetInteger("AttackIndex", Random.Range(1, 4));
                animator.SetTrigger("Attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;
      
        // 추적 범위 안에 들어왔을때
        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            Debug.Log("추적중");
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
        }

        // 탐지 범위 보다 멀어졌을때
        else if (Vector3.Distance(player.transform.position, transform.position) >= aggroRange)
        {
            Debug.Log("집가는 중");
            agent.SetDestination(startPosition);
        }
        newDestinationCD -= Time.deltaTime;

        transform.LookAt(player.transform);
    }

    public void HitVFX(Vector3 hitPosition)
    {
        // GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
        // Destroy(hit, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    public void Hit(IAttackable attackable)
    {
        Hp -= attackable.Damage;
    }

    public void Attack(IHitable hitable)
    {
        Damage -= hitable.Hp;
    }
}