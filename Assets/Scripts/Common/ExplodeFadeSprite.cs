using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplodeFadeSprite : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float defaultAlpha = 1.0f;
    public float eraseTime = 3.0f;
    public float scaleUpRate = 1.5f;
    public bool usingThis = false;

    private Vector3 defaultScale;
    private Vector3 targetScale;
    private bool defaultScaleInited = false;
    public bool autoActivate = false;

    public Color[] colors;
    public Material[] materials;

    public void Start()
    {
        if (autoActivate)
        {
            ActivateSprite();
        }
    }

    public void colorSet(int ID)
    {
        sprite.color = colors[ID];
        sprite.material = materials[ID];
    }
    public void ActivateSprite()
    {
        
        if (usingThis)
        {
            //Debug.LogError("Duplicate ExplodeActivate");
            return;
        }
        usingThis = true;
        if (!defaultScaleInited) { 
            defaultScale = transform.localScale;
            targetScale = defaultScale * scaleUpRate;
            defaultScaleInited = true;
        }
        sprite.enabled = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, defaultAlpha);

        transform.localScale = defaultScale;
        sprite.DOFade(0.0f, eraseTime);
        transform.DOScale(targetScale, eraseTime);

        Invoke("DisableSprite", eraseTime + 0.05f);
    }

    public void ActivateSprite(float setTime)
    {
        //Debug.Log("ActivateSprite");
        //if (usingThis)
        //{
        //    Debug.LogError("Duplicate ExplodeActivate");
        //    return;
        //}
        eraseTime = setTime;
        usingThis = true;
        if (!defaultScaleInited)
        {
            defaultScale = transform.localScale;
            targetScale = defaultScale * scaleUpRate;
            defaultScaleInited = true;
        }
        sprite.enabled = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, defaultAlpha);

        sprite.DOFade(0.0f, eraseTime);
        transform.localScale = defaultScale;
        transform.DOScale(targetScale, eraseTime);

        Invoke("DisableSprite", eraseTime + 0.05f);
    }

    public void LateDestroy()
    {

        Invoke("DestroySelf", 2.1f);
    }

    public void FastDestroy()
    {
        sprite.enabled = false;
        Invoke("DestroySelf", 0.92f);
    }

    public void DestroySelf()
    {
        sprite.enabled = false;
        Destroy(gameObject);
    }

    public void DisableSprite()
    {
        sprite.enabled = false;
        usingThis = false;
    }
}
