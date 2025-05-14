using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 startDrag, endDrag;
    public float throwForce = 5f;
    bool hasThrown = false;

    public GameObject currentEgg;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasThrown)
        {
            startDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && !hasThrown)
        {
            endDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 force = (startDrag - endDrag) * throwForce;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(force, ForceMode2D.Impulse);
            hasThrown = true;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Nest") && hasThrown)
        {
            GameManager.Instance.OnEggScored();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            GameManager.Instance.OnGameOver();
            Destroy(gameObject);
        }
    }
}
