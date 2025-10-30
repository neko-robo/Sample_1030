using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stick3Skill
{
    public SkillType skillType; // RangedまたはCloseを選択
    public float coolTime;      // CoolTimeを指定
    public float currentCoolTime;   // 現在のCoolTimeを指定
    public string animeName;    // animeNameを指定
    public Transform rangeJudgementPoint; // rangeJudgementPointを指定,Closeの場合はこの位置より攻撃対象が近くないと攻撃しない,Rangedの場合はこの位置より遠くないと攻撃しない
    public float distanceArrowRange; //Close/Range共に、RangeJudgementPointから許容範囲を指定したい場合値を入れる　値0なら許容範囲無限
    public WalkType walkType; //攻撃時に歩行を継続するかどうかを指定
    public ConditionType conditionType;
}

public enum SkillType
{
    Ranged,
    Close
}

public enum WalkType
{
    walkStop,
    keepWalk
}

public enum ConditionType
{
    none,
    ritual,
    allyExist4,
    lowHealth,
    lowHealth04,
    lowHealth06,
    flagA,
    inWalk
}