using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTouchController : MonoBehaviour
{
    [SerializeField] ContactFilter2D filter2d = default;

    public bool isTouching = false;

    Rigidbody2D rBody2D = default;
    
    // Start is called before the first frame update
    void Start()
    {
        rBody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 参考:
        // http://tsubakit1.hateblo.jp/entry/2018/04/07/234028

        // 接触しているかを判定する
        isTouching = rBody2D.IsTouching(filter2d);
    }
}
