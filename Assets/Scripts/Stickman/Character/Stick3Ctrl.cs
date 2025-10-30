using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using Unity.VisualScripting;
public class Stick3Ctrl : MonoBehaviour, IDamageReceiver
{

    private Stick3ViewCtrl viewCtrl;
    public Stick3Stats stats;
    private Stick3StateBrain brain;
    private Stick3ActionHandler actionHandler;
    private Stick3MoveCtrl move;
    private StickEnemyDetector detector;
    public StickmanGenerator stickManGenerator;

    [Header("参照")]
    public Animator animator;
    public GameObject topObjForDeleteBorder;
    public GameObject groundHeightObj;
    public GameObject damageTextPrefab;
    private GameObject blockTextPrefab;
    public GameObject healTextPrefab;


    [Header("設定値")]
    private GameObject destroyBorderObj;
    public List<Stick2ProjectileCtrl> projectileList;
    private HashSet<int> damagedHash;
    private bool[] effectRecieved;

    public GameObject reflectEffectPrefab;
    public GameObject reflectPosObj;

    public bool isUndead = false;
    public bool isDowned = false;
    public float defaultHP = 100f;
    private float reviveGrace = 25.0f;
    private float reviveTimer = 0.0f;

    private bool isReviveCanceled = false;
    public bool isDeadIgnore = false;

    [HideInInspector]
    public bool animeFlagA = true;
    public float totalFreezeAmount = 0f;

    public float dealtDamageCount = 0;
    private IPublisher<DamageTraceData> damageTracePublisher;

    [Inject]
    public void Inject(StickmanGenerator _stickmanGenerator, Stick3ViewCtrl viewCtrl, Stick3Stats stats, Stick3StateBrain brain, Stick3ActionHandler actionHandler, Stick3MoveCtrl move, StickEnemyDetector detector, IPublisher<DamageTraceData> _damageTracePublisher)
    {
        this.stickManGenerator = _stickmanGenerator;
        this.viewCtrl = viewCtrl;
        this.stats = stats;
        this.brain = brain;
        this.actionHandler = actionHandler;
        this.move = move;
        this.detector = detector;
        this.damageTracePublisher = _damageTracePublisher;
    }
    
    
    public void Init(int team, GameObject destroyBorderObj)
    {
        this.destroyBorderObj = destroyBorderObj;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + (transform.position.y - groundHeightObj.transform.position.y), transform.position.z);
        transform.position = pos;

