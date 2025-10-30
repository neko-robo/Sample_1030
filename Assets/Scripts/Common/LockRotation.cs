using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    public Vector3 defaultAngles;
    void Update()
    {
        gameObject.transform.rotation = Quaternion.Euler(defaultAngles.x, defaultAngles.y, defaultAngles.z);
    }
}
