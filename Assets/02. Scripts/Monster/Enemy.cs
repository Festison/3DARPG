using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character, IAttackable, IHitable
{
    [SerializeField] float health = 3;

    [Header("Combat")]
    [SerializeField] float attackCoolTime = 3f;
    [SerializeField] float attackableRange = 2f;
    [SerializeField] float defectiveRange = 6f;

    public Transform player;
    NavMeshAgent navMesh;
    Animator animator;
    float timePassed;
    [SerializeField]  float newDestinationCD = 0.5f;
    public Vector3 startPosition;
    

    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = player.transform;
        startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void Update()
    {
        // 움직일 때
        animator.SetFloat("Move", navMesh.velocity.magnitude / navMesh.speed);

        if (player.Equals(null))
        {
            return;
        }

        // 공격 범위
        if (timePassed >= attackCoolTime)
        {
            // 공격 딜레이
            if (Vector3.Distance(player.transform.position, transform.position) <= attackableRange)
            {
                animator.SetInteger("AttackIndex", Random.Range(1, 4));
                animator.SetTrigger("Attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;
      
        // 추적 범위 안에 들어왔을때
        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= defectiveRange)
        {
            Debug.Log("추적중");
            newDestinationCD = 0.5f;
            navMesh.SetDestination(player.transform.position);
        }

        // 탐지 범위 보다 멀어졌을때
        else if (Vector3.Distance(player.transform.position, transform.position) >= defectiveRange)
        {
            Debug.Log("집가는 중");
            navMesh.SetDestination(startPosition);
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
        Gizmos.DrawWireSphere(transform.position, attackableRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, defectiveRange);
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