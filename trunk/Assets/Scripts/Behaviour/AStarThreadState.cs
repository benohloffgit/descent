using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public class AStarThreadState {
	public LinkedList<AStarNode> roomPath;
	public LinkedList<AStarNode> zonePath;
	
//	private float startTime;
	private bool isProcessing = false;
	private bool isFinished = true;

	public enum Mode { ROOM=0, ZONE=1 }
	
	public AStarThreadState() {
//		zonePath = new LinkedList<AStarNode>();
	}
	
	public void Start() {
//		startTime = Time.realtimeSinceStartup;
		isProcessing = true;
		isFinished = false;
		roomPath = new LinkedList<AStarNode>();
		zonePath = new LinkedList<AStarNode>();
	}
	
	public void Finish() {
		isFinished = true;
	}
	
	public void Complete() {
		isProcessing = false;
	//	Debug.Log("AStar completed in " + (Time.realtimeSinceStartup-startTime));
	}
	
	public bool IsFinishedNow() {
		return (isFinished && isProcessing);
	}
}

