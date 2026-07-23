using System.Collections.Generic;

public class StateMachine
{
    private Dictionary<GameStateType, IState> states = new();
    private IState currentState;
    public GameStateType CurrentStateType {get; set;}
    public void RegisterState(GameStateType gameStateType, IState state)
    {
        states[gameStateType] = state;
    }
    public void ChangeState(GameStateType newGameStateType)
    {
        if (!states.TryGetValue(newGameStateType, out IState newState))
        {
            return;
        }

        currentState?.Exit();

        CurrentStateType = newGameStateType;
        currentState = newState;

        currentState?.Enter();
    }
    public void Excute()
    {
        currentState.Excute();
    }
}