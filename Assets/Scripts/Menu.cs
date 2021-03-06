using System;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {	
	private Game game;
//	private Play play;
	private State state;
	private MyGUI gui;

	private int container;
	private int dialogContainer;
		
	public void Activate() {
		gui.containers[container].gameObject.SetActiveRecursively(true);
	}
	
	public void Deactivate() {
		gui.containers[container].gameObject.SetActiveRecursively(false);
	}
	
	public void Restart() {
		
		/*if (state.jniBridge.IsProductAcquired(0)) {
			game.SwitchToProductAcquired(0);
		} else {
			game.SwitchToProductAcquired(2);
		}
		
		if (game.showTrialDialog) {
			game.showTrialDialog = false;
			ToTrial();
		}*/
	}
	
	public void Initialize(Game g) {
		game = g;
		state = game.state;
		gui = game.gui;
		
		container = gui.AddContainer();
		
		gui.AddImage(container, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.containers[container].AddZLevel();
		gui.AddImage(container, new Vector3(1f,1f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_FULL, 6);
		gui.containers[container].AddZLevel();
		gui.AddImage(container, new Vector3(0.34f,0.34f,1f), MyGUI.GUIAlignment.Center, -0.4f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_FULL, 7);
		gui.containers[container].AddZLevel();
		
		if (Game.IS_DEBUG_ON) {
			TouchDelegate toZone01 = new TouchDelegate(ToZone01);
			gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone01, state.GetDialog(13), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, -0.4f, MyGUI.GUIAlignment.Center, 0.35f, Game.GUI_UV_COLOR_BLACK, 0);
			TouchDelegate toZone09 = new TouchDelegate(ToZone09);
			gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone09, state.GetDialog(14), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0.35f, Game.GUI_UV_COLOR_BLACK, 0);
			TouchDelegate toZone18 = new TouchDelegate(ToZone18);
			gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone18, state.GetDialog(15), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0.35f, Game.GUI_UV_COLOR_BLACK, 0);
			TouchDelegate toZone24 = new TouchDelegate(ToZone24);
			gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone24, state.GetDialog(16), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0.35f, Game.GUI_UV_COLOR_BLACK, 0);
		}		
		TouchDelegate toNewGame = new TouchDelegate(ToNewGame);
		gui.AddLabelButtonF(0.25f, container, new Vector3(0.04f,0.04f,1f), toNewGame, state.GetDialog(0), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0.15f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toContinueGame = new TouchDelegate(ToContinueGame);
		gui.AddLabelButtonF(0.25f, container, new Vector3(0.04f,0.04f,1f), toContinueGame, state.GetDialog(1), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0.05f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toOptions = new TouchDelegate(ToOptions);
		gui.AddLabelButtonF(0.25f, container, new Vector3(0.04f,0.04f,1f), toOptions, state.GetDialog(3), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, -0.05f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toCredits = new TouchDelegate(ToCredits);
		gui.AddLabelButtonF(0.25f, container, new Vector3(0.04f,0.04f,1f), toCredits, state.GetDialog(4), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, -0.15f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toQuit = new TouchDelegate(ToQuit);
		gui.AddLabelButtonF(0.25f, container, new Vector3(0.04f,0.04f,1f), toQuit, state.GetDialog(2), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, -0.25f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		
		// 1 level demo
/*		TouchDelegate to1LevelDemo = new TouchDelegate(To1LevelDemo);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), to1LevelDemo, state.GetDialog(45), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.1f, Game.GUI_UV_COLOR_BLACK, 0);*/

	}
	
	private void ToZone01() {
		game.state.SetPreferenceLevel(8);
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone09() {
		game.state.SetPreferenceLevel(10);
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone18() {
		game.state.SetPreferenceLevel(25);
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone24() {
		game.state.SetPreferenceLevel(31);
		game.SetGameMode(Game.Mode.Play);
	}
	
	public void ToNewGame() {
		if (game.state.GetPreferenceLevel() != -1) {
			gui.OpenDialog();
			dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
	//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
			int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0); 
			gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);
	
			int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
			gui.AddLabel(game.state.GetDialog(0), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
				1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
			gui.AddLabel(game.state.GetDialog(29), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Center, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.1f, 
				0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
			
			TouchDelegate toGame = new TouchDelegate(ToGame);
			gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, game.state.GetDialog(30), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	
			TouchDelegate toNewGame = new TouchDelegate(ToNewGameOK);
			gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toNewGame, game.state.GetDialog(6), 1.0f, 1.0f, 3, 
				MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		} else {
			ToNewGameOK();
		}
	}
	
	public void ToNewGameOK() {
		CloseDialog();
		game.state.SetPreferenceSokobanSolved(false);
		game.state.SetPreferenceLevel(0);
		game.state.ClearPowerUpStates();
		game.state.ResetPowerUpStates();
		game.state.SetPreferenceCaveSeed(UnityEngine.Random.Range(1000000,9999999));
		game.SetGameMode(Game.Mode.Play);
	}

	public void To1LevelDemo() {
		game.state.SetPreferenceLevel(9);
		game.SetGameMode(Game.Mode.Play);
	}

	public void ToContinueGame() {
		if (game.state.GetPreferenceLevel() == -1) {
			ToNewGameOK();
		} else {
			game.SetGameMode(Game.Mode.Play);
		}
	}
	
	public void ToOptionsMouseSteering(MyGUI.GUIState selectState) {
		state.SetPreferenceMouseYAxisInverted(selectState == MyGUI.GUIState.On ? 1 : -1);
	}

	public void ToOptionsMusicOn(MyGUI.GUIState selectState) {
		state.SetPreferenceMusicOn(selectState == MyGUI.GUIState.On ? true : false);
	}

	public void ToOptionsSoundOn(MyGUI.GUIState selectState) {
		state.SetPreferenceSoundOn(selectState == MyGUI.GUIState.On ? true : false);
	}

	public void ToOptions() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(game.state.GetDialog(12), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		gui.AddLabel(game.state.GetDialog(43), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0.125f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.AddLabel(game.state.GetDialog(63), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.AddLabel(game.state.GetDialog(64), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, -0.125f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.containers[container].AddZLevel();
		CheckboxDelegate toOptionsMouseSteering = new CheckboxDelegate(ToOptionsMouseSteering);
		MyGUI.GUIState selectState = state.GetPreferenceMouseYAxisInverted() == 1 ? MyGUI.GUIState.On : MyGUI.GUIState.Off;
		gui.AddCheckbox(dialogBox, new Vector3(0.1f, 0.1f, 0.1f), toOptionsMouseSteering, selectState, MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0.125f, Game.GUI_UV_CHECKBOX_BACKGROUND, 0, Game.GUI_UV_CHECKBOX_CHECKMARK, 0);
		CheckboxDelegate toOptionsMusicOn = new CheckboxDelegate(ToOptionsMusicOn);
		selectState = state.GetPreferenceMusicOn() ? MyGUI.GUIState.On : MyGUI.GUIState.Off;
		gui.AddCheckbox(dialogBox, new Vector3(0.1f, 0.1f, 0.1f), toOptionsMusicOn, selectState, MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_CHECKBOX_BACKGROUND, 0, Game.GUI_UV_CHECKBOX_CHECKMARK, 0);
		CheckboxDelegate toOptionsSoundOn = new CheckboxDelegate(ToOptionsSoundOn);
		selectState = state.GetPreferenceSoundOn() ? MyGUI.GUIState.On : MyGUI.GUIState.Off;
		gui.AddCheckbox(dialogBox, new Vector3(0.1f, 0.1f, 0.1f), toOptionsSoundOn, selectState, MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, -0.125f, Game.GUI_UV_CHECKBOX_BACKGROUND, 0, Game.GUI_UV_CHECKBOX_CHECKMARK, 0);
		
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, game.state.GetDialog(37), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.1f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

/*	public void ToUpgrade() {
		game.BuyProduct(0);
	}*/
	
	public void ToCredits() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(game.state.GetDialog(4), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		gui.AddLabel(game.state.GetDialog(41), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, -0.4f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.AddLabel(game.state.GetDialog(42), dialogBox, new Vector3(0.03f,0.03f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, game.state.GetDialog(37), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.1f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

	private void CloseDialog() {
		if (gui.isInDialogMode) {
			gui.CloseDialog(dialogContainer);
		}
	}
		
	public void ToQuit() {
		Application.Quit();
	}
	
	private void ToGame() {
		CloseDialog();
	}
	
	void onDisable() {
		CancelInvoke();
	}
}

