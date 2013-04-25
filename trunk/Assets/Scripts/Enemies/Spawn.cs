using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {
	public static string TAG = "Spawn";
	
	public bool isBoss;
//	private Game game;
	private Play play;
	private EnemyDistributor enemyDistributor;
	
	private GridPosition gridPos;
	private Vector3 worldPos;
	private int enemyClazz;
	private int enemyModel;
	private int enemyEquivalentClazzAModel;
	private float frequency;
	private int maxLiving;
	private int maxGenerated;
	private bool isActive;
	
	private float lastTimeGenerated;
	private int numberGenerated;
	private int currentlyLiving;
	private Renderer myRenderer;
	
	public const int INFINITY = -1;
	
	void Awake() {
		myRenderer = GetComponentInChildren<Renderer>();
	}
	
	public void Initialize(EnemyDistributor enemyDistributor_, Play play_, GridPosition gridPos_,
				int enemyClazz_, int enemyModel_, int enemyEquivalentClazzAModel_ ,float frequency_, int maxLiving_, int maxGenerated_,
				bool isBoss_) {
		enemyDistributor = enemyDistributor_;
		play = play_;
		isBoss = isBoss_;
//		game = play.game;
		gridPos = gridPos_;
		worldPos = gridPos.GetWorldVector3();
		enemyClazz = enemyClazz_;
		enemyModel = enemyModel_;
		enemyEquivalentClazzAModel = enemyEquivalentClazzAModel_;
		frequency = frequency_;
		maxLiving = maxLiving_;
		maxGenerated = maxGenerated_;
	}
	
	void Start() {
		lastTimeGenerated = Time.time;
		numberGenerated = 0;
		currentlyLiving = 0;
		isActive = true;
		DistributeInitialSet();
	}
		
	void FixedUpdate() {
		if (isActive) {
			Vector3 isShipVisible =  play.ship.IsVisibleFrom(transform.position);
			if (isShipVisible != Vector3.zero) {
				transform.LookAt(play.GetShipPosition(), play.ship.transform.up);
			}
	
			if ( (maxGenerated == INFINITY || numberGenerated < maxGenerated) && currentlyLiving < maxLiving) { 
				if (Time.time > lastTimeGenerated + frequency) {
					Enemy e = enemyDistributor.CreateEnemy(this, enemyClazz, enemyModel, enemyEquivalentClazzAModel);
					e.transform.position = worldPos;
					numberGenerated++;
					currentlyLiving++;
					lastTimeGenerated = Time.time;		
					if (numberGenerated == maxGenerated) {
						Deactivate();
					}
				}
			}
		}
	}
	
	private void DistributeInitialSet() {
		Room r = play.cave.zone.GetRoom(gridPos);
		for (int i=0; i<maxLiving; i++) {
			Enemy e = enemyDistributor.CreateEnemy(this, enemyClazz, enemyModel, enemyEquivalentClazzAModel);
			e.transform.position = r.GetRandomNonExitGridPosition().GetWorldVector3();
			numberGenerated++;
			currentlyLiving++;
		}
		if (numberGenerated >= maxGenerated) {
			Deactivate();
		}
	}
	
	public void Die(Enemy e) {
		currentlyLiving--;
		enemyDistributor.RemoveEnemy(e);
		lastTimeGenerated = Time.time;
		if (currentlyLiving == 0 && !isActive) {
			Destroy(gameObject);
		}
	}
	
	public void LoseHealth(Enemy e, int loss) {
		enemyDistributor.LoseHealth(e, loss);
	}
	
	public void ActivateEnemy(Enemy e) {
		enemyDistributor.ActivateEnemy(e);
	}
	
	public void DeactivateEnemy(Enemy e) {
		enemyDistributor.DeactivateEnemy(e);
	}
	
	private void Deactivate() {
		isActive = false;
		myRenderer.enabled = false;
	}
	
}

