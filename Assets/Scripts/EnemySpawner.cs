using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class EnemySpawner : MyMonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] float span = 5f;

    int queueCount = 0;

    // -----------
    void Start()
    {
        // spanごとにqueueがあれば、生成する
        SetRepeatCall(()=> queueCount > 0, span, ()=>Spawn());
    }

    protected override void DoUpdate()
    {
    }

    // -----
    public void AddQueue()
    {
        queueCount ++;
    }

    public void Spawn()
    {
		// 生成
		PhotonNetwork.Instantiate(
			enemyPrefab.name, transform.position, enemyPrefab.transform.rotation);

        queueCount --;
    }
}
