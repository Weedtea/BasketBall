using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Nest.cs
public class Nest : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Egg"))
        {
            Egg egg = other.GetComponent<Egg>();
            int bounce = egg != null ? egg.bounceCount : 0;
            bool touchedNest = egg != null ? egg.touchedNest : false;

            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.scoreClip);
            GameManager.Instance.OnEggScored(bounce, touchedNest);
            Destroy(other.gameObject);
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

