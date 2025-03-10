using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : Singleton<ProjectileManager>
{
    public Vector3 LastDestroyedProjectilePosition{get; private set;}
    public bool HitIndestructible {get; private set;}

    public void SetLastDestroyedProjectile(Vector3 position, bool hitIndestructible){

        LastDestroyedProjectilePosition = position;
        HitIndestructible = hitIndestructible;
    }
}
