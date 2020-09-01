using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
// using Photon.Realtime;

[RequireComponent(typeof(Rigidbody2D))]
public class ColliderController2D : MonoBehaviour
{
    [Tooltip("衝突してきたオブジェクトのTag")]
    public string Tag;

    [Tooltip("ここにPrefabを指定すると、衝突時にオブジェクトを生成する")]
    public GameObject createPrefab = null;

    [Tooltip("これにチェックを付けると、衝突してきた相手をDestroyする")]
    public bool destroy = false;

    [Tooltip("これにチェックを付けると、衝突後、自身をDestroyする")]
    public bool destroyMe = false;

    [Tooltip("これにチェックを付けると、衝突後、自身をDestroyする。タグ判断あり")]
    public bool tagDestroyMe = false;


    [Tooltip("衝突時に他に処理を登録したい場合、ここに追加する")]
	public UnityEvent actions;

    [Tooltip("自クライアントキャラと衝突時のみ処理する")]
	public bool photonIsMine = false;


    // -----
    void OnCollisionEnter2D(Collision2D col)
    {
        if (string.IsNullOrEmpty(Tag) == false
            && MyCompareTag(col.gameObject))
        {
            if (tagDestroyMe)
                DestroyMe();

            collideAction(col.gameObject);
        }

        // 衝突相手に関わらず、自身を削除する
        if (destroyMe && !tagDestroyMe)
            DestroyMe();
    }

    // すり抜け時のイベント
    void OnTriggerEnter2D(Collider2D col)
    {
        if (string.IsNullOrEmpty(Tag) == false
            && MyCompareTag(col.gameObject))
        {
            collideAction(col.gameObject);

            if (tagDestroyMe)
                DestroyMe();
        }

        // 衝突相手に関わらず、自身を削除する
        if (destroyMe && !tagDestroyMe)
            DestroyMe();
    }

    bool MyCompareTag(GameObject gameObject_)
    {
        string[] tags = MyUtil.Split(Tag, ',');
        foreach(string tag in tags)
        {
            if (gameObject_.CompareTag(tag))
                return true;
        }

        return false;
    }

    // -----
    // 共通処理
    void collideAction(GameObject other)
    {
        // prefabが指定されているなら生成
        if (createPrefab != null)
            Instantiate(createPrefab, transform.position, createPrefab.transform.rotation);

        // 削除する指定があるなら、削除
        if (destroy)
            Destroy(other);

        if (photonIsMine)
        {
            PhotonView photonView = other.GetComponent<PhotonView>();
            if (photonView != null)
            {
                // Photon用　（対象相手が自クライアントで作成した場合のみ）
                // → 後にPhotonNetwork.Instantiateする等のケース
                if (photonView.IsMine)
                    actions.Invoke();   // 他に処理が登録されているなら実行
            }
        }
        else
        {
            // photonであれば、Bullet等を、PunRPCで生成するケース
            // （Bullet１つずつはPUNで同期しない）
            actions.Invoke();
        }    
    }

    void DestroyMe()
    {
        // 相手側の衝突判定が発生するよう、少し時間を待つ
        MyUtil.DelayCall(0.2f, ()=>Destroy(gameObject));
    }
}
