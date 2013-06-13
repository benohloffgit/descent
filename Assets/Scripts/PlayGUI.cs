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
	private Image primaryWeapon;
	private Image secondaryWeapon;
	private Image healthDigit0;
	private Image healthDigit1;
	private Image healthDigit2;
	private Image shieldDigit0;
	private Image shieldDigit1;
	private Image shieldDigit2;
	private Image zoneIDDigit0;
	private Image zoneIDDigit1;
	private Image[] keysFound;
	private Image[] keysEmpty;
	private Image[] damageIndicators;
	private Image doorClosed;
	private Image doorOpen;
	private Image lights;
	private Image exitHelper;
	private Image shipBoost;
	private int[] healthCount;
	private int[] shieldCount;
	
	private float lastHealthCountTime;
	private float lastShieldCountTime;
	private float[] damageIndicatorTimers;
	private bool isOneDamageIndicatorActive;
	
	private List<Enemy> enemyHUDInfo;
	private int topContainer;
	private Transform shipTransform;
	private Camera shipCamera;
	private int[] enemyHUDInfoLabels;
	private int enemyLockMissileLabel;
	
	private static int MAX_ENEMY_HUD_INFOS = 5;
	private static float ENEMY_HUD_INFO_MAX_TIME = 5.0f;
	private static float DAMAGE_INDICATOR_DURATION = 1.0f;
	
	private static Vector3 ENEMY_HUD_OFFSET_GLOBAL = new Vector3(0.5f, 0.5f, 0f);
	private static Vector3 ENEMY_HUD_OFFSET_LOCAL = new Vector3(-3.0f, -3.0f, 0f);
	private static Vector3 ENEMY_LOCK_OFFSET_LOCAL = new Vector3(3.0f, 3.0f, 0f);
	private static float TICK_DELTA = 0.05f;
	private static Vector4[] DIGITS = new Vector4[] {Game.GUI_UV_NUMBER_0, Game.GUI_UV_NUMBER_1, Game.GUI_UV_NUMBER_2, Game.GUI_UV_NUMBER_3, Game.GUI_UV_NUMBER_4,
												Game.GUI_UV_NUMBER_5, Game.GUI_UV_NUMBER_6, Game.GUI_UV_NUMBER_7, Game.GUI_UV_NUMBER_8, Game.GUI_UV_NUMBER_9};
	
	private static Vector4[] PRIMARY_WEAPONS = new Vector4[] {Game.GUI_UV_GUN,Game.GUI_UV_LASER,Game.GUI_UV_TWINGUN,Game.GUI_UV_PHASER,Game.GUI_UV_TWINLASER,Game.GUI_UV_GAUSS,Game.GUI_UV_TWINPHASER,Game.GUI_UV_TWINGAUSS};
	private static Vector4[] SECONDARY_WEAPONS = new Vector4[] {Game.GUI_UV_MISSILE,Game.GUI_UV_GUIDEDMISSILE,Game.GUI_UV_CHARGEDMISSILE,Game.GUI_UV_DETONATORMISSILE};
	
	public PlayGUI(Play p) {
		play = p;
//		Reset();
		
		keysFound = new Image[2];
		keysEmpty = new Image[2];
		damageIndicators = new Image[4];
		damageIndicatorTimers = new float[4];
			
		gui = play.game.gui;
		container = gui.AddContainer();
		
		Vector3 fullSize = gui.containers[container].GetSize();
		Vector3 screenCenter = gui.containers[container].GetCenter();
		topContainer = gui.AddContainer(container, fullSize, new Vector2(screenCenter.x, screenCenter.y), true);
		
		int imageId;
		// zone ID
		imageId = gui.AddImage(topContainer, new Vector3(0.1f, 0.1f, 1f), MyGUI.GUIAlignment.Center, 0.04f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_NUMBER_0, 0);
		zoneIDDigit0 = gui.images[imageId];
		imageId = gui.AddImage(topContainer, new Vector3(0.1f, 0.1f, 1f), MyGUI.GUIAlignment.Center, -0.04f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_NUMBER_0, 0);
		zoneIDDigit1 = gui.images[imageId];
		
		// power ups / special capabilities
		int powerUpContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Right, 0.02f, MyGUI.GUIAlignment.Top, 0.03f);
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_LIGHTS, 0);
		lights = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 2.2f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_EXITHELPER, 0);
		exitHelper = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 4.4f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_SHIPBOOST, 0);
		shipBoost = gui.images[imageId];
		
		// ship health
		int healthContainer = gui.AddContainer(topContainer, new Vector3(0.06f, 0.06f, 1.0f), true, MyGUI.GUIAlignment.Center, 0.25f, MyGUI.GUIAlignment.Top, 0.01f);
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, -1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 2);
		healthDigit0 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 2);
		healthDigit1 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 2);
		healthDigit2 = gui.images[imageId];
		gui.AddImage(healthContainer, new Vector3(0.05f, 0.05f, 1f), MyGUI.GUIAlignment.Center, -0.1f, MyGUI.GUIAlignment.Top, 0.1f, Game.GUI_UV_HEALTH, 0);
		
		// ship shield
		int shieldContainer = gui.AddContainer(topContainer, new Vector3(0.06f, 0.06f, 1.0f), true, MyGUI.GUIAlignment.Center, -0.25f, MyGUI.GUIAlignment.Top, 0.01f);
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, -1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit0 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit1 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 1.5f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit2 = gui.images[imageId];
		gui.AddImage(shieldContainer, new Vector3(0.05f, 0.05f, 1f), MyGUI.GUIAlignment.Center, 0.1f, MyGUI.GUIAlignment.Top, 0.1f, Game.GUI_UV_SHIELD, 0);
		
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
		
		// damage indicators
		int damageIndicatorContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f);
		imageId = gui.AddImage(damageIndicatorContainer, MyGUI.GUIAlignment.Left, 6f, MyGUI.GUIAlignment.Center, 0.0f, Game.GUI_UV_DAMAGEINDICATOR, 0);
		damageIndicators[0] = gui.images[imageId];
		damageIndicators[0].transform.Rotate(Vector3.forward, -90f);
		imageId = gui.AddImage(damageIndicatorContainer, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -6f, Game.GUI_UV_DAMAGEINDICATOR, 0);
		damageIndicators[1] = gui.images[imageId];
		imageId = gui.AddImage(damageIndicatorContainer, MyGUI.GUIAlignment.Right, 6f, MyGUI.GUIAlignment.Center, 0.0f, Game.GUI_UV_DAMAGEINDICATOR, 0);
		damageIndicators[2] = gui.images[imageId];
		damageIndicators[2].transform.Rotate(Vector3.forward, 90f);
		imageId = gui.AddImage(damageIndicatorContainer, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, -6f, Game.GUI_UV_DAMAGEINDICATOR, 0);
		damageIndicators[3] = gui.images[imageId];
		damageIndicators[3].transform.Rotate(Vector3.forward, 180f);
		
		enemyHUDInfoLabels = new int[MAX_ENEMY_HUD_INFOS];
		gui.SetActiveTextMaterial(5);
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			enemyHUDInfoLabels[i] = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		}

		enemyLockMissileLabel = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		
		// weapons
		int weaponsContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.03f);
		imageId = gui.AddImage(weaponsContainer, MyGUI.GUIAlignment.Center, -0.5f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_TRANS, 0);
		primaryWeapon = gui.images[imageId];
		imageId = gui.AddImage(weaponsContainer, MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_TRANS, 0);
		secondaryWeapon = gui.images[imageId];
		gui.SetActiveTextMaterial(4);
		primaryWeaponLabel = gui.AddLabel("", weaponsContainer, new Vector3(0.04f,0.04f,0.1f), MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		secondaryWeaponLabel = gui.AddLabel("", weaponsContainer, new Vector3(0.04f,0.04f,0.1f), MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		
		
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
		for (int i=0; i<4; i++) {
			damageIndicators[i].myRenderer.enabled = false;
		}
		isOneDamageIndicatorActive = false;
		doorOpen.myRenderer.enabled = false;
		doorClosed.myRenderer.enabled = true;
		gui.labelsCC[primaryWeaponLabel].SetText("");
		gui.labelsCC[secondaryWeaponLabel].SetText("");
		primaryWeapon.SetUVMapping(Game.GUI_UV_TRANS);
		secondaryWeapon.SetUVMapping(Game.GUI_UV_TRANS);
		SwitchHeadlight();
		SwitchExitHelper();
		SwitchShipBoost();
		DisplayZoneID();
	}

	public void Activate() {
		gui.containers[container].gameObject.SetActiveRecursively(true);
	}
	
	public void Deactivate() {
		gui.containers[container].gameObject.SetActiveRecursively(false);
	}

	public void SwitchHeadlight() {
		lights.myRenderer.enabled = ship.isHeadlightOn;
	}

	public void SwitchExitHelper() {
		exitHelper.myRenderer.enabled = ship.isExitHelperLaunched;
	}

	public void SwitchShipBoost() {
		shipBoost.myRenderer.enabled = ship.isBoosterOn;
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
	
	private void DisplayZoneID() {
		zoneIDDigit0.SetUVMapping(DIGITS[MyGUI.GetDigitOfNumber(0, play.zoneID+1)]);
		zoneIDDigit1.SetUVMapping(DIGITS[MyGUI.GetDigitOfNumber(1, play.zoneID+1)]);
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
		
		if (isOneDamageIndicatorActive) {
			isOneDamageIndicatorActive = false;
			for (int i=0; i<4; i++) {
				if (damageIndicators[i].myRenderer.enabled) {
					if ( Time.fixedTime > damageIndicatorTimers[i] + DAMAGE_INDICATOR_DURATION) {
						damageIndicators[i].myRenderer.enabled = false;
					} else {
						isOneDamageIndicatorActive = true;
					}
				}
			}
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
		gui.labelsCC[enemyHUDInfoLabels[index]].SetText(
			e.clazz + " " + e.displayModel.ToString("00")
				+ " [H:" + e.health + "]"
				+ " (F:"+ Mathf.RoundToInt(e.firepowerPerSecond) + ")" 
		);
		gui.labelsCC[enemyHUDInfoLabels[index]].transform.localPosition =
			Calculate2DHUDPosition(e.transform.position, ship.transform.TransformDirection(ENEMY_HUD_OFFSET_LOCAL) * e.radius * 0.5f, e);
		gui.labelsCC[enemyHUDInfoLabels[index]].myRenderer.enabled = true;
	}
	
	private void ShowEnemyMissileLockInfo() {
		Enemy e = ship.lockedEnemy;
		if (e != null) {
			gui.labelsCC[enemyLockMissileLabel].SetText("L: " 
				+ (Ship.MISSILE_LOCK_DURATION - Mathf.Clamp(Mathf.FloorToInt(Time.time-ship.missileLockTime),0,Ship.MISSILE_LOCK_DURATION)));
			gui.labelsCC[enemyLockMissileLabel].transform.localPosition =
				Calculate2DHUDPosition(e.transform.position, ship.transform.TransformDirection(ENEMY_LOCK_OFFSET_LOCAL) * e.radius * 0.5f, e);
		}
	}
	
	private Vector2 Calculate2DHUDPosition(Vector3 pos, Vector3 offset, Enemy e) {
		Vector3 p = shipCamera.WorldToViewportPoint(pos + offset);
		if (p.z > 0) { // in front of us
			return new Vector2(
				Mathf.Clamp(p.x - ENEMY_HUD_OFFSET_GLOBAL.x, -0.45f, 0.45f),
				Mathf.Clamp(p.y - ENEMY_HUD_OFFSET_GLOBAL.y, -0.45f, 0.45f)
			);
		} else {
			float x = -Mathf.Clamp(p.x - ENEMY_HUD_OFFSET_GLOBAL.x, -0.45f, 0.45f);
			float y = -Mathf.Clamp(p.y - ENEMY_HUD_OFFSET_GLOBAL.y, -0.45f, 0.45f);
			if (Mathf.Abs(x) > Mathf.Abs(y)) {
				y = 0.45f * Mathf.Sign(y);
			} else {
				x = 0.45f * Mathf.Sign(x);
			}
			return new Vector2(x, y);
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
	
	public void IndicateDamage(Vector3 worldPos) {
		isOneDamageIndicatorActive = true;
		Vector3 p = shipCamera.WorldToViewportPoint(worldPos); // 1.0, -1.7, -0.3
//		Vector3 q = shipCamera.WorldToScreenPoint(worldPos); // 1.0, -1.7, -0.3
//		Debug.Log (p + " " + q);
		if (p.z < 0) p *= -1f;
		if (p.x < 0.5f) {
			damageIndicators[2].myRenderer.enabled = true;
			damageIndicatorTimers[2] = Time.fixedTime;
		} else if (p.x > 0.5f) {
			damageIndicators[0].myRenderer.enabled = true;
			damageIndicatorTimers[0] = Time.fixedTime;
		}
		if (p.y < 0.5f) {
			damageIndicators[3].myRenderer.enabled = true;
			damageIndicatorTimers[3] = Time.fixedTime;
		} else if (p.y > 0.5f) {
			damageIndicators[1].myRenderer.enabled = true;
			damageIndicatorTimers[1] = Time.fixedTime;
		}
	}
	
	public void CloseDialog() {
		gui.CloseDialog(dialogContainer);
		Screen.showCursor = false;
		Screen.lockCursor = true;
	}
	
	public void DisplayPrimaryWeapon(Weapon w) {
		primaryWeapon.SetUVMapping(PRIMARY_WEAPONS[w.type]);
		gui.labelsCC[primaryWeaponLabel].SetText(Weapon.PRIMARY_TYPES[w.type] + " FP: " + w.damage);
	}
	
	public void DisplaySecondaryWeapon(Weapon w) {
		secondaryWeapon.SetUVMapping(SECONDARY_WEAPONS[w.type]);
		gui.labelsCC[secondaryWeaponLabel].SetText(Weapon.SECONDARY_TYPES[w.type] + " A: "+ w.ammunition);
	}
	
	private void ToMenu() {
		CloseDialog();
		play.BackToMenu();
	}
	
	private void ToGame() {
		CloseDialog();
		play.SetPaused(false);
	}

	private void ToNextZone() {
		CloseDialog();
		play.NextZone();
	}
	
	private void ToRetrySokoban() {
		play.RetrySokoban();
	}
	
	private void ToQuitSokoban() {
		play.SwitchMode();
	}
	
	public void ToStory() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, gui.GetSize(),
			new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-1f), true);
		int scrollableContainer = gui.AddContainerScrollable(dialogBox, new Vector3(gui.GetSize().x, gui.GetSize().y * 0.4f,1f),
			new Vector2(gui.GetCenter().x, gui.GetCenter().y + gui.GetSize().y/2f * 0.5f), MyGUI.GUIBackground.QuadWithCollider,
//			new Vector2(gui.GetCenter().x, gui.GetCenter().y + gui.GetSize().y/2f * 0.5f), MyGUI.GUIBackground.QuadWithCollider,
			0, Game.GUI_UV_COLOR_BLACK, 0, Game.GUI_UV_NULL, 0, Game.GUI_UV_NULL);
		gui.AddLabel("How the hell did I end up in this situation? I tried to ignore the fact that I knew the answer as I armed and charged the pulsar lasers on the only ship on the planet. You see I had graduated from the cybernautics and neural networking division of the university with barely a pass. My compatriots were all off to Starship headquarters, or the prestigious planets, Veluma and Tratoria. Whilst I had been commissioned to join the prison company, Illicitus. Not the worst assignment for a third-rate robotic neural scientist I thought at the time. But the last few days on this hell-hole had changed my opinion somewhat. The hum of the charging lasers stopped, signaling it was time to die ...",
			scrollableContainer, MyGUI.GUIAlignment.Center, 0f, 0f, 0.3f, 3);
		
		gui.containers[dialogBox].AddZLevel(2f);
		gui.AddImage(dialogBox, new Vector3(gui.GetSize().x, gui.GetSize().y*0.2f, gui.GetSize().z),
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.AddImage(dialogBox, new Vector3(gui.GetSize().x, gui.GetSize().y*0.2f, gui.GetSize().z),
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.AddLabel(play.game.state.GetDialog(8) + (play.zoneID+1), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center,
			0f, MyGUI.GUIAlignment.Top, 0.1f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButton(dialogBox, new Vector3(0.05f,0.05f,1f), toMenu, play.game.state.GetDialog(9), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.1f, MyGUI.GUIAlignment.Bottom, 0.1f, Game.GUI_UV_COLOR_BLACK, 0);
		TouchDelegate toNextZone = new TouchDelegate(ToNextZone);
		gui.AddLabelButton(dialogBox, new Vector3(0.05f,0.05f,1f), toNextZone, play.game.state.GetDialog(10), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.1f, MyGUI.GUIAlignment.Bottom, 0.1f, Game.GUI_UV_COLOR_BLACK, 0);
	}
	
	public void ToQuit() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, closeDialog, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(play.game.state.GetDialog(5), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toMenu, play.game.state.GetDialog(6), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.1f, MyGUI.GUIAlignment.Center, -0.1f, Game.GUI_UV_NULL, 0);
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toGame, play.game.state.GetDialog(7), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.1f, MyGUI.GUIAlignment.Center, -0.1f, Game.GUI_UV_NULL, 0);
	}

	public void ToSokoban() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);
		
		int dialogBox = gui.AddContainer(dialogContainer, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		TouchDelegate toRetrySokoban = new TouchDelegate(ToRetrySokoban);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toRetrySokoban, play.game.state.GetDialog(11), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -(gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_NULL, 0);
		TouchDelegate toQuitSokoban = new TouchDelegate(ToQuitSokoban);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toQuitSokoban, play.game.state.GetDialog(12), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, (gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_NULL, 0);
	}
	
}