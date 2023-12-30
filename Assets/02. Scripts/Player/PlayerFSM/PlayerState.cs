using UnityEngine;
using UnityEngine.InputSystem;
using Festison;

public abstract class PlayerState
{
    public PlayerController player;
    public StateMachine stateMachine;

    public PlayerState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        
    }

    public virtual void Enter()
    {
        Debug.Log("Enter State: " + this.ToString());
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
    public DefaultState(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
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
        // 기본 상태에서 할 수 있는 행동들

        player.Roll();
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

        if (player.Grounded)
            stateMachine.ChangeState(player.defaultState);
    }

    public override void Exit()
    {
        base.Exit();
       
    }
}








