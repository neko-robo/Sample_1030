using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHitboxCtrl : MonoBehaviour
{
    public BoxCollider2D col;
    public int damage;
    private List<GameObject> collidedObjects = new List<GameObject>();
    private int id;

    public float damageRate = 1.0f;
    public int damageAddition = 0;
    public Stick3Ctrl stick;
    public bool isWithKnockBack;

    public void Init(int damage, int layermask)
    {
        this.damage = damage;
        col.contactCaptureLayers = layermask;
    }

    public void InitByTeam(int damage, int team)
    {
        this.damage = damage;
        if(team == 0)
        {
            col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK0;
        }
        else
        {
            col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK1;
        }

    }

    public void OnEnable()
    {
        collidedObjects = new List<GameObject>();
        col.enabled = true;
        id = Random.Range(-10000000, 10000000);
    }

    public void OnDisable()
    {
        collidedObjects = new List<GameObject> ();
        col.enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collidedObjects.Contains(collision.gameObject))
        {
            collidedObjects.Add(collision.gameObject);

            HitNoticeSender hit = collision.GetComponent<HitNoticeSender>();
            if (hit != null)
            {
                int damageBuffed = Mathf.FloorToInt(damage * damageRate) + damageAddition;
                hit.HitNotice(damageBuffed, id, stick, isWithKnockBack);
            }
        }
    }
}
