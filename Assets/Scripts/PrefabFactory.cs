using System;
using UnityEngine;
using System.Collections;

public class PrefabFactory {
	private Game game;
	private Play play;

	private GameObject gunBulletTemplate;
	private GameObject laserShotTemplate;
	private GameObject phaserShotTemplate;
	private GameObject gaussShotTemplate;
	private GameObject missileShotTemplate;
	private GameObject guidedMissileShotTemplate;
	private GameObject chargedMissileShotTemplate;
	private GameObject detonatorMissileShotTemplate;
	private GameObject detonatorBombShotTemplate;
	private GameObject breadcrumbTemplate;
	private GameObject miniMapBreadcrumbTemplate;
	private GameObject explosionTemplate;
	private GameObject hitTemplate;
	private GameObject healthDropTemplate;
	private GameObject shieldDropTemplate;
	private GameObject[] missileDropTemplates;
	private GameObject mineTouchTemplate;
	
	public PrefabFactory(Game g) {
		game = g;
	}
	
	public void Initialize(Play p) {
		play = p;
		gunBulletTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.BULLET]) as GameObject;
		gunBulletTemplate.GetComponent<Shot>().enabled = false;
		laserShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.LASER]) as GameObject;
		laserShotTemplate.GetComponent<Shot>().enabled = false;
		phaserShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.PHASER]) as GameObject;
		phaserShotTemplate.GetComponent<Shot>().enabled = false;
		gaussShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.GAUSS]) as GameObject;
		gaussShotTemplate.GetComponent<Shot>().enabled = false;
		missileShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.MISSILE]) as GameObject;
		missileShotTemplate.GetComponent<Shot>().enabled = false;
		guidedMissileShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.GUIDED_MISSILE]) as GameObject;
		guidedMissileShotTemplate.GetComponent<Shot>().enabled = false;
		chargedMissileShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.CHARGED_MISSILE]) as GameObject;
		chargedMissileShotTemplate.GetComponent<Shot>().enabled = false;
		detonatorMissileShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.DETONATOR_MISSILE]) as GameObject;
		detonatorMissileShotTemplate.GetComponent<Shot>().enabled = false;
		detonatorBombShotTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.DETONATOR_BOMB]) as GameObject;
		detonatorBombShotTemplate.GetComponent<Shot>().enabled = false;
		breadcrumbTemplate = GameObject.Instantiate(game.breadcrumbPrefab) as GameObject;
		breadcrumbTemplate.GetComponent<Breadcrumb>().enabled = false;
		miniMapBreadcrumbTemplate = GameObject.Instantiate(game.miniMapBreadcrumbPrefab) as GameObject;
		explosionTemplate = GameObject.Instantiate(game.explosionPrefab) as GameObject;
		explosionTemplate.GetComponent<Explosion>().enabled = false;
		HTSpriteSheet expl = explosionTemplate.GetComponent<HTSpriteSheet>();
		expl.enabled = false;
		expl.CreateParticle();
		hitTemplate = GameObject.Instantiate(game.hitPrefab) as GameObject;
		hitTemplate.GetComponent<Hit>().enabled = false;
		expl = hitTemplate.GetComponent<HTSpriteSheet>();
		expl.enabled = false;
		expl.CreateParticle();
		healthDropTemplate = GameObject.Instantiate(game.healthDropPrefab) as GameObject;
		healthDropTemplate.GetComponent<CollecteableHealth>().enabled = false;
		healthDropTemplate.tag = ""; // so it is not removed at the end of zone
		shieldDropTemplate = GameObject.Instantiate(game.shieldDropPrefab) as GameObject;
		shieldDropTemplate.GetComponent<CollecteableShield>().enabled = false;
		shieldDropTemplate.tag = "";
		missileDropTemplates = new GameObject[4];
		missileDropTemplates[Weapon.TYPE_MISSILE] = GameObject.Instantiate(game.missileDropPrefabs[Weapon.TYPE_MISSILE]) as GameObject;
		missileDropTemplates[Weapon.TYPE_MISSILE].GetComponent<CollecteableMissile>().enabled = false;
		missileDropTemplates[Weapon.TYPE_MISSILE].tag = "";
		missileDropTemplates[Weapon.TYPE_GUIDED_MISSILE] = GameObject.Instantiate(game.missileDropPrefabs[Weapon.TYPE_GUIDED_MISSILE]) as GameObject;
		missileDropTemplates[Weapon.TYPE_GUIDED_MISSILE].GetComponent<CollecteableMissile>().enabled = false;
		missileDropTemplates[Weapon.TYPE_GUIDED_MISSILE].tag = "";
		missileDropTemplates[Weapon.TYPE_CHARGED_MISSILE] = GameObject.Instantiate(game.missileDropPrefabs[Weapon.TYPE_CHARGED_MISSILE]) as GameObject;
		missileDropTemplates[Weapon.TYPE_CHARGED_MISSILE].GetComponent<CollecteableMissile>().enabled = false;
		missileDropTemplates[Weapon.TYPE_CHARGED_MISSILE].tag = "";
		missileDropTemplates[Weapon.TYPE_DETONATOR_MISSILE] = GameObject.Instantiate(game.missileDropPrefabs[Weapon.TYPE_DETONATOR_MISSILE]) as GameObject;
		missileDropTemplates[Weapon.TYPE_DETONATOR_MISSILE].GetComponent<CollecteableMissile>().enabled = false;
		missileDropTemplates[Weapon.TYPE_DETONATOR_MISSILE].tag = "";
		mineTouchTemplate = GameObject.Instantiate(game.shotPrefabs[Shot.MINE_TOUCH]) as GameObject;
		mineTouchTemplate.GetComponent<Shot>().enabled = false;
	}
	
	public Shot CreateGunShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newBullet = GameObject.Instantiate(gunBulletTemplate, pos, rot) as GameObject;
		Shot shot = newBullet.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.BULLET);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateLaserShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newLaser = GameObject.Instantiate(laserShotTemplate, pos, rot) as GameObject;
		Shot shot = newLaser.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.LASER);
		shot.enabled = true;
		return shot;
	}

	public Shot CreatePhaserShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newPhaser = GameObject.Instantiate(phaserShotTemplate, pos, rot) as GameObject;
		Shot shot = newPhaser.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.PHASER);
		shot.enabled = true;
		return shot;
	}

	public Shot CreateGaussShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newGauss = GameObject.Instantiate(gaussShotTemplate, pos, rot) as GameObject;
		Shot shot = newGauss.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.GAUSS);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(missileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.MISSILE);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}

	public Shot CreateGuidedMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(guidedMissileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.GUIDED_MISSILE);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}

	public Shot CreateChargedMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(chargedMissileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.CHARGED_MISSILE);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}

	public Shot CreateDetonatorMissileShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMissile = GameObject.Instantiate(detonatorMissileShotTemplate, pos, rot) as GameObject;
		Shot shot = newMissile.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.DETONATOR_MISSILE);
		shot.enabled = false;
		if (mountedTo == Game.ENEMY) {
			shot.gameObject.layer = Game.LAYER_GUN_ENEMY;
		}
		return shot;
	}

	public Shot CreateDetonatorBombShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newDetonatorBomb = GameObject.Instantiate(detonatorBombShotTemplate, pos, rot) as GameObject;
		Shot shot = newDetonatorBomb.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.DETONATOR_BOMB);
		shot.enabled = true;
		return shot;
	}
	
	public Shot CreateMineTouchShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject newMineTouch = GameObject.Instantiate(mineTouchTemplate, pos, rot) as GameObject;
		Shot shot = newMineTouch.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.MINE_TOUCH);
		shot.enabled = false;
		return shot;
	}
	
	public Shot CreateLaserBeamShot(Vector3 pos, Quaternion rot, int damage, int mountedTo) {
		GameObject laserBeam = GameObject.Instantiate(game.shotPrefabs[Shot.LASER_BEAM], pos, rot) as GameObject;
		Shot shot = laserBeam.GetComponent<Shot>();
		shot.Initialize(play, damage, mountedTo, Shot.LASER_BEAM);
		shot.enabled = false;
		return shot;
	}
	
	public GameObject CreateBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(breadcrumbTemplate, pos, rot) as GameObject;
		Breadcrumb breadcrumb = newBreadcrumb.GetComponent<Breadcrumb>();
		breadcrumb.Initialize(play);
		breadcrumb.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		breadcrumb.enabled = true;
		return newBreadcrumb;
	}

	public GameObject CreateMiniMapBreadcrumb(Vector3 pos, Quaternion rot) {
		GameObject newBreadcrumb = GameObject.Instantiate(miniMapBreadcrumbTemplate, pos, rot) as GameObject;
		return newBreadcrumb;
	}

	public GameObject CreateHealthDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newHealthDrop = GameObject.Instantiate(healthDropTemplate, pos, rot) as GameObject;
		newHealthDrop.tag = CollecteablePowerUp.TAG;
		CollecteableHealth healthDrop = newHealthDrop.GetComponent<CollecteableHealth>();
		healthDrop.Initialize(play, amount);
