using UnityEngine;

public class DamageTraceData
{
    public int team;
    public int typeID;
    public float damage;
    
    public DamageTraceData(int team, int typeID, float damage)
    {
        this.team = team;
        this.typeID = typeID;
        this.damage = damage;
    }
}
