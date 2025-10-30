using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class Stick3ViewCtrl : MonoBehaviour
{
    [Header("参照")]
    private Stick3Stats stats;
    private Stick3MoveCtrl move;
    private StickEnemyDetector detector;
    public Rigidbody2D rb;
    public GameObject chestObj;


    [Header("ボーン/パーツ/コンポーネント")]
    public GameObject bodyObj;
    private Rigidbody2D[] rbs;
    private Joint2D[] joints;
    public Collider2D[] landCols;
    public BoxCollider2D bodyBlockCol;
    public Collider2D[] boneCols;


    [Header("武器/エフェクト")]
    public Collider2D[] weaponCols;
    public GameObject[] weaponObjs;
    public TrailRenderer[] weaponTrails;
    public LineRenderer[] lines;
    public GameObject[] effectObjs;

    [Header("スプライト/カラー")]
    public SpriteRenderer[] stickSpritesA;
    public SpriteRenderer[] stickSpritesB;
    public Color[] spriteColorA;
    public Color[] spriteColorB;
    public Color deadColor;
    public bool isSetSortingLayer = true;
    public bool isSetColorAtInit = true;
    public bool isFreezeDefaultColorSet = false;
    public bool[] freezeColorChangeFlags;

    [Header("盾関連")]
    public Collider2D shieldCol;
    public GameObject shieldObj;
    public Rigidbody2D shieldRb;
    private bool shieldDropped = false;

    private List<Color> baseColorList;


    [HideInInspector] public float deadRbForceRate = 1.0f;
    [Inject]
    public void Inject(Stick3Stats stats, Stick3MoveCtrl move, StickEnemyDetector detector)
    {
        this.stats = stats;
        this.move = move;
        this.detector = detector;
    }
    
    public void Initialize()
    {

        rbs = bodyObj.GetComponentsInChildren<Rigidbody2D>();
        joints = bodyObj.GetComponentsInChildren<Joint2D>();
        
        if (stats.team == 0)
        {
            this.gameObject.layer = Constant.STICK0_LAYER;
            bodyBlockCol.gameObject.layer = Constant.STICK0_LAYER;
            bodyBlockCol.forceReceiveLayers = Constant.LAYERMASK_STICK1_ONLY;

            foreach (Collider2D col in boneCols)
            {
                col.gameObject.layer = Constant.STICK0_LAYER;
                col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK0;
            }

        }
        else if (stats.team == 1)
        {
            this.gameObject.layer = Constant.STICK1_LAYER;
            bodyBlockCol.gameObject.layer = Constant.STICK1_LAYER;
            bodyBlockCol.forceReceiveLayers = Constant.LAYERMASK_STICK0_ONLY;

            transform.parent.localScale = new Vector3(transform.parent.localScale.x * -1.0f, transform.parent.localScale.y, transform.parent.localScale.z);

            foreach (Collider2D col in boneCols)
            {
                col.gameObject.layer = Constant.STICK1_LAYER;
                col.contactCaptureLayers = Constant.LAYERMASK_ALL_WITHOUT_STICK1;
            }

        }


        foreach (SpriteRenderer spriteA in stickSpritesA)
        {
            if (isSetColorAtInit)
            {
                spriteA.color = spriteColorA[stats.team];
            }

            spriteA.DOFade(1.0f, 0.75f);
            if (isSetSortingLayer)
            {
                spriteA.sortingLayerName = GetSortingLayerName(stats.typeId);
            }
        }

        if (isFreezeDefaultColorSet)
        {
            int colorCount = 0;
            baseColorList = new List<Color>();

            foreach (SpriteRenderer spriteA in stickSpritesA)
            {
                baseColorList.Add(spriteA.color);
            }

        }


        foreach (SpriteRenderer spriteB in stickSpritesB)
        {
            if (isSetColorAtInit)
            {
                spriteB.color = spriteColorB[stats.team];
            }

            spriteB.DOFade(1.0f, 0.75f);
            if (isSetSortingLayer)
            {
                spriteB.sortingLayerName = GetSortingLayerName(stats.typeId);
            }

        }
    }

    public void LateBlow()
    {
        foreach (Rigidbody2D rbb in rbs)
        {
            if(rbb != null)
            {
                Vector3 moveDirection = new Vector3(move.moveVector.normalized.x + Random.Range(-0.05f, 0.05f), move.moveVector.normalized.y + Random.Range(0.4f, 0.2f), 0f);
                int randomValue = (Random.Range(0, 2) * 2) - 1;
                rbb.AddTorque(Random.Range(120f, 180f) * randomValue);
                rbb.AddForce(move.moveVector * (Random.Range(-35f, -30f) * deadRbForceRate));
            }

        }
    }

    public void Dead()
    {

        foreach (Collider2D landCol in landCols)
        {
            landCol.enabled = false;
        }

        foreach (Collider2D col in boneCols)
        {
            col.enabled = false;
        }

        if(bodyBlockCol != null)
        {
            bodyBlockCol.enabled = false;
        }


        rb.linearVelocity = Vector2.zero;
        rb.angularDamping = 0f;
        rb.isKinematic = true;

        foreach (Rigidbody2D rbb in rbs)
        {
            if(rbb != null)
            {
                rbb.isKinematic = false;
                rbb.gravityScale = 0.38f;
            }

        }
        Invoke("LateBlow", 0.02f);

        foreach (Joint2D joint in joints)
        {
            joint.breakForce = 500005.0f;
            joint.breakForce = 500005.0f;
            joint.enabled = true;
        }

        foreach (GameObject weaponObj in weaponObjs)
        {
            weaponObj.transform.parent = null;
        }

        if (weaponCols != null && weaponCols.Length >= 1)
        {
            foreach (Collider2D colw in weaponCols)
            {
                colw.enabled = false;
            }
        }

        if (shieldCol != null && !shieldDropped)
        {
            shieldCol.enabled = false;
        }

        if (weaponTrails != null && weaponTrails.Length >= 1)
        {
            foreach (TrailRenderer trail in weaponTrails)
            {
                trail.enabled = false;
            }
        }

        if(lines != null && lines.Length >= 1)
        {
            foreach(LineRenderer line in lines)
            {
                line.DOColor(new Color2(deadColor, deadColor), new Color2(deadColor, deadColor), 1.0f);
            }
        }


        foreach (SpriteRenderer spriteA in stickSpritesA)
        {
            spriteA.DOColor(deadColor, 1.0f);
        }
        foreach (SpriteRenderer spriteB in stickSpritesB)
        {
            spriteB.DOColor(deadColor, 1.0f);
        }

        if (effectObjs != null && effectObjs.Length >= 1)
        {
            foreach (GameObject effectObj in effectObjs)
            {
                if (effectObj != null)
                {
                    Destroy(effectObj);
                }
            }
        }
    }

    public void BodyColorAdjust(Color setColor, float rate)
    {

        if (!isFreezeDefaultColorSet)
        {
            if (rate < 1.0f)
            {
                setColor = Color.Lerp(spriteColorA[stats.team], setColor, rate);
            }
            foreach (SpriteRenderer spriteA in stickSpritesA)
            {
                spriteA.color = setColor;
            }
        }
        else
        {
            int colorCount = 0;
            foreach (SpriteRenderer spriteA in stickSpritesA)
            {
                if (freezeColorChangeFlags[colorCount])
                {
                    spriteA.color = Color.Lerp(baseColorList[colorCount], setColor, rate);
                }
               colorCount++;
            }
        }

    }

    public void Down()
    {

        foreach (Collider2D col in boneCols)
        {
            col.enabled = false;
        }

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        bodyBlockCol.enabled = false;

        if (weaponCols != null && weaponCols.Length >= 1)
        {
            foreach (Collider2D colw in weaponCols)
            {
                colw.enabled = false;
            }
        }

        if (weaponTrails != null && weaponTrails.Length >= 1)
        {
            foreach (TrailRenderer trail in weaponTrails)
            {
                trail.enabled = false;
            }
        }
        
        if (shieldCol != null && !shieldDropped)
        {
            shieldCol.enabled = false;
        }
    }

    public void Revive()
    {
        foreach (Collider2D col in boneCols)
        {
            col.enabled = true;
        }
        
        rb.isKinematic = false;

        if (weaponTrails != null && weaponTrails.Length >= 1)
        {
            foreach (TrailRenderer trail in weaponTrails)
            {
                trail.enabled = true;
            }
        }
        
        if (shieldCol != null)
        {
            shieldCol.enabled = true;
        }
    }

    public void OnDestroy()
    {
        foreach (GameObject weaponObj in weaponObjs)
        {
            if (weaponObj != null)
            {
                Destroy(weaponObj);
            }

            if (shieldObj != null)
            {
                Destroy(shieldObj);
            }

        }
    }

    public void ShieldDrop()
    {
        if (shieldObj != null)
        {
            shieldObj.transform.parent = null;
            shieldRb.isKinematic = false;
            shieldCol.enabled = false;
            shieldRb.AddForce(move.moveVector * (Random.Range(-35f, -30f) * deadRbForceRate));
            shieldDropped = true;
        }
    }


    private string GetSortingLayerName(int typeId)
    {
        return typeId switch
        {
            0 => "TopDefault",
            1 => "TopDefault2",
            2 => "TopDefault3",
            3 => "TopDefault4",
            4 => "TopDefault5",
            5 => "TopDefault6",
            _ => "TopDefault",
        };
    }
}
