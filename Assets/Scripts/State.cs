using System;
using UnityEngine;

public class State : MonoBehaviour {

	public Game.Mode gameMode = Game.Mode.None;
	public int startupCounter = 0;
	public bool hasStartupCounterBeenDisplayed = false;
	public int lang;
	
	public Game game;
	
	public bool isSoundOn;
	private int preferenceLevel;
	private int preferenceCaveSeed;
	private int preferenceIsSoundOn;
	private int preferenceIsMusicOn;
	private int preferenceMouseYAxisInverted;
	private float preferenceMiniMapMouseSensitivity;
	private int[] preferencePrimaryWeaponPowerUpStates;
	private int[] preferenceSecondaryWeaponPowerUpStates;
	private int[] preferenceHullPowerUpStates;
	private int[] preferenceSpecialPowerUpStates;
	private int preferenceSokobanSolved;
	
	private AudioSource sounds;
	private AudioSource music;
	private String[,] dialogs = new String[10,75];
			
	private bool isInitialized = false;
//	private int lastLevelsPage;
	
	void Awake() {
	
		DontDestroyOnLoad(this);
		if (!isInitialized) {
			//PlayerPrefs.DeleteAll();
			startupCounter = PlayerPrefs.GetInt("startupCounter", 0);
			startupCounter++;
			PlayerPrefs.SetInt("startupCounter", startupCounter);
			SetupDialogs();
			
			preferenceLevel = PlayerPrefs.GetInt("preferenceLevel", -1);
			preferenceCaveSeed = PlayerPrefs.GetInt("preferenceCaveSeed", -1);
			preferenceIsSoundOn = PlayerPrefs.GetInt("preferenceIsSoundOn", 1);
			isSoundOn = preferenceIsSoundOn == 1 ? true : false;
			preferenceIsMusicOn = PlayerPrefs.GetInt("preferenceIsMusicOn", 1);
			preferenceSokobanSolved = PlayerPrefs.GetInt("preferenceSokobanSolved", 0);
			preferenceMiniMapMouseSensitivity = PlayerPrefs.GetFloat("preferenceMiniMapMouseSensitivity", 100.0f);
			preferenceMouseYAxisInverted = PlayerPrefs.GetInt("preferenceMouseYAxisInverted", 1);
			ReadInPowerUpStates();
			AudioSource[] audioSources = GetComponents<AudioSource>();
			sounds = audioSources[0];
			music = audioSources[1];
//			if (isMusicOn) {
//				music.Play();
//			}
			
			switch (Application.systemLanguage) {
				case SystemLanguage.Catalan :
					lang = 6;
					break;
				case SystemLanguage.French :
					lang = 5;
					break;
				case SystemLanguage.Italian :
					lang = 4;
					break;
				case SystemLanguage.Japanese :
					lang = 8;
					break;
				case SystemLanguage.Korean :
					lang = 9;
					break;
				case SystemLanguage.Portuguese :
					lang = 3;
					break;
				case SystemLanguage.Russian :
					lang = 7;
					break;
				case SystemLanguage.Spanish :
					lang = 6;
					break;
				case SystemLanguage.German :
					lang = 1;
					break;
				case SystemLanguage.Chinese :
					lang = 2;
					break;
				default :
					lang = 0;
					break;
			}
			lang=1;

		}
		isInitialized = true;
	}
		
	public void Initialize(Game g) {
		game = g;
	}
	
	public void ResetPowerUpStates() {
		ReadInPowerUpStates();
	}
	
	public void ClearPowerUpStates() {
		for (int i=0; i<8; i++) {
			PlayerPrefs.SetInt(Weapon.PRIMARY_TYPES[i], 0);
			PlayerPrefs.SetInt(Ship.HULL_TYPES[i], 0);
		}
		for (int i=0; i<4; i++) {
			PlayerPrefs.SetInt(Weapon.SECONDARY_TYPES[i], 0);
			PlayerPrefs.SetInt(Ship.SPECIAL_NAMES[i], 0);
		}
	}
	
	private void ReadInPowerUpStates() {
		preferencePrimaryWeaponPowerUpStates = new int[8];
		preferenceHullPowerUpStates = new int[8];
		preferenceSecondaryWeaponPowerUpStates = new int[4];
		preferenceSpecialPowerUpStates = new int[4];
		for (int i=0; i<8; i++) {
			preferencePrimaryWeaponPowerUpStates[i] = PlayerPrefs.GetInt(Weapon.PRIMARY_TYPES[i], 0);
			preferenceHullPowerUpStates[i] = PlayerPrefs.GetInt(Ship.HULL_TYPES[i], 0);
		}
		for (int i=0; i<4; i++) {
			preferenceSecondaryWeaponPowerUpStates[i] = PlayerPrefs.GetInt(Weapon.SECONDARY_TYPES[i], 0);
			preferenceSpecialPowerUpStates[i] = PlayerPrefs.GetInt(Ship.SPECIAL_NAMES[i], 0);
		}
	}
		
	public bool HasPowerUp(int type, int id) {
		if (type == Game.POWERUP_PRIMARY_WEAPON) {
			return preferencePrimaryWeaponPowerUpStates[id] == 0 ? false : true;
		} else if (type == Game.POWERUP_SECONDARY_WEAPON) {
			return preferenceSecondaryWeaponPowerUpStates[id] == 0 ? false : true;
		} else if (type == Game.POWERUP_HULL) {
			return preferenceHullPowerUpStates[id] == 0 ? false : true;
		} else {
			return preferenceSpecialPowerUpStates[id] == 0 ? false : true;
		}
	}
	
	public void SetPowerUp(int type, int id) {
		if (type == Game.POWERUP_PRIMARY_WEAPON) {
			preferencePrimaryWeaponPowerUpStates[id] = 1;
			PlayerPrefs.SetInt(Weapon.PRIMARY_TYPES[id], 1);
		} else if (type == Game.POWERUP_SECONDARY_WEAPON) {
			preferenceSecondaryWeaponPowerUpStates[id] = 1;
			PlayerPrefs.SetInt(Weapon.SECONDARY_TYPES[id], 1);
		} else if (type == Game.POWERUP_HULL) {
			preferenceHullPowerUpStates[id] = 1;
			PlayerPrefs.SetInt(Ship.HULL_TYPES[id], 1);
		} else {
			preferenceSpecialPowerUpStates[id] = 1;
			PlayerPrefs.SetInt(Ship.SPECIAL_NAMES[id], 1);
		}
	}

	public int GetPreferenceLevel() {
		return preferenceLevel;
	}
	
	public void SetPreferenceLevel(int level) {
		preferenceLevel = level;
		PlayerPrefs.SetInt("preferenceLevel", preferenceLevel);
	}

	public int GetPreferenceCaveSeed() {
		return preferenceCaveSeed;
	}
	
	public void SetPreferenceCaveSeed(int caveSeed) {
		preferenceCaveSeed = caveSeed;
		PlayerPrefs.SetInt("preferenceCaveSeed", preferenceCaveSeed);
	}

	public bool GetPreferenceMusicOn() {
		return preferenceIsMusicOn == 0 ? false : true;
	}
	
	public void SetPreferenceMusicOn(bool s) {
		preferenceIsMusicOn = s == true ? 1 : 0;
		PlayerPrefs.SetInt("preferenceIsMusicOn", preferenceIsMusicOn);
	}

	public bool GetPreferenceSoundOn() {
		return preferenceIsSoundOn == 0 ? false : true;
	}
	
