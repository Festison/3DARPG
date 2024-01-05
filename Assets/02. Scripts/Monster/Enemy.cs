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
        // ������ ��
        animator.SetFloat("Move", navMesh.velocity.magnitude / navMesh.speed);

        if (player.Equals(null))
        {
            return;
        }

        // ���� ����
        if (timePassed >= attackCoolTime)
        {
            // ���� ������
            if (Vector3.Distance(player.transform.position, transform.position) <= attackableRange)
            {
                animator.SetInteger("AttackIndex", Random.Range(1, 4));
                animator.SetTrigger("Attack");
                timePassed = 0;
            }
        }
        timePassed += Time.deltaTime;
      
        // ���� ���� �ȿ� ��������
        if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= defectiveRange)
        {
            Debug.Log("������");
            newDestinationCD = 0.5f;
            navMesh.SetDestination(player.transform.position);
        }

        // Ž�� ���� ���� �־�������
        else if (Vector3.Distance(player.transform.position, transform.position) >= defectiveRange)
        {
            Debug.Log("������ ��");
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