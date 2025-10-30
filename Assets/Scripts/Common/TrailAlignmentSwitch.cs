using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAlignmentSwitch : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    private bool currentState = false;

    // Update is called once per frame
    void Update()
    {
        if(trailRenderer.emitting != currentState)
        {
            currentState = trailRenderer.emitting;
            EmitLinkSwitch();
        }
    }

    public void EmitLinkSwitch()
    {
        if(currentState)
        {
            trailRenderer.alignment = LineAlignment.TransformZ;
        }
        else
        {
            trailRenderer.alignment = LineAlignment.View;
        }
    }
}
