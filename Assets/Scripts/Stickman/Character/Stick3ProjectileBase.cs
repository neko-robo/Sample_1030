using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Stick3ProjectileBase : MonoBehaviour
{
    public int atk;
    private int team;
    public Rigidbody2D rb;
    public Collider2D col;
    public Stick2ProjectileHitboxCtrl hitboxCtrl;
    private bool destroyReserved = false;
    public bool isPierce = false;

    public GameObject effectInitDissappearObj;
    private IStickEffectInitDissappear effectInitDissappear;

    public abstract void Init(int team, int atk, Vector3 direction, bool isBoss);

    public abstract void StuckHit(GameObject obj, bool isParentChange);

    public abstract void KilledHit(GameObject obj);

    //public abstract void SpriteFadeOut();

    public void InitColAndValue(int team, int atk)
    {
        this.atk = atk;
        this.team = team;

        if (team == 0)
        {
            this.gameObject.layer = Constant.STICK0_LAYER;
            col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK0;
            hitboxCtrl.Init(atk, Constant.LAYERMASK_ALL_WITHOUT_STICK0, isPierce);
        }
        else if (team == 1)
        {
            this.gameObject.layer = Constant.STICK1_LAYER;
            col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK1;
            hitboxCtrl.Init(atk, Constant.LAYERMASK_ALL_WITHOUT_STICK1, isPierce);
        }
    }

    public void LateDestroy()
    {
        if (destroyReserved)
        {
            return;
        }
        destroyReserved = true;
        Destroy(this.gameObject);
    }

    public void InitializeEffects()
    {
        if (effectInitDissappearObj != null)
        {
            effectInitDissappear = effectInitDissappearObj.GetComponent<IStickEffectInitDissappear>();
        }


        if (effectInitDissappear != null)
        {
            effectInitDissappear.Init();
        }
    }

    public void DissappearEffects()
    {
        if (effectInitDissappear != null)
        {
            effectInitDissappear.Dissapear();
        }
    }
}
