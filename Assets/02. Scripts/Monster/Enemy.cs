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
        // ������ ��
        animator.SetFloat("Move", agent.velocity.magnitude / agent.speed);

        if (player.Equals(null))
        {
            return;
        }

        // ���� ����
        if (timePassed >= attackCD)
        {
            // ���� ������
            if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
            {
                animator.SetInteger("AttackIndex", Random.Range(1, 4));
                animator.SetTrigger("Attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;
      
        // ���� ���� �ȿ� ��������
        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
        {
            Debug.Log("������");
            newDestinationCD = 0.5f;
            agent.SetDestination(player.transform.position);
        }

        // Ž�� ���� ���� �־�������
        else if (Vector3.Distance(player.transform.position, transform.position) >= aggroRange)
        {
            Debug.Log("������ ��");
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