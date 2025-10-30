using UnityEngine;

public class ActionEventData
{
    public int team;
    public ActionType type;
    public Vector3 position;
    public float duration;
    public float value;

    public ActionEventData(int team, ActionType type, Vector3 position)
    {
        this.team = team;
        this.type = type;
        this.position = position;
        this.duration = 0f;
        this.value = 0f;
    }

    public ActionEventData(int team, ActionType type, Vector3 position, float duration, float value)
    {
        this.team = team;
        this.type = type;
        this.position = position;
        this.duration = duration;
        this.value = value; 
    }

    public enum ActionType
    {
        unitSlow,
        CameraShake,
        PhaseShift,
        None
    }
}
