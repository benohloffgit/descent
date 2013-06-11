using System;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {	
	public AnimationClip upgradeButtonAnimationClip;

	private Game game;
//	private GameInput gI;
	private State state;
	private MyGUI gui;

	private int container;
	private int dialogContainer;
	private int upgradeButton;
	private Animation upgradeButtonAnim;
	
	private bool hasDisclaimerDialogBeenShown;
	private bool hasFirstHelpDialogBeenShown;
	private bool inDisclaimerDialog;
	
	private static float UPGRADE_BUTTON_SCALE_INTERVAL = 3.0f;

	void Awake() {
		hasDisclaimerDialogBeenShown = false;
		inDisclaimerDialog = false;
	}
		
	void Update() {
		if (state.startupCounter == 1 && !hasDisclaimerDialogBeenShown) {
			ToDisclaimerDialog();
			hasDisclaimerDialogBeenShown = true;
			inDisclaimerDialog = true;
		}
	}
	
	public void Activate() {
		gui.containers[container].gameObject.SetActiveRecursively(true);
	}
	
	public void Deactivate() {
		gui.containers[container].gameObject.SetActiveRecursively(false);
	}
	
	public void Restart() {

/*		TouchDelegate toNewGame = new TouchDelegate(ToNewGame);
		gui.AddButton(topContainer, new Vector3(0.12f, 0.12f, 1.0f), toNewGame, MyGUI.GUIAlignment.Center, 0.025f, MyGUI.GUIAlignment.Top, 0.025f, Game.GUI_UV_CREDITS_BUTTON, 1);
/*		TouchDelegate toHelp = new TouchDelegate(ToHelp);
		gui.AddButton(topContainer, new Vector3(0.12f, 0.12f, 1.0f), toHelp, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.025f, Game.GUI_UV_HELP_BUTTON, 1);

		TouchDelegate toUpgrade = new TouchDelegate(ToUpgrade);
		upgradeButton = gui.AddButton(topContainer, new Vector3(0.15f, 0.15f, 1.0f), toUpgrade, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, 0.03f, Game.GUI_UV_UPGRADE, 0);
		upgradeButtonAnim = gui.buttons[upgradeButton].gameObject.GetComponent<Animation>() as Animation;
		upgradeButtonAnim.AddClip(upgradeButtonAnimationClip, upgradeButtonAnimationClip.name);
		
		gui.AddLabel(" ", topContainer,  new Vector3(0.35f, 0.35f, 1.0f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center,
			0.125f, 0.0125f, 0.25f, MyGUI.GUIBackground.Quad, Game.GUI_CUT, 3);
		
#if UNITY_ANDROID || UNITY_IPHONE
		TouchDelegate toServer = new TouchDelegate(ToServer);
		gui.AddButton(topContainer, new Vector3(0.24f, 0.24f, 1.0f), toServer, MyGUI.GUIAlignment.Center, -0.15f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_SERVER_BUTTON, 2);
		TouchDelegate toClient = new TouchDelegate(ToClient);
		gui.AddButton(topContainer, new Vector3(0.24f, 0.24f, 1.0f), toClient, MyGUI.GUIAlignment.Center, 0.15f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_CLIENT_BUTTON, 2);
#endif	
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
		TouchDelegate toClient = new TouchDelegate(ToClient);
		gui.AddButton(topContainer, new Vector3(0.24f, 0.24f, 1.0f), toClient, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_CLIENT_BUTTON, 2);
#endif	

//		gui.AddLabel(state.GetDialog(36), topContainer,  new Vector3(0.5f, 0.5f, 1.0f), MyGUI.GUIAlignment.Center, -0.15f, MyGUI.GUIAlignment.Center,
//			-0.1f, 0.0125f, 0.4f, MyGUI.GUIBackground.Quad, Game.GUI_UV_BLANK, 1);
//		gui.AddLabel(state.GetDialog(37), topContainer,  new Vector3(0.5f, 0.5f, 1.0f), MyGUI.GUIAlignment.Center, 0.15f, MyGUI.GUIAlignment.Center,
//			-0.1f, 0.0125f, 0.4f, MyGUI.GUIBackground.Quad, Game.GUI_UV_BLANK, 1);
		
//		int bottomContainer = gui.AddContainer(container, new Vector3(fullSize.x, fullSize.y * 0.3f, fullSize.z), new Vector2(screenCenter.x, screenCenter.y-fullSize.y*0.35f));	
		TouchDelegate toPreferences = new TouchDelegate(ToPreferences);
		gui.AddButton(topContainer, new Vector3(0.12f, 0.12f, 1.0f), toPreferences, MyGUI.GUIAlignment.Right, 0.025f, MyGUI.GUIAlignment.Bottom, 0.025f, Game.GUI_UV_PREFERENCES_BUTTON, 1);
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			TouchDelegate toQuit = new TouchDelegate(ToQuit);
			gui.AddButton(topContainer, new Vector3(0.12f, 0.12f, 1.0f), toQuit, MyGUI.GUIAlignment.Left, 0.025f, MyGUI.GUIAlignment.Bottom, 0.025f, Game.GUI_UV_EXIT_BUTTON, 1);
		}
		
		if (state.jniBridge.IsProductAcquired(0)) {
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
		
		Vector3 fullSize = gui.containers[container].GetSize();
		Vector3 screenCenter = gui.containers[container].GetCenter();
		
		gui.AddImage(container, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.containers[container].AddZLevel();
		TouchDelegate toZone01 = new TouchDelegate(ToZone01);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone01, state.GetDialog(13), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.4f, MyGUI.GUIAlignment.Center, 0.2f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toZone09 = new TouchDelegate(ToZone09);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone09, state.GetDialog(14), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0.2f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toZone18 = new TouchDelegate(ToZone18);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone18, state.GetDialog(15), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0.2f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toZone24 = new TouchDelegate(ToZone24);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toZone24, state.GetDialog(16), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0.2f, Game.GUI_UV_COLOR_BLACK, 0);
		
		TouchDelegate toNewGame = new TouchDelegate(ToNewGame);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toNewGame, state.GetDialog(0), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.1f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toContinueGame = new TouchDelegate(ToContinueGame);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toContinueGame, state.GetDialog(1), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toQuit = new TouchDelegate(ToQuit);
		gui.AddLabelButton(container, new Vector3(0.05f,0.05f,1f), toQuit, state.GetDialog(2), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, -0.1f, Game.GUI_UV_COLOR_BLACK, 0);
	}
	
	public void RemovePaygate() {
		gui.imageButtons[upgradeButton].gameObject.SetActiveRecursively(false);
		CancelInvoke();
	}
	
	public void ShowPaygate() {
		gui.imageButtons[upgradeButton].gameObject.SetActiveRecursively(true);
		InvokeRepeating("ScaleUpgradeButton", 0f, UPGRADE_BUTTON_SCALE_INTERVAL);
	}
	
	private void ToZone01() {
		game.state.level = 1;
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone09() {
		game.state.level = 9;
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone18() {
		game.state.level = 18;
		game.SetGameMode(Game.Mode.Play);
	}

	private void ToZone24() {
		game.state.level = 24;
		game.SetGameMode(Game.Mode.Play);
	}
	
	public void ToNewGame() {
		game.state.level = 3;
		game.SetGameMode(Game.Mode.Play);
	}

	public void ToContinueGame() {
		game.SetGameMode(Game.Mode.Play);
	}

	public void ToUpgrade() {
		game.BuyProduct(0);
	}
	
	public void ToCredits() {
/*		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y + 0.3f, gui.containers[dialogContainer].GetCenter().z-2f), false);
		gui.AddLabel(state.GetDialog(5), dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.25f, 1, Game.GUI_UV_RADIO_BOX_HEADER);
		gui.AddImageLabel(state.GetDialog(6),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.0125f), 0.25f, 0, Game.GUI_UV_FIELD,
			1, Game.GUI_UV_CREDITS_BUTTON, 0.1f);
		gui.AddLabel(" ", dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.125f, 1, Game.GUI_UV_RADIO_BOX_FOOTER);*/
	}

	public void ToDisclaimerDialog() {
/*		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.95f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y + 0.3f, gui.containers[dialogContainer].GetCenter().z-2f), false);
		gui.AddLabel(state.GetDialog(13), dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.25f, 1, Game.GUI_UV_RADIO_BOX_HEADER);
		gui.AddImageLabel(state.GetDialog(14),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.0125f), 0.25f, 0, Game.GUI_UV_FIELD,
			1, Game.GUI_UV_CREDITS_BUTTON, 0.1f);
		gui.AddLabel(" ", dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.125f, 1, Game.GUI_UV_RADIO_BOX_FOOTER);*/
	}
	
	private void CloseDialog() {
		gui.CloseDialog(dialogContainer);
		if (inDisclaimerDialog) {
			ToHelp();
			inDisclaimerDialog = false;;
		}
	}
	
	public void ToRateIt() {
		game.RateIt();
	}
	
	public void ToHelp() {
/*		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.95f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y + 0.45f, gui.containers[dialogContainer].GetCenter().z-2f), false);
		gui.AddLabel(state.GetDialog(7), dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.25f, 1, Game.GUI_UV_RADIO_BOX_HEADER);
#if UNITY_ANDROID || UNITY_IPHONE
		gui.AddImageLabel(state.GetDialog(8),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.0125f), 0.25f, 0, Game.GUI_UV_FIELD,
			2, Game.GUI_UV_SERVER_BUTTON, 0.1f);
#endif
		gui.AddImageLabel(state.GetDialog(9),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.0125f), 0.25f, 0, Game.GUI_UV_FIELD,
			2, Game.GUI_UV_CLIENT_BUTTON, 0.1f);
		gui.AddImageLabel(state.GetDialog(10),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.075f), 0.25f, 0, Game.GUI_UV_FIELD,
			1, Game.GUI_UV_PREFERENCES_BUTTON, 0.1f);
		gui.AddImageLabel(state.GetDialog(11),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.075f), 0.25f, 0, Game.GUI_UV_FIELD,
			1, Game.GUI_UV_RATEIT_BUTTON, 0.1f);
		gui.AddImageLabel(state.GetDialog(12),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.075f), 0.25f, 0, Game.GUI_UV_FIELD,
			1, Game.GUI_UV_CREDITS_BUTTON, 0.1f);
		gui.AddLabel(" ", dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.125f, 1, Game.GUI_UV_RADIO_BOX_FOOTER);*/
	}

	public void ToTrial() {
/*		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y + 0.3f, gui.containers[dialogContainer].GetCenter().z-2f), false);
		gui.AddLabel(state.GetDialog(38), dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.25f, 1, Game.GUI_UV_RADIO_BOX_HEADER);
		gui.AddImageLabel(state.GetDialog(40),
			dialogBox, MyGUI.GUIAlignment.Left, MyGUI.GUIBackground.NinePatch, 0f, new Vector4(0.125f, 0f, 0.0125f, 0.0125f), 0.25f, 0, Game.GUI_UV_FIELD,
			0, Game.GUI_UV_WARNING_SIGN, 0.1f);
		TouchDelegate toUpgrade = new TouchDelegate(ToUpgrade);
		gui.AddButton(dialogBox, new Vector3(0.15f, 0.15f, 1.0f), toUpgrade, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, -0.15f, Game.GUI_UV_UPGRADE, 0);
		gui.AddLabel(" ", dialogBox, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.NinePatch, 0f, 0.0125f, 0.125f, 1, Game.GUI_UV_RADIO_BOX_FOOTER);*/
	}

	public void ToPreferences() {
		CancelInvoke();
		Destroy(gui.gameObject);
		game.SetGameMode(Game.Mode.Preferences);
	}
	
	public void ToQuit() {
		Application.Quit();
//		StartCoroutine(Quit());
	}
	
/*	private IEnumerator Quit() {
		yield return new WaitForSeconds(3.0f);
		Application.Quit();
	}*/
	
	private void ScaleUpgradeButton() {
		upgradeButtonAnim.Play("UpgradeButtonScale");
	}
	
	void onDisable() {
		CancelInvoke();
	}
}

