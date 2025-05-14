using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Nest.cs
public class Nest : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Egg"))
        {
            gameManager.OnEggScored();
            Destroy(other.gameObject); // 점수 후 제거
        }
    }

    public IEnumerator FadeIn()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;
        color.a = 0f;
        sr.color = color;

        for (float t = 0f; t < 1f; t += Time.deltaTime)
        {
            sr.color = new Color(color.r, color.g, color.b, t);
            yield return null;
        }

        sr.color = new Color(color.r, color.g, color.b, 1f);
    }

}
