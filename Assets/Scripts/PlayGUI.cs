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
	private int scrollableContainer;
	
	private int displayedHealth;
	private int displayedShield;
	private int toBeDisplayedHealth;
	private int toBeDisplayedShield;
	private int primaryWeaponLabel;
	private int secondaryWeaponLabel;
	private LabelCC notifyLabel;
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
	private Image lightsOn;
	private Image lightsOff;
	private Image exitHelperOn;
	private Image exitHelperOff;
	private Image shipBoostOn;
	private Image shipBoostOff;
	private Image shipCloakOn;
	private Image shipCloakOff;
	private Image shipInvincibleOn;
	private Image shipInvincibleOff;
	private Image missileLockOn;
	private Image missileLockOff;
	private Image shipCroosHair;
	public ProgressBar boosterProgressBar;
	public ProgressBar cloakProgressBar;
	public ProgressBar shipHealthProgressBar;
	public ProgressBar shipShieldProgressBar;
	public ProgressBar shipMissileLoadingProgressBar;
	public ProgressBar chargedMissileProgressBar;
	private int[] healthCount;
	private int[] shieldCount;
	
	private float lastHealthCountTime;
	private float lastShieldCountTime;
	private float[] damageIndicatorTimers;
	private bool isOneDamageIndicatorActive;
	private Vector3 exitHelperScale;
	private Vector3 boosterScale;
	private Vector3 cloakScale;
	private Vector3 lightsScale;
	private Vector3 invincibleScale;
	private float notifyTimer;
	
	private List<Enemy> enemyHUDInfo;
	private int topContainer;
//	private Transform shipTransform;
	private Camera shipCamera;
	private int[] enemyHUDInfoLabels;
	private ProgressBar[] enemyHealthBars;
	private int enemyLockMissileLabel;
