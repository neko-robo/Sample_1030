using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageReceiver
{
    /// <summary>
    /// �_���[�W���󂯎��܂��B
    /// </summary>
    /// <param name="damage">�󂯎��_���[�W��</param>
    /// <param name="id">�_���[�W�����GUID(����ID�̂��͈̂�x������e���Ȃ�)</param>
    /// <returns>�Ώۂ����S�������ǂ���</returns>
    bool DamageRecieve(int damage, int id, bool isKB);
}