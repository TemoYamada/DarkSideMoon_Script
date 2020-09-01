using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
// using InControl;

public partial class MyUtil {

    //-----------------------
    // UGUIが重なっている箇所のクリックは、オブジェクトは反応しないようにする
    // https://tyfkda.github.io/blog/2016/11/13/unity-tap-ui.html
    private static List<RaycastResult> raycastResults = new List<RaycastResult>();  // リストを使いまわす

    public static bool IsPointerOverUIObject(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
        // the ray cast appears to require only eventData.position.
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = screenPosition;

        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);
        bool isOver = raycastResults.Count > 0;
        raycastResults.Clear();
        return isOver;
    }

    //-----------------------
	// CanvasGroupをON,OFFする
	public static void CanvasGroupToggleONOFF(CanvasGroup canvasGroup, bool isOn)
	{
		canvasGroup.alpha = (isOn ? 1f : 0f);
		canvasGroup.interactable = isOn;
		canvasGroup.blocksRaycasts = isOn;
	}

    //-----------------------
	// 親子関係の取得処理
	// 参考　https://qiita.com/hiroyuki7/items/95c66aee26115cf24a19

	// 子オブジェクト → 親オブジェクトを取得
	public static GameObject GetParent(GameObject gameObject)
	{
		Transform parentTrn = GetParent(gameObject.transform);

		if (parentTrn == null)
			return null;
		else
			return parentTrn.gameObject;
	}

	// 子オブジェクト → 親オブジェクトを取得
	public static Transform GetParent(Transform transform)
	{
		return transform.parent;
	}

	// 親オブジェクト → 子オブジェクト（名前から取得。孫の取得は、"子/孫"と指定）
	public static GameObject GetChild(GameObject gameObject, string name)
	{
		Transform childTrn = GetChild(gameObject.transform, name);

		if (childTrn == null)
			return null;
		else
			return childTrn.gameObject;
	}

	// 親オブジェクト → 子オブジェクト（名前から取得。孫の取得は、"子/孫"と指定）
	public static Transform GetChild(Transform transform, string name)
	{
		return transform.Find(name);
	}

	// 親オブジェクト → 子オブジェクト（上から順のインデックスで取得）
	public static Transform GetChild(Transform transform, int index)
	{
		return transform.GetChild(index);
	}

	// 親オブジェクト → 子オブジェクト（1つしかないと判っている場合）
	public static GameObject GetChild(GameObject gameObject)
	{
		Transform childTrn = GetChild(gameObject.transform);

		if (childTrn == null)
			return null;
		else
			return childTrn.gameObject;
	}

	// 親オブジェクト → 子オブジェクト（1つしかないと判っている場合）
	public static Transform GetChild(Transform transform)
	{
		return transform.GetChild(0);
	}

	// 親 → 子オブジェクト（複数）
	public static List<GameObject> GetChildren(GameObject gameObject)
	{
		List<GameObject> childList = new List<GameObject>();

		foreach (Transform child in gameObject.transform)
        {
			childList.Add(child.gameObject);
        }

		return childList;
	}

	// 親 → 子オブジェクト（複数）
	public static List<Transform> GetChildren(Transform transform)
	{
		List<Transform> childList = new List<Transform>();

		foreach (Transform child in transform)
        {
			childList.Add(child);
        }

		return childList;
	}

	// 親 → すべての子・孫オブジェクト（複数）
	public static Transform[] GetAllChildren(Transform transform)
	{
		return transform.gameObject.GetComponentsInChildren<Transform>();
	}

	// 親 → 末端のみのオブジェクト（複数）
	public static List<Transform> GetEndChildren(Transform transform)
	{
		List<Transform> childList = new List<Transform>();

		SubGetEndChildren(childList, transform);

		return childList;
	}

	// 親 → 末端のみのオブジェクト（複数） - 再帰関数
	public static List<Transform> SubGetEndChildren(List<Transform> childList, Transform transform)
	{
		if (transform.childCount > 0)
		{
			foreach (Transform child in transform)
			{
				SubGetEndChildren(childList, child);
			}
		}
		else
		{
			childList.Add(transform);
		}

		return childList;
	}

	// // 親子関係を構築
	// public static void BuildParentChild(GameObject parent, GameObject child)
	// {
	// 	child.transform.parent = parent.transform;
	// }

	// // 親子関係を構築
	// public static void BuildParentChild(Transform parent, Transform child)
	// {
	// 	child.parent = parent;
	// }

	// 親子関係を構築
	public static void SetParent(GameObject child, GameObject parent)
	{
		child.transform.SetParent(parent.transform);
	}

	// 親子関係を構築
	public static void SetParent(Transform child, Transform parent)
	{
		// child.parent = parent;
		child.SetParent(parent);
	}

	// 子から見た、親子関係を解除
	public static void ReleaseParent(GameObject child)
	{
		child.transform.parent = null;
	}

	// 子から見た、親子関係を解除
	public static void ReleaseParent(Transform child)
	{
		child.parent = null;
	}

	// 親から見た、親子関係を一括解除
	public static void ReleaseChildren(GameObject parent)
	{
		parent.transform.DetachChildren();
	}

	// 親から見た、親子関係を一括解除
	public static void ReleaseChildren(Transform parent)
	{
		parent.DetachChildren();
	}

	// 親子であるかを判定する
	public static bool IsChildOf(Transform parent, Transform child)
	{
		return child.IsChildOf(parent);
	}

	// 子オブジェクトの数を取得
	public static int GetChildCount(GameObject parent)
	{
		return parent.transform.childCount;
	}

	// 子オブジェクトの数を取得
	public static int GetChildCount(Transform parent)
	{
		return parent.childCount;
	}

    //-----------------------
	// 画面サイズ取得関連
	private static Vector2 _minPos = Vector2.zero;
	private static Vector2 _maxPos = Vector2.zero;


	/// 画面の左下のワールド座標を取得する.
	public static Vector2 GetWorldMin ()
	{
		if (_minPos != Vector2.zero)
			return _minPos;

		// ※ 2Dで、mainCameraがOrthograhicである前提
		// （もし3Dである場合などは、Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 10));などのように、
		//   Zに、カメラと平面との距離が必要となる）
		// 参考
		// https://tama-lab.net/2018/08/%E3%80%90unity%E3%80%91%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%81%8C%E7%94%BB%E9%9D%A2%E5%A4%96%E3%81%8B%E3%81%A9%E3%81%86%E3%81%8B%E3%82%92%E5%88%A4%E5%AE%9A%E3%81%99%E3%82%8B/

		_minPos = Camera.main.ViewportToWorldPoint (Vector2.zero);
		return _minPos;
	}

	/// 画面右上のワールド座標を取得する.
	public static Vector2 GetWorldMax ()
	{
		if (_maxPos != Vector2.zero)
			return _maxPos;

		// Viewportが右上(1,1)をWorldPointに変換する
		_maxPos = Camera.main.ViewportToWorldPoint (Vector2.one);
		return _maxPos;
	}

	// 最大横幅を取得
	public static float GetWidth()
	{
		return GetWorldMax().x - GetWorldMin().x;
	}


	// 最大縦幅を取得
	public static float GetHeight()
	{
		return GetWorldMax().y - GetWorldMin().y;
	}

	// 端末の現在の画面の向きを取得する
	public static DeviceOrientation GetDeviceOrientation()
	{
		// 参考
		// https://indie-du.com/entry/2016/08/16/083000

        DeviceOrientation result = Input.deviceOrientation;

		// Screen.orientationを使う手もあるようだ
		// https://qiita.com/satotin/items/2009788da11805a4a9cf


        // Unkownならピクセル数から判断
        if (result == DeviceOrientation.Unknown
		|| result == DeviceOrientation.FaceUp
		|| result == DeviceOrientation.FaceDown)
        {
            if (Screen.width < Screen.height)
            {
                result = DeviceOrientation.Portrait;
            }
            else
            {
                result = DeviceOrientation.LandscapeLeft;
            }
        }

        return result;
    }

	// 端末の現在の画面の向きを取得する
	public static ScreenOrientation GetScreenOrientation()
	{
        ScreenOrientation result = Screen.orientation;

		#if UNITY_EDITOR
		result = ScreenOrientation.AutoRotation;
		#endif

        // Unkownならピクセル数から判断
        if (result == (ScreenOrientation)1	// Unknown(Obsolete)
		|| result == ScreenOrientation.AutoRotation)
        {
            if (Screen.width < Screen.height)
            {
                result = ScreenOrientation.Portrait;
            }
            else
            {
                result = ScreenOrientation.LandscapeLeft;
            }
        }

        return result;
    }

	// 画面の自動回転のON, OFF
	public static void SetAutoRotation(bool isOn)
	{
		if (isOn)
		{
			// 自動回転をONとする
			Screen.orientation = ScreenOrientation.AutoRotation;
		}
		else
		{
			// 向きを固定する
			ScreenOrientation currentOrientation = MyUtil.GetScreenOrientation();
			switch(currentOrientation)
			{
				// 横向き
				case ScreenOrientation.LandscapeLeft:
					Screen.orientation = ScreenOrientation.LandscapeLeft;
		            break;
				case ScreenOrientation.LandscapeRight:
					Screen.orientation = ScreenOrientation.LandscapeRight;
		            break;
				// 縦向き
				case ScreenOrientation.Portrait:
					Screen.orientation = ScreenOrientation.Portrait;
		            break;
				case ScreenOrientation.PortraitUpsideDown:
					Screen.orientation = ScreenOrientation.PortraitUpsideDown;
					break;
			}
		}
	}

    //-----------------------
	// UI要素のCanvas中心を起点とした座標位置を取得
	public static Vector2 GetUIPosition(GameObject uiElement, GameObject canvas)
	{
		return GetUIPosition(uiElement.transform, canvas.transform);
	}

	// UI要素のCanvas中心を起点とした座標位置を取得
	public static Vector2 GetUIPosition(Transform uiElement, Transform canvas)
	{
		return uiElement.position - canvas.position;
	}

	// UI要素のCanvas中心を起点とした座標位置セット
	public static void SetUIPosition(GameObject uiElement, Vector2 pos)
	{
		uiElement.GetComponent<RectTransform>().anchoredPosition = pos;
	}

	// UI要素のCanvas中心を起点とした座標位置セット
	public static void SetUIPosition(Transform uiElement, Vector2 pos)
	{
		uiElement.GetComponent<RectTransform>().anchoredPosition = pos;
	}

    //-----------------------
	// 遅延実行関連

	// /// <summary>
	// /// 渡された処理を指定時間後に実行する（要 StartCoroutine で呼び出し）
	// /// </summary>
	// /// <param name="waitTime">遅延時間[ミリ秒]</param>
	// /// <param name="action">実行したい処理</param>
	// /// <returns></returns>
	// public static IEnumerator DelayMethod(float waitTime, Action action)
	// {
	// 	yield return new WaitForSeconds(waitTime);
	// 	action();
	// }

	/// <summary>
	/// 渡された処理を指定時間後に実行する（StartCoroutine なし）
	/// </summary>
	/// <param name="waitTime">遅延時間[ミリ秒]</param>
	/// <param name="action">実行したい処理</param>
	/// <returns></returns>
	public static void DelayCall(float waitTime, Action action)
	{
		DOVirtual.DelayedCall(waitTime, ()=>action());
	}


	//-----------------------
	// 座標操作(Position)関連
	
	/// 座標(X)にセット
	public static void SetPosX (Transform transform, float value)
	{
		Vector3 pos = transform.position;
		pos.x = value;
		transform.position = pos;
	}

	/// 座標(Y)にセット
	public static void SetPosY (Transform transform, float value)
	{
		Vector3 pos = transform.position;
		pos.y = value;
		transform.position = pos;
	}

	/// 座標(X)にセット
	public static void SetLocalPosX (Transform transform, float value)
	{
		Vector3 pos = transform.localPosition;
		pos.x = value;
		transform.localPosition = pos;
	}

	/// 座標(Y)にセット
	public static void SetLocalPosY (Transform transform, float value)
	{
		Vector3 pos = transform.localPosition;
		pos.y = value;
		transform.localPosition = pos;
	}

	/// RectTransformのanchoredPositionに座標(X)(Y)をセット
	public static void SetAnchoredPosition (GameObject obj, float x, float y)
	{
		RectTransform rectTransform = obj.GetComponent<RectTransform>();
		if (rectTransform == null)
			return;
		rectTransform.anchoredPosition = new Vector2(x, y);
	}

	/// RectTransformのanchoredPositionに座標(X)をセット
	public static void SetAnchoredPosX (GameObject obj, float x)
	{
		RectTransform rectTransform = obj.GetComponent<RectTransform>();
		if (rectTransform == null)
			return;

		rectTransform.anchoredPosition = new Vector2(x, rectTransform.anchoredPosition.y);
	}

	/// RectTransformのanchoredPositionに座標(X)をセット
	public static void SetAnchoredPosY (GameObject obj, float y)
	{
		RectTransform rectTransform = obj.GetComponent<RectTransform>();
		if (rectTransform == null)
			return;

		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
	}

	///-----------------------
	// 大きさ操作(Scale)関連
	
	/// Scale(X)にセット
	public static void SetScaleX (Transform transform, float value)
	{
		Vector3 scale = transform.localScale;
		scale.x = value;
		transform.localScale = scale;
	}

	/// Scale(Y)にセット
	public static void SetScaleY (Transform transform, float value)
	{
		Vector3 scale = transform.localScale;
		scale.y = value;
		transform.localScale = scale;
	}

	/// Scale(X, Y)にセット
	public static void SetScaleXY (Transform transform, float value)
	{
		Vector3 scale = transform.localScale;
		scale.x = value;
		scale.y = value;
		transform.localScale = scale;
	}

	/// Scale(X, Y)にセット
	public static void SetScaleXY (RectTransform rectTransform, float value)
	{
		Vector3 scale = rectTransform.localScale;
		scale.x = value;
		scale.y = value;
		rectTransform.localScale = scale;
	}

	//-----------------------
	// 回転（rotation） 関連

	// 回転角度を指定する（即座にその向きにする。主にカメラなどで使用）
	public static void SetLookAt(Transform transform, Transform target)
	{
		// 実はLookAtには第２引数があり、デフォルトでVector3 worldUp= Vector3.upとなっている
		transform.LookAt(target);
	}

	// 回転角度を指定する（即座にその向きにする）
	// 徐々に回転させたい場合には、第２引数にTime.deltaTimeをかける
	public static void SetRotate(Transform transform, Vector3 eulerAngles)
	{
		// transform.rotation = Quaternion.Euler(eulerAngles);
		transform.Rotate(eulerAngles, Space.World);
	}

	// 徐々に回転させる （主に Update()内から呼び出し）
	public static void RotateGradually(Transform transform, Vector3 eulerAnglePerSec, bool isPlus)
	{
        transform.Rotate(eulerAnglePerSec * Time.deltaTime * (isPlus ? 1 : -1), Space.World);
	}

	// 指定した軸の回転方向で回転させる(axisは2Dであれば、Vector3.forwardなど)
	public static void RotateAxis(Transform transform, Vector3 axis, float angle)
	{
        transform.Rotate(axis, angle);
	}

	// 指定したPivot位置を中心に回転させる
	public static void RotatePivotAxis(Transform transform, Vector3 pivotPoint, Vector3 axis, float angle)
	{
        transform.RotateAround(pivotPoint, axis, angle);
	}

	// 他、以下をもとに適宜追加する
	// https://tama-lab.net/2017/06/unity%E3%81%A7%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%82%92%E5%9B%9E%E8%BB%A2%E3%81%95%E3%81%9B%E3%82%8B%E6%96%B9%E6%B3%95%E3%81%BE%E3%81%A8%E3%82%81/#rotate_specific_angle-immediately

	//-----------------------
	// 色の取得（1マテリアルの1色のみ取得）
	public static Color GetColor(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer> ();

        if (renderer != null && renderer.materials != null) {
            foreach (Material material in renderer.materials) {
                // 色を取得
                return material.color;
            }
            
            // 警告(黄色)のメッセージを出力する
            Debug.LogWarning("要 終了時にmaterialの破棄");
        }
		else
		{
			// ImageやText
			Graphic graphic = obj.GetComponent<Graphic>();
			if (graphic != null)
				return graphic.color;
		}

		return Color.black;
	}

	// 色の変更
	public static void SetColor(GameObject obj, Color color)
    {
		ChangeColor(obj, color);
	}

	// 色の変更
	public static void ChangeColor(GameObject obj, Color color)
    {
        Renderer renderer = obj.GetComponent<Renderer> ();

        if (renderer != null && renderer.materials != null)
		{
			// ☆ ここは、Materialのカラーを変えるため、以下のケースに注意
			// ・複数オブジェで同一のMaterialを使っているケース（Default-Material含む）
			//   ⇢ オブジェごとに呼び出すのではなく、どこか１箇所で１回で良い
			//   ⇢ なお、この場合、materialの破棄も必要になる（新たに複製されているので）
			//   参考: http://nextsystemkinectblog.seesaa.net/article/365538668.html
			// ・Prefabの色を変える場合
			//   ⇢ renderer.material(s)でなく、sharedMaterial(s)に自前アクセス実装する
            foreach (Material material in renderer.materials) {
                // 指定した色に変更
                material.color = color;
            }
            
            // 警告(黄色)のメッセージを出力する
            Debug.LogWarning("要 終了時にmaterialの破棄");

			// そもそもSpriteRendererであれば、spriteRender.colorが使える！
        }
		else
		{
			// ImageやText
			Graphic graphic = obj.GetComponent<Graphic>();
			if (graphic != null)
				graphic.color = color;
		}
	}

	/// アルファ値を設定（TextやImage）
	public static void SetAlpha (Graphic text, float a)
	{
		var c = text.color;
		c.a = a;
		text.color = c;
	}

	/// Spriteのアルファ値を設定（gameobjectにSpriteRendererがあることが必要）
	public static void SetSpriteAlpha (Transform transform, float a)
	{
		SpriteRenderer spRenderer = transform.GetComponent<SpriteRenderer>();
		if (spRenderer != null)
			spRenderer.color = new Color(spRenderer.color.r, spRenderer.color.g, spRenderer.color.b, a);
	}
	public static void SetSpriteAlpha (GameObject gameObject, float a)
	{
		SpriteRenderer spRenderer = gameObject.GetComponent<SpriteRenderer>();
		if (spRenderer != null)
			spRenderer.color = new Color(spRenderer.color.r, spRenderer.color.g, spRenderer.color.b, a);
	}

	/// アルファ値を徐々に変更（TextやImage）
	public static void ToAlpha (Graphic graphicObj, float toAlpha, float time)
	{
		// 引数は、getterのメソッド（ラムダ式）, setterのメソッド（ラムダ式）, 終了値、期間
		DOTween.ToAlpha(() => graphicObj.color, a => graphicObj.color = a, toAlpha, time);
	}

	/// Spriteを設定（gameobjectにSpriteRendererがあることが必要）
	public static void SetSprite (Transform transform, Sprite sprite)
	{
		SpriteRenderer spRenderer = transform.GetComponent<SpriteRenderer>();
		if (spRenderer != null)
			spRenderer.sprite = sprite;
		else
		{
			Image image = transform.GetComponent<Image>();
			if (image != null)
				image.sprite = sprite;
		}
	}

	/// Spriteを設定（gameobjectにSpriteRendererがあることが必要）
	public static void SetSprite (GameObject gameObject, Sprite sprite)
	{
		if (gameObject == null)
			return;

		SpriteRenderer spRenderer = gameObject.GetComponent<SpriteRenderer>();
		if (spRenderer != null)
			spRenderer.sprite = sprite;
		else
		{
			Image image = gameObject.GetComponent<Image>();
			if (image != null)
				image.sprite = sprite;
		}
	}

	// マテリアルの変更（MeshRendererがあれば良い。マテリアル１つ目にセット）
	public static void SetMaterial(GameObject obj, Material material)
    {
        Renderer renderer = obj.GetComponent<Renderer> ();

        if (renderer != null) {
			renderer.material = material;
        }

		// なお、materialの他に、sharedMaterialというのものもある。
		// sharedMaterialは「大元のマテリアル」で、生成前から操作可能
		// 一方、material(Instance)は「生成後のマテリアル」で、生成されないと操作出来ない
	}

	//-----------------------
	// 動きを止める
	public static void StopMove(GameObject obj)
    {
        Rigidbody rigidbody = obj.GetComponent<Rigidbody> ();

        if (rigidbody != null) {
			StopMove(rigidbody);
        }

        Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D> ();

        if (rigidbody2D != null) {
			StopMove(rigidbody2D);
        }

	}

	// 動きを止める
	public static void StopMove(Rigidbody rigidbody)
    {
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}

	public static void StopMove(Rigidbody2D rigidbody)
    {
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0f;
	}

	//-----------------------
	// 2Dで２つのオブジェクトの距離を求める
	public static float GetDistance(GameObject obj1, GameObject obj2)
    {
		return Vector2.Distance (obj1.transform.position, obj2.transform.position);
	}

	// WorldPositionに対応した、UI上のポジションを求める（この戻り値をセットするUI要素は、左下アンカーにしておく）
	public static Vector2 GetUIPosition(Vector2 worldPos, float canvasWidth, float canvasHeight)
    {
		Vector2 posViewport = Camera.main.WorldToViewportPoint(worldPos);
		return new Vector2(posViewport.x * canvasWidth, posViewport.y * canvasHeight);
	}

	//-----------------------
	// 何かキーが押されたか、またはタッチされたか (update内で呼ばれることを想定)
	public static bool isAnyKey()
	{
		// // 何かキーが押されるかタップをされたらGameOverなどに移行する
		// bool isKeyDown = false;

#if UNITY_SWITCH && !UNITY_EDITOR
		if (HidMultiController.IsAnyKey())
			return true;

#endif
		if (Input.anyKeyDown)
			return true;

		// （以下はなくてもOK。Input.anyKeyDownで、マウスクリック・タッチも取得している）
		// if (Application.isEditor)
		// {
		// 	// Unity製作時マウス用
		// 	// タッチされたとき
		// 	if (Input.GetMouseButtonDown(0))
		// 		isKeyDown = true;
		// }
		// else
		// {
		// 	// Release時マルチタッチ用
		// 	foreach (Touch touch in Input.touches)
		// 	{
		// 		if (touch.phase == TouchPhase.Began)
		// 		{
		// 			isKeyDown = true;
		// 			break;
		// 		}
		// 	}
		// }

		return false;
	}

	// 通常、Update()内で呼ぶこと前提。マウスクリック対応用
	// 第２引数は、ターゲットとカメラとの距離を渡す模様。
	public static Vector3 MousePosToWorldPos(float distanceZ = 10f)
	{	
		return MousePosToWorldPos(Input.mousePosition, distanceZ);
	}

	public static Vector3 MousePosToWorldPos(Vector3 mousePosition, float distanceZ = 10f)
	{	
		// Vector3でマウス位置座標を取得する
		Vector3 position = mousePosition;
		// Z軸修正
		position.z = distanceZ;
		// マウス位置座標をスクリーン座標からワールド座標に変換する
		return Camera.main.ScreenToWorldPoint(position);
		// // ワールド座標に変換されたマウス座標を代入
		// gameObject.transform.position = screenToWorldPointPosition;
	}

	public static Vector3 MousePosToViewportPoint(float posZ = 10f)
	{	
		return MousePosToViewportPoint(Input.mousePosition, posZ);
	}

	// 通常、Update()内で呼ぶこと前提。マウスクリック対応用
	public static Vector3 MousePosToViewportPoint(Vector3 mousePosition, float posZ = 10f)
	{	
		// Vector3でマウス位置座標を取得する
		Vector3 position = mousePosition;
		// Z軸修正
		position.z = posZ;
		// マウス位置座標をスクリーン座標からViewport座標(左下が0, 0、右上が1, 1)に変換する
		return Camera.main.ScreenToViewportPoint(position);
	}
	
	//-----------------------
	// フォーカスのセット（Selectableコンポーネントを持つか、buttonやSliderなどであることが必要）
	// 参考: http://westhillapps.blog.jp/archives/43683528.html
	public static void FocusSelect(GameObject gameObject)
	{
		Selectable selectable = gameObject.GetComponent<Selectable>();
		if (selectable != null)
		{
			selectable.Select();

			// Select時のイベントがうまく呼ばれないケースは以下を呼ぶと良いらしい（今はコメント）
			// selectable.OnSelect(null);
		}
	}

	// UIで現在選択されているオブジェクトを取得
	// (uGUIでEventSystemがあること前提。戻り値は、Selectableを持つか、Button・Sliderなど)
	// 参考: http://kurora-shumpei.hatenablog.com/entry/2018/09/01/Unity%E3%81%A7%E9%81%B8%E6%8A%9E%E3%81%95%E3%82%8C%E3%81%A6%E3%81%84%E3%82%8B%E3%83%9C%E3%82%BF%E3%83%B3%E3%81%AE%E3%83%86%E3%82%AD%E3%82%B9%E3%83%88%E3%82%92%E7%82%B9%E6%BB%85_%2B_%E7%B0%A1
	public static GameObject GetSelectedGameObject()
	{
		if (EventSystem.current == null)
			return null;

		return EventSystem.current.currentSelectedGameObject;
	}

	// UIで現在選択されているかどうか
	public static bool IsSelected(GameObject gameObject)
	{
		if (EventSystem.current == null)
			return false;

		GameObject selected = EventSystem.current.currentSelectedGameObject;
		if (selected == null)
			return false;
		return gameObject.GetInstanceID() == selected.GetInstanceID();
	}

	//-----------------------
	// ネットワーク接続可能か
	public static bool IsExistNetwork()
	{
		return (Application.internetReachability != NetworkReachability.NotReachable);
	}

	//-----------------------
	// longを上位と下位の2つのintに分解し、格納する
	public static void LongToInt(long longValue, out int intHigh, out int intLow)
	{
		// https://www.atmarkit.co.jp/ait/articles/0307/04/news005.html
		int intSize = sizeof(int);

		// longをバイト配列に変換
		byte[] byteArray = BitConverter.GetBytes(longValue);

		// // int用のバイト配列
		// byte[] byteHigh = new byte[intSize];
		// byte[] byteLow = new byte[intSize];

		// for (int i = 0; i < byteArray.Length; i++)
		// {
		// 	if (i < intSize)
		// 		byteHigh[i] = byteArray[i];
		// 	else
		// 		byteHigh[i - intSize] = byteArray[i];
		// }

		intHigh = BitConverter.ToInt32(byteArray, 0);
		intLow = BitConverter.ToInt32(byteArray, intSize);
	}

	// 2つのintをlongに合成する
	public static long IntToLong(int intHigh, int intLow)
	{
		int intSize = sizeof(int);

		byte[] byteHigh = BitConverter.GetBytes(intHigh);
		byte[] byteLow = BitConverter.GetBytes(intLow);

		byte[] byteLong = new byte[sizeof(long)];

		System.Array.Copy(byteHigh, byteLong, intSize);
		System.Array.Copy(byteLow, 0, byteLong, intSize, intSize);

		return BitConverter.ToInt64(byteLong, 0);
	}

	//-----------------------
	// // ■ウオリスDX固有
	// // ボタン押下時共通処理
	// public static bool OnButton(GameObject buttonObj, ref bool isTransition)
	// {
	// 	Button button = buttonObj.GetComponent<Button>();
	// 	return SubOnButton(button, buttonObj.GetComponent<FocusAnimator>(), ref isTransition);
	// }

	// public static bool OnButton(Button button, ref bool isTransition)
	// {
	// 	return SubOnButton(button, button.GetComponent<FocusAnimator>(), ref isTransition);
	// }

	// private static bool SubOnButton(Button button, FocusAnimator focusAnimator, ref bool isTransition)
	// {
	// 	// （ボタンタッチ時に他のボタンのフォーカスを外すため）
	// 	if (button != null)
	// 		button.Select();

	// 	// ボタンシェイク
	// 	if (focusAnimator != null)
	// 		focusAnimator.ShakeButton();

	// 	if (isTransition)
	// 		return false;	// すでにTransition中の場合、処理は進ませない

	// 	isTransition = true;

	// 	return true;	// 処理進む
	// }

	//-----------------------
	// ■ウオリスDX固有
	// データセーブ表示処理
	// public static void ShowSaveIcon(float waitSecond = 1f)
	// public static void ShowSaveIcon()
	// {
    //     // 「データ保存中」の表示開始
	// 	GameObject saveIconObj = GameObject.FindWithTag("SaveIcon");
	// 	// saveIconObj.GetComponent<SaveIconController>().SetIconOn(waitSecond);
	// 	// saveIconObj.GetComponent<SaveIconController>().SetIconOn(1f);
	// }

	// // シーン切替時 波紋エフェクト
	// public static void WaveEffect()
	// {
	// 	// mainCameraに、RippleEffect、 RippleGeneratorを付けて、エフェクトを発動

	// 	// 波紋エフェクト
	// 	if (PlayData.IsWaveEffect())
	// 	{
	// 		// Camera mainCamera = Camera.main;

	// 		// WaterRippleForScreens.RippleEffect rippleEffect = mainCamera.GetComponent<WaterRippleForScreens.RippleEffect>();
	// 		// if (rippleEffect == null)
	// 		// 	return;
	// 		// 	// rippleEffect = mainCamera.gameObject.AddComponent<RippleEffect>();
	// 		// rippleEffect.waveExternalRadio = 0.2f;
	// 		// rippleEffect.waveScale = 8f;

	// 		// rippleEffect.enabled = true;

	// 		// WaterRippleForScreens.RippleGenerator rippleGenerator = mainCamera.GetComponent<WaterRippleForScreens.RippleGenerator>();
	// 		// if (rippleGenerator == null)
	// 		// 	return;
	// 		// 	// rippleGenerator = mainCamera.gameObject.AddComponent<RippleGenerator>();
	// 		// rippleGenerator.timeBetweenRippleMedian = 0.75f;
	// 		// rippleGenerator.enabled = true;

	// 		Camera mainCamera = Camera.main;

	// 		RippleEffect rippleEffect = mainCamera.GetComponent<RippleEffect>();
	// 		if (rippleEffect == null)
	// 			return;

	// 		rippleEffect.enabled = true;
	// 	}
	// }

	// // 波紋エフェクト終了
	// public static void WaveEffectEnd()
	// {
	// 	// 波紋エフェクト終了
	// 	// if (PlayData.IsWaveEffect())
	// 	{
	// 		// Camera mainCamera = Camera.main;

	// 		// WaterRippleForScreens.RippleEffect rippleEffect = mainCamera.GetComponent<WaterRippleForScreens.RippleEffect>();
	// 		// if (rippleEffect == null)
	// 		// 	return;

	// 		// rippleEffect.enabled = false;

	// 		// WaterRippleForScreens.RippleGenerator rippleGenerator = mainCamera.GetComponent<WaterRippleForScreens.RippleGenerator>();
	// 		// if (rippleGenerator == null)
	// 		// 	return;

	// 		// rippleGenerator.enabled = false;

	// 		Camera mainCamera = Camera.main;

	// 		RippleEffect rippleEffect = mainCamera.GetComponent<RippleEffect>();
	// 		if (rippleEffect == null)
	// 			return;

	// 		rippleEffect.enabled = false;
	// 	}
	// }

	//-----------------------
	// // ■ InControl用
	// public static InputControl GetInputControl(InputDevice inputDevice, KeyCode keyCode)	
	// {
	// 	InputControlType inputControlType = InputControlType.None;

	// 	if (inputDevice.IsKnown)
	// 	{
	// 		switch(keyCode)
	// 		{
	// 			case KeyCode.LeftArrow:	inputControlType = InputControlType.DPadLeft; break;
	// 			case KeyCode.RightArrow: inputControlType = InputControlType.DPadRight; break;
	// 			case KeyCode.UpArrow:	inputControlType = InputControlType.DPadUp; break;
	// 			case KeyCode.DownArrow: inputControlType = InputControlType.DPadDown; break;
	// 			case KeyCode.C: inputControlType = InputControlType.Action2; break;
	// 			case KeyCode.B: inputControlType = InputControlType.Action1; break;
	// 			case KeyCode.Z: inputControlType = InputControlType.Action3; break;
	// 			case KeyCode.X: inputControlType = InputControlType.Action4; break;
	// 		}
	// 	}
	// 	else
	// 	{
	// 		var nativeDevice = inputDevice as NativeInputDevice;
	// 		if (nativeDevice != null)
	// 		{
	// 			switch(keyCode)
	// 			{
	// 				case KeyCode.LeftArrow:	inputControlType = InputControlType.Button14; break;
	// 				case KeyCode.RightArrow: inputControlType = InputControlType.Button15; break;
	// 				case KeyCode.UpArrow:	inputControlType = InputControlType.Button12; break;
	// 				case KeyCode.DownArrow: inputControlType = InputControlType.Button13; break;
	// 				case KeyCode.C: inputControlType = InputControlType.Button3; break;
	// 				case KeyCode.B: inputControlType = InputControlType.Button2; break;
	// 				case KeyCode.Z: inputControlType = InputControlType.Button0; break;
	// 				case KeyCode.X: inputControlType = InputControlType.Button1; break;
	// 			}
	// 		}
	// 	}

	// 	if (inputControlType == InputControlType.None)
	// 		return InputControl.Null;
	// 	else
	// 		return inputDevice[inputControlType];
	// }

	// // メニューUI操作 (現在のフォーカス位置を取得し、次のコントロールにフォーカスを移す)
    // public static void MenuOperate()
    // {
    //     GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
    //     if (currentSelected == null)
    //         return;

    //     Selectable currentSelectable = currentSelected.GetComponent<Selectable>();
    //     if (currentSelectable == null)
    //         return;

    //     // （以下は、currentSelectableがnullでないこと前提）

    //     // 上下左右のキー操作を取得
	// 	bool inputLeftDown = GetWasPressed(KeyCode.LeftArrow);
	// 	bool inputRightDown = GetWasPressed(KeyCode.RightArrow);
	// 	bool inputDownDown = GetWasPressed(KeyCode.DownArrow);
	// 	bool inputUpDown = GetWasPressed(KeyCode.UpArrow);

    //     // 上下左右で操作した場合
    //     if (inputLeftDown || inputRightDown || inputDownDown ||inputUpDown)
    //     {
    //         // キー操作による移動先の格納用
    //         Selectable newSel = null;

    //         Navigation navigation = currentSelectable.navigation;
    //         if (navigation.mode == Navigation.Mode.Explicit)
    //         {
    //             if (inputLeftDown)
    //                 newSel = navigation.selectOnLeft;
    //             if (inputRightDown)
    //                 newSel = navigation.selectOnRight;
    //             if (inputDownDown)
    //                 newSel = navigation.selectOnDown;
    //             if (inputUpDown)
    //                 newSel = navigation.selectOnUp;
    //         }
    //         // Explicitでない場合は、以下より取得
    //         else
    //         {
    //             Vector3 moveDir = Vector3.zero;
    //             if (inputLeftDown)
    //                 moveDir = Vector3.left;
    //             if (inputRightDown)
    //                 moveDir = Vector3.right;
    //             if (inputDownDown)
    //                 moveDir = Vector3.down;
    //             if (inputUpDown)
    //                 moveDir = Vector3.up;

    //             // キー操作による移動先を取得
    //             newSel = currentSelectable.FindSelectable(moveDir);
    //         }

    //         // フォーカスを移動
    //         if (newSel != null)
    //             newSel.Select();
    //     }

    //     // 決定ボタンを押された時
    //     if (KeyWasPressed(KeyCode.C))
    //     {
    //         Button currentButton = currentSelected.GetComponent<Button>();
    //         if (currentButton.onClick != null)
    //             currentButton.onClick.Invoke();

    //         // ☆ ⇢ 決定ボタン以外でも、ショートカット的なボタンで決定するケースがあるため、
    //         // FocusAnimatorによるボタンシェイクは、各ボタン押下メソッドから呼び出すようにする
    //         // ここでは音のみ

    //         FocusAnimator focusAnimator = currentSelected.GetComponent<FocusAnimator>();
    //         if (focusAnimator != null)
    //         {
    //             // focusAnimator.ShakeButton();
    //             focusAnimator.DicisionSound();
    //         }
    //     }
    // }

	// public static bool GetWasPressed(KeyCode keycode)
	// {
	// 	float analogThreshold = 0.5f;
	// 	bool wasPressed = false;

	// 	foreach (var inputDevice in InputManager.Devices)
	// 	{
	// 		InputControl inputControl = MyUtil.GetInputControl(inputDevice, keycode);

	// 		wasPressed = inputControl.WasPressed;
	// 		if (wasPressed)
	// 			break;

	// 		InputControl analogIC = inputDevice[InputControlType.Analog1];
	// 		if (inputControl.Target == InputControlType.Button12)
	// 		{
	// 			wasPressed = (analogIC.Value <= -analogThreshold && analogIC.LastValue > -analogThreshold);
	// 		}
	// 		else if (inputControl.Target == InputControlType.Button13)
	// 		{
	// 			wasPressed = (analogIC.Value >= analogThreshold && analogIC.LastValue < analogThreshold);
	// 		}

	// 		if (wasPressed)
	// 			break;
	// 	}

	// 	return wasPressed;
	// }

	// public static bool KeyWasPressed(KeyCode keycode)
	// {
	// 	bool wasPressed = false;

	// 	foreach (var inputDevice in InputManager.Devices)
	// 	{
	// 		InputControl inputControl = MyUtil.GetInputControl(inputDevice, keycode);

	// 		wasPressed = inputControl.WasPressed;
	// 		if (wasPressed)
	// 			break;
	// 	}

	// 	return wasPressed;
	// }
}