//	private int shipSecondaryWeaponDisplayed;
	private bool shipSecondaryWeaponLoadState;
	
	private static int MAX_ENEMY_HUD_INFOS = 5;
	private static float ENEMY_HUD_INFO_MAX_TIME = 5.0f;
	private static float DAMAGE_INDICATOR_DURATION = 1.0f;
	
	private static float POWER_UP_ANIM_SCALE = 0.10f;
	private static float POWER_UP_ANIM_SPEED = 4f;
	
	private static float NOTIFY_TIMER_BLEND_IN = 0.3f;
	private static float NOTIFY_TIMER_SHOW = 1.0f;
	
	private NotificationMode notificationMode;
	public enum NotificationMode { Off=0, BlendIn=2, Show=3, BlendOut=4 }
	
	private static Vector3 ENEMY_HUD_OFFSET_GLOBAL = new Vector3(0.5f, 0.5f, 0f);
	private static Vector3 ENEMY_HUD_OFFSET_LOCAL = new Vector3(-3.0f, 3.0f, 0f);
	private static Vector3 ENEMY_HEALTH_BAR_OFFSET_LOCAL = new Vector3(3.0f, -3.0f, 0f);
	private static Vector3 ENEMY_LOCK_OFFSET_LOCAL = new Vector3(3.0f, 3.0f, 0f);
	private static float TICK_DELTA = 0.05f;
	private static Vector4[] DIGITS_WHITE = new Vector4[] {Game.GUI_UV_NUMBER_0, Game.GUI_UV_NUMBER_1, Game.GUI_UV_NUMBER_2, Game.GUI_UV_NUMBER_3, Game.GUI_UV_NUMBER_4,
												Game.GUI_UV_NUMBER_5, Game.GUI_UV_NUMBER_6, Game.GUI_UV_NUMBER_7, Game.GUI_UV_NUMBER_8, Game.GUI_UV_NUMBER_9};
	private static Vector4[] DIGITS_RED = new Vector4[] {Game.GUI_UV_NUMBER_0+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_1+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_2+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_3+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_4+Game.GUI_UV_NUMBERS_RED_OFFSET,
												Game.GUI_UV_NUMBER_5+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_6+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_7+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_8+Game.GUI_UV_NUMBERS_RED_OFFSET, Game.GUI_UV_NUMBER_9+Game.GUI_UV_NUMBERS_RED_OFFSET};
	private static Vector4[] DIGITS_BLUE = new Vector4[] {Game.GUI_UV_NUMBER_0+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_1+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_2+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_3+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_4+Game.GUI_UV_NUMBERS_BLUE_OFFSET,
												Game.GUI_UV_NUMBER_5+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_6+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_7+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_8+Game.GUI_UV_NUMBERS_BLUE_OFFSET, Game.GUI_UV_NUMBER_9+Game.GUI_UV_NUMBERS_BLUE_OFFSET};
	
	public static Vector4[] PRIMARY_WEAPONS = new Vector4[] {Game.GUI_UV_GUN,Game.GUI_UV_LASER,Game.GUI_UV_TWINGUN,Game.GUI_UV_PHASER,Game.GUI_UV_TWINLASER,Game.GUI_UV_GAUSS,Game.GUI_UV_TWINPHASER,Game.GUI_UV_TWINGAUSS};
	public static Vector4[] SECONDARY_WEAPONS = new Vector4[] {Game.GUI_UV_MISSILE,Game.GUI_UV_GUIDEDMISSILE,Game.GUI_UV_CHARGEDMISSILE,Game.GUI_UV_DETONATORMISSILE};
	public static Vector4[] SECONDARY_WEAPONS_LOADING = new Vector4[] {Game.GUI_UV_MISSILE_LOADING,Game.GUI_UV_GUIDED_MISSILE_LOADING,Game.GUI_UV_CHARGED_MISSILE_LOADING,Game.GUI_UV_DETONATOR_MISSILE_LOADING};
	public static Vector4[] SPECIALS = new Vector4[] {Game.GUI_UV_LIGHTS, Game.GUI_UV_SHIPBOOST, Game.GUI_UV_CLOAK_ON, Game.GUI_UV_INVINCIBLE_ON};
	
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
		imageId = gui.AddImage(topContainer, new Vector3(0.1f, 0.1f, 1f), MyGUI.GUIAlignment.Center, 0.04f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_NUMBER_0, 1);
		zoneIDDigit0 = gui.images[imageId];
		imageId = gui.AddImage(topContainer, new Vector3(0.1f, 0.1f, 1f), MyGUI.GUIAlignment.Center, -0.04f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_NUMBER_0, 1);
		zoneIDDigit1 = gui.images[imageId];
		
		// notify label
		imageId = gui.AddLabel("", topContainer, new Vector3(0.1f,0.1f,0.1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.2f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		notifyLabel = gui.labelsCC[imageId];
		
		// Cross hair
		imageId = gui.AddImage(topContainer, new Vector3(0.075f, 0.075f, 1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_CROSS_HAIR, 0);
		shipCroosHair = gui.images[imageId];
			
		// power ups / special capabilities
		int powerUpContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Right, 0.02f, MyGUI.GUIAlignment.Top, 0.03f);
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_EXITHELPER_OFF, 0);
		exitHelperOff = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 0f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_EXITHELPER, 0);
		exitHelperOn = gui.images[imageId];
		exitHelperScale = exitHelperOn.transform.localScale;
		imageId = gui.AddImage(powerUpContainer, new Vector3(0.12f, 0.12f, 1.0f), MyGUI.GUIAlignment.Right, 2.2f, MyGUI.GUIAlignment.Top, -0.13f, Game.GUI_UV_LIGHTS_OFF, 0);
		lightsOff = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, new Vector3(0.12f, 0.12f, 1.0f), MyGUI.GUIAlignment.Right, 2.2f, MyGUI.GUIAlignment.Top, -0.13f, Game.GUI_UV_LIGHTS, 0);
		lightsOn = gui.images[imageId];
		lightsScale = lightsOn.transform.localScale;
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 4.4f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_SHIPBOOST_OFF, 0);
		shipBoostOff = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 4.4f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_SHIPBOOST, 0);
		shipBoostOn = gui.images[imageId];
		boosterScale = shipBoostOn.transform.localScale;
		imageId = gui.AddProgressBar(powerUpContainer, new Vector3(0.08f, 0.01f, 1f), MyGUI.GUIAlignment.Right, 4.4f, MyGUI.GUIAlignment.Top, 1.4f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_PROGRESS_FORE, 0);
		boosterProgressBar = gui.progressBars[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 6.6f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_CLOAK_OFF, 0);
		shipCloakOff = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 6.6f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_CLOAK_ON, 0);
		shipCloakOn = gui.images[imageId];
		cloakScale = shipCloakOn.transform.localScale;
		imageId = gui.AddProgressBar(powerUpContainer, new Vector3(0.08f, 0.01f, 1f), MyGUI.GUIAlignment.Right, 6.6f, MyGUI.GUIAlignment.Top, 1.4f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_PROGRESS_FORE, 0);
		cloakProgressBar = gui.progressBars[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 8.8f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_INVINCIBLE_OFF, 0);
		shipInvincibleOff = gui.images[imageId];
		imageId = gui.AddImage(powerUpContainer, MyGUI.GUIAlignment.Right, 8.8f, MyGUI.GUIAlignment.Top, 0f, Game.GUI_UV_INVINCIBLE_ON, 0);
		shipInvincibleOn = gui.images[imageId];
		invincibleScale = shipInvincibleOn.transform.localScale;
		
		
		// ship health
		int healthContainer = gui.AddContainer(topContainer, new Vector3(0.04f, 0.04f, 1.0f), true, MyGUI.GUIAlignment.Center, 0.235f, MyGUI.GUIAlignment.Top, 0.07f);
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Left, 6.5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		healthDigit0 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Left, 5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		healthDigit1 = gui.images[imageId];
		imageId = gui.AddImage(healthContainer, MyGUI.GUIAlignment.Left, 3.5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		healthDigit2 = gui.images[imageId];
		gui.AddImage(healthContainer, new Vector3(0.04f, 0.04f, 1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_HEALTH, 0);
		imageId = gui.AddProgressBar(healthContainer, new Vector3(0.3f, 0.0375f, 1f), MyGUI.GUIAlignment.Left, 0.25f, MyGUI.GUIAlignment.Top, -1f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_HEALTH_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_HEALTH_PROGRESS_FORE, 0);
		shipHealthProgressBar = gui.progressBars[imageId];
		
		// ship shield
		int shieldContainer = gui.AddContainer(topContainer, new Vector3(0.04f, 0.04f, 1.0f), true, MyGUI.GUIAlignment.Center, -0.235f, MyGUI.GUIAlignment.Top, 0.07f);
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 3.5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit0 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit1 = gui.images[imageId];
		imageId = gui.AddImage(shieldContainer, MyGUI.GUIAlignment.Right, 6.5f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_NUMBER_0, 1);
		shieldDigit2 = gui.images[imageId];
		gui.AddImage(shieldContainer, new Vector3(0.04f, 0.04f, 1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, 1.5f, Game.GUI_UV_SHIELD, 0);
		imageId = gui.AddProgressBar(shieldContainer, new Vector3(0.3f, 0.0375f, 1f), MyGUI.GUIAlignment.Right, 0.25f, MyGUI.GUIAlignment.Top, -1f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_SHIELD_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_SHIELD_PROGRESS_FORE, 0);
		shipShieldProgressBar = gui.progressBars[imageId];
		
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
		enemyHealthBars = new ProgressBar[MAX_ENEMY_HUD_INFOS];
		gui.SetActiveTextMaterial(5);
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			enemyHUDInfoLabels[i] = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
			imageId = gui.AddProgressBar(topContainer, new Vector3(0.08f, 0.01f, 1f), MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_HEALTH_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_HEALTH_PROGRESS_FORE, 0);
			enemyHealthBars[i] = gui.progressBars[imageId];
		}

		enemyLockMissileLabel = gui.AddLabel("", topContainer, new Vector3(0.03f,0.03f,1.0f),MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, 0f, 0.2f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		imageId = gui.AddImage(container, MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, Game.GUI_UV_MISSILE_LOCK_OFF, 0);
		missileLockOff = gui.images[imageId];
		missileLockOff.transform.localScale *= 0.05f;
		imageId = gui.AddImage(container, MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Top, 0.5f, Game.GUI_UV_MISSILE_LOCK_ON, 0);
		missileLockOn = gui.images[imageId];
		missileLockOn.transform.localScale *= 0.05f;
		
		// weapons
		int weaponsContainer = gui.AddContainer(topContainer, new Vector3(0.1f, 0.1f, 1.0f), true, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.03f);
		imageId = gui.AddImage(weaponsContainer, MyGUI.GUIAlignment.Center, -0.5f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_TRANS, 0);
		primaryWeapon = gui.images[imageId];
		imageId = gui.AddImage(weaponsContainer, MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_TRANS, 0);
		secondaryWeapon = gui.images[imageId];
		gui.SetActiveTextMaterial(4);
		primaryWeaponLabel = gui.AddLabel("", weaponsContainer, new Vector3(0.04f,0.04f,0.1f), MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, 0f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		secondaryWeaponLabel = gui.AddLabel("", weaponsContainer, new Vector3(0.04f,0.04f,0.1f), MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, 0f, 0f, 0.3f, 3, MyGUI.GUIBackground.None, Game.GUI_UV_NULL,0);
		imageId = gui.AddProgressBar(weaponsContainer, new Vector3(0.08f, 0.01f, 1f), MyGUI.GUIAlignment.Center, 0.5f, MyGUI.GUIAlignment.Center, -0.05f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_PROGRESS_FORE, 0);
		shipMissileLoadingProgressBar = gui.progressBars[imageId];
		
		// charged Missile Progress Bar
		imageId = gui.AddProgressBar(topContainer, new Vector3(0.3f, 0.0375f, 1f), MyGUI.GUIAlignment.Center, 00f, MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_YELLOW_PROGRESS_BACK, 0, MyGUI.GUIBackground.Quad, Game.GUI_UV_YELLOW_PROGRESS_FORE, 0);
		chargedMissileProgressBar = gui.progressBars[imageId];

		healthCount = new int[3];
		shieldCount = new int[3];
		shipSecondaryWeaponLoadState = false;
	}
	
	public void Initialize() {
		ship = play.ship;
		displayedHealth = ship.health;
		displayedShield = ship.shield;
		toBeDisplayedHealth = displayedHealth;
		toBeDisplayedShield = displayedShield;
		DisplayHealth(new int[] { MyGUI.GetDigitOfNumber(0, ship.health), MyGUI.GetDigitOfNumber(1, ship.health), MyGUI.GetDigitOfNumber(2, ship.health)});
		DisplayShield(new int[] { MyGUI.GetDigitOfNumber(0, ship.shield), MyGUI.GetDigitOfNumber(1, ship.shield), MyGUI.GetDigitOfNumber(2, ship.shield)});
//		shipTransform = ship.transform;
		shipCamera = ship.shipCamera;
	}
	
	public void Reset() {
		enemyHUDInfo = new List<Enemy>();
		notificationMode = NotificationMode.Off;
		Color c = notifyLabel.myRenderer.material.color;
		c.a = 0f;
		notifyLabel.myRenderer.material.color = c;
		keysFound[CollecteableKey.TYPE_SILVER].myRenderer.enabled = false;
		keysFound[CollecteableKey.TYPE_GOLD].myRenderer.enabled = false;
		keysEmpty[CollecteableKey.TYPE_SILVER].myRenderer.enabled = true;
		keysEmpty[CollecteableKey.TYPE_GOLD].myRenderer.enabled = true;
		for (int i=0; i<4; i++) {
			damageIndicators[i].myRenderer.enabled = false;
		}
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			enemyHealthBars[i].DisableRenderer();
		}
		isOneDamageIndicatorActive = false;
		doorOpen.myRenderer.enabled = false;
		doorClosed.myRenderer.enabled = true;
		missileLockOff.myRenderer.enabled = false;
		missileLockOn.myRenderer.enabled = false;
		shipMissileLoadingProgressBar.DisableRenderer();
		chargedMissileProgressBar.DisableRenderer();
			
		if (ship.currentPrimaryWeapon != -1) {
			DisplayPrimaryWeapon(ship.primaryWeapons[ship.currentPrimaryWeapon]);
			DisplayCrossHair();
		} else {
			gui.labelsCC[primaryWeaponLabel].SetText("");
			primaryWeapon.SetUVMapping(Game.GUI_UV_TRANS);
			HideCrossHair();
		}
		if (ship.currentSecondaryWeapon != -1) {
			DisplaySecondaryWeapon();
		} else {
			gui.labelsCC[secondaryWeaponLabel].SetText("");
			secondaryWeapon.SetUVMapping(Game.GUI_UV_TRANS);
		}
		
		DisplayZoneID();
		SwitchHeadlight();
		SwitchExitHelper();
		SwitchShipBoost();
		SwitchShipCloak();
		SwitchShipInvincible();
	}

	public void Activate() {
		gui.containers[container].gameObject.SetActiveRecursively(true);
	}
	
	public void Deactivate() {
		gui.containers[container].gameObject.SetActiveRecursively(false);
	}
	
	public void DisplayNotification(string text) {
		notifyLabel.SetText(text);
		notifyTimer = Time.fixedTime;
		notificationMode = NotificationMode.BlendIn;
		notifyLabel.myRenderer.enabled = true;
	}
		
	public void SwitchHeadlight() {
		if (ship.hasSpecial[Ship.SPECIAL_LIGHTS]) {
			if (ship.isHeadlightOn) {
				lightsOn.myRenderer.enabled = true;
				lightsOff.myRenderer.enabled = false;
			} else {
				lightsOn.myRenderer.enabled = false;
				lightsOff.myRenderer.enabled = true;
				lightsOn.transform.localScale = lightsScale;
			}
		} else {
			lightsOn.myRenderer.enabled = false;
			lightsOff.myRenderer.enabled = false;
		}
	}

	public void SwitchExitHelper() {
		exitHelperOn.myRenderer.enabled = true;
		exitHelperOff.myRenderer.enabled = false;
	}

	public void SwitchShipBoost() {
		if (ship.hasSpecial[Ship.SPECIAL_BOOST]) {
			if (!ship.isBoosterOn) {
				shipBoostOn.transform.localScale = boosterScale;
				if (ship.isBoosterLoading) {
					shipBoostOn.myRenderer.enabled = false;
					shipBoostOff.myRenderer.enabled = true;
					boosterProgressBar.EnableRenderer();
				} else {
					shipBoostOn.myRenderer.enabled = true;
					shipBoostOff.myRenderer.enabled = false;
					boosterProgressBar.DisableRenderer();
				}
			}
		} else {
			shipBoostOn.myRenderer.enabled = false;
			shipBoostOff.myRenderer.enabled = false;
			boosterProgressBar.DisableRenderer();
		}
	}

	public void SwitchShipCloak() {
		if (ship.hasSpecial[Ship.SPECIAL_CLOAK]) {
			if (!ship.isCloakOn) {
				shipCloakOn.transform.localScale = cloakScale;
				if (ship.isCloakLoading) {
					shipCloakOn.myRenderer.enabled = false;
					shipCloakOff.myRenderer.enabled = true;
					cloakProgressBar.EnableRenderer();
				} else {
					shipCloakOn.myRenderer.enabled = true;
					shipCloakOff.myRenderer.enabled = false;
					cloakProgressBar.DisableRenderer();
				}
			}
		} else {
			shipCloakOn.myRenderer.enabled = false;
			shipCloakOff.myRenderer.enabled = false;
			cloakProgressBar.DisableRenderer();
		}
	}

	public void SwitchShipInvincible() {
		if (ship.hasSpecial[Ship.SPECIAL_INVINCIBLE]) {
			if (!ship.hasBeenInvincibleInThisZone || ship.isInvincibleOn) {
				shipInvincibleOn.myRenderer.enabled = true;
				shipInvincibleOff.myRenderer.enabled = false;
			} else {
				shipInvincibleOn.myRenderer.enabled = false;
				shipInvincibleOff.myRenderer.enabled = true;
				shipInvincibleOn.transform.localScale = invincibleScale;
			}
		} else {
			shipInvincibleOn.myRenderer.enabled = false;
			shipInvincibleOff.myRenderer.enabled = false;
		}
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
//		lastHealthCountTime = Time.fixedTime;
	}

	public void SetShieldCount(int v) {
		shieldCount = new int[] { MyGUI.GetDigitOfNumber(0, v), MyGUI.GetDigitOfNumber(1, v), MyGUI.GetDigitOfNumber(2, v)}; // right to left
//		lastShieldCountTime = Time.fixedTime;
	}
	
	public void DisplayHealth(int[] digits) {
		healthCount = digits;
		DisplayHealth();
	}
	
	private void DisplayZoneID() {
		zoneIDDigit0.SetUVMapping(DIGITS_WHITE[MyGUI.GetDigitOfNumber(0, play.zoneID+1)]);
		zoneIDDigit1.SetUVMapping(DIGITS_WHITE[MyGUI.GetDigitOfNumber(1, play.zoneID+1)]);
	}
	
	private void DisplayHealth() {
//		Debug.Log (count[2] +" " + count[1] +" " +count[0]);
		healthDigit0.SetUVMapping(DIGITS_RED[healthCount[0]]);
		healthDigit1.SetUVMapping(DIGITS_RED[healthCount[1]]);
		healthDigit2.SetUVMapping(DIGITS_RED[healthCount[2]]);
	}
	
	public void DisplayShield(int[] digits) {
		shieldCount = digits;
		DisplayShield();
	}
	
	private void DisplayShield() {
		shieldDigit0.SetUVMapping(DIGITS_BLUE[shieldCount[0]]);
		shieldDigit1.SetUVMapping(DIGITS_BLUE[shieldCount[1]]);
		shieldDigit2.SetUVMapping(DIGITS_BLUE[shieldCount[2]]);
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
			
		if (displayedShield > toBeDisplayedShield && Time.fixedTime > lastShieldCountTime + TICK_DELTA) {
			LowerCount(ref shieldCount);
			lastShieldCountTime = Time.fixedTime;
			displayedShield--;
			DisplayShield();
		} else if (displayedShield < toBeDisplayedShield && Time.fixedTime > lastShieldCountTime + TICK_DELTA) {
			RaiseCount(ref shieldCount);
			lastShieldCountTime = Time.fixedTime;
			displayedShield++;
			DisplayShield();
		} else if (displayedHealth > toBeDisplayedHealth && Time.fixedTime > lastHealthCountTime + TICK_DELTA) {
			LowerCount(ref healthCount);
			lastHealthCountTime = Time.fixedTime;
			displayedHealth--;
			DisplayHealth();
		} else if (displayedHealth < toBeDisplayedHealth && Time.fixedTime > lastHealthCountTime + TICK_DELTA) {
			RaiseCount(ref healthCount);
			lastHealthCountTime = Time.fixedTime;
			displayedHealth++;
			DisplayHealth();
		}

		// Enemy target info
//		Debug.Log (shipTransform.position + " " + shipTransform.forward);
		int removed = 0;
		for (int i=0; i<MAX_ENEMY_HUD_INFOS; i++) {
			if (enemyHUDInfo.Count > i) {
				if (enemyHUDInfo[i-removed] != null && enemyHUDInfo[i-removed].lastTimeHUDInfoRequested + ENEMY_HUD_INFO_MAX_TIME > Time.fixedTime ) {					
/*					if (enemyHUDInfo[i-removed] == null) {
						Debug.Log ("enemy is null " + enemyHUDInfo[i-removed] + " " + Time.fixedTime);
						Debug.Log ("enemy is null " + enemyHUDInfo[i-removed].lastTimeHUDInfoRequested);
					}*/
					ShowEnemyHUDInfo(i, enemyHUDInfo[i-removed]);
				} else {
					if (enemyHUDInfo[i-removed] != null) {
						enemyHUDInfo[i-removed].RenderNormal();
					}
					enemyHUDInfo.RemoveAt(i-removed);
					removed++;
					gui.labelsCC[enemyHUDInfoLabels[i]].myRenderer.enabled = false;
					enemyHealthBars[i].DisableRenderer();
				}
			} else {
				gui.labelsCC[enemyHUDInfoLabels[i]].myRenderer.enabled = false;
				enemyHealthBars[i].DisableRenderer();
			}
		}
		
		if (ship.missileLockMode != Ship.MissileLockMode.None) {
			ShowEnemyMissileLockInfo();
		} else if (missileLockOn.myRenderer.enabled == true || missileLockOff.myRenderer.enabled == true) {
			gui.labelsCC[enemyLockMissileLabel].SetText("");
			missileLockOn.myRenderer.enabled = false;
			missileLockOff.myRenderer.enabled = false;
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
		
		if (ship.isExitHelperLaunched) {
			exitHelperOn.transform.localScale = exitHelperScale * (1f - POWER_UP_ANIM_SCALE * Mathf.Sin(Time.fixedTime*POWER_UP_ANIM_SPEED));
		}
		if (ship.isBoosterOn) {
			shipBoostOn.transform.localScale = boosterScale * (1f - POWER_UP_ANIM_SCALE * Mathf.Sin(Time.fixedTime*POWER_UP_ANIM_SPEED));
		}
		if (ship.isCloakOn) {
			shipCloakOn.transform.localScale = cloakScale * (1f - POWER_UP_ANIM_SCALE * Mathf.Sin(Time.fixedTime*POWER_UP_ANIM_SPEED));
		}
		if (ship.isInvincibleOn) {
			shipInvincibleOn.transform.localScale = invincibleScale * (1f - POWER_UP_ANIM_SCALE * Mathf.Sin(Time.fixedTime*POWER_UP_ANIM_SPEED));
		}
		if (ship.isHeadlightOn) {
			lightsOn.transform.localScale = lightsScale * (1f - POWER_UP_ANIM_SCALE * Mathf.Sin(Time.fixedTime*POWER_UP_ANIM_SPEED));
		}
		if (ship.currentSecondaryWeapon != -1) {
			if (!ship.secondaryWeapons[ship.currentSecondaryWeapon].IsReloaded() && ship.secondaryWeapons[ship.currentSecondaryWeapon].ammunition > 0) {
				shipMissileLoadingProgressBar.SetBar((Time.fixedTime-ship.secondaryWeapons[ship.currentSecondaryWeapon].lastShotTime)/ship.secondaryWeapons[ship.currentSecondaryWeapon].frequency);
			}
			if (ship.secondaryWeapons[ship.currentSecondaryWeapon].IsReloaded() != shipSecondaryWeaponLoadState) {
				SetLoadStateOfShipSecondaryWeapon();
			}
		}
		
		if (notificationMode != NotificationMode.Off) {
			Color c = notifyLabel.myRenderer.material.color;
			if (notificationMode == NotificationMode.BlendIn) {
				if (Time.fixedTime > notifyTimer + NOTIFY_TIMER_BLEND_IN) {
					notificationMode = NotificationMode.Show;
					notifyTimer = Time.fixedTime;
					c.a = 1f;
					notifyLabel.myRenderer.material.color = c;
				} else {
					c.a = Mathf.Lerp(0f, 1f, (Time.fixedTime-notifyTimer)/NOTIFY_TIMER_BLEND_IN);
					notifyLabel.myRenderer.material.color = c;
				}
			} else if (notificationMode == NotificationMode.Show) {
				if (Time.fixedTime > notifyTimer + NOTIFY_TIMER_SHOW) {
					notificationMode = NotificationMode.BlendOut;
					notifyTimer = Time.fixedTime;
				}
			} else if (notificationMode == NotificationMode.BlendOut) {
				if (Time.fixedTime > notifyTimer + NOTIFY_TIMER_BLEND_IN) {
					notificationMode = NotificationMode.Off;
					c.a = 0;
					notifyLabel.myRenderer.material.color = c;
					notifyLabel.myRenderer.enabled = false;
				} else {
					c.a = Mathf.Lerp(1f, 0f, (Time.fixedTime-notifyTimer)/NOTIFY_TIMER_BLEND_IN);
					notifyLabel.myRenderer.material.color = c;
				}	
			}
		}
	}
	
	public void EnemyInSight(Enemy e) {
		e.lastTimeHUDInfoRequested = Time.fixedTime;
		if (!enemyHUDInfo.Contains(e)) {
			enemyHUDInfo.Add(e);
			e.RenderWithGlow();
			if (enemyHUDInfo.Count > MAX_ENEMY_HUD_INFOS) {
				enemyHUDInfo[0].RenderNormal();
				enemyHUDInfo.RemoveAt(0);
				gui.labelsCC[enemyHUDInfoLabels[0]].myRenderer.enabled = false;
				enemyHealthBars[0].DisableRenderer();
			}
		}
	}
		
	private void ShowEnemyHUDInfo(int index, Enemy e) {
//		if (e == null) Debug.Log ("enemy is null " + index);
//		Debug.Log (e.transform.TransformDirection(e.transform.localScale));
		gui.labelsCC[enemyHUDInfoLabels[index]].SetText(
			e.clazz + " " + e.displayModel.ToString("00")
//				+ " [H:" + e.health + "]"
//				+ " (F:"+ Mathf.RoundToInt(e.firepowerPerSecond) + ")" 
		);
		gui.labelsCC[enemyHUDInfoLabels[index]].transform.localPosition =
			Calculate2DHUDPosition(e.transform.position, ship.transform.TransformDirection(ENEMY_HUD_OFFSET_LOCAL) * e.radius * 0.5f, e);
		enemyHealthBars[index].SetBar(e.health/(float)e.maxHealth);
		enemyHealthBars[index].transform.localPosition =
			Calculate2DHUDPosition(e.transform.position, ship.transform.TransformDirection(ENEMY_HEALTH_BAR_OFFSET_LOCAL) * e.radius * 0.5f, e);
		if (gui.labelsCC[enemyHUDInfoLabels[index]].myRenderer.enabled == false) {
			gui.labelsCC[enemyHUDInfoLabels[index]].myRenderer.enabled = true;
			enemyHealthBars[index].EnableRenderer();
		}
	}
	
	private void ShowEnemyMissileLockInfo() {
		Enemy e = ship.lockedEnemy;
		if (e != null) {
			float t = Time.fixedTime-ship.missileLockTime;
			gui.labelsCC[enemyLockMissileLabel].SetText("L: " 
				+ (Ship.MISSILE_LOCK_DURATION - Mathf.Clamp(Mathf.FloorToInt(t),0,Ship.MISSILE_LOCK_DURATION)));
			gui.labelsCC[enemyLockMissileLabel].transform.localPosition =
				Calculate2DHUDPosition(e.transform.position, ship.transform.TransformDirection(ENEMY_LOCK_OFFSET_LOCAL) * e.radius * 0.5f, e);
			if (t > Ship.MISSILE_LOCK_DURATION) {
				missileLockOn.myRenderer.enabled = true;
				missileLockOff.myRenderer.enabled = false;
				missileLockOn.transform.localPosition = gui.labelsCC[enemyLockMissileLabel].transform.localPosition;
			} else {
				missileLockOn.myRenderer.enabled = false;
				missileLockOff.myRenderer.enabled = true;
				missileLockOff.transform.localPosition = gui.labelsCC[enemyLockMissileLabel].transform.localPosition;
			}
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
		e.RenderNormal();
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
	
	public void DisplayCrossHair() {
		shipCroosHair.renderer.enabled = true;
	}
	
	public void HideCrossHair() {
		shipCroosHair.renderer.enabled = false;
	}
	
	public void DisplayPrimaryWeapon(Weapon w) {
//		Debug.Log (primaryWeapon + " " + w + " " + PRIMARY_WEAPONS);
		primaryWeapon.SetUVMapping(PRIMARY_WEAPONS[w.type]);
		gui.labelsCC[primaryWeaponLabel].SetText(Weapon.PRIMARY_TYPES[w.type] + " FP: " + w.damage);
	}
	
	public void DisplaySecondaryWeapon() {
		gui.labelsCC[secondaryWeaponLabel].SetText(Weapon.SECONDARY_TYPES[ship.currentSecondaryWeapon] + " A: "+ ship.secondaryWeapons[ship.currentSecondaryWeapon].ammunition);
		SetLoadStateOfShipSecondaryWeapon();
	}
	
	private void SetLoadStateOfShipSecondaryWeapon() {
		if (ship.secondaryWeapons[ship.currentSecondaryWeapon].IsReloaded()) {
			secondaryWeapon.SetUVMapping(SECONDARY_WEAPONS[ship.currentSecondaryWeapon]);
			shipMissileLoadingProgressBar.DisableRenderer();
			shipSecondaryWeaponLoadState = true;
		} else {
			secondaryWeapon.SetUVMapping(SECONDARY_WEAPONS_LOADING[ship.currentSecondaryWeapon]);
			if (ship.secondaryWeapons[ship.currentSecondaryWeapon].ammunition > 0) {
				shipMissileLoadingProgressBar.EnableRenderer();
			}
			shipSecondaryWeaponLoadState = false;
		}
	}
	
	private void ToMenu() {
		CloseDialog();
		play.BackToMenu();
	}
	
	private void ToGame() {
		CloseDialog();
		play.SetPaused(false);
	}

/*	private void ToNextZone() {
		CloseDialog();
//		play.NextZone();
	}*/
	
	private void ToRetrySokoban() {
		play.RetrySokoban();
	}
	
	public void ToQuitSokoban() {
		play.SwitchMode();
	}

	public void ToHasDied() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(play.game.state.GetDialog(8) + (play.zoneID+1), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.AddLabel(play.game.state.GetDialog(44), dialogBox, new Vector3(0.04f,0.04f,1f), MyGUI.GUIAlignment.Center, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButton(dialogBox, new Vector3(0.05f,0.05f,1f), toGame, play.game.state.GetDialog(37), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.05f, Game.GUI_UV_NULL, 0);
	}

	public void ToStory() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		//TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, gui.GetSize(),
			new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-1f), true);
		gui.AddImage(dialogBox, new Vector3(gui.GetSize().x/2f, gui.GetSize().y, gui.GetSize().z),
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.containers[dialogBox].AddZLevel(1f);
		scrollableContainer = gui.AddContainerScrollable(dialogBox, new Vector3(gui.GetSize().x/2f, gui.GetSize().y * 0.6f,1f),
			new Vector2(gui.GetCenter().x, gui.GetCenter().y + gui.GetSize().y/2f * 0.5f), MyGUI.GUIBackground.QuadWithCollider,
			0, Game.GUI_UV_COLOR_BLACK, 0, Game.GUI_UV_SCROLL_BUTTON_BACK_4T1, 0, Game.GUI_UV_SCROLL_BUTTON_BACK_4T1);
		gui.AddLabel(play.storyChapter,	scrollableContainer, MyGUI.GUIAlignment.Center, MyGUI.GUIBackground.Quad, 0.1f, 0.1f,
					0.3f, 3, 0, Game.GUI_UV_COLOR_BLACK);
//		TouchDelegate scrollContainerOn = new TouchDelegate(ScrollContainerOn);
//		gui.containers[scrollableContainer].SetTouchDelegate(scrollContainerOn);
		gui.containers[dialogBox].AddZLevel(2f);
		gui.AddImage(dialogBox, new Vector3(gui.GetSize().x/2f, gui.GetSize().y*0.2f, gui.GetSize().z),
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.01f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.AddImage(dialogBox, new Vector3(gui.GetSize().x/2f, gui.GetSize().y*0.2f, gui.GetSize().z),
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, -0.1f, Game.GUI_UV_COLOR_BLACK, 0);
		gui.containers[dialogBox].AddZLevel(1f);
		gui.AddLabel("\n" + play.game.state.GetDialog(8) + (play.zoneID+1) + "\n\n", dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center,
			0f, MyGUI.GUIAlignment.Top, 0.05f, 1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		gui.AddLabel("\n...\n", dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center,
			0f, MyGUI.GUIAlignment.Bottom, 0.05f, 1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButtonF(0.25f, dialogBox, new Vector3(0.04f,0.04f,1f), toMenu, play.game.state.GetDialog(9), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -(gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogBox, new Vector3(0.04f,0.04f,1f), toGame, play.game.state.GetDialog(10), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, (gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}
		
	public void ToQuit() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(play.game.state.GetDialog(5), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.1f, 
			1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toMenu, play.game.state.GetDialog(6), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -0.2f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, play.game.state.GetDialog(7), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0.2f, MyGUI.GUIAlignment.Center, -0.2f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

	public void ToPowerUpFound(string headline, string description, Vector4 uvMap) {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(headline, dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		gui.AddImage(dialogBox, new Vector3(0.15f,0.15f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, uvMap, 0);
		gui.AddLabel(description, dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);

		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, play.game.state.GetDialog(37), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, (gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

	public void ToSokoban() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);
		
		int dialogBox = gui.AddContainer(dialogContainer, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		TouchDelegate toRetrySokoban = new TouchDelegate(ToRetrySokoban);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toRetrySokoban, play.game.state.GetDialog(11), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, -(gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
		TouchDelegate toQuitSokoban = new TouchDelegate(ToQuitSokoban);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toQuitSokoban, play.game.state.GetDialog(47), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, (gui.GetSize().x/2f) * 0.75f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

	public void ToHelp() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(play.game.state.GetDialog(38), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.NinePatch, Game.GUI_UV_BACK_NINEPATCH, 0);
		gui.AddLabel(play.game.state.GetDialog(39), dialogBox, new Vector3(0.03f,0.03f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, -0.4f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_COLOR_BLACK, 0);
		string helpString = play.game.state.GetDialog(40) +
			(ship.hasSpecial[Ship.SPECIAL_BOOST] ? play.game.state.GetDialog(58) : "") +
			(ship.hasSpecial[Ship.SPECIAL_CLOAK] ? play.game.state.GetDialog(59) : "") +
			(ship.hasSpecial[Ship.SPECIAL_INVINCIBLE] ? play.game.state.GetDialog(60) : "")
			+ "\n";
		gui.AddLabel(helpString, dialogBox, new Vector3(0.03f,0.03f,1f), MyGUI.GUIAlignment.Left, MyGUI.GUIAlignment.Center, 0.4f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_COLOR_BLACK, 0);
		
		TouchDelegate toGame = new TouchDelegate(ToGame);
		gui.AddLabelButtonF(0.25f, dialogContainer, new Vector3(0.04f,0.04f,1f), toGame, play.game.state.GetDialog(37), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.05f, Game.GUI_UV_BUTTON_BACK_4T1, 0);
	}

	public void To1LevelDemoEnd() {
		gui.OpenDialog();
		dialogContainer = gui.AddContainer(container, gui.GetSize(), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[container].transform.position.z-10f), true);
//		TouchDelegate closeDialog = new TouchDelegate(CloseDialog);
		int dim = gui.AddDim(dialogContainer, null, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, Game.GUI_UV_DIM, 0); 
			//gui.AddDim(dialogContainer, closeDialog);
		gui.SetGameInputZLevel(gui.dims[dim].transform.position.z);

		int dialogBox = gui.AddContainer(dialogContainer, new Vector3(gui.GetSize().x * 0.85f, gui.GetSize().y * 0.75f, 1.0f), new Vector3(gui.GetCenter().x, gui.GetCenter().y, gui.containers[dialogContainer].GetCenter().z-2f), true);
		gui.AddLabel(play.game.state.GetDialog(45), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Top, -0.2f, 
			1f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_NULL, 0);
		gui.AddLabel(play.game.state.GetDialog(46), dialogBox, new Vector3(0.05f,0.05f,1f), MyGUI.GUIAlignment.Center, MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Center, 0f, 
			0f, 1f, 3, MyGUI.GUIBackground.Quad, Game.GUI_UV_COLOR_BLACK, 0);
		
		TouchDelegate toMenu = new TouchDelegate(ToMenu);
		gui.AddLabelButton(dialogContainer, new Vector3(0.05f,0.05f,1f), toMenu, play.game.state.GetDialog(9), 1.0f, 1.0f, 3, 
			MyGUI.GUIAlignment.Center, 0f, MyGUI.GUIAlignment.Bottom, 0.1f, Game.GUI_UV_NULL, 0);
	}
}