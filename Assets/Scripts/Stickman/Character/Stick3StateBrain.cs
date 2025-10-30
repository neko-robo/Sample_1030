using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

public class Stick3StateBrain : MonoBehaviour
{
    [Header("参照")] 
    private Stick3Stats stats;
    public Stick3ActionHandler _actionHandler;
    private StickEnemyDetector detector;
    public Animator animator;
    public Animator subAnimator;
    public Stick3Ctrl stick3Ctrl;

    [Header("設定値")]
    public BodyState currentBodyState = BodyState.Idle;
    public LegState currentLegState = LegState.Idle;

    [Header("スキル管理")]
    public Stick3SkillList skillList;
    private Stick3Skill currentSkill;

    // 歩行待機関連
    public float walkWaitDuration = 1.0f;
    private float walkWaitTimer = 0.0f;

    [Header("フラグ管理")]
    public bool isFightKeepTimeSet = false;
    private float fightKeepTime = 0.0f;
    public bool isMoveRateFloatUpdate = false;
    private bool isFightInitialized = false;
    private bool isFightReset = false;
    public bool isWalkRestartFreeze = false;
    private bool stopIgnore = false;
    public bool isDefaultPoseValueManage = false;
    public float speedMultiplier = 1.0f;
    public bool isSpeedRateChange = false;
    private Stick3SkillConditionJudge conditionJudge;

    public bool isResetDebug = false;
    public float lockTime = 0.0f;
    
    
    [Inject]
    public void Inject(Stick3Stats _stats, StickEnemyDetector enemyDetector)
    {
        this.stats = _stats;
        this.detector = enemyDetector;
    }

    public void Initialize()
    {
        currentBodyState = BodyState.Idle;
        currentLegState = LegState.Idle;
        isFightInitialized = false;
        isFightReset = false;
        walkWaitTimer = walkWaitDuration - 0.1f;
        conditionJudge = new Stick3SkillConditionJudge(stick3Ctrl);
    }

    private void Update()
    {
        if (!stats.isAlive)
        {
            return;
        }

        if(lockTime > 0.0f)
        {
            lockTime -= Time.deltaTime;
            return;
        }
        UpdateCooldowns();

        BodyUpdate();
        LegUpdate();
        AnimeFloatUpdate();
    }

    private void BodyUpdate()
    {
        fightKeepTime += Time.deltaTime;
        if (isFightKeepTimeSet)
        {
            animator.SetFloat("FightKeepTime", fightKeepTime);
        }

        switch (currentBodyState)
        {
            case BodyState.Idle:
                HandleBodyIdleState();
                break;
            case BodyState.Fighting:
                HandleBodyFightingState();
                break;
        }
    }

    private void LegUpdate()
    {
        switch (currentLegState)
        {
            case LegState.Idle:
                HandleLegIdleState();
                break;
            case LegState.Walking:
                HandleLegWalkingState();
                break;
        }
    }

    private void AnimeFloatUpdate()
    {
        if (isMoveRateFloatUpdate)
            SetFloat("MoveRateValue", _actionHandler.MoveRate);

        if (isDefaultPoseValueManage)
        {
            float value = animator.GetFloat("DefaultPoseValue");
            value -= Time.deltaTime;
            SetFloat("DefaultPoseValue", value);
        }
        if (isSpeedRateChange)
        {
            SetFloat("SpeedRate", speedMultiplier);
            isSpeedRateChange = false;
        }
    }

    private void HandleBodyIdleState()
    {
        detector.isDetecting = true;

        if (detector.enemyDetected && !isFightInitialized && !isWalkRestartFreeze)
        {
            if (FightInit())
            {
                detector.DetectEnd();
            }
        }
    }

    private void HandleBodyFightingState()
    {
        // 戦闘中の処理はアニメーションイベントで管理
    }

    private void HandleLegIdleState()
    {
        if (!isWalkRestartFreeze)
        {
            walkWaitTimer += Time.deltaTime;
            if (walkWaitTimer >= walkWaitDuration)
            {
                WalkStart();
            }
        }
    }

    private void HandleLegWalkingState()
    {
        SetBool("IsWalking", true);
    }