//		healthDrop.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		healthDrop.enabled = true;
		return newHealthDrop;
	}

	public GameObject CreateShieldDrop(Vector3 pos, Quaternion rot, int amount) {
		GameObject newShieldDrop = GameObject.Instantiate(shieldDropTemplate, pos, rot) as GameObject;
		newShieldDrop.tag = CollecteablePowerUp.TAG;
		CollecteableShield shieldDrop = newShieldDrop.GetComponent<CollecteableShield>();
		shieldDrop.Initialize(play, amount);
//		shieldDrop.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		shieldDrop.enabled = true;
		return newShieldDrop;
	}

	public GameObject CreateMissileDrop(Vector3 pos, Quaternion rot, int type, int amount) {
		GameObject newMissileDrop = GameObject.Instantiate(missileDropTemplates[type], pos, rot) as GameObject;
		newMissileDrop.tag = CollecteablePowerUp.TAG;
		CollecteableMissile missileDrop = newMissileDrop.GetComponent<CollecteableMissile>();
		missileDrop.Initialize(play, type, amount);
//		missileDrop.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		missileDrop.enabled = true;
		return newMissileDrop;
	}

	public GameObject CreateKeyDrop(Vector3 pos, Quaternion rot, int keyType) {
		GameObject newKeyDrop = GameObject.Instantiate(game.keyPrefab, pos, rot) as GameObject;
		CollecteableKey keyDrop = newKeyDrop.GetComponent<CollecteableKey>();
		keyDrop.Initialize(play, keyType, play.game.keyTextures[keyType]);
		keyDrop.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
//		keyDrop.renderer.material.mainTexture = play.game.keyTextures[keyType];
		keyDrop.enabled = true;
		return newKeyDrop;
	}

	public GameObject CreatePowerUpDrop(Vector3 pos, Quaternion rot, int type, int id) {
		GameObject newPowerUpDrop;
		if (type == Game.POWERUP_PRIMARY_WEAPON) {
			newPowerUpDrop = GameObject.Instantiate(game.primaryWeaponPrefabs[id], pos, rot) as GameObject;
			newPowerUpDrop.tag = CollecteablePowerUp.TAG;
		} else if (type == Game.POWERUP_SECONDARY_WEAPON) {
			newPowerUpDrop = GameObject.Instantiate(game.powerUpSecondaryPrefabs[id], pos, rot) as GameObject;
		} else if (type == Game.POWERUP_HULL) {
			newPowerUpDrop = GameObject.Instantiate(game.powerUpHullPrefab, pos, rot) as GameObject;
		} else {
//			newPowerUpDrop = GameObject.Instantiate(game.primaryWeaponPrefabs[id], pos, rot) as GameObject;
			newPowerUpDrop = GameObject.Instantiate(game.powerUpSpecialPrefab, pos, rot) as GameObject;
		}
		CollecteablePowerUp powerUpDrop = newPowerUpDrop.AddComponent<CollecteablePowerUp>();
		SphereCollider col = newPowerUpDrop.AddComponent<SphereCollider>();
		col.isTrigger = true;
		col.radius *= 2;
		powerUpDrop.Initialize(play, type, id);
		powerUpDrop.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		powerUpDrop.enabled = true;
		return newPowerUpDrop;
	}
	
	public GameObject CreateExplosion(Vector3 pos, Quaternion rot) {
		GameObject newExplosion = GameObject.Instantiate(explosionTemplate, pos, rot) as GameObject;
		Explosion explosion = newExplosion.GetComponent<Explosion>();
		explosion.enabled = true;
		HTSpriteSheet spriteSheet = explosion.GetComponent<HTSpriteSheet>();
		spriteSheet.Initialize(play, play.ship.shipCamera.transform, game.explosionMaterials[UnityEngine.Random.Range(0,2)]);
		spriteSheet.enabled = true;
		explosion.Initialize(play);
		explosion.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		return newExplosion;
	}

	public GameObject CreateHit(Vector3 pos, Quaternion rot, string tag) {
		GameObject newHit = GameObject.Instantiate(hitTemplate, pos, rot) as GameObject;
		Hit hit = newHit.GetComponent<Hit>();
		HTSpriteSheet spriteSheet = hit.GetComponent<HTSpriteSheet>();
		if (tag == Enemy.TAG || tag == Ship.TAG) {
			spriteSheet.Initialize(play, play.ship.shipCamera.transform, game.explosionMaterials[UnityEngine.Random.Range(0,2)]);
		} else {
			spriteSheet.Initialize(play, play.ship.shipCamera.transform, game.explosionMaterials[2]); // smoke
		}
		spriteSheet.enabled = true;
		hit.enabled = true;
		hit.transform.localScale *= (RoomMesh.MESH_SCALE/5f);
		return newHit;
	}
	
}