	public void SetPreferenceSoundOn(bool s) {
		preferenceIsSoundOn = s == true ? 1 : 0;
		PlayerPrefs.SetInt("preferenceIsSoundOn", preferenceIsSoundOn);
		isSoundOn = preferenceIsSoundOn == 1 ? true : false;
	}

	public bool GetPreferenceSokobanSolved() {
		return preferenceSokobanSolved == 0 ? false : true;
	}
	
	public void SetPreferenceSokobanSolved(bool s) {
		preferenceSokobanSolved = s == true ? 1 : 0;
		PlayerPrefs.SetInt("preferenceSokobanSolved", preferenceSokobanSolved);
	}
	
	public int GetPreferenceMouseYAxisInverted() {
		return preferenceMouseYAxisInverted;
	}

	public void SetPreferenceMouseYAxisInverted(int i) {
		preferenceMouseYAxisInverted = i;
		PlayerPrefs.SetInt("preferenceMouseYAxisInverted", preferenceMouseYAxisInverted);
	}

	public float GetPreferenceMiniMapMouseSensitivity() {
		return preferenceMiniMapMouseSensitivity;
	}
	
	public void SetPreferenceMiniMapMouseSensitivity(float f) {
		preferenceMiniMapMouseSensitivity = f;
		PlayerPrefs.SetFloat("preferenceMiniMapMouseSensitivity", preferenceMiniMapMouseSensitivity);
	}
	
	public String GetDialog(int taskID) { // task id
		return dialogs[lang,taskID];
	}
	
/*	public int[] GetAsianDialog(int language,int taskID) {
		return asianDialogs[language][taskID];
	}*/
	
	public void SwitchSound() {
		if (preferenceIsSoundOn == 1) {
			SetPreferenceSoundOn(false);
		} else {
			SetPreferenceSoundOn(true);
		}
	}
	
	public void PlaySound(int id) {
		if (isSoundOn) {
			sounds.PlayOneShot(game.audioClips[id]);
		}
	}
	
	public void PlayMusic(int id) {
		if (GetPreferenceMusicOn()) {
			music.PlayOneShot(game.audioClips[id]);
		}
	}
	
	public void SwitchMusic() {
		if (preferenceIsMusicOn == 1) {
			SetPreferenceMusicOn(false);
//			music.Stop();
		} else {
			SetPreferenceMusicOn(true);
//			music.Play();
		}
	}
	
	public bool IsMusicPlaying() {
		return music.isPlaying;
	}
	
