using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {
	public static string TAG = "Enemy";
	
	public abstract void Damage(int damage);
	

}
