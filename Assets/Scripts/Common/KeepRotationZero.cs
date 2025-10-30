using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotationZero : MonoBehaviour
{
    void LateUpdate()
    {
        transform.localRotation = Quaternion.identity;
    }
}