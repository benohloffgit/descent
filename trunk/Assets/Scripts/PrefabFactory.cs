using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;
	private Play play;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	private GameObject missileShotTemplate;
	private GameObject breadcrumbTemplate;
	private GameObject explosionTemplate;
	private GameObject hitTemplate;
	private GameObject healthDropTemplate;
	private GameObject shieldDropTemplate;
	private GameObject missileDropTemplate;
	private GameObject mineTouchTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize(Play p) {
		play = p;
		gunBulletTemplate = GameObject.Instantiate(game.gunBulletPrefab) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.laserShotPrefab) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
		missileShotTemplate = GameObject.Instantiate(game.missileShotPrefab) as GameObject;
		missileShotTemplate.GetComponent<Shot>().enabled = false;
		breadcrumbTemplate = GameObject.Instantiate(game.breadcrumbPrefab) as GameObject;
		breadcrumbTemplate.GetComponent<Breadcrumb>().enabled = false;
		explosionTemplate = GameObject.Instantiate(game.explosionPrefab) as GameObject;
		explosionTemplate.GetComponent<Explosion>().enabled = false;
		hitTemplate = GameObject.Instantiate(game.hitPrefab) as GameObject;
		hitTemplate.GetComponent<Hit>().enabled = false;
		healthDropTemplate = GameObject.Instantiate(game.healthDropPrefab) as GameObject;
		healthDropTemplate.GetComponent<CollecteableHealth>().enabled = false;
		shieldDropTemplate = GameObject.Instantiate(game.shieldDropPrefab) as GameObject;
		shieldDropTemplate.GetComponent<CollecteableShield>().enabled = false;
		missileDropTemplate = GameObject.Instantiate(game.missileDropPrefab) as GameObject;
		missileDropTemplate.GetComponent<CollecteableMissile>().enabled = false;
		mineTouchTemplate = GameObject.Instantiate(game.mineTouchShotPrefab) as GameObject;
		mineTouchTemplate.GetComponent<Shot>().enabled = false;
	}
	
	public Shot CreateGunShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.BULLET);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateLaserShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.BULLET);
		shot.enabled = true;
		return shot;
	}

	public Shot CreateMissileShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.MISSILE);
		shot.enabled = false;
		return shot;
	}

	public Shot CreateMineTouchShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newMineTouch = GameObject.Instantiate(mineTouchTemplate, pos, rot) as GameObject;
		Shot shot = newMineTouch.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.MINE_TOUCH);
		shot.enabled = false;
		return shot;
	}
	
	public Shot CreateGuidedMissileShot(Vector3 pos, Quaternion rot, int damage, int source) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, source, Shot.GUIDED);
		shot.enabled = false;
		return shot;
	}
	
	public GameObject CreateBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(breadcrumbTemplate, pos, rot) as GameObject;
		Breadcrumb breadcrumb = newBreadcrumb.GetComponent<Breadcrumb>();
		breadcrumb.Initialize(play);
		breadcrumb.enabled = true;
		return newBreadcrumb;
	}

	public GameObject CreateHealthDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newHealthDrop = GameObject.Instantiate(healthDropTemplate, pos, rot) as GameObject;
		CollecteableHealth healthDrop = newHealthDrop.GetComponent<CollecteableHealth>();
		healthDrop.Initialize(play, amount);
		healthDrop.enabled = true;
		return newHealthDrop;
	}

	public GameObject CreateShieldDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newShieldDrop = GameObject.Instantiate(shieldDropTemplate, pos, rot) as GameObject;
		CollecteableShield shieldDrop = newShieldDrop.GetComponent<CollecteableShield>();
		shieldDrop.Initialize(play, amount);
		shieldDrop.enabled = true;
		return newShieldDrop;
	}

	public GameObject CreateMissileDrop(Vector3 pos, Quaternion rot, int type, int amount) {
		GameObject newMissileDrop = GameObject.Instantiate(missileDropTemplate, pos, rot) as GameObject;
		CollecteableMissile missileDrop = newMissileDrop.GetComponent<CollecteableMissile>();
		missileDrop.Initialize(play, type, amount);
		missileDrop.enabled = true;
		return newMissileDrop;
	}

/*	public GameObject CreateScrollDrop(Vector3 pos, Quaternion rot) {
		GameObject newScrollDrop = GameObject.Instantiate(game.scrollDropPrefab, pos, rot) as GameObject;
		CollecteableScroll scrollDrop = newScrollDrop.GetComponent<CollecteableScroll>();
		scrollDrop.Initialize(play);
		scrollDrop.enabled = true;
		return newScrollDrop;
	}*/

	public GameObject CreatePowerUpDrop(Vector3 pos, Quaternion rot, int weaponType, int index) {
		GameObject newPowerUpDrop;
		int wType, wModel;
		if (weaponType == Weapon.PRIMARY) {
			wType = Weapon.SHIP_PRIMARY_WEAPON_TYPES[index];
			wModel = Weapon.SHIP_PRIMARY_WEAPON_MODELS[index];
			newPowerUpDrop = GameObject.Instantiate(game.primaryWeaponPrefabs[wType-1], pos, rot) as GameObject;
		} else {
			wType = Weapon.SHIP_SECONDARY_WEAPON_TYPES[index];
			wModel = Weapon.SHIP_SECONDARY_WEAPON_MODELS[index];
			newPowerUpDrop = GameObject.Instantiate(game.powerUpPrefabs[wType-1], pos, rot) as GameObject;
		}
		CollecteablePowerUp powerUpDrop = newPowerUpDrop.AddComponent<CollecteablePowerUp>();
		SphereCollider col = newPowerUpDrop.AddComponent<SphereCollider>();
		col.isTrigger = true;
		powerUpDrop.Initialize(play, weaponType, wType, wModel);
		powerUpDrop.enabled = true;
		return newPowerUpDrop;
	}
	
	public GameObject CreateExplosion(Vector3 pos, Quaternion rot) {
		GameObject newExplosion = GameObject.Instantiate(explosionTemplate, pos, rot) as GameObject;
		Explosion explosion = newExplosion.GetComponent<Explosion>();
		explosion.enabled = true;
		explosion.Initialize(play);
		return newExplosion;
	}

	public GameObject CreateHit(Vector3 pos, Quaternion rot) {
		GameObject newHit = GameObject.Instantiate(hitTemplate, pos, rot) as GameObject;
		Hit hit = newHit.GetComponent<Hit>();
		hit.enabled = true;
		hit.Initialize(play);
		return newHit;
	}
	
}

