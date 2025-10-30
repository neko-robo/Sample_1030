using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stick3Skill
{
    public SkillType skillType; // Ranged�܂���Close��I��
    public float coolTime;      // CoolTime���w��
    public float currentCoolTime;   // ���݂�CoolTime���w��
    public string animeName;    // animeName���w��
    public Transform rangeJudgementPoint; // rangeJudgementPoint���w��,Close�̏ꍇ�͂��̈ʒu���U���Ώۂ��߂��Ȃ��ƍU�����Ȃ�,Ranged�̏ꍇ�͂��̈ʒu��艓���Ȃ��ƍU�����Ȃ�
    public float distanceArrowRange; //Close/Range���ɁARangeJudgementPoint���狖�e�͈͂��w�肵�����ꍇ�l������@�l0�Ȃ狖�e�͈͖���
    public WalkType walkType; //�U�����ɕ��s���p�����邩�ǂ������w��
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