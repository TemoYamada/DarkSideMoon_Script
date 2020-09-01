using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FlagController : MonoBehaviour
{
    [Tooltip("衝突してきたオブジェクトのTag")]
    public string Tag;

    // -----
    // void Start()
    // {        
    // }

    // void Update()
    // {  
    // }

    // ----------
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Tag))
            collideAction(col.gameObject);
    }

    // -----
    // 共通処理
    void collideAction(GameObject gameObject)
    {
        // 色を変える
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color32(0xFF, 0xAD, 0x5B, 0xFF);
    }
}
