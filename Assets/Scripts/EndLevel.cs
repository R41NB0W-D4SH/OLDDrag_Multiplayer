using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class EndLevel : NetworkBehaviour
{
    public static bool isOver = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var playerEntered = collision.CompareTag("Player");

        if (playerEntered)
        {
           isOver = true;
        }
    }
}