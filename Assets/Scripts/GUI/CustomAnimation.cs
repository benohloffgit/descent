using UnityEngine;
using System.Collections;

public class CustomAnimation : MonoBehaviour {
	public enum Mode { Stopped=0, Paused=1, Running=2 }
	public enum Direction { Forward=0, Backward=1 }
	
	private float lastRun;
	private Mode mode;
	private Direction direction;
	private float minValue = 1.0f;
	private float maxValue = 1.2f;
	private float currentValue;
	
	private static float SPEED = 0.5f;
	private static float INTERVAL = 3.0f;
	
	void Start() {
		lastRun = Time.time;
		mode = Mode.Paused;
	}
	
	void Update () {
		if (mode == Mode.Paused && Time.time > lastRun + INTERVAL) {
				mode = Mode.Running;
				direction = Direction.Forward;
				currentValue = minValue;
		} else if (mode == Mode.Running && direction == Direction.Forward) {
			currentValue += Time.deltaTime * SPEED;
			transform.localScale = new Vector3(currentValue, currentValue, currentValue);
			if (currentValue >= maxValue) {
				direction = Direction.Backward;
			}
		} else if (mode == Mode.Running && direction == Direction.Backward) {
			currentValue -= Time.deltaTime * SPEED;
			transform.localScale = new Vector3(currentValue, currentValue, currentValue);
			if (currentValue <= minValue) {
				lastRun = Time.time;
				mode = Mode.Paused;
			}
		}
	}
}
