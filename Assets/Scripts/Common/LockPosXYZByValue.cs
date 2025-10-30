using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPosXYZByValue : MonoBehaviour
{
    public bool activated = false;
    public Vector3 targetPosition;
    public bool isLockX;
    public bool isLockY;
    public bool isLockZ;

    public void Update()
    {
        if (activated)
        {
            Vector3 currentPosition = transform.position;

            // X, Y, Zの各座標がロックされているかどうかをチェック
            float newX = isLockX ? targetPosition.x : currentPosition.x;
            float newY = isLockY ? targetPosition.y : currentPosition.y;
            float newZ = isLockZ ? targetPosition.z : currentPosition.z;

            // 新しい座標を設定
            transform.position = new Vector3(newX, newY, newZ);
        }
    }
}
