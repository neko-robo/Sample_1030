using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageReceiver
{
    /// <summary>
    /// ダメージを受け取ります。
    /// </summary>
    /// <param name="damage">受け取るダメージ量</param>
    /// <param name="id">ダメージ判定のGUID(同一IDのものは一度しか被弾しない)</param>
    /// <returns>対象が死亡したかどうか</returns>
    bool DamageRecieve(int damage, int id, bool isKB);
}