using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BossManager : MyMonoBehaviourPunCallbacks
{
    [SerializeField] float changeSpan = 3f;

    private SimpleAnimation simpleAnimation = null;

    //アニメーションの名前のリスト
    private List<string> nameList = new List<string>();
    int index = 0;

    [SerializeField] int bossHp = 64; 

    // -----
    void Awake()
    {
        // プレイヤー人数 x  10の体力とする。
        bossHp = PhotonNetwork.CountOfPlayers * 10;
        // (無いと思うが念の為)
        if (bossHp == 0)
           bossHp = 10;

        simpleAnimation = GetComponent<SimpleAnimation>();

        // ボス生成のフラグを立てる
        PlayData.bossSpawned = true;

        // カメラ振動エフェクト
        CameraPlay.EarthQuakeShake(2f);
    }

    IEnumerator Start() {
        SoundManager.Play(SoundManager.SEClip.bossStart);

        yield return new WaitForSeconds(3f);

        foreach (var state in simpleAnimation.GetStates())
        {
            nameList.Add(state.name);
        }

        // 一定時間ごとにアニメーションを繰り返す
        SetRepeatCall(()=> !PlayData.bossDied, changeSpan, ()=>{
            int rnd = Random.Range(0, 3);
            // SE
            SoundManager.Play(SoundManager.SEClip.bossAtack1 + rnd);

            simpleAnimation.CrossFade(nameList[index], 0.5f);
            index++;
            index = Mathf.Clamp(index, 0, nameList.Count - 1);
        });
    }

    protected override void DoUpdate()
    {        
    }

    // -----
    public void MyBossDamage()
    {
        // ボスにダメージを与える（RPCで実行する）
        photonView.RPC(nameof(RpcBossDamage), RpcTarget.All);
    }

    [PunRPC]
    public void RpcBossDamage()
    {
        bossHp--;

        if (bossHp <= 0)
        {
            // ★ エフェクトとSE
            // カメラ振動エフェクト
            CameraPlay.EarthQuakeShake(2f);

            // SE
            SoundManager.Play(SoundManager.SEClip.bossDeath);

            PlayData.bossDied = true;
            // PhotonNetwork.Destroy(this.gameObject);
            // Ownerからしか削除できないので、とりあえず普通で
            Destroy(gameObject);
        }
    }
}
