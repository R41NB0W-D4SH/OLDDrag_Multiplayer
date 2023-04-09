using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Position_Script : NetworkBehaviour
{
    public Transform fin_pos;
    public Transform player;
    public Transform ai;
    public Text current_pos;

    private float finish_pos_y;
    private float player_pos_y;
    private float ai_pos_y;

    void Start()
    {
        finish_pos_y = fin_pos.transform.position.y;
    }

    void Update()
    {
        player_pos_y = player.transform.position.y;
        ai_pos_y = ai.transform.position.y;
        
        if ((finish_pos_y - player_pos_y) < (finish_pos_y - ai_pos_y))
        {
            current_pos.text = (1).ToString();
        }
        else
        {
            current_pos.text = (2).ToString();
        }
    }
}
