using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Stick2ProjectileHitboxCtrl : MonoBehaviour
{
    public BoxCollider2D col;
    public int damage;
    public bool isPierce;
    public bool isStuckIgnore;
    public bool isWithKnockBack;
    public float kbForce = 0;
    private List<GameObject> collidedObjects = new List<GameObject>();
    public Stick2ProjectileCtrl projectileCtrl;
    public Stick3ProjectileBase stick3ProjectileCtrl;
    //[SerializeField] SerializableInterface<IStick3Projectile> projectile;

    private int id;
    private int forceID;
    public HitNoticeSender lastHitNoticeSender = null;

    public GameObject hitEffectPrefab;
    public bool isAffectToTarget = false;
    public GameObject effectPrefabForEnemy;
    public int effectID = 1;

    public int team;
    public int typeID;
    public void Init(int damage, int layermask, bool isPierce)
    {
        this.damage = damage;
        col.contactCaptureLayers = layermask;
        this.isPierce = isPierce;
    }

    public void OnEnable()
    {
        collidedObjects = new List<GameObject>();
        col.enabled = true;
        id = Random.Range(-10000000, 10000000);
        if (isWithKnockBack)
        {
            forceID = Random.Range(-20000000, -30000000);
        }
    }

    public void OnDisable()
    {
        collidedObjects = new List<GameObject>();
        col.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Projectile" || collision.gameObject.tag == "HitBox")
        {
            return;
        }

        bool isEffected = false;
        bool isStucked = false;
        bool isKilled = false;
        if (!collidedObjects.Contains(collision.gameObject))
        {
            collidedObjects.Add(collision.gameObject);

            HitNoticeSender hit = collision.GetComponent<HitNoticeSender>();
            if(hit != null)
            {
                if (isAffectToTarget)
                {
                    isKilled = hit.HitNoticeWithEffect(damage, id, effectPrefabForEnemy, effectID, false, team, typeID);
                }
                else
                {
                    //isKilled = hit.HitNotice(damage, id, null, false);
                    isKilled = hit.HitNotice(damage, id, false, team, typeID);
                }

                if (isWithKnockBack)
                {
                    hit.KBNotice(kbForce, forceID);
                }
                isEffected = true;
            }

            if (collision.gameObject.tag == "ArrowStuck")
            {
                isStucked = true;
            }
            lastHitNoticeSender = hit;
        }

        if (isStucked)
        {
            Stuck(collision.gameObject, false);
        }
        else if (!isPierce)
        {
            if (!isKilled)
            {
                Stuck(collision.gameObject, true);
            }
            else
            {
                if(projectileCtrl != null)
                {
                    projectileCtrl.SpriteFadeOut();
                }else if(stick3ProjectileCtrl != null)
                {
                    stick3ProjectileCtrl.KilledHit(collision.gameObject);
                }
            }
        }

        if(hitEffectPrefab != null)
        {
            GameObject obj = Instantiate(hitEffectPrefab);
            obj.transform.position = collision.transform.position;
        }

        lastHitNoticeSender = null;

    }

    public void Stuck(GameObject stuckObj, bool isParentChange)
    {
        if (isStuckIgnore)
        {
            return;
        }
        if (projectileCtrl != null)
        {
            projectileCtrl.Stuck(stuckObj, isParentChange);
        }
        else if (stick3ProjectileCtrl != null)
        {
            stick3ProjectileCtrl.StuckHit(stuckObj, isParentChange);
        }

        col.enabled = false;
    }
}