    private bool FightInit()
    {
        if (detector.targetTransform == null)
        {
            return false;
        }

        foreach (var skill in skillList.skills)
        {
            bool canUseSkill = false;

            // ターゲットとの距離を計算
            float distanceToTarget = Vector3.Distance(detector.targetTransform.position, transform.position);
            float distanceToSkillRange = Vector3.Distance(skill.rangeJudgementPoint.position, transform.position);

            float minDistance = -10;
            float maxDistance = 1000;

            // スキルのタイプに応じて距離判定
            if (skill.skillType == SkillType.Close)
            {
                maxDistance = distanceToSkillRange;
                if(skill.distanceArrowRange >= 0.1f)
                {
                    minDistance = distanceToSkillRange - skill.distanceArrowRange;
                }
            }
            else if (skill.skillType == SkillType.Ranged)
            {
                minDistance = distanceToSkillRange;
                if(skill.distanceArrowRange >= 0.1f)
                {
                    maxDistance = distanceToSkillRange + skill.distanceArrowRange;
                }
            }

            if(minDistance <= distanceToTarget && maxDistance >= distanceToTarget)
            {
                canUseSkill = true;
            }

            // スキルのタイプに応じて距離判定
            if (skill.skillType == SkillType.Close)
            {
                if (distanceToTarget <= distanceToSkillRange)
                {
                    canUseSkill = true;
                }
            }
            else if (skill.skillType == SkillType.Ranged)
            {
                if (distanceToTarget > distanceToSkillRange)
                {
                    canUseSkill = true;
                }
            }

            if(!conditionJudge.CheckSkillCondition(skill))
            {
                canUseSkill = false;
            }

            if (canUseSkill)
            {
                if (skill.currentCoolTime <= 0 )
                {
                    // 戦闘開始
                    currentBodyState = BodyState.Fighting;
                    isFightInitialized = true;
                    isFightReset = false;
                    walkWaitTimer = 0f;

                    // アニメーションパラメータの設定
                    SetBool(skill.animeName, true);

                    // 歩行の制御
                    if (skill.walkType == WalkType.walkStop)
                    {
                        WalkStop();
                    }
                    // else if (skill.walkType == WalkType.keepWalk)
                    // {
                    //     currentLegState = LegState.Walking;
                    // }

                    // 現在のスキルを記録
                    currentSkill = skill;

                    return true;
                }
            }
        }

        // どのスキルも使用できない場合
        return false;
    }

    public void FightInitForce(Stick3Skill skill, string animeStateName)
    {
        currentBodyState = BodyState.Fighting;
        isFightInitialized = true;
        isFightReset = false;
        walkWaitTimer = 0f;
        SetBool(skill.animeName, true);
        animator.Play(animeStateName);
        if (skill.walkType == WalkType.walkStop)
        {
            WalkStop();
        }
        currentSkill = skill;
        detector.DetectEnd();
    }

    // 戦闘のリセット
    public void FightReset()
    {
        isFightReset = true;
        isFightInitialized = false;
        currentBodyState = BodyState.Idle;
        detector.DetectStart();

        if (currentSkill != null)
        {
            SetBool(currentSkill.animeName, false);
            currentSkill = null;
        }
    }

    // 歩行開始
    public void WalkStart()
    {
        currentLegState = LegState.Walking;
        walkWaitTimer = 0f;

        // アニメーションパラメータの更新
        SetBool("IsWalking", true);

        // 歩行再開フリーズを解除
        isWalkRestartFreeze = false;
    }

    // 歩行停止
    public void WalkStop()
    {
        if (stopIgnore)
            return;

        currentLegState = LegState.Idle;

        // アニメーションパラメータの更新
        SetBool("IsWalking", false);

        // 歩行再開フリーズを設定
        isWalkRestartFreeze = true;
    }

    // 死亡処理
    public void Dead()
    {
        currentBodyState = BodyState.Idle;
        currentLegState = LegState.Idle;
        animator.enabled = false;
        if (subAnimator != null)
            subAnimator.enabled = false;
        // その他の死亡処理
    }

    public void Down()
    {
        currentBodyState = BodyState.Idle;
        currentLegState = LegState.Idle;
        float rnd = Random.Range(0.001f, 0.999f);
        
        animator.SetFloat("Blend", rnd);
        detector.DetectEnd();
        animator.SetBool("Down", true);
        //animator.Play("Down");
    }
    
    public void ReviveAnimePlay()
    {
        animator.SetBool("Down", false);
        animator.Play("Revive");

    }

    // アニメーションイベントから呼ばれるメソッド
    public void OnBodyIdleEnter()
    {
        // BodyLayerがIdle状態に入ったときの処理
        isFightInitialized = false;
        isFightReset = false;
        currentBodyState = BodyState.Idle;
        detector.DetectStart();
    }

    public void OnFightAnimationEnd()
    {
        //if (isResetDebug)
        //{
        //    Debug.Log("fightEnd" + this.gameObject.name + currentSkill.animeName);
        //}

        // 戦闘アニメーションが終了したときの処理
        FightReset();
        isWalkRestartFreeze = false;
    }

    // スキルのクールダウンをリセット（アニメーションイベントから呼び出し）
    public void ResetSkillCooldown()
    {
        if (currentSkill != null)
        {
            currentSkill.currentCoolTime = currentSkill.coolTime;
        }
    }

    private void SetBool(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
        if (subAnimator != null)
            subAnimator.SetBool(parameter, value);
    }

    public void SetFloat(string parameter, float value)
    {
        animator.SetFloat(parameter, value);
        if (subAnimator != null)
            subAnimator.SetFloat(parameter, value);
    }
    
    private void UpdateCooldowns()
    {
        foreach (var skill in skillList.skills)
        {
            if (skill.currentCoolTime > 0)
            {
                skill.currentCoolTime -= Time.deltaTime;
            }
        }
    }

    // 上半身と下半身の状態を管理する列挙型
    public enum BodyState
    {
        Idle,
        Fighting
    }

    public enum LegState
    {
        Idle,
        Walking
    }
}
