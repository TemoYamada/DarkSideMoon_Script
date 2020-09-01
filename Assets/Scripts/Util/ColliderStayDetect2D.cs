using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class ColliderStayDetect2D : MonoBehaviour
{
    [Tooltip("衝突してきたオブジェクトのTag")]
    public string Tag;

    // [Tooltip("接触している時に処理を登録したい場合、ここに追加する")]
	// public UnityEvent actions;

    [Tooltip("自クライアントキャラと接触時のみ処理する")]
	public bool photonIsMine = false;

    // public float pow = 100f;

    [HideInInspector] public List<GameObject> stayList = new List<GameObject>();

    // -----
    void OnCollisionEnter2D(Collision2D col)
    {
        if (string.IsNullOrEmpty(Tag) == false
            && MyCompareTag(col))
            stayList.Add(col.gameObject);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (string.IsNullOrEmpty(Tag) == false
            && MyCompareTag(col))
        {
            if (stayList.Contains(col.gameObject))
                stayList.Remove(col.gameObject);
        }
    }

    bool MyCompareTag(Collision2D col)
    {
        string[] tags = MyUtil.Split(Tag, ',');
        foreach(string tag in tags)
        {
            if (col.gameObject.CompareTag(tag))
                return true;
        }

        return false;
    }

    // -----
    // // 共通処理
    // void collideAction(GameObject other)
    // {
    //     // prefabが指定されているなら生成
    //     if (photonIsMine)
    //     {
    //         PhotonView photonView = other.GetComponent<PhotonView>();
    //         if (photonView != null)
    //         {
    //             // Photon用　（対象相手が自クライアントで作成した場合のみ）
    //             // → 後にPhotonNetwork.Instantiateする等のケース
    //             if (photonView.IsMine)
    //                 // actions.Invoke();   // 他に処理が登録されているなら実行
    //                 DoAction(other);
    //         }
    //     }
    //     else
    //     {
    //         // photonであれば、Bullet等を、PunRPCで生成するケース
    //         // （Bullet１つずつはPUNで同期しない）
    //         // actions.Invoke();
    //         DoAction(other);
    //     }    
    // }

    public void DoAction()
    {
        foreach(GameObject target in stayList)
        {
            if (photonIsMine)
            {
                PhotonView photonView = target.GetComponent<PhotonView>();
                if (photonView != null)
                {
                    // Photon用　（対象相手が自クライアントで作成した場合のみ）
                    // → 後にPhotonNetwork.Instantiateする等のケース
                    if (photonView.IsMine)
                    {
                        // target.GetComponent<Rigidbody2D>().AddForce(new Vector3(0, pow, 0));
                        PlayerManager playerManager = target.GetComponent<PlayerManager>();
                        if (playerManager != null)
                            playerManager.AddUpForce();
                    }
                }
            }
        }
    }
}
