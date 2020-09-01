 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class StartController : MyMonoBehaviour
{
    [SerializeField] Slider audioSlider = default;
    [SerializeField] AudioMixer audioMixer = default;
    [SerializeField] Text verText = default;
    [SerializeField] Button startButton = default;
    [SerializeField] InputField inputField = default;

    bool isStart = false;

    // ----------
    void Start()
    {
        // プレイデータのクリア
        PlayData.Init();
        // ランキング名はセットする （一度プレイ後のスタート画面に戻った時）
        inputField.text = PlayData.rankingName;

        // 最初はボタンを無効に
        startButton.interactable = false;

     	// BGM
        BgmManager.Play("TheLaboratory");

        verText.text = "Ver." + Application.version;

        // 名前を入力されたら、ボタンを有効に
        SetOnceCall(()=> string.IsNullOrEmpty(PlayData.rankingName) == false,
            ()=> startButton.interactable = true);
    }

    protected override void DoUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Debug.Log("Enter!");
            if (string.IsNullOrEmpty(PlayData.rankingName) == false)
                OnButtonStart();
        }
    }

    // -----
    public void OnSliderChanged()
    {
        audioMixer.SetFloat("MasterVolume", audioSlider.value);
    }

    public void OnInputValueChanged()
    {
        // プレイヤー名の格納
        OnInputFieldEnd(inputField.text);
    }

    public void OnInputFieldEnd(string str)
    {
        // プレイヤー名の格納
        PlayData.rankingName = str;
    }

    public void OnButtonStart()
    {
        if (isStart)
            return;

        isStart = true;

        SoundManager.Play(SoundManager.SEClip.ok);
        FadeManager.Instance.LoadScene("TitleScene", 0.2f);
    }

}
