using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStick3Projectile
{
    public void DisableProjectile();
    public void Stuck(GameObject obj, bool isParentChange);

}
