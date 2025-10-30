using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class CoreDeadEffectCtrlStandAlone : MonoBehaviour
{
    public VisualEffect deadVFX;
    public ExplodeFadeSprite coreExplode;
    public float deadTime = 5.0f;
    public float lastExplodeFadingTime = 3.0f;
    public bool isAutoActivate = true;

    public void Start()
    {
        if (isAutoActivate)
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (deadVFX != null)
        {
            deadVFX.SetFloat("Rate", 0.2f);
            Invoke("VFXBigger", 1.5f);
        }
        Invoke("VFXDisable", deadTime);
        Invoke("CoreBigExplode", deadTime);
    }

    public void VFXBigger()
    {
        if (deadVFX != null)
        {
            deadVFX.SetFloat("Rate", 0.6f);
        }
    }

    public void CoreBigExplode()
    {
        coreExplode.ActivateSprite(lastExplodeFadingTime);
    }

    public void VFXDisable()
    {
        if (deadVFX != null)
        {
            deadVFX.SetFloat("Rate", 0.0f);
        }
    }
}
