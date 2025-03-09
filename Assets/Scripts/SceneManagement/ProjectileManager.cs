using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : Singleton<ProjectileManager>
{
    public Vector3 LastDestroyedProjectilePosition{get; private set;}
    public bool HitIndestructible {get; private set;}

    public void SetLastDestroyedProjectile(Vector3 position, bool hitIndestructible){
string caller = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name; // Get function name that called this
    Debug.Log($"Updating ProjectileManager from {caller}: Position = {position}, Hit Indestructible = {hitIndestructible}");

    // Detect when false overwrites true
    if (HitIndestructible && !hitIndestructible)
    {
        Debug.LogWarning($"WARNING: HitIndestructible was previously TRUE but is now being set to FALSE! Called by {caller}");
    }
    
        LastDestroyedProjectilePosition = position;
        HitIndestructible = hitIndestructible;
    }
}
