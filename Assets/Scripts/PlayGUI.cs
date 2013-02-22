using UnityEngine;
using System.Collections;

public class PlayGUI {
	private Play play;
	
	private MyGUI gui;
	private int container;
	private int dialogContainer;
	
	public PlayGUI(Play p) {
		play = p;
		
		// GUI stuff
		gui = (GameObject.Instantiate(play.game.guiPrefab) as GameObject).GetComponent<MyGUI>();
		gui.Initialize(play.game, play.game.gameInput);
		MyGUI.CenterOnScreen(gui.transform);
		MyGUI.ResizeToScreenSize(gui.transform);
		container = gui.AddContainer();
		
		Vector3 fullSize = gui.containers[container].GetSize();
		Vector3 screenCenter = gui.containers[container].GetCenter();
		int topContainer = gui.AddContainer(container, fullSize, new Vector2(screenCenter.x, screenCenter.y), true);

		// ship health
		int healthContainer = gui.AddContainer(topContainer, new Vector3(0.06f, 0.06f, 1.0f), true, MyGUI.GUIAlignment.Right, 0.025f, MyGUI.GUIAlignment.Top, 0.01f);
		gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 0.0f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_TITLE, 0);
		gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 0.05f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_TITLE, 0);
		gui.AddImage(healthContainer, MyGUI.GUIAlignment.Right, 0.1f, MyGUI.GUIAlignment.Top, 0.0f, Game.GUI_UV_TITLE, 0);
	}
	
	private void CloseDialog() {
		GameObject.Destroy(gui.containers[dialogContainer].gameObject);
		gui.ResetGameInputZLevel();
		gui.DeleteGUIInFocus();
	}
	

}

/*
 * 
private var currentScore : int;
private var ticks : int;
private var count : int[];
private var lastTick : float;
private var challengeModeOn : boolean;

private static var tickDelta : float = 0.05;

function Awake() {
	currentScoreSection = transform.Find("CurrentScore").gameObject;
	scoreDigits = currentScoreSection.GetComponentsInChildren.<GUITexture>();
	newPointsSection = transform.Find("NewPoints").gameObject;
	newPointsDigits = newPointsSection.GetComponentsInChildren.<GUITexture>();
}

public function Initialize(g : Game, p : Play) {
	game = g;
	play = p;
}

function Update() {
	if (challengeModeOn && ticks > 0 && Time.time > lastTick + tickDelta) {
		if (count[1] == 9) {
			count[1] = 0;
			if (count[2] == 9) {
				count[2] = 0;
				if (count[3] == 9) {
					count[3] = 0;
					if (count[4] == 9) {
						// should not happen
					} else {
						count[4]++;
					}
				} else {
					count[3]++;
				}
			} else {
				count[2]++;
			}
		} else {
			count[1]++;
		}
		if (ticks == 1) {
			count[0] = Play.GetDigitOfNumber(0, currentScore);
		} else {
			count[0] = Random.Range(0,10);
		}
		DisplayScore(count);
		lastTick = Time.time;
		ticks--;
	}
}

public function Reset(cMOn : boolean) {
	challengeModeOn = cMOn;
	currentScore = 0;
	ticks = 0;
	count = [0,0,0,0,0];
	DisplayScore(count);
	if (challengeModeOn) {
		currentScoreSection.SetActiveRecursively(true);
		newPointsSection.SetActiveRecursively(true);
	} else {
		currentScoreSection.SetActiveRecursively(false);
		newPointsSection.SetActiveRecursively(false);
	}
}

public function SetScore(nP : int, cS : int) {
	ticks = Mathf.Floor(cS/10.0) - Mathf.Floor(currentScore/10.0);
	currentScore = cS;
	DisplayNewPoints(nP);
	lastTick = Time.time;
}

private function DisplayScore(digits : int[]) {
	if (challengeModeOn) {
		scoreDigits[0].texture = game.numberTextures[digits[0]];
		scoreDigits[1].texture = game.numberTextures[digits[1]];
		scoreDigits[2].texture = game.numberTextures[digits[2]];
		scoreDigits[3].texture = game.numberTextures[digits[3]];
		scoreDigits[4].texture = game.numberTextures[digits[4]];
	}	
}
*/