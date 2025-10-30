using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageTextCtrl : MonoBehaviour
{
    public Color[] colorsByTeam;
    public TMPro.TMP_Text text;
    public float duration = 1.0f;

    public float initialVelocityX = 5f;  // ���������̏���
    public float initialVelocityY = 10f; // ���������̏���
    public float gravity = -9.8f;        // �d�͉����x
    //public float duration = 2f;          // �A�j���[�V�����̎�������

    public void Init(int team, int damage)
    {
        text.text = damage.ToString();
        text.color = colorsByTeam[team];

        transform.Translate(new Vector3( Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0f));

        float log = Mathf.Log(damage, 20);
        float scaleRate = Mathf.Lerp(1.15f, 2.1f, Mathf.InverseLerp(1.0f, 2.2f, log));
        transform.localScale = new Vector3(scaleRate, scaleRate, 1.0f);


        if (team == 0)
        {
            initialVelocityX = Random.Range(-1.0f, -8.0f);
        }
        else
        {
            initialVelocityX = Random.Range(1.0f, 8.0f);
        }

        initialVelocityY = Random.Range(5f, 10f);

        float startRnd = Random.Range(0.0f, 0.05f);
        Invoke("BlinkStart", startRnd);
    }

    public void InitWithText(int team, int damageForScale, string str)
    {
        text.text = str;
        text.color = colorsByTeam[team];

        transform.Translate(new Vector3(Random.Range(-0.4f, 0.4f), Random.Range(-0.4f, 0.4f), 0f));

            float log = Mathf.Log(damageForScale, 20);
            float scaleRate = Mathf.Lerp(1.1f, 2.75f, Mathf.InverseLerp(1.3f, 2.0f, log));
            transform.localScale = new Vector3(scaleRate, scaleRate, 1.0f);


        if (team == 0)
        {
            initialVelocityX = Random.Range(-1.0f, -8.0f);
        }
        else
        {
            initialVelocityX = Random.Range(1.0f, 8.0f);
        }

        initialVelocityY = Random.Range(5f, 10f);

        float startRnd = Random.Range(0.0f, 0.05f);
        Invoke("BlinkStart", startRnd);
    }

    public void BlinkStart()
    {
        text.DOFade(0.75f, duration * 0.15f);
        MoveParabola();
        Invoke("LateProcess", duration * 0.5f);
        Invoke("DestroySelf", duration + 0.2f);
    }

    public void MoveParabola()
    {
        Vector3 startPos = transform.position;

        // x�����̈ړ�
        float targetX = startPos.x + (initialVelocityX * duration);

        // DoTween�V�[�P���X�̍쐬
        Sequence sequence = DOTween.Sequence();

        // ���������̈ړ�
        sequence.Append(transform.DOMoveX(targetX, duration).SetEase(Ease.Linear));

        // ���������̈ړ��i�������̌`���v�Z���邽�߁A���Ԃɉ�����Y�ʒu���X�V�j
        sequence.Join(DOTween.To(() => transform.position,
                                 pos => transform.position = new Vector3(pos.x, CalculateYPosition(startPos.y, pos.x - startPos.x), pos.z),
                                 new Vector3(targetX, 0, 0), duration)
                         .SetEase(Ease.Linear));
    }

    public void LateProcess()
    {
        text.DOFade(0.0f, duration * 0.5f);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private float CalculateYPosition(float startY, float xDelta)
    {
        float t = xDelta / initialVelocityX; // ���� t = ���� / ���x
        return startY + (initialVelocityY * t) + (0.5f * gravity * t * t);
    }
}