        stats.Initialize(team);
        viewCtrl.Initialize();
        actionHandler.Initialize();
        move.Initialize();
        detector.Init(team);
        brain.Initialize();
        projectileList = new List<Stick2ProjectileCtrl>();
        damagedHash = new HashSet<int>();
        effectRecieved = new bool[5];
        defaultHP = stats.HP;
    }

    public void Update()
    {

        
        if (isDowned)
        {
            ReviveTimeUpdate();
        }
        if (!stats.isAlive)
        {
            DestroyCheck();
            return;
        }
    }

    public bool DamageRecieveWithText(int damage, int id, GameObject prefab)
    {
        if (stats.isDamageIgnore)
        {
            return false;
        }

        if (damagedHash.Contains(id))
        {
            return false;
        }

        damagedHash.Add(id);

        GameObject obj = Instantiate(prefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
        DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
        ctrl.Init(stats.team, damage);

        bool isDead = stats.DamageHPDecrease(damage);

        if (isDead)
        {
            Dead();
        }
        return isDead;
    }

    public bool DamageRecieve(int damage, int id, bool isKB)
    {
        if (stats.isDamageIgnore)
        {
            return false;
        }

        if (damagedHash.Contains(id))
        {
            return false;
        }

        damagedHash.Add(id);

        if (stats.isEvading)
        {
            if (blockTextPrefab != null)
            {
                GameObject obj2 = Instantiate(damageTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
                DamageTextCtrl ctrl2 = obj2.GetComponent<DamageTextCtrl>();
                ctrl2.InitWithText(stats.team, 50, "EVADE");
            }
            return false;
        }

        GameObject obj = Instantiate(damageTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
        DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
        ctrl.Init(stats.team, damage);

        bool isDead = stats.DamageHPDecrease(damage);

        if (isDead)
        {
            Dead();
        }else if (isKB)
        {
            float force = Mathf.Lerp(3.0f, 7.0f, Mathf.InverseLerp(10.0f, 150.0f, damage));
            ForceRecieveWithoutHash(force);
        }
        return isDead;
    }

    public void GuardTextGenerate(int id)
    {
        if (damagedHash.Contains(id))
        {
            return;
        }

        damagedHash.Add(id);
        if (blockTextPrefab != null)
        {
            GameObject obj = Instantiate(blockTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
            DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
            ctrl.InitWithText(stats.team, 50, "BLOCK");
        }
        if (reflectEffectPrefab != null)
        {
            GameObject obj = Instantiate(reflectEffectPrefab);

            Vector3 pos = new Vector3(reflectPosObj.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f), reflectPosObj.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
            obj.transform.position = pos;
        }
    }

    public void HealRecieve(int amount)
    {
        stats.HP += amount;
        if(stats.HP > defaultHP)
        {
            stats.HP = defaultHP;
        }

        GameObject obj = Instantiate(healTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
        DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
        ctrl.Init(stats.team, amount);
    }

    public bool Block(int id)
    {
        if (damagedHash.Contains(id))
        {
            return false;
        }

        damagedHash.Add(id);
        if (blockTextPrefab != null)
        {
            GameObject obj = Instantiate(blockTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
            DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
            ctrl.InitWithText(stats.team, 10, "BLOCK");
        }

        return true;
    }

    public void ShieldDrop()
    {
        viewCtrl.ShieldDrop();
    }
    public void Counter(int id, Stick3Ctrl attackerCtrl)
    {
        if (damagedHash.Contains(id))
        {
            return;
        }

        damagedHash.Add(id);

        if (blockTextPrefab != null)
        {
            GameObject obj = Instantiate(blockTextPrefab, viewCtrl.chestObj.transform.position, Quaternion.identity);
            DamageTextCtrl ctrl = obj.GetComponent<DamageTextCtrl>();
            ctrl.InitWithText(stats.team, 100, "COUNTER");
        }
        if (reflectEffectPrefab != null)
        {
            GameObject obj = Instantiate(reflectEffectPrefab);

            Vector3 pos = new Vector3(reflectPosObj.transform.position.x + UnityEngine.Random.Range(-0.2f, 0.2f), reflectPosObj.transform.position.y + UnityEngine.Random.Range(-0.2f, 0.2f), 0f);
            obj.transform.position = pos;
        }

        int newId = Random.Range(50000000, 10000000);
        attackerCtrl.DamageRecieve(400, newId, false);
        if (attackerCtrl.stats.isAlive == true)
        {
            attackerCtrl.move.DamageKnockBack(15);
        }
    }

    public bool HashContainsCheck(int id)
    {
        if (damagedHash.Contains(id))
        {
            return true;
        }
        return false;
    }

    public bool EffectRecieve(int damage, int id, GameObject prefab, int effectID)
    {
        if (damagedHash.Contains(id))
        {
            return false;
        }

        if(effectID == 3)
        {
            isUndead = false;
            isDeadIgnore = false;
        }

        bool result = DamageRecieve(damage, id, false);

        if (effectID == 0)
        {
            if (!result && !effectRecieved[0])
            {
                effectRecieved[0] = true;
                //イタチの炎　使わないからけしてる
                //GameObject obj = Instantiate(prefab);
                //obj.transform.position = viewCtrl.chestObj.transform.position;
                //obj.transform.parent = viewCtrl.chestObj.transform.parent;
                //Stick2BurningEffect burn = obj.GetComponent<Stick2BurningEffect>();
                //burn.Init(null, stats);
            }
        }else if(effectID == 1)
        {
            if(prefab != null)
            {
                //if(freezeEffect != null)
                //{
                //    if(freezeEffect.freezeForce > damage)
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        freezeEffect.EndFreeze(true);
                //        freezeEffect = null;
                //    }
                //}

                //GameObject obj = Instantiate(prefab);
                //freezeEffect = obj.GetComponent<Stick3FreezeEffect>();
                //if (freezeEffect != null)
                //{
                //    freezeEffect.Init(this, brain, move, viewCtrl, stats, damage);
                //}
            }
        }
        return result;
    }


    public void ForceRecieve(float recieveForce, int id)
    {
        if (damagedHash.Contains(id))
        {
            return;
        }
        damagedHash.Add(id);
        
        move.ForceRecieveFloat(recieveForce);
        if (projectileList.Count >= 1)
        {
            foreach (Stick2ProjectileCtrl projectileCtrl in projectileList)
            {
                if (projectileCtrl != null)
                {
                    projectileCtrl.LateDestroy();
                }
            }
        }
    }

    public void ForceRecieveWithoutHash(float recieveForce)
    {
        move.ForceRecieveFloat(recieveForce);
        if (projectileList.Count >= 1)
        {
            foreach (Stick2ProjectileCtrl projectileCtrl in projectileList)
            {
                if (projectileCtrl != null)
                {
                    projectileCtrl.LateDestroy();
                }
            }
        }
    }

    public void Dead()
    {
        if (!stats.isAlive)
        {
            return;
        }

        if (isDeadIgnore)
        {
            return;
        }
        
        if(isUndead)
        {
            Down();
            return;
        }
        
        stats.isAlive = false;
        move.rb.isKinematic = false;
        brain.Dead();
        viewCtrl.Dead();
        actionHandler.Dead();

        if (projectileList.Count >= 1)
        {
            foreach (Stick2ProjectileCtrl projectileCtrl in projectileList)
            {
                if (projectileCtrl != null)
                {
                    projectileCtrl.LateDestroy();
                }
            }
        }
    }

    public void Down()
    {
        isDowned = true;
        stats.isAlive = false;
        stats.HP = Mathf.FloorToInt(defaultHP * 0.8f);
        brain.Down();
        viewCtrl.Down();
        actionHandler.Down();
    }

    public void ReviveAnimePlay()
    {
        reviveTimer = 0.0f;
        brain.ReviveAnimePlay();
        isUndead = false;
    }
    
    public void ReviveFinishFromAnime()
    {
        if(isReviveCanceled) { return; }
        
        viewCtrl.Revive();
        isDowned = false;
        stats.isAlive = true;
    }

    public void DestroyCheck()
    {

        if (topObjForDeleteBorder.transform.position.y <= destroyBorderObj.transform.position.y)
        {
            Destroy(this.gameObject);
        }
    }

    public void ProjectileStuck(Stick2ProjectileCtrl ctrl)
    {
        projectileList.Add(ctrl);
    }

    public void LateDestroy()
    {
        Destroy(this.gameObject);
    }

    public void ReviveTimeUpdate()
    {
        reviveTimer += Time.deltaTime;
        if(reviveTimer >= reviveGrace)
        {
            isUndead = false;
            stats.isAlive = true;
            viewCtrl.deadRbForceRate = 0.2f;
            Dead();
        }
    }

    public void ReviveWaitBoneKill()
    {
        isUndead = false;
        stats.isAlive = true;
        viewCtrl.deadRbForceRate = 0.2f;
        isReviveCanceled = true;
        Dead();
    }

    public void SetSlowRate(float rate)
    {
        brain.speedMultiplier = rate;
        brain.isSpeedRateChange = true;
        move.speedMultiplier = rate;
    }

    public void DamageTraceNotice(int team, int typeID, float damage)
    {
        DamageTraceData data = new DamageTraceData(team, typeID, damage);
        damageTracePublisher.Publish(data);
    }
}
