using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace BT
{
    #region BT 기본 기능
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
        [Header("셀럭터 노드")]
        SelectorNode rootNode;                       // 루트 노드
        SelectorNode attackSortSelector;             // 공격 방식 셀렉터
        SelectorNode targetSettingSelector;          // 타겟 설정 셀렉터

        [Header("시퀀스 노드")]
        SequenceNode attackSequence;                 // 공격 시퀀스
        SequenceNode detectiveSequence;              // 탐지 시퀀스

        [Header("액션 노드")]
        ActionNode returnAction;                     // 귀환 액션

        private NavMeshAgent navMesh;
        private Animator animator;
        private float timePassed;
        [SerializeField] private float newDestinationCoolTime = 0.5f;

        [Header("몬스터 탐지 정보")]
        [Tooltip("몬스터 공격 쿨타임")] [SerializeField] private float attackCoolTime = 3f;
        [Tooltip("몬스터 공격 범위")] [SerializeField] private float attackableRange = 2f;
        [Tooltip("몬스터 탐지 범위")] [SerializeField] private float defectiveRange = 6f;

        [Header("몬스터가 알아야할 정보")]
        [Tooltip("플레이어 위치")] [SerializeField] private Transform player;
        [Tooltip("몬스터 시작 포인트")] [SerializeField] private Vector3 startPosition;


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
            // 루트 노드 : 적의 행동 양식 셀렉터
            rootNode = new SelectorNode();

            // 공격 시퀀스
            attackSequence = new SequenceNode();                                    // 공격 시퀀스 생성
            rootNode.Add(attackSequence);                                           // 공격 시퀀스를 루트노드에 추가

            // 탐지 시퀀스
            detectiveSequence = new SequenceNode();                                 // 탐지 시퀀스 생성
            rootNode.Add(detectiveSequence);                                        // 탐지 시퀀스를 루트 노드에 추가

            // 귀환 액션
            returnAction = new ActionNode(ReturnAction);                            // 귀환 액션을 생성
            rootNode.Add(returnAction);                                             // 귀환 액션을 루트 노드에 추가

            attackSequence.Add(new ActionNode(AttackRangeCheckAction));             // 공격범위 체크 액션을 생성 후 공격 시퀀스에 추가
                                                                                    // 
            attackSortSelector = new SelectorNode();                                // 공격 방식 셀렉터 생성
            attackSequence.Add(attackSortSelector);                                 // 공격 방식 셀렉터를 공격 시퀀스에 추가

            detectiveSequence.Add(new ActionNode(DetectiveRangeCheckAction));       // 탐지 범위 체크 액션을 탐지 시퀀스에 추가

            targetSettingSelector = new SelectorNode();                             // 타겟 설정 셀렉터 생성
            detectiveSequence.Add(targetSettingSelector);                           // 타겟 설정 셀렉터를 탐지 시퀀스에 추가

            // 공격 방식 셀렉터              
            attackSortSelector.Add(new ActionNode(DefaultAttackAction));            // 기본 공격하기 액션을 공격 셀렉터에 추가

            // 타겟 설정 셀렉터      
            targetSettingSelector.Add(new ActionNode(PlayerTargetAction));          // 플레이어를 타겟으로 설정하는 액션을 타겟 설정 셀렉터에 추가
        }

        #region 액션 노드에 들어갈 함수

        INode.STATE DefaultAttackAction()
        {
            Debug.Log("기본 공격 중");
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
                // 공격 딜레이
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
                Debug.Log("근거리 적 타겟 중");
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
                Debug.Log("탐지 됨");
                player = cols[0].transform;
                return INode.STATE.SUCCESS;
            }
            return INode.STATE.FAIL;
        }

        INode.STATE ReturnAction()
        {
            if (Vector3.Distance(player.transform.position, transform.position) >= defectiveRange)
            {
                Debug.Log("집가는 중");
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
