using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CreatureController : MyMonoBehaviourPunCallbacks
{
    [Tooltip("寿命")]
    [SerializeField] float limitTime = 10f;

    [Tooltip("最後、落下するか")]
    [SerializeField] bool fallEnd = false;

    float aliveTime = 0f;

    // ----------
    void Start()
    {
        // 寿命が来たら、落下する または 死亡する
        if (fallEnd)
        {
            SetOnceCall(() => aliveTime > limitTime,
                // RPCで実行する
                () => photonView.RPC(nameof(FallDown), RpcTarget.All));

            SetOnceCall(() => aliveTime  > limitTime + 3, () => PhotonDestroy());
        }
        else
        {
            SetOnceCall(() => aliveTime > limitTime, () => PhotonDestroy());
        }
    }

    protected override void DoUpdate()
    {
        // 落下した場合は削除
        if (transform.position.y < PlayData.deadYPos)
        {
            // Destroy(gameObject);
            PhotonDestroy();
        }

        aliveTime += Time.deltaTime;
    }

    // ----------
	[PunRPC]
    void FallDown()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;

        Projectile projectile = GetComponent<Projectile>();
        if (projectile != null)
            projectile.enabled = false;

        SprytLite sprytLite = GetComponent<SprytLite>();
        if (sprytLite != null)
            sprytLite.enabled = false;
    }

    void PhotonDestroy()
    {
        // PhotonNetwork.Destroy(gameObject);

        // Ownerからしか削除できないので、とりあえず普通で
        Destroy(gameObject);
    }
}