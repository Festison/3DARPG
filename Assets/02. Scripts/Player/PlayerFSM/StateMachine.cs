public class StateMachine
{
    public PlayerState currentState;

    public void StateInit(PlayerState startingState)
    {
        currentState = startingState;
        startingState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();

        currentState = newState;

        newState.Enter();
    }
}
