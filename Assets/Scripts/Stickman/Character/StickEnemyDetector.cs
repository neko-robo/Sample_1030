using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using VContainer;

public class StickEnemyDetector : MonoBehaviour
{
    public Stick3Stats stats;
    public bool enemyDetected = false;
    public BoxCollider2D col;
    public BoxCollider2D col2;
    private float timer = 0.0f;
    public float detectFreq = 0.25f;
    public bool isDetecting = false;
    private int detectBuffer = -1;
    public float scanDelaymin = 0;
    public float scanDelaymax = 0.15f;
    public GameObject playerObj;
    public float detectDistance = 10000f;
    public Transform targetTransform;
    public GameObject targetOldposObj;
    public Vector3 defaultOffset = Vector3.zero;

    public bool isOldPosResetByPosY = false;
    private float groundY = 1.0f;
    

    [Inject]
    public void Inject(Stick3Stats stats)
    {
        this.stats = stats;
    }

    public void Start()
    {
        defaultOffset = targetOldposObj.transform.position - transform.position;
    }


    public void Init(int team)
    {
        if(team == 0)
        {
            col.contactCaptureLayers = Constant.LAYERMASK_STICK1_ONLY;
            if (col2 != null)
            {
                col2.contactCaptureLayers = Constant.LAYERMASK_STICK1_ONLY;

            }
        }else if(team == 1)
        {
            col.contactCaptureLayers = Constant.LAYERMASK_STICK0_ONLY;
            if (col2 != null)
            {
                col2.contactCaptureLayers = Constant.LAYERMASK_STICK0_ONLY;

            }
        }
        isDetecting = true;
    }
    

    public void DetectStart()
    {
        detectDistance = 10000f;
        isDetecting = true;
        timer = detectFreq;
        detectBuffer = -1;
        enemyDetected = false;
        targetTransform = null;

    }

    public void DetectEnd()
    {
        detectDistance = 10000f;
        isDetecting = false;
        timer = detectFreq;
        detectBuffer = -1;
        enemyDetected = false;

    }

    public void Update()
    {
        if(targetOldposObj != null && targetTransform != null)
        {
            targetOldposObj.transform.position = targetTransform.position;
        }
        
        
        if (isOldPosResetByPosY)
        {
            if (targetOldposObj.transform.position.y <= groundY)
            {
                targetOldposObj.transform.position = transform.position + defaultOffset;
            }
        }




        if (isDetecting && !col.enabled)
        {
            col.enabled = true;
        }

        if(!isDetecting && col.enabled)
        {
            col.enabled = false;
        }

        if(col2 != null)
        {
            if (isDetecting && !col2.enabled)
            {
                col2.enabled = true;
            }

            if (!isDetecting && col2.enabled)
            {
                col2.enabled = false;
            }
        }
    }
    
    public Transform GetTargetTransform()
    {
        if(targetTransform != null)
        {
            return targetTransform;
        }
        else
        {
            if (targetOldposObj != null)
            {
                return targetOldposObj.transform;
            }
            else
            {
                return null;
            }

        }
    }

    public Vector3 GetTargetPos()
    {
        Vector3 result;
        if(targetTransform != null)
        {
            result = new Vector3(targetTransform.position.x, targetOldposObj.transform.position.y, targetTransform.position.z);
            return result;
        }
        else
        {
            if (targetOldposObj != null)
            {
                result = new Vector3(targetOldposObj.transform.position.x, targetOldposObj.transform.position.y, targetOldposObj.transform.position.z);
                return result;
            }
            else
            {
                return Vector3.zero;
            }

        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        float newDetectDistance = (playerObj.transform.position - collision.transform.position).sqrMagnitude;

        if (newDetectDistance < detectDistance)
        {
            detectDistance = newDetectDistance;
            targetTransform = collision.transform;
            Invoke("LateDetect", Random.Range(scanDelaymin, scanDelaymax));
            /*Debug.Log("DetectSucceed new:" + newDetectDistance + " distance" + detectDistance);*/
        }
        else
        {
            /*Debug.Log("DetectFailedByDistance new:" + newDetectDistance + " distance" + detectDistance);*/
        }


    }

    public void LateDetect()
    {
        if (isDetecting)
        {
            enemyDetected = true;
        }
    }
}
