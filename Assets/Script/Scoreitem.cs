using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreitem : MonoBehaviour
{
    [Header("加算するスコア")] public int myScore;
    [Header("プレイヤーの判定")] public PlayerTriggerCheck playerCheck;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(GManager.instance != null)
        {
            GManager.instance.score += myScore;
            Destroy(this.gameObject);
        }
    }
}
