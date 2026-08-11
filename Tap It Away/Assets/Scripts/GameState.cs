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
		Panel gameplayPanel = UIManager.Instance.GetPanel(GameConfig.GAMEPLAY_PANEL);

		if (gameplayPanel == null || !gameplayPanel.gameObject.activeSelf)
		{
			UIManager.Instance.OpenPanel(GameConfig.GAMEPLAY_PANEL);
		}
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.LockInput();
		// UIManager.Instance.ClosePanel(GameConfig.GAMEPLAY_PANEL);
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
		UIManager.Instance.OpenPanel(GameConfig.FAIL_PANEL);
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.UnlockInput();
		UIManager.Instance.ClosePanel(GameConfig.FAIL_PANEL);
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
		UIManager.Instance.OpenPanel(GameConfig.SUCCESS_PANEL);
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.UnlockInput();
		UIManager.Instance.ClosePanel(GameConfig.SUCCESS_PANEL);
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
		UIManager.Instance.ClosePanel(GameConfig.GAMEPLAY_PANEL);
		UIManager.Instance.OpenPanel(GameConfig.MENU_PANEL);
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.UnlockInput();
		UIManager.Instance.ClosePanel(GameConfig.MENU_PANEL);
	}
}

public class SettingInGameState : IState
{
	private GameManager gameManager;
	private InputManager inputManager;

	public SettingInGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.LockInput();
		UIManager.Instance.OpenPanel(GameConfig.SETTING_IN_GAME_PANEL);
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.UnlockInput();
		UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_GAME_PANEL);
	}
}

public class LoadingGameState : IState
{
	private GameManager gameManager;
	private InputManager inputManager;

	public LoadingGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public void Enter()
	{
		inputManager.LockInput();
		UIManager.Instance.OpenPanel(GameConfig.LOADING_PANEL);
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.UnlockInput();
		UIManager.Instance.ClosePanel(GameConfig.LOADING_PANEL);
	}
}
