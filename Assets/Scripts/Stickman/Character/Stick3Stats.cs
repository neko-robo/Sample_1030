using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick3Stats : MonoBehaviour
{
    public int team;
    public int typeId;
    public float HP = 60.0f;
    public int atk = 20;
    public float baseMoveSpeed = 1.0f;
    public bool isDamageIgnore = false;
    public bool isEvading = false;
    public float barrierValue = 0.0f;
    public float recieveDamageRate = 1.0f;
    public string mobName = "";
    public bool isBoss = false;

    [HideInInspector]
    public float slowRate = 1.0f;
    

    [HideInInspector]
    public bool isAlive = true;
    [HideInInspector]
    public float animeMoveSpeed = 0.0f;

    public void Initialize(int team)
    {
        this.team = team;
        isAlive = true;
    }

    public bool DamageHPDecrease(float damage)
    {
        if(barrierValue > 0.0f)
        {
            if (barrierValue >= damage)
            {
                barrierValue -= damage;
                return false;
            }
            else
            {
                damage -= barrierValue;
                barrierValue = 0.0f;
            }
        }

        if(recieveDamageRate < 1.0f)
        {
            HP -= 1.00f * damage * recieveDamageRate;
        }
        else
        {
            HP -= damage;
        }

        if (HP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
