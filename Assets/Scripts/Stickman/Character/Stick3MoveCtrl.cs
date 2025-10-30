using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class Stick3MoveCtrl : MonoBehaviour
{
    private Stick3Stats stats;
    public Rigidbody2D rb;
    [HideInInspector]
    public Vector3 moveVector;     // �ړ�����
    private float knockbackVelocity = 0f;   // ���݂̃m�b�N�o�b�N���x
    public float knockbackResistance = 0f;  // �m�b�N�o�b�N�ϐ��i0�`1�j
    public float knockbackDampingRate = 5f; // �m�b�N�o�b�N�̌������i�P�ʁF���x/�b�j
    public float maxKnockbackVelocity = 20f;    // �ő�m�b�N�o�b�N���x
    public float speedMultiplier = 1.0f;
    
    [Inject]
    public void Inject(Stick3Stats stats)
    {
        this.stats = stats;
    }

    public void Initialize()
    {
        // �`�[���ɉ����Ĉړ�������ݒ�
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

        // �A�j���[�V��������̈ړ����x���擾
        float animeMoveRateValue = stats.animeMoveSpeed;

        // ��{�ړ����x
        float moveX = moveVector.normalized.x * stats.baseMoveSpeed * animeMoveRateValue * speedMultiplier;

        // �m�b�N�o�b�N���x���������������ړ����x
        float totalMoveX = (moveX + knockbackVelocity) * Time.deltaTime;

        // �ړ���K�p
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
        // �m�b�N�o�b�N�ϐ����l�������͂̒���
        float adjustedForce = force * (1f - knockbackResistance);

        // �m�b�N�o�b�N�̕����i�ړ������Ƌt�j
        float knockbackDir = -moveVector.normalized.x;

        // �m�b�N�o�b�N�����W���̌v�Z
        float attenuationFactor = 1f;

        if (knockbackVelocity >= adjustedForce)
        {
            // ���݂̃m�b�N�o�b�N���x���V���ȗ͂��傫���ꍇ�A������K�p
            attenuationFactor = Mathf.Clamp01(1f - (knockbackVelocity / maxKnockbackVelocity));
        }

        // �ǉ�����m�b�N�o�b�N���x�̌v�Z
        float additionalKnockback = adjustedForce * attenuationFactor;

        // �m�b�N�o�b�N���x�̍X�V
        knockbackVelocity += additionalKnockback * knockbackDir;
    }
}
