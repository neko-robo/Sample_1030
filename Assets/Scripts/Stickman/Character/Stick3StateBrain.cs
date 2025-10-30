using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

public class Stick3StateBrain : MonoBehaviour
{
    [Header("�Q��")] 
    private Stick3Stats stats;
    public Stick3ActionHandler _actionHandler;
    private StickEnemyDetector detector;
    public Animator animator;
    public Animator subAnimator;
    public Stick3Ctrl stick3Ctrl;

    [Header("�ݒ�l")]
    public BodyState currentBodyState = BodyState.Idle;
    public LegState currentLegState = LegState.Idle;

    [Header("�X�L���Ǘ�")]
    public Stick3SkillList skillList;
    private Stick3Skill currentSkill;

    // ���s�ҋ@�֘A
    public float walkWaitDuration = 1.0f;
    private float walkWaitTimer = 0.0f;

    [Header("�t���O�Ǘ�")]
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
        // �퓬���̏����̓A�j���[�V�����C�x���g�ŊǗ�
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

            // �^�[�Q�b�g�Ƃ̋������v�Z
            float distanceToTarget = Vector3.Distance(detector.targetTransform.position, transform.position);
            float distanceToSkillRange = Vector3.Distance(skill.rangeJudgementPoint.position, transform.position);

            float minDistance = -10;
            float maxDistance = 1000;

            // �X�L���̃^�C�v�ɉ����ċ�������
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

            // �X�L���̃^�C�v�ɉ����ċ�������
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
                    // �퓬�J�n
                    currentBodyState = BodyState.Fighting;
                    isFightInitialized = true;
                    isFightReset = false;
                    walkWaitTimer = 0f;

                    // �A�j���[�V�����p�����[�^�̐ݒ�
                    SetBool(skill.animeName, true);

                    // ���s�̐���
                    if (skill.walkType == WalkType.walkStop)
                    {
                        WalkStop();
                    }
                    // else if (skill.walkType == WalkType.keepWalk)
                    // {
                    //     currentLegState = LegState.Walking;
                    // }

                    // ���݂̃X�L�����L�^
                    currentSkill = skill;

                    return true;
                }
            }
        }

        // �ǂ̃X�L�����g�p�ł��Ȃ��ꍇ
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

    // �퓬�̃��Z�b�g
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

    // ���s�J�n
    public void WalkStart()
    {
        currentLegState = LegState.Walking;
        walkWaitTimer = 0f;

        // �A�j���[�V�����p�����[�^�̍X�V
        SetBool("IsWalking", true);

        // ���s�ĊJ�t���[�Y������
        isWalkRestartFreeze = false;
    }

    // ���s��~
    public void WalkStop()
    {
        if (stopIgnore)
            return;

        currentLegState = LegState.Idle;

        // �A�j���[�V�����p�����[�^�̍X�V
        SetBool("IsWalking", false);

        // ���s�ĊJ�t���[�Y��ݒ�
        isWalkRestartFreeze = true;
    }

    // ���S����
    public void Dead()
    {
        currentBodyState = BodyState.Idle;
        currentLegState = LegState.Idle;
        animator.enabled = false;
        if (subAnimator != null)
            subAnimator.enabled = false;
        // ���̑��̎��S����
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

    // �A�j���[�V�����C�x���g����Ă΂�郁�\�b�h
    public void OnBodyIdleEnter()
    {
        // BodyLayer��Idle��Ԃɓ������Ƃ��̏���
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

        // �퓬�A�j���[�V�������I�������Ƃ��̏���
        FightReset();
        isWalkRestartFreeze = false;
    }

    // �X�L���̃N�[���_�E�������Z�b�g�i�A�j���[�V�����C�x���g����Ăяo���j
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

    // �㔼�g�Ɖ����g�̏�Ԃ��Ǘ�����񋓌^
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
