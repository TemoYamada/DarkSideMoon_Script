using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public partial class MyUtil
{
	//-----------------------
	// Layer関連
	// 名前からLayerナンバーを取得
	public static int LayerNameToNumber(string name)
	{
		return LayerMask.NameToLayer(name);
	}

	// Layerナンバーから名前を取得
	public static string LayerNumberToName(int number)
	{
		return LayerMask.LayerToName(number);
	}

	// カメラのcullingMaskに、特定レイヤーのみを指定
	public static void SetCameraLayerOnly(Camera camera, params string[] names)
	{
		camera.cullingMask = 0;
		// 追加
		SetCameraLayerAdd(camera, names);
	}

	// カメラのcullingMaskに、特定レイヤーを追加
	public static void SetCameraLayerAdd(Camera camera, params string[] names)
	{
		foreach(string name in names)
		{
			int number = LayerNameToNumber(name);
			camera.cullingMask |= (1 << number);
		}

		// レイヤーマスクの仕組みは以下を参照
		// https://qiita.com/ptkyoku/items/5602733ba9cff0ccd54d
		// https://qiita.com/JunShimura/items/df4cf9228cef9e5a73ed
	}

	// GameObjectのレイヤーを変更する
	public static void ChangeLayer(GameObject gameObject, string name)
	{
		gameObject.layer = LayerMask.NameToLayer(name);
	}


}
