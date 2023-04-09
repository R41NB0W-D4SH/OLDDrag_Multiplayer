using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RaceTimer : NetworkBehaviour
{
    private bool timer;
    public float timerInput = 0.00f;
    public Text HudtimerMinText;
    public Text HudtimerSecText;

    // Update is called once per frame
    void Update()
    {
        if (timer)
        {
            timerInput += Time.deltaTime;
            HudtimerMinText.text = (Mathf.RoundToInt(timerInput / 60)).ToString();
            HudtimerSecText.text = Mathf.RoundToInt(timerInput).ToString();
        }
    }
}
