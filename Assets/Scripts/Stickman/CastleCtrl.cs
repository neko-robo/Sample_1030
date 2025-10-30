using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleCtrl : MonoBehaviour, IDamageReceiver
{
    public int team = 0;
    public TMPro.TMP_Text hpText;
    public float HP = 60.0f;
    public GameObject damageTextPrefab;
    public GameObject castleSpriteObj;
    private bool isAlive = true;
    private float currentShakePower = 0f;
    private Tweener shakeTween;
    public GameObject explodePosObj;
    public GameObject explodePrefab;
    public bool isDying = false;
    private HashSet<int> damagedHash;
    private Vector3 spriteDefaultPos;
    public Collider2D[] deadDisableCols;

    public void Start()
    {
        damagedHash = new HashSet<int>();
        spriteDefaultPos = castleSpriteObj.transform.position;
    }

    public void Update()
    {
        currentShakePower -= Time.deltaTime * 100f;
    }

    public bool HashContainsCheck(int id)
    {
        if (damagedHash.Contains(id))
        {
            return true;
        }
        return false;
    }

    public bool DamageRecieve(int damage, int id, bool isKB)
    {
        if (!isAlive)
        {
            return false;
        }
        if (damagedHash.Contains(id))
        {
            return false;
        }

        damagedHash.Add(id);

        GameObject obj = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
        ctrl.Init(team, damage);

        HP -= damage;
        hpText.text = Mathf.FloorToInt(HP).ToString();

        if (HP <= 0)
        {
            hpText.text = "0";
            Dead();
        }
        else
        {
            CastleShake(damage);
        }
        
        return false;
    }

    public void CastleShake(int damage)
    {
        if(damage <= currentShakePower)
        {
            return;
        }

        currentShakePower = damage;
        float log = Mathf.Log(damage, 20);
        float duration = Mathf.Lerp(0.3f, 1.2f, Mathf.InverseLerp(1.0f, 1.85f, log));
        float shakeForce = Mathf.Lerp(0.25f, 2.0f, Mathf.InverseLerp(1.0f, 1.85f, log));
        int frequency = 50;
        if (shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
            castleSpriteObj.transform.position = spriteDefaultPos;
        }
        shakeTween = castleSpriteObj.transform.DOShakePosition(duration, shakeForce, frequency, 1, false, true);
    }


    public void Dead()
    {
        isDying = true; 
        isAlive = false;

        GameObject obj = Instantiate(explodePrefab);
        obj.transform.position = explodePosObj.transform.position;
        currentShakePower = 100000f;
        shakeTween = castleSpriteObj.transform.DOShakePosition(5.0f, 2.0f, 50, 1, false, false);
        Invoke("CastleFade", 4.0f);
        Invoke("CastleColDisable", 3.5f);
    }

    public void CastleFade()
    {
        castleSpriteObj.SetActive(false);

    }

    public void CastleColDisable()
    {
        if (deadDisableCols != null && deadDisableCols.Length > 0)
        {
            for (int i = 0; i < deadDisableCols.Length; i++)
            {
                deadDisableCols[i].enabled = false;
            }
        }
    }
}
