 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class TitleController : MonoBehaviour
{
    bool isStart = false;

    // ----------
    void Start()
    {
     	// BGM
        BgmManager.Play("Hurry");
    }

    // -----
    public void OnButtonStart()
    {
        if (isStart)
            return;

        isStart = true;

        SoundManager.Play(SoundManager.SEClip.ok);
        FadeManager.Instance.LoadScene("PlayScene", 0.2f);
    }

    // public void OnButtonNewRoom()
    // {
    //     PlayData.createNewRoom = true;
    //     OnButtonStart();
    // }

}
