using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stick2ProjectileCtrl : MonoBehaviour, IStick3Projectile
{
    public SpriteRenderer projectileSprite;
    public SpriteRenderer projectileSprite2;
    public TrailRenderer trailRenderer;
    public GameObject spriteParent;
    public int atk;
    private int team;
    public Rigidbody2D rb;
    public Collider2D col;
    public Stick2ProjectileHitboxCtrl hitboxCtrl;
    private bool destroyReserved = false;
    private bool isFlying = false;
    public Vector3 initialDirection = Vector3.zero;
    public float bulletSpeed = 2.0f;
    public SpriteSpin spin;

    public bool isStuckable = true;
    public float fadeOutTime = 2.0f;
    public GameObject effectPrefab;
    public float fadeOutTweenTime = 0.45f;
    public bool isPierce = false;
    public bool isParabola = false;
    public bool isSpriteDirectionAdjust = false;
    public bool isForceInit = false;
    public bool isSignMove = false;
    private float time;
    private Vector2 shootDirection;
    private float waveAmplitude = 0.6f; // �h�ꕝ
    private float waveFrequency = 9f; // �h��̑���
    public GameObject effectInitDissappearObj;
    private IStickEffectInitDissappear effectInitDissappear;

    public float decreaseRate = 0.2f;

    public float angleRate = 1.0f;
    
    
    
    public void Init(int team, int atk, Vector3 direction, bool isBoss)
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


        /*initialDirection = new Vector3(direction.x + Random.Range(-0.1f, 0.1f), direction.y + Random.Range(-0.05f, 0.05f), 0f);*/
        initialDirection = direction;
        if (!isForceInit)
        {
            rb.linearVelocity = initialDirection.normalized * bulletSpeed;
        }
        else
        {
            rb.linearVelocity = initialDirection * bulletSpeed;
        }

        //Debug.Log("Projectile Init" + " x:" + rb.velocity.x + " y:" + rb.velocity.y);
        isFlying = true;
        InitializeEffects();
    }
    
    public void Prepare(int team, int atk, Stick2WeaponSpriteCtrl weaponCtrl)
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
        
        
        // �����ݒ�i�ړ��ƃq�b�g�{�b�N�X�𖳌����j
        rb.isKinematic = true;
        col.enabled = false;
        hitboxCtrl.enabled = false;
        time += Random.Range(0.01f, 4.99f);
        // �G�t�F�N�g�̏������i�K�v�ɉ����āj
        
        
        InitializeEffects();
    }

    public void Activate(Vector3 direction)
    {
        // �ړ��ƃq�b�g�{�b�N�X��L����
        rb.isKinematic = false;
        col.enabled = true;
        hitboxCtrl.enabled = true;

        // ���˕�����ݒ肵�Ĉړ��J�n
        initialDirection = direction;
        rb.linearVelocity = initialDirection.normalized * bulletSpeed;

        isFlying = true;
    }

    public void RotationSet(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle - (90f * angleRate));
    }
    

    void FixedUpdate()
    {
        if (isFlying)
        {
            if (!isParabola)
            {
                if(isSignMove)
                {
                    SignMoveUpdate();
                }
                else
                {
                    // �܂�������ԏꍇ�̑��x����
                    rb.linearVelocity = new Vector2(
                        Mathf.Lerp(rb.linearVelocity.x, initialDirection.normalized.x * bulletSpeed, 0.8f),
                        Mathf.Lerp(rb.linearVelocity.y, initialDirection.normalized.y * bulletSpeed, 0.8f)
                    );
                }

            }
            // `isParabola`��`true`�̏ꍇ�A�d�͂ɏ]��

            // �X�v���C�g�̌����𑬓x�ɍ��킹�Ē���
            if (isSpriteDirectionAdjust)
            {
                Vector2 velocity = rb.linearVelocity;
                if (velocity != Vector2.zero)
                {
                    float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                    projectileSprite.transform.rotation = Quaternion.Euler(0f, 0f, angle - (90f * angleRate));
                }
            }
        }
    }

    public void Stuck(GameObject obj, bool isParentChange)
    {

        col.enabled = false;
        if(trailRenderer != null)
            trailRenderer.emitting = false;
        Invoke("SpriteFadeOut", fadeOutTime);
        isFlying = false;

        if(effectPrefab != null) {
        
            if(obj.gameObject.tag != "Walls")
            {
                GameObject effect = Instantiate(effectPrefab);
                Debug.Log("HitEffect fireSummoned");
                effect.transform.position = rb.transform.position;
            }

        }

        if (!isStuckable)
        {
            rb.linearVelocity *= decreaseRate;
            return;
        }


        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;

        if(spin != null)
        {
            spin.enabled = false;
        }




        if (isParentChange)
        {
            spriteParent.transform.parent = obj.transform;
            HitNoticeSender hit = obj.GetComponent<HitNoticeSender>();
            if (hit != null)
            {
                hit.StuckNotice(this);
            }
        }


    }

    public void DisableProjectile()
    {
        SpriteFadeOut();
    }

    public void SpriteFadeOut()
    {
        if (trailRenderer != null)
            trailRenderer.emitting = false;
        col.enabled = false;
        projectileSprite.DOFade(0.0f, fadeOutTweenTime);
        if(projectileSprite2 != null)
        {
            projectileSprite2.DOFade(0.0f, fadeOutTweenTime);
        }
        Invoke("LateDestroy", 1.1f);
        if (spin != null)
        {
            spin.enabled = false;
        }
        
        DissappearEffects();
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

    private void InitializeEffects()
    {
        if (effectInitDissappearObj != null)
        {
            effectInitDissappear = effectInitDissappearObj.GetComponent<IStickEffectInitDissappear>();
        }
        
        
        if(effectInitDissappear != null)
        {
            effectInitDissappear.Init();
        }
    }
    
    private void DissappearEffects()
    {
        if(effectInitDissappear != null)
        {
            effectInitDissappear.Dissapear();
        }
    }

    private void SignMoveUpdate()
    {
        // ���Ԃ��X�V
        time += Time.deltaTime;

        Vector2 defaultMoveDirection = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, initialDirection.x * bulletSpeed, 0.8f),
            Mathf.Lerp(rb.linearVelocity.y, initialDirection.y * bulletSpeed, 0.8f));
        
        // ��{�̒����^�������ɑ΂��Đ����ȕ������쐬
        Vector2 perpendicularDirection = new Vector2(0f, -1.0f);

        // �T�C���g�𗘗p���āA�h����v�Z
        Vector2 waveOffset = perpendicularDirection * Mathf.Sin(time * waveFrequency) * waveAmplitude;

        // �h����������ړ�����
        Vector2 finalDirection = defaultMoveDirection + waveOffset;

        // Rigidbody2D �ɑ��x��K�p
        rb.linearVelocity = finalDirection;
    }
}
