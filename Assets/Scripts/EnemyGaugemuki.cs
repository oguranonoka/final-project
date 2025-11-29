using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyGaugemuki : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    void Update()
    {
        //EnemyGauge‚ðMain Camera‚ÖŒü‚©‚¹‚é
        canvas.transform.rotation =
            Camera.main.transform.rotation;
    }
}