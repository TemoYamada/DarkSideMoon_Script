using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BGMChanger : MonoBehaviour
{
    [Tooltip("衝突してきたオブジェクトのTag")]
    public string Tag;

    [Tooltip("BGM名")]
    public string bgmName;

    // ----------
    // void Start()
    // {        
    // }

    // void Update()
    // {        
    // }

    // ----------
    // すり抜け時のイベント
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Tag))
            collideAction(col.gameObject);
    }

    // -----
    // 共通処理
    void collideAction(GameObject gameObject)
    {
        // BGMを変更する
        BgmManager.Play(bgmName);
    }
}