	private void SetupDialogs() {
		// language 0, level 0, text 0-3 english
		dialogs[0,0] = "Help";
		dialogs[0,1] = "Microphone sensitivity from left (highest) to right (lowest). Only sound above level is transmitted (as indicated by red LEDs). Tap on video to force transmission. Turn video with finger, Zoom in/out with pinch gesture.";
		dialogs[0,2] = "Large lamp indicates battery level of recording device. Small lamp for this device.";
		dialogs[0,3] = "Connection status:\nconnected (green), searching (yellow)\n \n";
		dialogs[0,4] = "Volume slider";
		dialogs[0,5] = "Credits";
		dialogs[0,6] = "Idea & Realisation\n\nBen Ohloff\nAndruids\nwww.andruids.com\n \n";
		dialogs[0,7] = "Welcome to Baby Monitor AV!";
//		dialogs[0,8] = "Hit the recording button\nand put the phone close to\nyour baby. Make sure recording\nand listening devices\nare in the same WiFi network.";
		dialogs[0,8] = "Hit the recording button and put the phone close to your baby.\n\nMake sure recording and listening devices are in the same WiFi network.";
		dialogs[0,9] = "Hit the listening button on one or more other smartphones or download the client for Windows PC/Mac from our website www.andruids.com";
		dialogs[0,10] = "Preferences";
		dialogs[0,11] = "Rate this app";
		dialogs[0,12] = "Options";
		dialogs[0,13] = "Disclaimer";
		dialogs[0,14] = "While we have done our best to ensure this app works as intended it is still your sole responsibility to always ensure that your baby is safe and well.\n\nDo not rely exclusively on the technical means provided by this application.\n \n";
		dialogs[0,15] = "Select a microphone device and camera device if your phone offers more than one.\n\nSet Recording device port to an available port number different from Broadcast port.\n\nChoose for how long camera transmission should continue after last detected sound.\n\nDetermine how many times per\nsecond a camera image will be\ntransmitted.\n\nSet alert sound on/off\nfor transmission.\n \n";
		dialogs[0,16] = "Options";
		dialogs[0,17] = "Microphone device";
		dialogs[0,18] = "Camera device";
		dialogs[0,19] = "Recording device port";
		dialogs[0,20] = "Broadcast port";
		dialogs[0,21] = "Transmit after activity";
		dialogs[0,22] = "Images per second";
		dialogs[0,23] = "1/second";
		dialogs[0,24] = "2/second";
		dialogs[0,25] = "4/second";
		dialogs[0,26] = "5 sec";
		dialogs[0,27] = "15 sec";
		dialogs[0,28] = "30 sec";
		dialogs[0,29] = "1 min";
		dialogs[0,30] = "Please set Broadcast port to different value than Recording device port";
		dialogs[0,31] = "Please set Recording device port to different value than Broadcast port";
		dialogs[0,32] = "Warning!";
		dialogs[0,33] = "Waiting for connection\n \n \n \n \n \n \n \n \n \n";
		dialogs[0,34] = "Trying to connect to\nRecording device.\n \n \n \n \n \n \n \n \n \n";
		dialogs[0,35] = "Battery of Recording device is below 20 %";
		dialogs[0,36] = "Recording";
		dialogs[0,37] = "Listening";
		dialogs[0,38] = "Trial";
		dialogs[0,39] = "Your are using the trial version. It will stop recording after 10 minutes. You can manually restart the recording.\n\nPlease upgrade to the full version of this app.\n \n \n \n \n \n \n \n";
		dialogs[0,40] = "Recording has ended after 10 minutes because you are using the Trial version. You can manually restart the recording.\n\nPlease upgrade to the full version of this app.\n \n \n \n \n \n \n \n";
		dialogs[0,41] = "Recording has ended after 10 minutes because you are using the Trial version. You can manually restart the recording.\n\nPlease upgrade to the full version of this app on the recording device.\n \n \n \n \n \n";
		dialogs[0,42] = "Don't forget to download our Windows PC or Mac OSX version to listen to your baby on your desktop! It's free!";
		dialogs[0,43] = "Transmission alert sound";
		dialogs[0,44] = "Yes";
		dialogs[0,45] = "No";
		
		// language 1, german
		dialogs[1,0] = "\nStart New Game\n\n";
		dialogs[1,1] = "\nContinue Game\n\n";
		dialogs[1,2] = "\nExit\n\n";
		dialogs[1,3] = "\nOptions\n\n";
		dialogs[1,4] = "\nCredits\n\n";
		dialogs[1,5] = "\nBack to Main Menu?\n\n";
		dialogs[1,6] = "\nYes\n\n";
		dialogs[1,7] = "\nNo\n\n";
		dialogs[1,8] = "Chapter ";
		dialogs[1,9] = "\nMenu\n\n";
		dialogs[1,10] = "\nGo\n\n";
		dialogs[1,11] = "\nRetry\n\n";
		dialogs[1,12] = "\nOptions\n\n";
		dialogs[1,13] = "Zone 02";
		dialogs[1,14] = "Zone 10";
		dialogs[1,15] = "Zone 19";
		dialogs[1,16] = "Zone 25";
		dialogs[1,17] = "\nYou found a new primary weapon system\n\n";
		dialogs[1,18] = "\nYou found a new secondary weapon system\n\n";
		dialogs[1,19] = "\nYou found an improved hull for your ship\n\n";
		dialogs[1,20] = "\nYou found a special capability to equip your ship\n\n";
		dialogs[1,21] = "\nThe Gun is the basic weapon for every cave ship\n\n";
		dialogs[1,22] = "\nThe Laser is faster than a Gun and is more destructive\n\n";
		dialogs[1,23] = "\nThe Twin Gun is the improved version of the Gun\n\n";
		dialogs[1,24] = "\nThe Phaser is a high velocity weapon\n\n";
		dialogs[1,25] = "\nThe Twin Laser is the improved version of the Laser\n\n";
		dialogs[1,26] = "\nThe Gauss is the ultimate weapon in terms of damage\n\n";
		dialogs[1,27] = "\nThe Twin Phaser is the improved version of the Phaser\n\n";
		dialogs[1,28] = "\nThe Twin Gauss is twice as deadly as a single Gauss weapon\n\n";
		dialogs[1,29] = "\nDo you really want to start a new game?\n\nThis erases any progress you've made so far!\n\n";
		dialogs[1,30] = "\nCancel\n\n";
		dialogs[1,31] = "";
		dialogs[1,32] = "\nYour defensive capabilities have been improved.\n\n";
		dialogs[1,33] = "\nYou found headlights that allow you to see better in dark caves.\n\n";
		dialogs[1,34] = "\nYou found a device to boost the speed of your ship temporarily.\n\n";
		dialogs[1,35] = "\nYou found a device to make your ship temporarily invisible for enemie bots. You cannot shoot while cloaked.\n\n";
		dialogs[1,36] = "\nYou found a device to make your ship temporarily indestructible.\n\n";
		dialogs[1,37] = "\nOK\n\n";
		dialogs[1,38] = "\nHelp\n\n";
		dialogs[1,39] = "\nFly Forward		W\n" +
						"Fly Backwards		S\n" +
						"Strafe Left		A\n" +
						"Strafe Right		D\n" +
						"Fly Up			Z\n" +
						"Fly Down			X\n" +
						"Roll Left			Q\n" +
						"Roll Right			E\n" +
						"Yaw Left/Right		Mouse L/R\n" +
						"Pitch Up/Down		Mouse U/D\n" +
						"Fire Primary		L Mouse\n" +
						"Fire Secondary	R Mouse\n" +
						"Open Secret Door	W,A,S,D\n" +
						"Help				F5\n" +
						"Pause			ESC\n\n";
		dialogs[1,40] = "\nSelect Primary		1-8\n" +
						"Select Secondary	F1-F4\n" +
						"Cycle Primary		Mouse Wheel\n" +
						"Cycle Secondary	Page Up/Down\n" +
						"Cycle Camera		F10\n" +
						"Breadcrumb		B\n" +
						"Mini Map			M\n" +
						"- Follow On/Off	F\n" +
						"- Zoom In/Out		Alt+M. Wheel\n" +
						"- Turn			Alt+M. Move\n" +
						"6D2F			T\n" +
						"Lights On/Off		L\n";
		dialogs[1,41] = "\nIdea, Programming, Music\n" +
			"   Ben Ohloff\n\n" +
			"Short story\n" +
			"   Stuart McKenzie-Walker\n\n" +
			"Copyright\n" +
			"   2013 Ben Ohloff, 7Druids GmbH\n\n" +
			"Sokoban level design by\n" +
		 	"   David W. Skinner\n" +
			"   http://users.bentonrea.com/~sasquatch/\n";
		dialogs[1,42] = 
			"Most sound samples from\n   freesound.org\nand the following users\n\n" +
			"PhreaKsAccount, unfa, mik300z, Omar Alvarado, steveygos93, EcoDTR, jobro, simpsi, kantouth, Skullsmasha, IFartInUrGenera, jorickhoofd, Julien Nicolas, CGEffex, junggle, CosmicD, smcameron, StonedB, kwandalist, DJ Chronos, mareproduction, GregsMedia, themfish, daveincamas, poots, drminky, klankbeeld, argitoth, erh, xDimebagx\n\n";
		dialogs[1,43] = "Mouse Y-Axis inverted";
		dialogs[1,44] = "\nPhew... that was close!\n\n";
		dialogs[1,45] = "The End";
		dialogs[1,46] = "\nThank you for playing the Demo!\n\nYou can replay the same chapter. Cave and enemy encounters will differ each time.\n\n";
		dialogs[1,47] = "\nGive Up\n\n";
		dialogs[1,48] = "\nThe Missile also damages nearby objects as it explodes.\n\n";
		dialogs[1,49] = "\nThe Guided Missile can follow slow moving targets if locked on.\n\n";
		dialogs[1,50] = "\nThe Charged Missile can be boosted by converting the shield energy of your ship so to increase its lethality.\n\n";
		dialogs[1,51] = "\nThe Detonator Missile must be triggered again while flying and it will discharge number of cluster bombs.\n\n";
		dialogs[1,52] = "A Power Up is still in use!";
		dialogs[1,53] = "This Power Up is still recharging!";
		dialogs[1,54] = "This Power Up can only be used once per Zone!";
		dialogs[1,55] = "Your maximum capacity for missiles is reached!";
		dialogs[1,56] = "You cannot shoot while cloaked!";
		dialogs[1,57] = "Find the 2 keys to open this door!";
		dialogs[1,58] = "Boost			V\n"; // concatenated with dialog 40
		dialogs[1,59] = "Cloak			C\n"; // concatenated with dialog 40
		dialogs[1,60] = "Invincible			I\n"; // concatenated with dialog 40
		dialogs[1,61] = "Charging Missile with shield energy";
		dialogs[1,62] = "You need a weapon before leaving this zone! Look for a secret chamber and open it.";
		dialogs[1,63] = "Music/Atmosphere sounds on";
		dialogs[1,64] = "All sounds on";
		dialogs[1,65] = "Help\nF5";
		dialogs[1,66] = "Pause\nESC";
		dialogs[1,67] = "You need an improved ship hull before leaving this zone! Look for a secret chamber and open it.";
		
		// language 2 chinese
		dialogs[2,0] =  "帮助";
		dialogs[2,1] =  "麦克风敏感度从左（最高） 至右（最低）。只有一定等 级以上的声音才会被传输（红 LED灯提示）。点选视 频强制传输。用手 指打开视频，用捏指 手势放大、缩小。\n \n";
		dialogs[2,2] =  "大灯指示录音设备的电池电 量。小灯指示 本设备。\n \n";
		dialogs[2,3] =  "连接状态：已连接（绿）， 寻找中（黄） \n \n";
		dialogs[2,4] =  "音量滑块\n \n";
		dialogs[2,5] =  "人员表";
		dialogs[2,6] =  "想法 & 实现\nBen Ohloff\nAndruids\nwww.andruids.com";
		dialogs[2,7] =  "欢迎来到Baby Monitor AV！";
		dialogs[2,8] =  "点击记录按钮并将手机靠近 您的宝宝。确 保录音和收听设备在相 同的WiFi网络。\n\n";
		dialogs[2,9] =  "按下一个或多个手机（Android或iOS 系统）的接听键 ，或从我们的网站 www.andruids.com下载 Windows PC/Mac版本的客户端。";
		dialogs[2,10] = "偏好";
		dialogs[2,11] = "为此应用评分";
		dialogs[2,12] = "人员表";
		dialogs[2,13] = "免责声明";
		dialogs[2,14] = "虽然我们已经尽了最 大努力确保此应用程 序按预期工作，但是 确保你的宝宝安全健康 仍是您的唯一职责。 不要完全依赖该应用 程序提供的技术手段。\n \n";
		dialogs[2,15] = "如果您的手机有一个 以上的麦克风设备和 照相机设备，请分别 选择一个。设置录音设 备端口到与广播端口 不同的可用端口上。 选择在最后检测到声音 后相机应继续传输多 久。确定每秒传输相 机图像多少次。设 定传输开/关警告声。\n \n";
		dialogs[2,16] = "选项";
		dialogs[2,17] = "麦克风设备";
		dialogs[2,18] = "照相机设备";
		dialogs[2,19] = "录音设备端口";
		dialogs[2,20] = "广播端口";
		dialogs[2,21] = "活动后传输";
		dialogs[2,22] = "图像每秒";
		dialogs[2,23] = "1/秒";
		dialogs[2,24] = "2/秒";
		dialogs[2,25] = "4/秒";
		dialogs[2,26] = "5秒";
		dialogs[2,27] = "15秒";
		dialogs[2,28] = "30秒";
		dialogs[2,29] = "1分钟";      
		dialogs[2,30] = "请设置广播端口号与 录音设备端口号不同";
		dialogs[2,31] = "请设置录音设备端口 号与广播设备端口 号不同";
		dialogs[2,32] = "警告";
		dialogs[2,33] = "等待连接";
		dialogs[2,34] = "尝试连接到录音设备\n \n \n \n \n \n \n \n \n \n \n \n \n";
		dialogs[2,35] = "录音设备电池电量小于20%";
		dialogs[2,36] = "录音";
		dialogs[2,37] = "收听";
		dialogs[2,38] = "试用";
		dialogs[2,39] = "您正在使用试用 版。程序将于10分钟 后停止记录。您可 以手动重新开始录 制。请将本应用程 序升级到完整版本。\n \n \n \n \n \n \n \n \n \n \n \n \n \n";
		dialogs[2,40] = "因为您使用的是 试用版，录制10分钟 后已经结束。您可 以手动重新开始录 制。请将本应用程 序升级到完整版本。\n \n \n \n \n \n \n \n \n \n \n \n \n \n";
		dialogs[2,41] = "因为您使用的是试用版，录制10分钟 后已经结束。您可以 手动重新开始录 制。请将录音设备上 的应用程序升级到完整版本。\n \n \n \n \n \n \n \n \n \n \n \n";
		dialogs[2,42] = "别忘了下载我们的 Windows PC或Mac OSX 版本，让您在桌 面上听到宝 宝的声音。这是免费的！";
		dialogs[2,43] = "发送警报声";
		dialogs[2,44] = "是";
		dialogs[2,45] = "否";
		
		// language 3 portugese
		dialogs[3,0] =  "Ajuda";
		dialogs[3,1] =  "Sensibilidade do microfone da esquerda (maior) para a direita (menor). É transmitido apenas o som acima do nível (indicado por LEDs vermelhos). Toque no vídeo para forçar a transmissão. Rode o vídeo com o dedo, faça Zoom in ou Zoom out com gesto de pinça.";
		dialogs[3,2] =  "Uma lâmpada grande indica o nível da bateria do dispositivo de gravação. Uma lâmpada pequena indica o nível da bateria deste dispositivo.";
		dialogs[3,3] =  "Estado da ligação:\nligado (verde),\nà procura (amarelo)\n \n";
		dialogs[3,4] =  "Ajuste de volume\n \n \n";
		dialogs[3,5] =  "Créditos";
		dialogs[3,6] =  "Ideia e Conceção\n\nBen Ohloff\nAndruids\nwww.andruids.com\n \n";
		dialogs[3,7] =  "Bem-vindo ao Baby Monitor AV!";
		dialogs[3,8] =  "Carregue no botão de gravação e coloque o telefone próximo do seu bebé. Assegure-se de que os dispositivos de gravação e de escuta estão na mesma rede WiFi.";
		dialogs[3,9] =  "Carregue no botão de escuta em um ou mais telefones (Android ou iOS) ou descarregue o cliente para PC Windows ou Mac a partir do nosso site de Internet: www.andruids.com";
		dialogs[3,10] = "Preferências";
		dialogs[3,11] = "Avaliar esta aplicação";
		dialogs[3,12] = "Créditos";
		dialogs[3,13] = "Isenção de Responsabilidade";
		dialogs[3,14] = "Embora tenhamos feito o nosso melhor para garantir que esta aplicação funciona como pretendido é da sua exclusiva responsabilidade assegurar-se sempre que o seu bebé está seguro e bem.\n\nNão confie exclusivamente nos meios técnicos fornecidos por esta aplicação.\n \n";
		dialogs[3,15] = "Selecione um microfone e uma câmara caso o seu telefone ofereça mais à escolha.\n\nConfigure a porta do dispositivo de gravação para um número de porta diferente do número de porta de Transmissão.\n\nEscolha por quanto tempo deve continuar a transmissão da câmara depois do último som ter sido detetado.\n\nDetermine quanta vezes por segundo será transmitida uma imagem da câmara.\n\nDefina o alerta de som como ligado ou desligado para a transmissão.\n \n";
		dialogs[3,16] = "Opções";
		dialogs[3,17] = "Microfone";
		dialogs[3,18] = "Câmara";
		dialogs[3,19] = "Porta do dispositivo de gravação";
		dialogs[3,20] = "Porta de Transmissão";
		dialogs[3,21] = "Transmitir após atividade";
		dialogs[3,22] = "Imagens por segundo";
		dialogs[3,23] = "1/segundo";
		dialogs[3,24] = "2/segundos";
		dialogs[3,25] = "4/segundos";
		dialogs[3,26] = "5 segundos";
		dialogs[3,27] = "15 seg";
		dialogs[3,28] = "30 seg";
		dialogs[3,29] = "1 min";
		dialogs[3,30] = "Por favor configure a porta de Transmissão com um valor diferente do valor da porta do dispositivo de Gravação.";
		dialogs[3,31] = "Por favor configure a porta do dispositivo de Gravação com um valor diferente do valor da porta de Transmissão.";
		dialogs[3,32] = "Aviso!";
		dialogs[3,33] = "A aguardar a ligação";
		dialogs[3,34] = "A tentar ligar ao dispositivo\nde Gravação.\n \n \n \n \n \n \n \n \n \n";
		dialogs[3,35] = "A bateria do dispositivo de Gravação está com uma carga inferior a 20 %";
		dialogs[3,36] = "Gravação";
		dialogs[3,37] = "Escuta";
		dialogs[3,38] = "Experimentação";
		dialogs[3,39] = "Está a utilizar a versão de experimentação. Irá parar de gravar após decorridos 10 minutos. Pode reiniciar a gravação manualmente.\n\nPor favor faça a atualização para a versão completa desta aplicação.\n \n \n \n \n \n \n \n";
		dialogs[3,40] = "A gravação terminou após 10 minutos porque está a utilizar a versão de experimentação. Pode reiniciar a gravação manualmente.\n\nPor favor faça a atualização para a versão completa desta aplicação.\n \n \n \n \n \n \n \n";
		dialogs[3,41] = "A gravação terminou após 10 minutos porque está a utilizar a versão de experimentação. Pode reiniciar a gravação manualmente. Por favor faça a atualização para a versão completa desta aplicação no dispositivo de gravação.\n \n \n \n \n \n";
		dialogs[3,42] = "Não se esqueça de descarregar a nossa versão para PC Windows ou Mac OSX para escutar o seu bebé na sua secretária!\n\nÉ gratuito!";
		dialogs[3,43] = "Som de alerta de Transmissão ";
		dialogs[3,44] = "Sim";
		dialogs[3,45] = "Não";
		
		// language 4 italian
		dialogs[4,0] =  "Aiuto";
		dialogs[4,1] =  "Sensitività del Microfono da sinistra (più alta) a destra (più bassa). Solo il suono sopra il livello è trasmesso (come indicato dai LED rossi). Premi sul video per forzare la trasmissione. Gira il video con il dito, fai zoom avanti/indietro con un gesto delle due dita.";
		dialogs[4,2] =  "Un grosso segno indica il livello della batteria dello strumento di registrazione. Un piccolo segno indica lo stesso livello per questo strumento.";
		dialogs[4,3] =  "Stato della Connessione:\nconnesso (verde),\nsto cercando di connettermi (giallo)\n \n";
		dialogs[4,4] =  "Selettore del Volume\n \n \n";
		dialogs[4,5] =  "Crediti";
		dialogs[4,6] =  "Idea & Realizzazione di\n\nBen Ohloff\nAndruids\nwww.andruids.com\n \n";
		dialogs[4,7] =  "Benvenuto sul Baby Monitor AV!";
		dialogs[4,8] =  "Tocca il tasto registrazione e metti il telefono vicino al tuo bambino. Assicurati che i dispositivi di registrazione ed ascolto sono nella stessa rete WiFi.";
		dialogs[4,9] =  "Tocca il tasto di ascolto su uno o più altri telefoni (Android o iOS) oppure scarica il programma per Windows PC/Mac dal nostro sito web www.andruids.com";
		dialogs[4,10] = "Preferenze";
		dialogs[4,11] = "Vota questa app";
		dialogs[4,12] = "Crediti";
		dialogs[4,13] = "Avvertimento";
		dialogs[4,14] = "Anche se abbiamo fatto del nostro meglio per assicurare che questa app funzioni come voluto è sempre di tua esclusiva responsabilità assicurare che il tuo bambino sia sicuro e stia bene. Non fare esclusivo affidamento sui mezzi tecnici offerti da questa applicazione.\n \n";
		dialogs[4,15] = "Seleziona un dispositivo microfono ed un dispositivo fotocamera se il tuo telefono ne offre più di uno.\n\nSeleziona la porta del dispositivo di Registrazione ad un numero di porta disponibile e diverso dalla porta per la Trasmissione.\n\nScegli per quanto la trasmissione dalla videocamera deve continuare dopo l'ultimo suono captato.\n\nScegli quante volte al secondo una immagine per fotocamera sarà trasmessa.\n\nSeleziona l'allarme sonoro acceso/spento per la trasmissione.\n \n";
		dialogs[4,16] = "Opzioni";
		dialogs[4,17] = "Dispositivo per Microfono";
		dialogs[4,18] = "Dispositivo per Fotocamera";
		dialogs[4,19] = "Porta del Dispositivo per la Registrazione";
		dialogs[4,20] = "Porta di Trasmissione";
		dialogs[4,21] = "Trasmetti dopo l'attività";
		dialogs[4,22] = "Immagini al secondo";
		dialogs[4,23] = "1/secondo";
		dialogs[4,24] = "2/secondi";
		dialogs[4,25] = "4/secondi";
		dialogs[4,26] = "5 sec";
		dialogs[4,27] = "15 sec";
		dialogs[4,28] = "30 sec";
		dialogs[4,29] = "1 min";
		dialogs[4,30] = "Cortesemente seleziona la porta di Trasmissione ad un valore diverso dalla porta del dispositivo di Registrazione";
		dialogs[4,31] = "Cortesemente seleziona la porta per il dispositivo di Registrazione ad un valore diverso dal valore della porta di Trasmissione";
		dialogs[4,32] = "Attenzione!";
		dialogs[4,33] = "Attesa della connessione";
		dialogs[4,34] = "Tentativo di connessione\nal Dispositivo per la\nRegistrazione.\n \n \n \n \n \n \n \n \n \n";
		dialogs[4,35] = "La batteria del Dispositivo per la Registrazione è sotto al 20 %";
		dialogs[4,36] = "Registrazione";
		dialogs[4,37] = "Ascolto";
		dialogs[4,38] = "Prova";
		dialogs[4,39] = "Stai usando la versione di prova. Smetterà di registrare dopo 10 minuti. Puoi far ripartire la registrazione a mano.\n\nCortesemente fai un upgrade alla versione completa di questa app.\n \n \n \n \n \n \n \n";
		dialogs[4,40] = "La registrazione è terminata dopo 10 minuti perchè stavi usando la versione di Prova. Puoi ricominciare la registrazione manualmente. Cortesemente fai l'upgrade alla versione completa di questa app sul dispositivo per la registrazione.\n \n \n \n \n \n \n \n";
		dialogs[4,41] = "La registrazione è terminata dopo 10 minuti perchè stavi usando la versione di Prova. Puoi ricominciare la registrazione manualmente. Cortesemente fai l'upgrade alla versione completa di questa app sul dispositivo per la registrazione.\n \n \n \n \n \n \n \n";
		dialogs[4,42] = "Non dimenticarti di scaricare la nostra versione per Windows PC o Mac OSX per ascoltare il tuo bambino suo desktop!\n\nE' gratis!\n";
		dialogs[4,43] = "Suono Allarme per la Trasmissione";
		dialogs[4,44] = "Si";
		dialogs[4,45] = "No";
		
		// language 5 french
		dialogs[5,0] =  "Aide";
		dialogs[5,1] =  "Sensibilité du microphone de gauche (la plus élevée) à droite (la plus basse). Seuls les sons d'un niveau au-dessus sont transmis (comme indiqué par les LED rouges). Tapotez sur la vidéo pour mettre en marche la transmission. Tournez la vidéo avec le doigt, zoomez / dézoomez par un geste de pincement.";
		dialogs[5,2] =  "La grande lampe indique le niveau de la batterie du périphérique d'enregistrement. La petite lampe pour ce périphérique.";
		dialogs[5,3] =  "État de la connexion :\nconnecté (vert),\nrecherche en cours (jaune)\n \n";
		dialogs[5,4] =  "Curseur de volume\n \n \n";
		dialogs[5,5] =  "Crédits";
		dialogs[5,6] =  "Idée & réalisation\n\nBen Ohloff\nAndruids\nwww.andruids.com\n \n";
		dialogs[5,7] =  "Bienvenue à l'écoute-bébé AV téléphone !";
		dialogs[5,8] =  "Pressez sur le bouton d'enregistrement et placez le téléphone près de votre bébé. Assurez-vous que les appareils d'enregistrement et d'écoute sont dans le même réseau WiFi.";
		dialogs[5,9] =  "Pressez le bouton écoute sur un ou plusieurs autres téléphones (Android ou iOS) ou téléchargez le client pour PC Windows/Mac à partir de notre site web www.andruids.com";
		dialogs[5,10] = "Préférences";
		dialogs[5,11] = "Notez cette appli";
		dialogs[5,12] = "Crédits";
		dialogs[5,13] = "Avertissement";
		dialogs[5,14] = "Nous avons fait de notre mieux pour assurer que cette application fonctionne comme prévu, cependant il est de votre seule responsabilité de toujours vous assurer que votre bébé est bien et en sécurité.\n\nNe comptez pas exclusivement sur les moyens techniques fournis par cette application.\n \n";
		dialogs[5,15] = "Sélectionnez un microphone périphérique et un appareil photo si votre téléphone en offre plus d'un.\n\nRéglez le port du périphérique d'enregistrement sur un numéro de port disponible, différent du port d'émission.\n\nChoisissez pour combien de temps l'appareil de transmission devrait rester en marche après la dernière détection sonore.\n\nDéterminez le nombre de fois par seconde qu'une image de la caméra sera transmise.\n\nActivez / désactivez l'alerte sonore pour la transmission.\n \n";
		dialogs[5,16] = "Options";
		dialogs[5,17] = "Microphone périphérique";
		dialogs[5,18] = "Appareil photo périphérique";
		dialogs[5,19] = "Port de l'appareil d'enregistrement";
		dialogs[5,20] = "Port d'émission";
		dialogs[5,21] = "Transmettre après activité";
		dialogs[5,22] = "Images par seconde";
		dialogs[5,23] = "1 / seconde";
		dialogs[5,24] = "2 / seconde";
		dialogs[5,25] = "4 / seconde";
		dialogs[5,26] = "5 sec";
		dialogs[5,27] = "15 sec";
		dialogs[5,28] = "30 sec";
		dialogs[5,29] = "1 min";
		dialogs[5,30] = "Veuillez définir le port d'émission de valeur différente du port du périphérique d'enregistrement";
		dialogs[5,31] = "Veuillez définir le port du périphérique d'enregistrement de valeur différente du port d'émission";
		dialogs[5,32] = "Attention !";
		dialogs[5,33] = "En attente de connexion";
		dialogs[5,34] = "Tentative de connexion au\npériphérique d'enregistrement.\n \n \n \n \n \n \n \n \n \n";
		dialogs[5,35] = "Le niveau de la batterie du périphérique d'enregistrement est inférieure à 20 %" ;
		dialogs[5,36] = "Enregistrement";
		dialogs[5,37] = "Écoute";
		dialogs[5,38] = "Essai";
		dialogs[5,39] = "Vous utilisez la version d'essai. Il arrêtera l'enregistrement après 10 minutes.\n\nVous pouvez redémarrer l'enregistrement manuellement. Veuillez effectuer une mise à niveau vers la version complète de cet appli.\n \n \n \n \n \n \n \n";
		dialogs[5,40] = "L'enregistrement s'est terminé après 10 minutes parce que vous utilisez la version d'essai. Vous pouvez redémarrer l'enregistrement manuellement. Veuillez effectuer une mise à niveau vers la version complète de cet appli.\n \n \n \n \n \n \n \n";
		dialogs[5,41] = "L'enregistrement s'est terminé après 10 minutes parce que vous utilisez la version d'essai. Vous pouvez redémarrer l'enregistrement manuellement. Veuillez effectuer une mise à niveau vers la version complète de cette application sur le périphérique d'enregistrement.\n \n \n \n \n";
		dialogs[5,42] = "N'oubliez pas de télécharger notre version PC Windows ou Mac OSX pour écouter votre bébé de votre bureau !\n\nC'est gratuit !";
		dialogs[5,43] = "Son d'alerte de transmission";
		dialogs[5,44] = "Oui";
		dialogs[5,45] = "Non";
		
		// language 6 spanish
		dialogs[6,0]  ="Ayuda";
		dialogs[6,1]  ="Sensibilidad del micrófono de izquierda (más alto) a derecha (más bajo). Sólo serán transmitidos sonidos bajo los niveles legalmente establecidos (LEDs luz roja). Haga clic en iniciar transmisión de imagen de vídeo. Gire las imágenes con los dedos, el zoom se regula con dos dedos.";
		dialogs[6,2]  ="Lámpara grande muestra el estado de la batería del aparato transmisor. Lámpara pequeña para este aparato.";
		dialogs[6,3]  ="Mode de conexión:\nconectado (verde),\nbuscando (amarillo)\n \n"; 
		dialogs[6,4]  ="Regulador de volumen\n \n";
		dialogs[6,5]  ="Créditos";
		dialogs[6,6]  ="Idea y realización\n\nBen Ohloff\nAndruids\nwww.andruids.com\n \n";
		dialogs[6,7]  ="Bienvenido al Baby Monitor AV!";
		dialogs[6,8]  ="Inicie la aplicación en el modo Monitor y colóque el aparáto cerca de su hijo. Asegúrese de que el aparato reproductor se encuentra en su propia red WiFi.";
		dialogs[6,9]  ="Inicie un aparato en el modo de repetición. Este aparato puede ser otro Smartphone (Android o iOS) o puede también descargar la aplicación para Windows PC/Mac de nuestra Web www.andruids.com.";
		dialogs[6,10] ="Ajustes";
		dialogs[6,11] = "Califique esta aplicación";
		dialogs[6,12] = "Créditos";
		dialogs[6,13] = "Indicaciones";
		dialogs[6,14] = "Hemos hecho todo lo posible para que esta aplicación funcione correctamente, pero los problemas técnicos no pueden descartarse. Por favor asegúrese de que su bebé se encuentra bien y nunca lo deje únicamente con esta aplicación.\n \n"; 
		dialogs[6,15] = "Elija un micrófono y una cámara de las opciones que le ofrezca su Smartphone. \n\nColoque el puerto del aparato transmisor en un puerto libre disponible de su red que no sea idéntico al puerto del Broadcast. \n\nElija cuanto tiempo la transmisión debe prolongarse después de que el sonido sea reconocido. \n\nElija cuantas imágenes por segundo debe ser transmitidas. Coloque la advertencia al comienzo de la transmisión así como al final.\n \n";
		dialogs[6,16] = "Opciones";
		dialogs[6,17] = "Micrófono";
		dialogs[6,18] = "Cámara";
		dialogs[6,19] = "Puerto del Monitor";
		dialogs[6,20] = "Puerto Broadcast";
		dialogs[6,21] = "Transmisión del sonido";
		dialogs[6,22] = "Imágines por segundo";
		dialogs[6,23] = "1/Segundo";
		dialogs[6,24] = "2/Segundo";
		dialogs[6,25] = "4/Segundos"; 
		dialogs[6,26] = "5 Segundos"; 
		dialogs[6,27] = "15 Segundos";
		dialogs[6,28] = "30 Segundos"; 
		dialogs[6,29] = "1 Minuto";
		dialogs[6,30] = "Por favor, coloque el puerto Broadcast en un valor distinto al del puerto del monitor."; 
		dialogs[6,31] = "Por favor, coloque el puerto del monitor en un valor distinto al del puerto del Broadcast.";
		dialogs[6,32] = "Advertencia";
		dialogs[6,33] = "Espere conexión";
		dialogs[6,34] = "Espere conexión con\nel monitor.\n \n \n \n \n \n \n \n \n \n \n";
		dialogs[6,35] = "Batería del monitor por debajo del 20%";
		dialogs[6,36] = "Monitor";
		dialogs[6,37] = "Repetidor";
		dialogs[6,38] = "Trial";
		dialogs[6,39] = "Está utilizando la versión Trial de la aplicación. La recepción termina después de 10 minutos. Puede iniciarla en cualquier momento de   forma manual y probarla cuantas veces quiera. Por favor, si le gusta la aplicación, adquiera la versión completa.\n \n \n \n \n \n \n \n";
		dialogs[6,40] = "Está utilizando la versión Trial de la aplicación. La recepción termina después de 10 minutos. Puede iniciarla en cualquier momento de   forma manual y probarla cuantas veces quiera. Por favor, si le gusta la aplicación, adquiera la versión completa.\n \n \n \n \n \n \n \n"; 
		dialogs[6,41] = "Está utilizando la versión Trial de la aplicación. La recepción termina después de 10 minutos. Puede iniciarla en cualquier momento de   forma manual y probarla cuantas veces quiera. Por favor, si le gusta la aplicación, adquiera la versión completa.\n \n \n \n \n \n \n \n";
		dialogs[6,42] = "No olvide descargar la aplicación para Windows PC o Mac OXS desde nuestra página Web www.andruids.com. Es gratis!";
		dialogs[6,43] = "Tono de advertencia en la transmisión";
		dialogs[6,44] = "Sí";
		dialogs[6,45] = "No";
	
		// language 7 russian
		dialogs[7,0] =  "Помощь";
		dialogs[7,1] =  "Чувствительность микрофона слева (максимальная) направо (минимальная). Передаются звуки только выше заданного уровня (как показывает индикация  красных светодиодов). Нажмите на видео, чтобы начать принудительную передачу. Поворачивайте видео пальцем, увеличивайте / уменьшайте изображение с помощью пинч-жеста.";
		dialogs[7,2] =  "Большая лампочка показывает уровень аккумулятора записывающего устройства. Маленькая лампочка - для данного устройства.";
		dialogs[7,3] =  "Статус подключения:\nподключено (зеленый),\nпоиск (желтый)\n \n";
		dialogs[7,4] =  "Регулятор громкости\n \n \n";
		dialogs[7,5] =  "Сведения об авторах";
		dialogs[7,6] =  "Идея и исполнение\nБен Охлофф\nкомпания Andruids\nwww.andruids.com\n\n";
		dialogs[7,7] =  "Добро пожаловать в приложение Baby Monitor AV!";
		dialogs[7,8] =  "Нажмите кнопку \"Запись\" и разместите телефон вблизи вашего малыша. Убедитесь в том, что записывающие и прослушивающие устройства находятся в одной и той же сети WiFi.";
		dialogs[7,9] =  "Нажмите кнопку \"Прослушивание\" на одном или более телефонах (с операционной системой Android или iOS) или загрузите программу -клиент для Windows PC/Mac с нашего веб-сайта www.andruids.com";
		dialogs[7,10] = "Предпочтения";
		dialogs[7,11] = "Оцените данное приложение";
		dialogs[7,12] = "Сведения об авторах";
		dialogs[7,13] = "Ограничение ответственности";
		dialogs[7,14] = "При том, что мы сделали все возможное, чтобы данное приложение работало надлежащим образом, ваша ответственность - всегда проверять, что жизни и здоровью вашего малыша ничто не угрожает. Не полагайтесь исключительно на технические средства, предоставляемые данным приложением.\n \n";
		dialogs[7,15] = "Выберите соответствующий микрофон и камеру, если ваш телефон предлагает более одного варианта. Установите параметр порта для записывающего устройства на соответствующий номер порта, отличный от порта для вещания.\n\nВыберите, в течение какого времени передача с камеры будет продолжаться после того, как будет зафиксирован последний звук.\n\nОпределите, сколько раз в секунду будет передаваться изображение с камеры.\n\nУстановите звук сигнала оповещения в положение вкл выкл для режима передачи.\n \n";
		dialogs[7,16] = "Опции";
		dialogs[7,17] = "Устройство микрофона";
		dialogs[7,18] = "Устройство камеры";
		dialogs[7,19] = "Порт для устройства записи";
		dialogs[7,20] = "Порт для трансляции";
		dialogs[7,21] = "Передавать после активности";
		dialogs[7,22] = "Изображений в секунду";
		dialogs[7,23] = "1/сек.";
		dialogs[7,24] = "2/сек.";
		dialogs[7,25] = "4/сек.";
		dialogs[7,26] = "5 сек.";
		dialogs[7,27] = "15 сек.";
		dialogs[7,28] = "30 сек.";
		dialogs[7,29] = "1 мин.";
		dialogs[7,30] = "Пожалуйста, установите порт для передачи на другое значение, нежели для прибора записи";
		dialogs[7,31] = "Пожалуйста, установите порт для записывающего устройства на другое значение, нежели для порта передачи";
		dialogs[7,32] = "Предупреждение!";
		dialogs[7,33] = "Ожидает соединения";
		dialogs[7,34] = "Выполняется попытка\nподключения к\nзаписывающему устройству.\n \n \n \n \n \n \n \n \n \n";
		dialogs[7,35] = "Заряд батареи записывающего устройства ниже 20 %";
		dialogs[7,36] = "Запись";
		dialogs[7,37] = "Прослушивание";
		dialogs[7,38] = "Пробная версия";
		dialogs[7,39] = "Вы используете пробную версию. Приложение прекратит запись через 10 минут. Вы можете возобновить запись вручную. Пожалуйста, перейдите на полную версию данного приложения.\n \n \n \n \n \n \n \n";
		dialogs[7,40] = "Запись завершилась после 10 минут работы, поскольку вы используете тестовую версию. Вы можете вручную возобновить запись. Пожалуйста, перейдите на полную версию данного приложения.\n \n \n \n \n \n \n \n";
		dialogs[7,41] = "Запись завершилась после 10 минут работы, поскольку вы используете тестовую версию. Вы можете вручную возобновить запись. Пожалуйста, перейдите на полную версию данного приложения для записывающего устройства.\n \n \n \n \n \n";
		dialogs[7,42] = "Не забудьте загрузить нашу версию для Windows ПК или Mac OSX для прослушивания вашего малыша на своем настольном компьютере!\n\nЭто бесплатно!";
		dialogs[7,43] = "Передача сигнала звукового оповещения";
		dialogs[7,44] = "Да";
		dialogs[7,45] = "Нет";		

		// language 8 japanese
		dialogs[8,0] =  "ヘルプ";
		dialogs[8,1] =  "左（最高）から右（最低 ）のマイク感度。設定 レベル以上の音声の み送信されます（LED で表示）。ビデオをタッ プして送信開始。ビデオを 指で回転。つまむジェス チャーでズームイン/アウト。";
		dialogs[8,2] =  "大きなランプは録画デバ イスの電池レベルを示し ます。小さなランプはこ のデバイスの表示です。";
		dialogs[8,3] =  "接続状態：接続済（緑） 、検索中（黄色）\n \n \n";
		dialogs[8,4] =  "ボリュームスライダー\n \n \n \n";
		dialogs[8,5] =  "クレジット";
		dialogs[8,6] =  "アイディア&現実化\nBen Ohloff\nAndruids\nwww.andruids.com";
		dialogs[8,7] =  "Baby Monitor AVへ用こそ！ ";
		dialogs[8,8] =  "録画ボタンを押して携帯を 赤ちゃんのそばに置いてく ださい。録画とリスニン グデバイスが同じWiFi ネットワーク内にあ ることを確認してくだ さい。";
		dialogs[8,9] =  "リスニングボタンを1つま たは複数のほかの携帯（ AndroidまたはiOS）で押 すか、Windows PC/Mac 用クライアントを当社 ウェブサイト www.andruids.com からダウンロードしてください。";
		dialogs[8,10] = "設定";
		dialogs[8,11] = "このアプリを評価";
		dialogs[8,12] = "クレジット";
		dialogs[8,13] = "免責事項";
		dialogs[8,14] = "このアプリの正常動作には 最善を尽くしていますが、赤 ちゃんの安全確保は全てユ ーザーの責任となります。 本アプリケーションによ る技術的手段にのみ頼る ことは裂けてください。\n \n";
		dialogs[8,15] = "お持ちの携帯で複数のマイ クやカメラが搭載されている 場合は、各デバイスを選択 してください。録画デバイ スポートをブロードキャ ストポートとは異なる利用 可能なポート番号に設定してくだ さい。音声感知後カメラ通信を 継続する時間を選択しま す。毎秒何枚のカメラ画像 を送信するか選択します。 通信開始時のアラート 音のオン/オフを設定します。\n \n";
		dialogs[8,16] = "オプション";
		dialogs[8,17] = "マイクデバイス";
		dialogs[8,18] = "カメラデバイス";
		dialogs[8,19] = "録画デバイスポート";
		dialogs[8,20] = "ブロードキャストポート";
		dialogs[8,21] = "活動感知後通信開始";
		dialogs[8,22] = "毎秒の画像数";
		dialogs[8,23] = "1/秒";
		dialogs[8,24] = "2/秒";
		dialogs[8,25] = "4/秒";
		dialogs[8,26] = "5秒間";
		dialogs[8,27] = "15秒間";
		dialogs[8,28] = "30秒間";
		dialogs[8,29] = "1分間";
		dialogs[8,30] = "ブロードキャストポート を録画デバイスポートと 異なる値に設定してください";
		dialogs[8,31] = "録画デバイスポートをブ ロードキャストポートと 異なる値に設定してください";
		dialogs[8,32] = "警告！";
		dialogs[8,33] = "接続待ち";
		dialogs[8,34] = "録画デバイスに接続中。\n \n \n \n \n \n \n \n \n \n";
		dialogs[8,35] = "録画デバイスの電池 残量が20 %未満に なりました";
		dialogs[8,36] = "録画中";
		dialogs[8,37] = "リスニング中";
		dialogs[8,38] = "トライアル";
		dialogs[8,39] = "現在トライアルバージョ ンを使用しています。 録画は10分後に停止しま す。録画は手動で再開す ることができます。このア プリのフルバージョンにア ップグレードしてください。\n \n \n \n \n \n \n \n \n \n";
		dialogs[8,40] = "現在トライアルバージョ ンを使用しているため、 10分後に録画が停止しまし た。録画は手動で再開する ことができます。この アプリのフルバージョ ンにアップグレードして ください。\n \n \n \n \n \n \n \n \n \n";
		dialogs[8,41] = "現在トライアルバージョ ンを使用しているため、 10分後に録画が停止しまし た。録画は手動で再開する ことができます。このアプリ のフルバージョンにアップグ レードしてください。\n \n \n \n \n \n \n \n \n \n";
		dialogs[8,42] = "赤ちゃんをデスクトップ で監視できるWindows PC またはMac OSX版をダウ ンロードするのを忘れ ずに！無料です！";
		dialogs[8,43] = "通信アラート音 ";
		dialogs[8,44] = "はい";
		dialogs[8,45] = "いいえ";
		
		// language 9 korean
		dialogs[9,0] =  "도움";
		dialogs[9,1] =  "마이크 감도 왼쪽(가장 높음) 오른쪽(가장 낮음). 일정 수준 이상의 소리만 전송됩니다 (붉은 LED로 표시됨). 강제로 전송하시려면 비디오를 탭하십시오. 손가락으로 비디오를 돌리거나 꼬집는 동작으로 확대/축소할 수 있습니다.";
		dialogs[9,2] =  "큰 램프는 녹화하는 기기, 작은 램프는 해당 기기의 배터리 수준을 표시합니다.\n \n \n";
		dialogs[9,3] =  "연결 상태: 연결됨 (녹색), 검색중 (노란색)\n \n \n";
		dialogs[9,4] =  "음량 조정자\n \n";
		dialogs[9,5] =  "제작자";
		dialogs[9,6] =  "아이디어 & 실현\nBen Ohloff\nAndruids\nwww.andruids.com ";
		dialogs[9,7] =  "Baby Monitor AV 사용을 환영합니다!";
		dialogs[9,8] =  "녹화 버튼을 누르고 폰을 아기 가까이에 두십시오. 녹화 및 청취 기기가 같은 와이파이 네트워크상에 있는지 확인하십시오.";
		dialogs[9,9] =  "한 대의 전화기 또는 여러 대의 전화기 (안드로이드 또는 iOS)의 청취 버튼을 누르시거나 저희 웹사이트 www.andruids.com에서 윈도우즈 PC / MAC용의 단말 프로그램을 다운로드받으십시오.";
		dialogs[9,10] = "선택사항";
		dialogs[9,11] = "이 앱 평가하기";
		dialogs[9,12] = "제작자";
		dialogs[9,13] = "면책조항";
		dialogs[9,14] = "비록 저희가 이 앱이 의도된 대로 작동하도록 최선을 다했지만 당신의 아기가 항상 안전하고 건강하도록 지키는 것은 당신의 책임입니다. 이 애플리케션이 제공한 기술적인 방법에만 의존하지 마십시오.\n \n";
		dialogs[9,15] = "폰에 하나 이상의 녹음 및 카메라 기기가 있다면 사용할 기기를 선택하십시오. 녹화 기기 포트를 브로드캐스트 포트와 다른 사용가능한 포트 넘버로 설정하십시오. 소리가 마지막으로 감지된 때로부터 얼마나 오랫동안 카메라 전송이 이루어지도록 할 지 선택하십시오. 일초당 몇 번이나 카메라 이미지를 존송할 지 결정하십시오. 전송을 위한 경고음을 켤 지 끌지 설정하십시오.\n \n";
		dialogs[9,16] = "옵션";
		dialogs[9,17] = "마이크 기기";
		dialogs[9,18] = "카메라 기기";
		dialogs[9,19] = "녹화용 기기 포트";
		dialogs[9,20] = "브로드캐스트 포트";
		dialogs[9,21] = "활동 발생 이후 전송";
		dialogs[9,22] = "일초당 이미지";
		dialogs[9,23] = "1/초";
		dialogs[9,24] = "2/초";
		dialogs[9,25] = "4/초";
		dialogs[9,26] = "5 초";
		dialogs[9,27] = "15 초";
		dialogs[9,28] = "30 초";
		dialogs[9,29] = "1 분";
		dialogs[9,30] = "브로드캐스트 포트를 녹화용 기기 포트와 다른 값으로 설정하십시오.";
		dialogs[9,31] = "녹화용 기기 포트를 브로드캐스트 포트와 다른 값으로 설정하십시오.";
		dialogs[9,32] = "준비중!";
		dialogs[9,33] = "연결 대기중";
		dialogs[9,34] = "녹화용 기기와의 연결 시도중.\n \n \n \n \n \n \n \n \n \n";
		dialogs[9,35] = "녹화용 기기의 배터리 20% 미만 남음";
		dialogs[9,36] = "녹화중";
		dialogs[9,37] = "청취중";
		dialogs[9,38] = "시험";
		dialogs[9,39] = "시험용 버전을 사용하고 계십니다. 10분 후 녹화가 종료됩니다. 녹화를 수동을 재시작하실 수 있습니다. 이 앱의 풀 버전으로 업그레이드해 주십시오.\n \n \n \n \n \n \n \n \n \n";
		dialogs[9,40] = "시험용 버전을 사용하고 계시므로 녹화가 10분 후 종료되었습니다. 수동으로 녹화를 재시작하실 수 있습니다. 이 앱의 풀 버전으로 업그레이드해 주십시오.\n \n \n \n \n \n \n \n \n \n";
		dialogs[9,41] = "시험용 버전을 사용하고 계시므로 녹화가 10분 후 종료되었습니다. 수동으로 녹화를 재시작하실 수 있습니다. 녹화용 기기상에서 이 앱의 풀 버전으로 업그레이드해 주십시오.\n \n \n \n \n \n \n";
		dialogs[9,42] = "잊지 마시고 데스크탑에서 아기 소리를 들을 수 있도록 저희 윈도우즈 PC 또는 Mac OSX 버전을 다운로드 받으십시오! 무료입니다!";
		dialogs[9,43] = "전송 경고음";
		dialogs[9,44] = "예";
		dialogs[9,45] = "아니오";
	}
}
