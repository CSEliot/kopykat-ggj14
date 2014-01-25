using UnityEngine;
using System.Collections;

public class AmmoBoxLogic : MonoBehaviour {
	
	public int MissileAmmo;
	public int BulletAmmo;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnCollisionEnter(Collision collisionInfo)
	{
		//get the colliding mech's weapons
		ActorController colliderWepInput = ParentSearches.GetComponentInParents<ActorController>(collisionInfo.collider.gameObject);
		if(colliderWepInput != null)
		{
			if(colliderWepInput.MissileLauncher != null && colliderWepInput.MissileMaxAmmo > 0)
			{
				MissileAmmo -= colliderWepInput.MissileLauncher.AddAmmo(MissileAmmo);
			}
			if(colliderWepInput.Cannon != null && colliderWepInput.CannonMaxAmmo > 0)
			{
				BulletAmmo -= colliderWepInput.Cannon.AddAmmo(BulletAmmo);
			}
			
			//only remove the box if the box doesn't have any more ammo
			if(MissileAmmo <= 0 && BulletAmmo <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
