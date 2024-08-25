using System.Collections.Generic; // 不要なので削除できます
using UnityEngine;
using System; // Enumを使うために必要

public class SwitchInput : MonoBehaviour
{
    // Unityのフレームごとに呼ばれるメソッド
    void Update()
    {
        // 任意のキーが押されたかをチェックするメソッドを呼び出す
        SwitchControllerAnyKeyDown();
    }

    // 任意のキーが押されたかをチェックし、押された場合にはそのキーをログに出力するメソッド
    private void SwitchControllerAnyKeyDown()
    {
        // 任意のキーが押されたかを確認
        if (Input.anyKeyDown)
        {
            // SwitchController列挙型のすべての値をループする
            foreach (SwitchController code in Enum.GetValues(typeof(SwitchController)))
            {
                // スイッチコントローラーのキーが押されたかを確認し、押されていればログに出力
                if (Input.GetKeyDown((KeyCode)code)) Debug.Log(code);
            }
        }
    }
}

// SwitchControllerという名前の列挙型を定義
// 各列挙値には、スイッチコントローラーの特定のボタンや入力に対応する整数値が割り当てられている
public enum SwitchController
{
    A = 370,
    B = 372,
    X = 371,
    Y = 373,
    UpArrow = 352,
    LeftArrow = 350,
    RightArrow = 353,
    DownArrow = 351,
    LStick = 380,
    RStick = 361,
    L = 364,
    R = 384,
    ZL = 365,
    ZR = 385,
    LeftSL = 375,
    LeftSR = 374,
    RightSL = 355,
    RightSR = 354,
    Minus = 378,
    Plus = 359,
    HOME = 362,
    Capture = 383
}
