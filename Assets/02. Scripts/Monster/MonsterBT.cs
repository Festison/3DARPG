using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace BT
{
    #region BT �⺻ ���
    public interface INode
    {
        public enum STATE
        {
            RUN, SUCCESS, FAIL
        }

        public STATE Evaluate();
    }

    public class ActionNode : INode
    {
        public Func<INode.STATE> actionNode;

        public ActionNode(Func<INode.STATE> action)
        {
            this.actionNode = action;
        }

        public INode.STATE Evaluate()
        {
            return (actionNode == null) ? INode.STATE.FAIL : actionNode();
        }
    }

    public class SelectorNode : INode
    {
        List<INode> childs;

        public SelectorNode()
        {
            childs = new List<INode>();
        }

        public void Add(INode node)
        {
            childs.Add(node);
        }

        public INode.STATE Evaluate()
        {
            if (childs.Count <= 0)
                return INode.STATE.FAIL;

            foreach (INode childNode in childs)
            {
                switch (childNode.Evaluate())
                {
                    case INode.STATE.SUCCESS:
                        return INode.STATE.SUCCESS;
                    case INode.STATE.RUN:
                        return INode.STATE.RUN;
                }
            }
            return INode.STATE.FAIL;
        }
    }

    public class SequenceNode : INode
    {
        List<INode> childs;

        public SequenceNode()
        {
            childs = new List<INode>();
        }

        public void Add(INode node)
        {
            childs.Add(node);
        }

        public INode.STATE Evaluate()
        {
            if (childs.Count <= 0)
                return INode.STATE.FAIL;

            foreach (INode childNode in childs)
            {
                switch (childNode.Evaluate())
                {
                    case INode.STATE.SUCCESS:
                        continue;
                    case INode.STATE.RUN:
                        return INode.STATE.RUN;
                    case INode.STATE.FAIL:
                        return INode.STATE.FAIL;
                }
            }
            return INode.STATE.SUCCESS;
        }
    }
    #endregion

    public class MonsterBT : MonoBehaviour
    {
        [Header("������ ���")]
        SelectorNode rootNode;                       // ��Ʈ ���
        SelectorNode attackSortSelector;             // ���� ��� ������
        SelectorNode targetSettingSelector;          // Ÿ�� ���� ������

        [Header("������ ���")]
        SequenceNode attackSequence;                 // ���� ������
        SequenceNode detectiveSequence;              // Ž�� ������

        [Header("�׼� ���")]
        ActionNode returnAction;                     // ��ȯ �׼�

        private NavMeshAgent navMesh;
        private Animator animator;
        private float timePassed;
        [SerializeField] private float newDestinationCoolTime = 0.5f;

        [Header("���� Ž�� ����")]
        [Tooltip("���� ���� ��Ÿ��")] [SerializeField] private float attackCoolTime = 3f;
        [Tooltip("���� ���� ����")] [SerializeField] private float attackableRange = 2f;
        [Tooltip("���� Ž�� ����")] [SerializeField] private float defectiveRange = 6f;

        [Header("���Ͱ� �˾ƾ��� ����")]
        [Tooltip("�÷��̾� ��ġ")] [SerializeField] private Transform player;
        [Tooltip("���� ���� ����Ʈ")] [SerializeField] private Vector3 startPosition;


        private void OnEnable()
        {
            startPosition = new Vector3(transform.position.x, 0, transform.position.z);
        }
        void Start()
        {
            navMesh = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();          
            SetBT();
        }

        public void SetBT()
        {
            // ��Ʈ ��� : ���� �ൿ ��� ������
            rootNode = new SelectorNode();

            // ���� ������
            attackSequence = new SequenceNode();                                    // ���� ������ ����
            rootNode.Add(attackSequence);                                           // ���� �������� ��Ʈ��忡 �߰�

            // Ž�� ������
            detectiveSequence = new SequenceNode();                                 // Ž�� ������ ����
            rootNode.Add(detectiveSequence);                                        // Ž�� �������� ��Ʈ ��忡 �߰�

            // ��ȯ �׼�
            returnAction = new ActionNode(ReturnAction);                            // ��ȯ �׼��� ����
            rootNode.Add(returnAction);                                             // ��ȯ �׼��� ��Ʈ ��忡 �߰�

            attackSequence.Add(new ActionNode(AttackRangeCheckAction));             // ���ݹ��� üũ �׼��� ���� �� ���� �������� �߰�
                                                                                    // 
            attackSortSelector = new SelectorNode();                                // ���� ��� ������ ����
            attackSequence.Add(attackSortSelector);                                 // ���� ��� �����͸� ���� �������� �߰�

            detectiveSequence.Add(new ActionNode(DetectiveRangeCheckAction));       // Ž�� ���� üũ �׼��� Ž�� �������� �߰�

            targetSettingSelector = new SelectorNode();                             // Ÿ�� ���� ������ ����
            detectiveSequence.Add(targetSettingSelector);                           // Ÿ�� ���� �����͸� Ž�� �������� �߰�

            // ���� ��� ������              
            attackSortSelector.Add(new ActionNode(DefaultAttackAction));            // �⺻ �����ϱ� �׼��� ���� �����Ϳ� �߰�

            // Ÿ�� ���� ������      
            targetSettingSelector.Add(new ActionNode(PlayerTargetAction));          // �÷��̾ Ÿ������ �����ϴ� �׼��� Ÿ�� ���� �����Ϳ� �߰�
        }

        #region �׼� ��忡 �� �Լ�

        INode.STATE DefaultAttackAction()
        {
            Debug.Log("�⺻ ���� ��");
            animator.SetInteger("AttackIndex", UnityEngine.Random.Range(1, 4));
            animator.SetTrigger("Attack");
            timePassed = 0;
            return INode.STATE.RUN;
        }

        INode.STATE AttackRangeCheckAction()
        {
            if (player.Equals(null))
                return INode.STATE.FAIL;

            if (timePassed >= attackCoolTime)
            {
                // ���� ������
                if (Vector3.Distance(player.transform.position, transform.position) <= attackableRange)
                {
                    return INode.STATE.SUCCESS;
                }
            }

            return INode.STATE.FAIL;
        }

        INode.STATE PlayerTargetAction()
        {
            if (newDestinationCoolTime <= 0 && Vector3.Distance(player.transform.position, transform.position) <= defectiveRange)
            {
                Debug.Log("�ٰŸ� �� Ÿ�� ��");
                newDestinationCoolTime = 0.5f;
                navMesh.SetDestination(player.transform.position);
                return INode.STATE.RUN;
            }
            return INode.STATE.RUN;
        }

        INode.STATE DetectiveRangeCheckAction()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, defectiveRange, LayerMask.GetMask("Player"));
            if (cols.Length > 0)
            {
                Debug.Log("Ž�� ��");
                player = cols[0].transform;
                return INode.STATE.SUCCESS;
            }
            return INode.STATE.FAIL;
        }

        INode.STATE ReturnAction()
        {
            if (Vector3.Distance(player.transform.position, transform.position) >= defectiveRange)
            {
                Debug.Log("������ ��");
                navMesh.SetDestination(startPosition);
                return INode.STATE.RUN;
            }
            else
                return INode.STATE.FAIL;
        }
        #endregion

        void Update()
        {
            rootNode.Evaluate();
            animator.SetFloat("Move", navMesh.velocity.magnitude / navMesh.speed);
            timePassed += Time.deltaTime;
            newDestinationCoolTime -= Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackableRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, defectiveRange);
        }
    }
}
