using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitNoticeSender : MonoBehaviour
{
    public Stick3Ctrl stick3Ctrl;
    public CastleCtrl castleCtrl;
    public bool isCounter = false;
    public bool isBlock = false;
    public int blockLimit = 0;
    private int currentBlockValue = 0;

    public bool HitNotice(int damage, int id, Stick3Ctrl attackerStickCtrl, bool isKB)
    {
        bool result = false;
        if (stick3Ctrl != null)
        {
            bool check = stick3Ctrl.HashContainsCheck(id);
            if (!check)
            {
                stick3Ctrl.DamageTraceNotice(attackerStickCtrl.stats.team, attackerStickCtrl.stats.typeId, damage);
            }

            if (isBlock)
            {
                if (BlockCheck())
                {
                    bool blocked = stick3Ctrl.Block(id);
                    if (blocked)
                    {
                        currentBlockValue += damage;

                    }

                    return false;
                }

            }
            else if(isCounter)
            {
                if(attackerStickCtrl == null)
                {
                    stick3Ctrl.GuardTextGenerate(id);
                }
                else
                {
                    stick3Ctrl.Counter(id, attackerStickCtrl);
                }
            }
            else
            {
                result = stick3Ctrl.DamageRecieve(damage, id, isKB);

                if (isKB)
                {
                    //stick3Ctrl.ForceRecieveByDamage(damage);
                }
            }


        }
        
        if(castleCtrl != null)
        {
            castleCtrl.DamageRecieve(damage, id, false);
        }

        return result;
    }

    public bool HitNotice(int damage, int id, bool isKB, int team, int typeID)
    {
        bool result = false;
        if (stick3Ctrl != null)
        {
            bool check = stick3Ctrl.HashContainsCheck(id);
            if (!check)
            {
                stick3Ctrl.DamageTraceNotice(team, typeID, damage);
            }

            if (isBlock)
            {
                if (BlockCheck())
                {
                    bool blocked = stick3Ctrl.Block(id);
                    if (blocked)
                    {
                        currentBlockValue += damage;

                    }

                    return false;
                }

            }
            else
            {
                result = stick3Ctrl.DamageRecieve(damage, id, isKB);

                if (isKB)
                {
                    //stick3Ctrl.ForceRecieveByDamage(damage);
                    
                }
            }


        }

        if (castleCtrl != null)
        {
            castleCtrl.DamageRecieve(damage, id, false);
        }

        return result;
    }

    public bool HitNoticeWithEffect(int damage, int id, GameObject prefab, int effectID, bool isKB, int team, int typeID)
    {
        bool result = false;
        if (stick3Ctrl != null)
        {
            result = stick3Ctrl.HashContainsCheck(id);
            if (!result)
            {
                stick3Ctrl.DamageTraceNotice(team, typeID, damage);
            }

            stick3Ctrl.EffectRecieve(damage, id, prefab, effectID);

            //‘½•ª‚±‚ê‚¢‚ç‚È‚¢Bª‚ÅDamageRecieve‚æ‚Î‚ê‚Ä‚é
            //stick3Ctrl.DamageRecieve(damage, id);
        }

        if (castleCtrl != null)
        {
            castleCtrl.DamageRecieve(damage, id, false);
            result = castleCtrl.HashContainsCheck(id);
        }

        return result;
    }

    public void StuckNotice(Stick2ProjectileCtrl stick2ProjectileCtrl)
    {
        if(stick3Ctrl != null)
        {
            stick3Ctrl.ProjectileStuck(stick2ProjectileCtrl);
        }
    }

    public void KBNotice(float force, int id)
    {
        if (stick3Ctrl != null)
        {
            stick3Ctrl.ForceRecieve(force, id);
        }
    }

    public bool BlockCheck()
    {
        bool result = true;
        
        if(blockLimit > 0 && currentBlockValue >= blockLimit)
        {
            isBlock = false;
            stick3Ctrl.ShieldDrop();
        }

        return result;
    }
}
