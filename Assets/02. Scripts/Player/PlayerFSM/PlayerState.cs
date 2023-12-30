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
        player.Move();                                      // �����̱�
        player.JumpAndGravity();                            // �����ϱ�
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
        // �⺻ ���¿��� �� �� �ִ� �ൿ��

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








