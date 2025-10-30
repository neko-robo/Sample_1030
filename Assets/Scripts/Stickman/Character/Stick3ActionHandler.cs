using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using MessagePipe;

public class Stick3ActionHandler : MonoBehaviour
{
    [Header("アニメーションから設定する歩行移動速度")]
    public float MoveRate  = 0.0f;
    [Header("歩行以外のアニメーションから設定する移動速度")]
    public float ActionMoveRate  = 0.0f;
    [Header("全体移動速度補正")]
    public float MasterMoveRate = 1.0f;
    private Stick3Stats stats;
    private StickEnemyDetector detector;
    private Stick3StateBrain brain;

    [Header("武器/飛び道具参照")]
    public DamageHitboxCtrl[] hitboxCtrls;
    public Stick2WeaponSpriteCtrl[] weaponSpriteCtrls;

    [Header("特定の機能,index指定してFunctionActivate呼び出し")]
    public GameObject[] Stick3FunctionObjects;
    private List<IStick3Function> Stick3Functions;
    
    [Header("メソッド呼び出し制限")]
    public bool isSingleCallPerFrame = false;
    
    [Range(1, 10)]
    public int allowedFrameRange = 2;
    
    private int lastCallFrame = -9999;


    [Inject]
    public void Inject(Stick3Stats stats, StickEnemyDetector detector, Stick3StateBrain brain)
    {
        this.stats = stats;
        this.detector = detector;
        this.brain = brain;
    }

    public void Initialize()
    {
        if(hitboxCtrls != null && hitboxCtrls.Length > 0)
        {
            foreach(DamageHitboxCtrl hitboxCtrl in hitboxCtrls)
            {
                hitboxCtrl.InitByTeam(stats.atk, stats.team);
            }
        }

        if (weaponSpriteCtrls != null && weaponSpriteCtrls.Length >= 1)
        {
            foreach (Stick2WeaponSpriteCtrl ctrl in weaponSpriteCtrls)
            {
                ctrl.Init(stats.team, stats.atk, false);
            }
        }

        if (Stick3FunctionObjects != null && Stick3FunctionObjects.Length >= 1)
        {
            Stick3Functions = new List<IStick3Function>();
            for(int i = 0; i < Stick3FunctionObjects.Length; i++)
            {
                IStick3Function function = Stick3FunctionObjects[i].GetComponent<IStick3Function>();
                if (function != null)
                {
                    Stick3Functions.Add(function);
                }
            }
        }

    }

    public void Update()
    {
        if (ActionMoveRate > 0)
        {
            stats.animeMoveSpeed = ActionMoveRate * MasterMoveRate;
        }
        else 
        {
            if (MoveRate > 0)
            {
                stats.animeMoveSpeed = MoveRate * MasterMoveRate;
            }
            else
            {
                stats.animeMoveSpeed = 0;
            }
        }

    }

    public void Dead()
    {
        if (hitboxCtrls != null && hitboxCtrls.Length > 0)
        {
            foreach (DamageHitboxCtrl hitboxCtrl in hitboxCtrls)
            {
                hitboxCtrl.enabled = false;
            }
        }
        
        FunctionDisableOnDead();
    }

    public void Down()
    {
        if (hitboxCtrls != null && hitboxCtrls.Length > 0)
        {
            foreach (DamageHitboxCtrl hitboxCtrl in hitboxCtrls)
            {
                hitboxCtrl.enabled = false;
            }
        }


    }



  
    public void WeaponDisplayIndex(int index)
    {
        if (!CanExecute()) return;

        if (weaponSpriteCtrls[index] != null)
        {
            weaponSpriteCtrls[index].SpriteDisplay();
        }
    }

    public void WeaponHideIndex(int index)
    {
        if (!CanExecute()) return;

        if (weaponSpriteCtrls[index] != null)
        {
            weaponSpriteCtrls[index].SpriteHide();
        }
    }

    public void ProjectileShootIndex(int index)
    {
        if (!CanExecute()) return;

        if (weaponSpriteCtrls[index] != null)
        {
            weaponSpriteCtrls[index].ProjectileShoot(detector.detectDistance);
        }
    }

    public void OnFightAnimationEnd()
    {
        // このメソッドは制限対象外
        brain.OnFightAnimationEnd();
    }

    public void RangedCTSet()
    {
        // このメソッドは制限対象外
        brain.ResetSkillCooldown();
    }

    public void ClosedCTSet()
    {
        // このメソッドは制限対象外
        brain.ResetSkillCooldown();
    }

    public void CTSet()
    {
        brain.ResetSkillCooldown();
    }

    public void FunctionActivate(int index)
    {
        if (!CanExecute()) return;

        if (Stick3Functions != null && Stick3Functions.Count > 0)
        {
            Stick3Functions[index].Activate();
        }
    }

    public void FunctionEvcentCall(int index)
    {
        if (!CanExecute()) return;

        if (Stick3Functions != null && Stick3Functions.Count > 0)
        {
            Stick3Functions[index].EventCall();
        }
    }

    public void FunctionDisable(int index)
    {
        if (!CanExecute()) return;

        if (Stick3Functions != null && Stick3Functions.Count > 0)
        {
            Stick3Functions[index].Disable();
        }
    }

    public void FunctionDisableOnDead()
    {
        if (Stick3Functions != null && Stick3Functions.Count > 0)
        {
            foreach(IStick3Function function in Stick3Functions)
            {
                if (function != null)
                {
                    function.Disable();
                }
            }
        }
    }

    public void SetDefaultPoseValue()
    {
        brain.SetFloat("DefaultPoseValue", 1);
    }


    
    /// <summary>
    /// メソッドの実行が許可されているかどうかを判定するヘルパーメソッド
    /// </summary>
    /// <returns>実行が許可されていれば true、そうでなければ false</returns>
    private bool CanExecute()
    {
        if (!isSingleCallPerFrame)
            return true;

        int currentFrame = Time.frameCount;

        if (currentFrame - lastCallFrame < allowedFrameRange)
        {
            // 呼び出しが制限範囲内の場合は実行をスキップ
            return false;
        }

        // 実行を許可し、最後に呼び出されたフレームを更新
        lastCallFrame = currentFrame;
        return true;
    }



}