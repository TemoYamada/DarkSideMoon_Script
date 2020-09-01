using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletSpawner : MyMonoBehaviourPunCallbacks
{
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] float span = 3f;

    [SerializeField] bool seOn = false;

    private bool inSpawn = false;

    // -----------
    void Start()
    {
        // spanごとにフラグが立っていれば、生成する
        SetRepeatCall(()=> inSpawn, span, ()=>Spawn());
    }

    protected override void DoUpdate()
    {
    }

    // -----
    public void SetStart()
    {
        inSpawn = true;
    }

    public void SetEnd()
    {
        inSpawn = false;
    }

    // -----
    [PunRPC]
    public void Spawn()
    {
        if (seOn)
            SoundManager.Play(SoundManager.SEClip.beam);

		// 生成
		Instantiate(bulletPrefab, 
            transform.position, bulletPrefab.transform.rotation);
    }
}
