using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 startDrag, endDrag;

    public float throwForce = 5f;
    public float clickRadius = 2.0f;

    public LineRenderer lineRenderer;
    public int maxBounces = 2;
    public int segmentsPerBounce = 30;
    public float segmentTime = 0.05f; // 시간 기반으로 변경
    public LayerMask bounceMask;

    // 바운스
    public int bounceCount = 0;
    public bool touchedNest = false;

    bool hasThrown = false;
    bool canDrag = false;  // 드래그 허용 여부



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasThrown)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // 클릭 위치가 달걀 근처인지 체크
            if (Vector3.Distance(mouseWorldPos, transform.position) <= clickRadius)
            {
                canDrag = true;
                startDrag = mouseWorldPos;
            }
            else
            {
                canDrag = false;
            }
        }

        if (Input.GetMouseButton(0) && !hasThrown && canDrag)
        {
            Vector2 dragPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 force = (startDrag - dragPos) * throwForce;
            DrawBouncedTrajectory(transform.position, force);
        }

        if (Input.GetMouseButtonUp(0) && !hasThrown && canDrag)
        {
            endDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 force = (startDrag - endDrag) * throwForce;

            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.AddForce(force, ForceMode2D.Impulse);
            hasThrown = true;

            if (lineRenderer != null)
                lineRenderer.positionCount = 0;

            canDrag = false;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            bounceCount++;
            rb.velocity *= 0.8f;
            GameManager.Instance.audioSource.PlayOneShot(GameManager.Instance.bounceClip);
        }
        else if (collision.collider.CompareTag("Ground"))
        {
            GameManager.Instance.OnGameOver();
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Nest"))
        {
            touchedNest = true;
            rb.velocity *= 0.8f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Nest") && hasThrown)
        {
            // GameManager.Instance.OnEggScored(bounceCount);
            Destroy(gameObject);
        }
    }

    void DrawBouncedTrajectory(Vector2 startPos, Vector2 force)
    {
        if (lineRenderer == null) return;

        List<Vector3> points = new List<Vector3>();
        Vector2 velocity = force / rb.mass;
        Vector2 gravity = Physics2D.gravity;

        int bounces = 0;
        float time = 0f;
        Vector2 pos = startPos;

        points.Add(pos);

        while (bounces <= maxBounces && points.Count < segmentsPerBounce * (maxBounces + 1))
        {
            time += segmentTime;
            Vector2 nextPos = startPos + velocity * time + 0.5f * gravity * time * time;

            Vector2 segmentDir = nextPos - pos;
            float segmentDist = segmentDir.magnitude;

            RaycastHit2D hit = Physics2D.Raycast(pos, segmentDir.normalized, segmentDist, bounceMask);

            if (hit.collider != null)
            {
                points.Add(hit.point);

                bounces++;
                startPos = hit.point;
                velocity = Vector2.Reflect(velocity + gravity * time, hit.normal) * 0.8f; // 반사시 감속
                time = 0f;
                pos = hit.point + velocity.normalized * 0.01f;
                points.Add(pos);
            }
            else
            {
                points.Add(nextPos);
                pos = nextPos;
            }
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
