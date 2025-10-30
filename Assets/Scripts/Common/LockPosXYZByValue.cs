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

            // X, Y, Z�̊e���W�����b�N����Ă��邩�ǂ������`�F�b�N
            float newX = isLockX ? targetPosition.x : currentPosition.x;
            float newY = isLockY ? targetPosition.y : currentPosition.y;
            float newZ = isLockZ ? targetPosition.z : currentPosition.z;

            // �V�������W��ݒ�
            transform.position = new Vector3(newX, newY, newZ);
        }
    }
}
