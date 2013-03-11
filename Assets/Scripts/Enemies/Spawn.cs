using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {
		
	private Game game;
	private Play play;
	private EnemyDistributor enemyDistributor;
	
	private GridPosition gridPos;
	private Vector3 worldPos;
	private string enemyClazz;
	private int enemyModel;
	private float frequency;
	private int maxLiving;
	private int maxGenerated;
	
	private float lastTimeGenerated;
	private int numberGenerated;
	private int currentlyLiving;
	
	private const int INFINITY = -1;
	
	public void Initialize(EnemyDistributor enemyDistributor_, Play play_, GridPosition gridPos_,
				string enemyClazz_, int enemyModel_, float frequency_ = 5.0f, int maxLiving_ = 3, int maxGenerated_ = INFINITY) {
		enemyDistributor = enemyDistributor_;
		play = play_;
		game = play.game;
		gridPos = gridPos_;
		worldPos = gridPos.GetWorldVector3();
		enemyClazz = enemyClazz_;
		enemyModel = enemyModel_;
		frequency = frequency_;
		maxLiving = maxLiving_;
		maxGenerated = maxGenerated_;
	}
	
	void Start() {
		lastTimeGenerated = Time.time;
		numberGenerated = 0;
		currentlyLiving = 0;
	}
	
	void Update() {
		if ( (maxGenerated == INFINITY || numberGenerated < maxGenerated) && currentlyLiving < maxLiving) { 
			if (Time.time > lastTimeGenerated + frequency) {
				Enemy e = enemyDistributor.CreateEnemy(this, enemyClazz, enemyModel);
				e.transform.position = worldPos;
				numberGenerated++;
				currentlyLiving++;
				lastTimeGenerated = Time.time;		
			}
		}
	}
	
	public void Die(Enemy e) {
		currentlyLiving--;
	}
	
}

