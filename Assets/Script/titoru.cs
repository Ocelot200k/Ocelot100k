using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titoru : MonoBehaviour
{
    private bool firstPush = false;
    //ボタンを押されたら呼ばれる
   public void PressStart()
    {
        Debug.Log("Press Start!");

        if (!firstPush)
        {
            Debug.Log("Go Next Scene!");

            //ここに次のシーンへ行く命令を描く

            //
            firstPush = true;

        }
    }
}
