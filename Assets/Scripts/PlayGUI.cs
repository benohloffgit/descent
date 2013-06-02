using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayGUI {
//	public int currentHealth;
//	public int currentShield;
	
	private Play play;
	private Ship ship;
	
	private MyGUI gui;
	private int container;
	private int dialogContainer;
	
	private int displayedHealth;
	private int displayedShield;
	private int toBeDisplayedHealth;
	private int toBeDisplayedShield;
	private int primaryWeaponLabel;
	private int secondaryWeaponLabel;
	private Image healthDigit0;
	private Image healthDigit1;
	private Image healthDigit2;
	private Image shieldDigit0;
	private Image shieldDigit1;
	private Image shieldDigit2;
	private Image[] keysFound;
	private Image[] keysEmpty;
	private Image doorClosed;
	private Image doorOpen;
	private int[] healthCount;
	private int[] shieldCount;
	
	private float lastHealthCountTime;
	private float lastShieldCountTime;
	
	private List<Enemy> enemyHUDInfo;
	private int topContainer;
	private Transform shipTransform;
	private Camera shipCamera;
	private int[] enemyHUDInfoLabels;
	private int enemyLockMissileLabel;
	
	private static int MAX_ENEMY_HUD_INFOS = 5;
	private static float ENEMY_HUD_INFO_MAX_TIME = 5.0f;
	
	private static Vector3 ENEMY_HUD_OFFSET_GLOBAL = new Vector3(0.5f, 0.5f, 0f);
	private static Vector3 ENEMY_HUD_OFFSET_LOCAL = new Vector3(-1.0f, -1.0f, 0f);
	private static Vector3 ENEMY_LOCK_OFFSET_LOCAL = new Vector3(1.0f, 1.0f, 0f);
	private static float TICK_DELTA = 0.05f;
	private static Vector4[] DIGITS = new Vector4[] {Game.GUI_UV_NUMBER_0, Game.GUI_UV_NUMBER_1, Game.GUI_UV_NUMBER_2, Game.GUI_UV_NUMBER_3, Game.GUI_UV_NUMBER_4,
												Game.GUI_UV_NUMBER_5, Game.GUI_UV_NUMBER_6, Game.GUI_UV_NUMBER_7, Game.GUI_UV_NUMBER_8, Game.GUI_UV_NUMBER_9};
		
	public PlayGUI(Play p) {
		play = p;
//		Reset();
		
		keysFound = new Image[2];
		keysEmpty = new Image[2];
		
		gui = play.game.gui;
		container = gui.AddContainer();
		
		Vector3 fullSize = gui.containers[container].GetSize();
		Vector3 screenCenter = gui.containers[container].GetCenter();
		topContainer = gui.AddContainer(container, fullSize, new Vector2(screenCenter.x, screenCenter.y), true);

		// ship health
		int healthContainer = gui.AddContainer(topContainer, new Vector3(0.06f, 0.06f, 1.0f), true, MyGUI.GUIAlignment.Right, 0.02f, MyGUI.GUIAlignment.Top, 0.14f);
		int imageId;
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 0);
		healthDigit0 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 0);
		healthDigit1 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 3.0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 0);
		healthDigit2 = gui.images[imageId];
		
		// ship shield
		int shieldContainer = gui.AddContainer(topContainer, new Vector3(0.06f, 0.06f, 1.0f), true, MyGUI.GUIAlignment.Right, 0.02f, MyGUI.GUIAlignment.Top, 0.01f);
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 0.0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit0 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit1 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 3.0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit2 = gui.images[imageId];
		
		// keys and door
		int keyAndDoorContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Left, 0.02f, MyGUI.GUIAlignment.Top, 0.03f);
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_DOOR_CLOSED, 0);
		doorClosed = gui.images[imageId];
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_DOOR_OPEN, 0);
		doorOpen = gui.images[imageId];
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 2f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_KEY_EMPTY, 0);
		keysEmpty[CollecteableKey.TYPE_SILVER] = gui.images[imageId];
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 2f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_KEY_SILVER, 0);
		keysFound[CollecteableKey.TYPE_SILVER] = gui.images[imageId];
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 4f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_KEY_EMPTY, 0);
		keysEmpty[CollecteableKey.TYPE_GOLD] = gui.images[imageId];
		imageId = gui.AddImage(keyAndDoorContainer, MyGUI.GUIAlignment.Left, 4f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_KEY_GOLD, 0);
		keysFound[CollecteableKey.TYPE_GOLD] = gui.images[imageId];
		
		enemyHUDInfoLabels = new int[MAX_ENEMY_HUD_INFOS];
		gui.SetActiveTextMaterial(5);
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			enemyHUDInfoLabels[i] = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		}

		enemyLockMissileLabel = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		
		gui.SetActiveTextMaterial(4);
		primaryWeaponLabel = gui.AddLabel("", topContainer, new Vector3(0.05f,0.05f,0.1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.1f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		secondaryWeaponLabel = gui.AddLabel("", topContainer, new Vector3(0.05f,0.05f,0.1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		
		
//		ticks = 0;
		healthCount = new int[3];
		shieldCount = new int[3];
	}
	
	public void Initialize() {
		ship = play.ship;
		displayedHealth = ship.health;
		displayedShield = ship.shield;
		toBeDisplayedHealth = displayedHealth;
		toBeDisplayedShield = displayedShield;
		DisplayHealth(new int[] { MyGUI.GetDigitOfNumber(0, ship.health), MyGUI.GetDigitOfNumber(1, ship.health), MyGUI.GetDigitOfNumber(2, ship.health)});
		DisplayShield(new int[] { MyGUI.GetDigitOfNumber(0, ship.shield), MyGUI.GetDigitOfNumber(1, ship.shield), MyGUI.GetDigitOfNumber(2, ship.shield)});
		shipTransform = ship.transform;
		shipCamera = ship.shipCamera;
	}
	
	public void Reset() {
		enemyHUDInfo = new List<Enemy>();
		keysFound[CollecteableKey.TYPE_SILVER].myRenderer.enabled = false;
		keysFound[CollecteableKey.TYPE_GOLD].myRenderer.enabled = false;
		keysEmpty[CollecteableKey.TYPE_SILVER].myRenderer.enabled = true;
		keysEmpty[CollecteableKey.TYPE_GOLD].myRenderer.enabled = true;
		doorOpen.myRenderer.enabled = false;
		doorClosed.myRenderer.enabled = true;
	}

	public void Activate() {
		gui.containers[container].gameObject.SetActiveRecursively(true);
	}
	
	public void Deactivate() {
		gui.containers[container].gameObject.SetActiveRecursively(false);
	}

	public void DisplayKey(int keyType) {
		keysFound[keyType].myRenderer.enabled = true;
		keysEmpty[keyType].myRenderer.enabled = false;
	}

	public void DisplayDoorOpen() {
		doorClosed.myRenderer.enabled = false;
		doorOpen.myRenderer.enabled = true;
	}
	
	public void SetHealthCount(int v) {
		healthCount = new int[] { MyGUI.GetDigitOfNumber(0, v), MyGUI.GetDigitOfNumber(1, v), MyGUI.GetDigitOfNumber(2, v)}; // right to left
//		lastHealthCountTime = Time.time;
	}

	public void SetShieldCount(int v) {
		shieldCount = new int[] { MyGUI.GetDigitOfNumber(0, v), MyGUI.GetDigitOfNumber(1, v), MyGUI.GetDigitOfNumber(2, v)}; // right to left
//		lastShieldCountTime = Time.time;
	}
	
	public void DisplayHealth(int[] digits) {
		healthCount = digits;
		DisplayHealth();
	}
	
	private void DisplayHealth() {
//		Debug.Log (count[2] +" " + count[1] +" " +count[0]);
		healthDigit0.SetUVMapping(DIGITS[healthCount[0]]);
		healthDigit1.SetUVMapping(DIGITS[healthCount[1]]);
		healthDigit2.SetUVMapping(DIGITS[healthCount[2]]);
	}
	
	public void DisplayShield(int[] digits) {
		shieldCount = digits;
		DisplayShield();
	}
	
	private void DisplayShield() {
		shieldDigit0.SetUVMapping(DIGITS[shieldCount[0]]);
		shieldDigit1.SetUVMapping(DIGITS[shieldCount[1]]);
		shieldDigit2.SetUVMapping(DIGITS[shieldCount[2]]);
	}
		
//	public void DispatchUpdate() {
//	}
	
	public void DispatchFixedUpdate() {
		if (toBeDisplayedShield != ship.shield) {
//			Debug.Log ("toBeDisplayedShield " + ship.shield + " " + displayedShield);
			SetShieldCount(displayedShield);
			toBeDisplayedShield = ship.shield;
		} else if (displayedShield == ship.shield && toBeDisplayedHealth != ship.health) {
//			Debug.Log ("toBeDisplayedHealth " + ship.health + " "  + displayedHealth);
			SetHealthCount(displayedHealth);
			toBeDisplayedHealth = ship.health;
		}
			
		if (displayedShield > toBeDisplayedShield && Time.time > lastShieldCountTime + TICK_DELTA) {
			LowerCount(ref shieldCount);
			lastShieldCountTime = Time.time;
			displayedShield--;
			DisplayShield();
		} else if (displayedShield < toBeDisplayedShield && Time.time > lastShieldCountTime + TICK_DELTA) {
			RaiseCount(ref shieldCount);
			lastShieldCountTime = Time.time;
			displayedShield++;
			DisplayShield();
		} else if (displayedHealth > toBeDisplayedHealth && Time.time > lastHealthCountTime + TICK_DELTA) {
			LowerCount(ref healthCount);
			lastHealthCountTime = Time.time;
			displayedHealth--;
			DisplayHealth();
		} else if (displayedHealth < toBeDisplayedHealth && Time.time > lastHealthCountTime + TICK_DELTA) {
			RaiseCount(ref healthCount);
			lastHealthCountTime = Time.time;
			displayedHealth++;
			DisplayHealth();
		}

		// Enemy target info
//		Debug.Log (shipTransform.position + " " + shipTransform.forward);
		int removed = 0;
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			if (enemyHUDInfo.Count > i) {
				if (enemyHUDInfo[i-removed] != null && enemyHUDInfo[i-removed].lastTimeHUDInfoRequested + ENEMY_HUD_INFO_MAX_TIME > Time.time ) {					
/*					if (enemyHUDInfo[i-removed] == null) {
						Debug.Log ("enemy is null " + enemyHUDInfo[i-removed] + " " + Time.time);
						Debug.Log ("enemy is null " + enemyHUDInfo[i-removed].lastTimeHUDInfoRequested);
					}*/
					ShowEnemyHUDInfo(i, enemyHUDInfo[i-removed]);
				} else {
					enemyHUDInfo.RemoveAt(i-removed);
					removed++;
					gui.labelsCC[enemyHUDInfoLabels[i]].myRenderer.enabled = false;
				}
			} else {
				gui.labelsCC[enemyHUDInfoLabels[i]].myRenderer.enabled = false;
			}
		}
		
		if (ship.missileLockMode != Ship.MissileLockMode.None) {
			ShowEnemyMissileLockInfo();
		} else {
			gui.labelsCC[enemyLockMissileLabel].SetText("");
		}
	}
	
	public void EnemyInSight(Enemy e) {
		e.lastTimeHUDInfoRequested = Time.time;
		if (!enemyHUDInfo.Contains(e)) {
			enemyHUDInfo.Add(e);
			if (enemyHUDInfo.Count > MAX_ENEMY_HUD_INFOS) {
				enemyHUDInfo.RemoveAt(0);
			}
		}
	}
		
	private void ShowEnemyHUDInfo(int index, Enemy e) {
//		if (e == null) Debug.Log ("enemy is null " + index);
//		Debug.Log (e.transform.TransformDirection(e.transform.localScale));
		Vector3 p = shipCamera.WorldToViewportPoint(e.transform.position
			+ ship.transform.TransformDirection(ENEMY_HUD_OFFSET_LOCAL) * e.radius * 0.5f);
		gui.labelsCC[enemyHUDInfoLabels[index]].SetText(
			e.clazz.ToUpper() + " " + e.displayModel.ToString("00")
				+ " [H:" + e.health + "]"
				+ " (F:"+ Mathf.RoundToInt(e.firepowerPerSecond) + ")" 
		);
		gui.labelsCC[enemyHUDInfoLabels[index]].transform.localPosition = new Vector2(
			Mathf.Clamp(p.x - ENEMY_HUD_OFFSET_GLOBAL.x, -0.45f, 0.45f),
			Mathf.Clamp(p.y - ENEMY_HUD_OFFSET_GLOBAL.y, -0.45f, 0.45f)
		);
		gui.labelsCC[enemyHUDInfoLabels[index]].myRenderer.enabled = true;
	}
	
	private void ShowEnemyMissileLockInfo() {
		Enemy e = ship.lockedEnemy;
		if (e != null) {
			Vector3 p = shipCamera.WorldToViewportPoint(e.transform.position
				+ ship.transform.TransformDirection(ENEMY_LOCK_OFFSET_LOCAL) * e.radius * 0.5f);
			gui.labelsCC[enemyLockMissileLabel].SetText("L: " 
				+ (Ship.MISSILE_LOCK_DURATION - Mathf.Clamp(Mathf.FloorToInt(Time.time-ship.missileLockTime),0,Ship.MISSILE_LOCK_DURATION)));
			gui.labelsCC[enemyLockMissileLabel].transform.localPosition = new Vector2(
				Mathf.Clamp(p.x - ENEMY_HUD_OFFSET_GLOBAL.x, -0.45f, 0.45f),
				Mathf.Clamp(p.y - ENEMY_HUD_OFFSET_GLOBAL.y, -0.45f, 0.45f)
			);
		}
	}
	
	public void RemoveEnemy(Enemy e) {
		if (enemyHUDInfo.Contains(e)) {
//			Debug.Log ("removed " + e);
			enemyHUDInfo.Remove(e);
		}
	}
	
	private void LowerCount(ref int[] count) {	
		// count[0] is rightmost!
		if (count[0] == 0) {
			count[0] = 9;
			if (count[1] == 0) {
				count[1] = 9;
				if (count[2] == 0) {
					// should not happen
				} else {
					count[2]--;
				}
			} else {
				count[1]--;
			}
		} else {
			count[0]--;
		}
	}

	private void RaiseCount(ref int[] count) {		
		// count[0] is rightmost!
		if (count[0] == 9) {
			count[0] = 0;
			if (count[1] == 9) {
				count[1] = 0;
				if (count[2] == 9) {
					// should not happen
				} else {
					count[2]++;
				}
			} else {
				count[1]++;
			}
		} else {
			count[0]++;
		}
	}
	
	public void CloseDialog() {
		gui.CloseDialog(dialogContainer);
		Screen.showCursor = false;
	}
	
	public void DisplayPrimaryWeapon(Weapon w) {
		gui.labelsCC[primaryWeaponLabel].SetText("T: " + Weapon.PRIMARY_TYPES[w.type] + " FP: " + w.damage);
	}
	
	public void DisplaySecondaryWeapon(Weapon w) {
		gui.labelsCC[secondaryWeaponLabel].SetText("T: " + Weapon.SECONDARY_TYPES[w.type] + " ("+ w.ammunition +")");
	}
	
	private void ToMenu() {
		CloseDialog();
		play.BackToMenu();
	}
	
	private void ToGame() {
		CloseDialog();
		play.SetPaused(false);
	}
	
	public void ToQuit() {
		Screen.showCursor = true;
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), false);
		gui.AddLabel(play.game.state.GetDialog(5), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toMenu, play.game.state.GetDialog(6), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.1f, MyGUI.GUIAlignment.Center, -0.1f, Game.GUI_UV_NULL, 0);
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toGame, play.game.state.GetDialog(7), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.1f, MyGUI.GUIAlignment.Center, -0.1f, Game.GUI_UV_NULL, 0);
	}
	
}