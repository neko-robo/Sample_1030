using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class Stick3MoveCtrl : MonoBehaviour
{
    private Stick3Stats stats;
    public Rigidbody2D rb;
    [HideInInspector]
    public Vector3 moveVector;     // 移動方向
    private float knockbackVelocity = 0f;   // 現在のノックバック速度
    public float knockbackResistance = 0f;  // ノックバック耐性（0～1）
    public float knockbackDampingRate = 5f; // ノックバックの減衰率（単位：速度/秒）
    public float maxKnockbackVelocity = 20f;    // 最大ノックバック速度
    public float speedMultiplier = 1.0f;
    
    [Inject]
    public void Inject(Stick3Stats stats)
    {
        this.stats = stats;
    }

    public void Initialize()
    {
        // チームに応じて移動方向を設定
        if (stats.team == 0)
        {
            moveVector = new Vector3(1, 0, 0);
        }
        else if (stats.team == 1)
        {
            moveVector = new Vector3(-1, 0, 0);
        }

        maxKnockbackVelocity *= 0.5f;
    }

    void FixedUpdate()
    {
        if (!stats.isAlive)
        {
            return;
        }

        // アニメーションからの移動速度を取得
        float animeMoveRateValue = stats.animeMoveSpeed;

        // 基本移動速度
        float moveX = moveVector.normalized.x * stats.baseMoveSpeed * animeMoveRateValue * speedMultiplier;

        // ノックバック速度を加味した総合移動速度
        float totalMoveX = (moveX + knockbackVelocity) * Time.deltaTime;

        // 移動を適用
        Vector2 currentPosition = rb.position;
        Vector2 newPosition = currentPosition + new Vector2(totalMoveX, 0);
        rb.MovePosition(newPosition);

        if (knockbackVelocity != 0f)
        {
            knockbackVelocity = Mathf.Lerp(knockbackVelocity, 0f, 2.2f * Time.deltaTime);
            if (Mathf.Abs(knockbackVelocity) < 0.01f)
            {
                knockbackVelocity = 0f;
            }
        }
    }

    public void DamageKnockBack(float damage)
    {
        float forceRate = -50.0f * (damage / 20.0f);
        rb.AddForce(moveVector * forceRate);
    }
    
    public void ForceRecieveFloat(float force)
    {
        force *= 2.0f;
        // ノックバック耐性を考慮した力の調整
        float adjustedForce = force * (1f - knockbackResistance);

        // ノックバックの方向（移動方向と逆）
        float knockbackDir = -moveVector.normalized.x;

        // ノックバック減衰係数の計算
        float attenuationFactor = 1f;

        if (knockbackVelocity >= adjustedForce)
        {
            // 現在のノックバック速度が新たな力より大きい場合、減衰を適用
            attenuationFactor = Mathf.Clamp01(1f - (knockbackVelocity / maxKnockbackVelocity));
        }

        // 追加するノックバック速度の計算
        float additionalKnockback = adjustedForce * attenuationFactor;

        // ノックバック速度の更新
        knockbackVelocity += additionalKnockback * knockbackDir;
    }
}
