using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BossSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject enemyPrefab;

    // -----------
    // void Start()
    // {
    // }

    // protected override void DoUpdate()
    // {
    // }

    // -----
    public void Spawn()
    {
        if (PlayData.bossSpawned)
            return;

        PlayData.bossSpawned = true;

		// 生成
		// bossObj = 
        PhotonNetwork.Instantiate(
			enemyPrefab.name, transform.position, enemyPrefab.transform.rotation);
    }
}
