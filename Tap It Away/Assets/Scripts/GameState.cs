using System;

public class PlayGameState : IState
{
    private GameManager gameManager;
	private InputManager inputManager;

	public PlayGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.UnlockInput();
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.LockInput();
	}
}

public class FailGameState : IState
{
    private GameManager gameManager;
	private InputManager inputManager;

	public FailGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.LockInput();
	}

	public void Excute()
	{
	}

	public void Exit()
	{
	}
}
public class CompleteGameState : IState
{
    private GameManager gameManager;
	private InputManager inputManager;

	public CompleteGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.LockInput();
	}

	public void Excute()
	{
	}

	public void Exit()
	{
	}
}

public class MenuGameState : IState
{
    private GameManager gameManager;
	private InputManager inputManager;

	public MenuGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.LockInput();
	}

	public void Excute()
	{
	}

	public void Exit()
	{
	}
}
