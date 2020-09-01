using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeeSawController : MyMonoBehaviour
{
    // Rigidbody2D rb;

    Vector3 rightDownPos = new Vector3(0, 0, -30f);
    Vector3 leftDownPos = new Vector3(0, 0, 30f);

    [SerializeField] ColliderStayDetect2D leftDetect2D;
    [SerializeField] ColliderStayDetect2D rightDetect2D;

    Sequence balanceSequence;
    bool inTorque = false;  // 回転中か

    // -----
    void Start()
    {
        // 1秒ごとに水平にするか判断
        SetRepeatCall(
            ()=> (inTorque == false
                && leftDetect2D.stayList.Count == 0 && rightDetect2D.stayList.Count == 0),
            1f,
            () => SetBalance()
        );
    }

    protected override void DoUpdate()
    {
    }

    // -----
    void SetBalance()
    {
        balanceSequence = DOTween.Sequence()
            .Append(transform.DOLocalRotate(Vector3.zero, 0.2f));
    }

    public void AddTorque(bool isPlus)
    {
        if (inTorque)
            return;

        // 水平に保とうとしている状態であれば、それをkill
        if (balanceSequence != null)
            balanceSequence.Kill();

        inTorque = true;

        if (isPlus)
        {
            // 右が上がる場合
            rightDetect2D.DoAction();

            balanceSequence = DOTween.Sequence()
            .Append(transform.DOLocalRotate(leftDownPos, 0.4f).SetDelay(0.1f))
            .OnComplete(() => {
                inTorque = false;
            });
        }
        else
        {
            // 左が上がる場合
            leftDetect2D.DoAction();
    
            balanceSequence = DOTween.Sequence()
            .Append(transform.DOLocalRotate(rightDownPos, 0.4f).SetDelay(0.1f))
            .OnComplete(() => {
                inTorque = false;
            });
        }
    }
}
