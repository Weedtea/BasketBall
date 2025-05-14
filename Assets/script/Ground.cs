using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameManager gameManager;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Egg"))
        {
            gameManager.OnGameOver();
        }
    }
}
