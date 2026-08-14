using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayGameState : IState
{
	private GameManager gameManager;
	private InputManager inputManager;

	public PlayGameState(GameManager gameManager, InputManager inputManager)
	{
		this.gameManager = gameManager;
		this.inputManager = inputManager;
	}
	public async void Enter()
	{
		inputManager.UnlockInput();
		UIManager.Instance.OpenPanel(GameConfig.GAMEPLAY_PANEL, true);
		LoadingPanel loadingPanel = UIManager.Instance.GetPanel(GameConfig.LOADING_PANEL) as LoadingPanel;
		await new WaitUntil(() => loadingPanel.IsDone);
		LevelManager.Instance.PlayGame();
	}

	public void Excute()
	{
	}

	public void Exit()
	{
		inputManager.LockInput();
		UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_GAME_PANEL);
		UIManager.Instance.ClosePanel(GameConfig.GAMEPLAY_PANEL);
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
		GameThemeController.Instance?.SetBackgroundType(GameThemeBackgroundType.Menu);
		UIManager.Instance.OpenPanel(GameConfig.MENU_PANEL, true);
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

// public class SettingInGameState : IState
// {
// 	private GameManager gameManager;
// 	private InputManager inputManager;

// 	public SettingInGameState(GameManager gameManager, InputManager inputManager)
// 	{
// 		this.gameManager = gameManager;
// 		this.inputManager = inputManager;
// 	}
// 	public void Enter()
// 	{
// 		inputManager.LockInput();
// 		UIManager.Instance.OpenPanel(GameConfig.SETTING_IN_GAME_PANEL);
// 	}

// 	public void Excute()
// 	{
// 	}

// 	public void Exit()
// 	{
// 		inputManager.UnlockInput();
// 		UIManager.Instance.ClosePanel(GameConfig.SETTING_IN_GAME_PANEL);
// 	}
// }

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
		GameThemeController.Instance?.SetBackgroundType(GameThemeBackgroundType.Menu);
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
