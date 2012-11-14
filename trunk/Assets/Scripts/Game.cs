using System;
using UnityEngine;

public class Game : MonoBehaviour {
	public GameObject statePrefab;
	
	public State state;
	public GameInput gameInput;
	public bool isInitialized = false;
	public enum Mode { Menu=0, Dialog=1, Play=2, None=3, Preferences=4 }

	public static Vector4 GUI_UV_TITLE = new Vector4(0f,0.5f,0.5f,1.0f);
	
	private Menu menu;
	private Play play;
	public bool showTrialDialog;
	private float volume;
	
	public static int MAX_WIDTH = 1536;
	
	void Awake() {
		DontDestroyOnLoad(this);
		volume = AudioListener.volume;
		showTrialDialog = false;
	}
	
	public void Initialize(Mode m) {
		GameObject o;
		if (!isInitialized) {
				
			o = Instantiate(statePrefab) as GameObject;
			state = o.GetComponent<State>();
			state.Initialize(this);
			
			gameInput = GetComponent<GameInput>();
										
			isInitialized = true;
		}
		
		gameInput.enabled = true;
		
		o = GameObject.Find("/Menu");
		if (o != null) {
			menu = o.GetComponent<Menu>();
			menu.Initialize(this, gameInput);
		}
		o = GameObject.Find("/Play");
		if (o != null) {
			play = o.GetComponent<Play>();
			play.Initialize(this, gameInput);
		}

		if (state.gameMode != Game.Mode.None) {
			SetGameMode(state.gameMode);
		} else {
			SetGameMode(m);
		}
	}
	
	public void SetGameMode(Mode mode) {
		gameInput.DeRegisterGUI();
		state.gameMode = mode;
		if (mode == Mode.Menu) {
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
			menu.gameObject.SetActiveRecursively(true);
			menu.Restart();
			play.gameObject.SetActiveRecursively(false);
		} else if (mode == Mode.Play) {
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
			play.gameObject.SetActiveRecursively(true);
			play.Restart();
			menu.gameObject.SetActiveRecursively(false);			
		}
	}
		
	public void NonGUIClickInDialog() {
	}
	
	public void DispatchGameInput() {
		if (state.gameMode == Mode.Play) {
			play.DispatchGameInput();
		}
	}
	
	public void PlayPayed() {
		BuyProduct(0);
	}
			
	public void RateIt() {
		state.jniBridge.ShowMarketDialog();
	}
			
	public void SwitchToProductAcquired(int id) {
		if (id == 0) {
			state.HideAd();
			if (state.gameMode == Game.Mode.Menu) {
				menu.RemovePaygate();
			}
		} else {
			state.ShowAd();
			if (state.gameMode == Game.Mode.Menu) {
				menu.ShowPaygate();
			}
		}
	}
	
	public void BuyProduct(int id) {
		state.jniBridge.BuyProduct(id);
//		state.jniBridge.TrackByFlurry(1);
	//	Debug.Log("buying product");
	}
	
}

