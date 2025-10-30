using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick3SkillConditionJudge
{
    public Stick3Ctrl stick3Ctrl;
    public Stick3SkillConditionJudge(Stick3Ctrl _stick3Ctrl)
    {
        stick3Ctrl = _stick3Ctrl;
    }

    public bool CheckSkillCondition(Stick3Skill skill)
    {
        bool result = true;

        switch (skill.conditionType)
        {
            case ConditionType.none:
                return true;
            case ConditionType.ritual:
                result = JudgeForRitual();
                return result;
            case ConditionType.allyExist4:
                result = JudgeForAllyExist(4);
                return result;
            case ConditionType.lowHealth:
                result = JudgeForLowHealth(0.25f);
                return result;
            case ConditionType.lowHealth04:
                result = JudgeForLowHealth(0.4f);
                return result;
            case ConditionType.lowHealth06:
                result = JudgeForLowHealth(0.6f);
                return result;
            case ConditionType.flagA:
                result = JudgeForFlagA();
                return result;
            case ConditionType.inWalk:
                result = JudgeForInWalk();
                return result;
            default:
                return true;
                
        }

        return result;
    }

    private bool JudgeForRitual()
    {
        int count = 0;
        List<Stick3Ctrl> stick3Ctrls = stick3Ctrl.stickManGenerator.GetStickListByTeam(stick3Ctrl.stats.team);
        foreach (Stick3Ctrl ctrl in stick3Ctrls)
        {
            if (ctrl != null && ctrl.isDowned && ctrl.isUndead)
            {
                count++;
            }
        }

        if( count >= 4)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private bool JudgeForAllyExist(int targetCount)
    {
        int count = 0;
        List<Stick3Ctrl> stick3Ctrls = stick3Ctrl.stickManGenerator.GetStickListByTeam(stick3Ctrl.stats.team);
        foreach (Stick3Ctrl ctrl in stick3Ctrls)
        {
            if (ctrl != null && ctrl.stats.isAlive)
            {
                count++;
            }
        }

        if (count >= 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool JudgeForLowHealth(float borderRate)
    {
        float rate = 1.000f * stick3Ctrl.stats.HP / stick3Ctrl.defaultHP;
        if(rate < borderRate)
        {
            return true;
        }
        return false;
    }

    private bool JudgeForFlagA()
    {
        return stick3Ctrl.animeFlagA;
    }

    private bool JudgeForInWalk()
    {
        AnimatorClipInfo[] clips = stick3Ctrl.animator.GetCurrentAnimatorClipInfo(0);
        AnimatorStateInfo stateInfo = stick3Ctrl.animator.GetCurrentAnimatorStateInfo(0);
        if (clips.Length > 0)
        {
            string clipName = clips[0].clip.name;
            if(clipName == "Walk")
            {
                if (stateInfo.normalizedTime > 0.5f)
                return true;
            }

        }
        //return stick3Ctrl.animator.GetBool("IsWalking");
        return true;
    }
}

