using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BeamController : MonoBehaviourPunCallbacks
{
    [SerializeField] BulletSpawner[] spawners;
    [SerializeField] GameObject spike;
    [SerializeField] GameObject bulletTrigger;

    // -----
    // void Start()
    // {        
    // }

    // void Update()
    // {        
    // }

    // -----
    public void MyBulletStart()
    {
        // RPCで実行する
        photonView.RPC(nameof(RPCBulletStart), RpcTarget.All);   
    }

    public void MyBulletEnd()
    {
        // RPCで実行する
        photonView.RPC(nameof(RPCBulletEnd), RpcTarget.All);   
    }

    // public void MySpikeOn()
    // {
    //     // RPCで実行する
    //     photonView.RPC(nameof(RPCSpikeOnOFf), RpcTarget.All, true);   
    // }

    public void MySpikeOff()
    {
        // RPCで実行する
        photonView.RPC(nameof(RPCSpikeOnOFf), RpcTarget.All, false);   
    }

    // -----
    [PunRPC]
    public void RPCBulletStart()
    {
        foreach(var spawner in spawners)
        {
            spawner.SetStart();
        }

        // Computer周りのTrapは非表示にする
        RPCSpikeOnOFf(false);
    }

    [PunRPC]
    public void RPCBulletEnd()
    {
        foreach(var spawner in spawners)
        {
            spawner.SetEnd();
        }

        // Computer周りのTrapは表示にする
        RPCSpikeOnOFf(true);

        // 弾発射トリガーはOFFにする
        bulletTrigger.SetActive(false);
    }

    [PunRPC]
    public void RPCSpikeOnOFf(bool isActive)
    {
        bool active_ = spike.activeInHierarchy;
        if (active_ == isActive)
            return;

        // SE音
        SoundManager.Play(SoundManager.SEClip.pibo);

        // カメラ振動エフェクト
    	CameraPlay.EarthQuakeShake(1f);

        spike.SetActive(isActive);
    }

}
