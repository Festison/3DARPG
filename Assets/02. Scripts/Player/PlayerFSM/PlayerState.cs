using UnityEngine;
using UnityEngine.InputSystem;
using Festison;

public abstract class PlayerState
{
    public PlayerController player;
    public StateMachine stateMachine;

    public InputAction rollAction;
    public InputAction drawWeaponAction;
    public InputAction attackAction;

    public PlayerState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;

        rollAction = player.playerInput.actions["Roll"];
        drawWeaponAction = player.playerInput.actions["DrawWeapon"];
        attackAction = player.playerInput.actions["Attack"];
    }

    public virtual void Enter()
    {
        Debug.Log("Enter State: " + this.ToString());
    }

    public virtual void Update()
    {
        Debug.Log("상태 실행중" + this.ToString());
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
        if (drawWeaponAction.triggered)
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

        if (player.Grounded&& player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle Walk Run Blend"))
            stateMachine.ChangeState(player.defaultState);
        else if(player.Grounded && player.animator.GetCurrentAnimatorStateInfo(1).IsName("CombatBleendTree"))
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
    bool sheathWeapon,attack;
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
        //player.animator.applyRootMotion = true;
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






