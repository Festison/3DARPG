# 3D ARPG 포트폴리오
  포토폴리오 목적으로 제작된 간단한 3D ARPG 전투 시스템입니다.
 - 엔진 : Unity Engine 2021.3.31f1
 - 에디터 : Microsoft Visual Studio Community 2019
 - 제작기간 : 2023.12.28~2024.01.14 (약 3주)
 - 개발규모 : 1인개발

# 데모 동영상
> Youtube

[<img width="1000" alt="화면_캡쳐" src="https://github.com/Festison/3DARPG/assets/105289311/dccb9bad-1d7d-4d32-8daa-8ddd5a1134b2">](https://youtu.be/bDbR7Q208qs)

# 기술 설명서

## Player 구조
<img width="1000" alt="Untitled" src="https://github.com/Festison/3DARPG/assets/105289311/0cf8dc0d-0567-43a1-bceb-f7a18ef2874e">

## PlayerState
- FSM을 통해서 각 상태를 관리
- 생성자를 통해서 기본 상태 초기화
- 추상 클래스를 이용해 쉽게 상태를 확장
- InputSystem을 통해서 조작에 따른 상태 변경
- 파생 클래스를 이용해 각 상태에 따른 행동 구현

```cs
using UnityEngine;
using UnityEngine.InputSystem;
using System.Reflection;
using Festison;

public abstract class PlayerState
{
    protected PlayerController player;
    protected StateMachine stateMachine;

    protected InputAction rollAction;
    protected InputAction drawWeaponAction;
    protected InputAction attackAction;
    protected InputAction dashAttackAction;
    protected InputAction SpecialAttackAction;

    public PlayerState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;

        rollAction = player.playerInput.actions["Roll"];
        drawWeaponAction = player.playerInput.actions["DrawWeapon"];
        attackAction = player.playerInput.actions["Attack"];
        dashAttackAction = player.playerInput.actions["DashAttack"];
        SpecialAttackAction = player.playerInput.actions["SpecialAttack"];
    }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
        player.Move();                                      // 움직이기
        player.JumpAndGravity();                            // 점프하기
    }

    public virtual void Exit()
    {
    }
}

public class DefaultState : PlayerState
{
    bool drawWeapon, roll;
    public DefaultState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        roll = false;
        drawWeapon = false;
    }

    public override void Update()
    {
        base.Update();
        // 기본 상태에서 할 수 있는 행동들                            
        if (rollAction.triggered) // 구르기행동
        {
            roll = true;
            player.Roll();
        }
        if (drawWeaponAction.triggered) // 전투 상태 진입
        {
            drawWeapon = true;
        }
        if (drawWeapon)
        {
            player.animator.SetTrigger("drawWeapon");
            player.animator.SetTrigger("combat");
            stateMachine.ChangeState(player.combatState);
        }
        if (roll)
            stateMachine.ChangeState(player.rollState);
        if (!player.Grounded)
            stateMachine.ChangeState(player.jumpState);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class JumpState : PlayerState
{
    public JumpState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Update()
    {
        base.Update();

        if (player.Grounded && player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
            stateMachine.ChangeState(player.defaultState);
        else if (player.Grounded && player.animator.GetCurrentAnimatorStateInfo(1).IsName("CombatBleendTree"))
            stateMachine.ChangeState(player.combatState);
    }

    public override void Exit()
    {
        base.Exit();

    }
}

public class RollState : PlayerState
{

    public RollState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        Debug.Log("상태 실행중" + this.ToString());
        if (!player.animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling"))
            stateMachine.ChangeState(player.defaultState);
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.applyRootMotion = false;
    }
}

public class CombatState : PlayerState
{
    bool sheathWeapon, attack, dashAttack, specialAttack;
    public CombatState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }
    public override void Enter()
    {
        base.Enter();
        sheathWeapon = false;
        attack = false;
        dashAttack = false;
        specialAttack = false;
    }

    public override void Update()
    {
        base.Update();

        if (!player.Grounded)
            stateMachine.ChangeState(player.jumpState);
        if (drawWeaponAction.triggered)
        {
            sheathWeapon = true;
        }
        if (sheathWeapon)
        {
            player.animator.SetTrigger("sheathWeapon");
            player.animator.SetTrigger("default");
            stateMachine.ChangeState(player.defaultState);
        }
        if (attackAction.triggered)
        {
            attack = true;
        }
        if (attack)
        {
            player.animator.SetTrigger("attack");
            stateMachine.ChangeState(player.attackState);
        }
        if (dashAttackAction.triggered)
        {
            dashAttack = true;
        }
        if (dashAttack)
        {
            player.animator.Play("DashAttack");
            stateMachine.ChangeState(player.dashAttackState);
        }
        if (SpecialAttackAction.triggered)
        {
            specialAttack = true;
        }
        if (specialAttack)
        {
            player.animator.Play("SpecialAttackReady");
            stateMachine.ChangeState(player.specialAttackState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class AttackState : PlayerState
{
    float currentAttackTime;
    float clipLength;
    float clipSpeed;
    bool attack;
    public AttackState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();

        attack = false;
        player.animator.applyRootMotion = true;
        currentAttackTime = 0f;
        player.animator.SetTrigger("attack");
        player.animator.SetFloat("Speed", 0f);
    }

    public override void Update()
    {
        if (attackAction.triggered)
            attack = true;

        currentAttackTime += Time.deltaTime;
        clipLength = player.animator.GetCurrentAnimatorClipInfo(1)[0].clip.length;
        clipSpeed = player.animator.GetCurrentAnimatorStateInfo(1).speed;

        if (currentAttackTime >= clipLength / clipSpeed && attack)
        {
            stateMachine.ChangeState(player.attackState);
        }
        if (currentAttackTime >= clipLength / clipSpeed)
        {
            stateMachine.ChangeState(player.combatState);
            player.animator.SetTrigger("CombatIdle");
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.animator.applyRootMotion = false;
    }
}

public class DashAttackState : PlayerState
{
    public DashAttackState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.applyRootMotion = true;
    }

    public override void Update()
    {
        Debug.Log("Stating" + this.ToString());

        player.animator.SetTrigger("CombatIdle");
        if (!player.animator.GetCurrentAnimatorStateInfo(1).IsName("DashAttack"))
        {
            stateMachine.ChangeState(player.combatState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        player.animator.applyRootMotion = false;
    }
}

public class SpecialAttackState : PlayerState
{
    private float SpeciaAttackTime;
    public SpecialAttackState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        SpeciaAttackTime = 0f;
        player.virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority -= 2;
    }

    public override void Update()
    {
        if (!player.animator.GetCurrentAnimatorStateInfo(1).IsName("SpecialAttack") && !player.animator.GetCurrentAnimatorStateInfo(1).IsName("SpecialAttackReady"))
        {
            stateMachine.ChangeState(player.combatState);
        }

        SpeciaAttackTime += Time.deltaTime;

        if (SpeciaAttackTime>=5f)
        {
            player.animator.SetTrigger("CombatIdle");
        }

        RaycastHit hit;

        int layerMask = 1 << 9;

        Vector3 CameraCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        Ray ray = Camera.main.ScreenPointToRay(CameraCenter);

        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.transform.TryGetComponent<Monster>(out Monster monster))
            {
                Debug.Log("레이 적에게 맞는중");
                Vector3 dir = monster.transform.position - player.transform.position;
                dir.y = 0;
                dir.Normalize();
                Vector3 targetPos = monster.transform.position + (dir * 1.2f);

                player.transform.forward = dir;
                player.transform.position = targetPos;
                monster.Hit(15);
                player.animator.SetTrigger("attack");
            }
        }
    }

    public override void Exit()
    {
        player.virtualCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority += 2;
        SpeciaAttackTime = 0f;
        base.Exit();
    }
}
```
