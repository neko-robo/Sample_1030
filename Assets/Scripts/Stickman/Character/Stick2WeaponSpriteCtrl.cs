using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stick2WeaponSpriteCtrl : MonoBehaviour
{
    public SpriteRenderer weaponSprite;
    private int atk;
    private int team;
    private bool isBoss;
    public GameObject projectilePrefab;
    public GameObject shootDirectionFrom;
    public GameObject shootDirectionTo;
    private Tweener fadeInTweener;
    public GameObject shootEffectPrefab;
    private Vector2 shootDirectionReserve = Vector2.zero;
    private bool isShootDirectionReserved = false;
    [HideInInspector]
    public Stick2ProjectileCtrl preGeneratedProjectile;
    
     public void Init(int team, int atk, bool isBoss)
    {
        this.atk = atk;
        this.team = team;
        this.isBoss = isBoss;
    }

    public void SpriteDisplay()
    {
        fadeInTweener = weaponSprite.DOFade(1.0f, 0.45f);
    }

    public void SpriteHide()
    {
        fadeInTweener = weaponSprite.DOFade(0.0f, 0.3f);
    }

    public void ProjectileShoot(float distanceSqr)
    {

        //Debug.Log("shoot");
        if(fadeInTweener != null && fadeInTweener.IsActive())
        {
            fadeInTweener.Kill();
        }

        weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, 0.0f);
        GameObject obj = Instantiate(projectilePrefab);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.localScale = transform.lossyScale;

        Stick2ProjectileCtrl ctrl = obj.GetComponent<Stick2ProjectileCtrl>();
        Vector3 direction = shootDirectionTo.transform.position - shootDirectionFrom.transform.position;
        direction = direction.normalized;
        if (isShootDirectionReserved)
        {
            direction = shootDirectionReserve;
        }
        ctrl.Init(team, atk, direction, isBoss);
        

        if(shootEffectPrefab != null)
        {
            GameObject effect = Instantiate(shootEffectPrefab);
            effect.transform.position = transform.position;
            effect.transform.rotation = transform.rotation;
        }
        //float distance = Mathf.Sqrt(distanceSqr);
        //float forceRate = Mathf.Lerp(0.6f, 2.4f, Mathf.InverseLerp(4f, 30f, distance));
        //Debug.Log("ArrowShoot" + team + " sqr:" + distanceSqr + " rate:" + forceRate);
        //ctrl.Init(team, atk, direction.normalized, isBoss, forceRate);
    }
    
    public void ReserveShootDirection(Vector2 direction)
    {
        /*Debug.Log("ReserveShootDirection" + " x:" + direction.x + " y:" + direction.y);*/
        if (team == 1)
        {
            direction = new Vector2(direction.x * -1.00f, direction.y);
        }
        shootDirectionReserve = direction;
        isShootDirectionReserved = true;
    }
    
    public void ProjectileGenerate()
    {
        // 既に生成されている場合は何もしない
        if (preGeneratedProjectile != null)
            return;

        // プロジェクタイルを生成
        GameObject obj = Instantiate(projectilePrefab);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        obj.transform.localScale = transform.lossyScale;

        preGeneratedProjectile = obj.GetComponent<Stick2ProjectileCtrl>();

        // プロジェクタイルを初期化（ただし移動とヒットボックスは無効）
        if (preGeneratedProjectile != null)
        {
            preGeneratedProjectile.Prepare(team, atk, this);
            preGeneratedProjectile.transform.parent = transform;
        }
    }

    public void ProjectileActivate()
    {
        if (fadeInTweener != null && fadeInTweener.IsActive())
        {
            fadeInTweener.Kill();
        }

        weaponSprite.color = new Color(weaponSprite.color.r, weaponSprite.color.g, weaponSprite.color.b, 0.0f);

        if (preGeneratedProjectile != null)
        {
            // 事前に生成したプロジェクタイルを発射
            Vector3 direction = shootDirectionTo.transform.position - shootDirectionFrom.transform.position;
            if (isShootDirectionReserved)
            {
                direction = shootDirectionReserve;
            }

            //Debug.Log("Projectile Activate");
            preGeneratedProjectile.transform.parent = null;
            preGeneratedProjectile.Activate(direction.normalized);
            preGeneratedProjectile = null; // 使い終わったのでクリア
        }

        if (shootEffectPrefab != null)
        {
            GameObject effect = Instantiate(shootEffectPrefab);
            effect.transform.position = transform.position;
            effect.transform.rotation = transform.rotation;
        }
    }
}
