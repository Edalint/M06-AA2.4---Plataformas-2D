using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class MovingPlatform2D : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float arriveDistance = 0.05f;

    [Header("Passenger detection")]
    public float topTolerance = 0.05f; // cuánto “por encima” debe estar el player para considerarlo encima

    Rigidbody2D rb;
    Vector2 target;
    bool goingToB = true;

    readonly HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (pointA != null && pointB != null)
            target = pointB.position;
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        Vector2 oldPos = rb.position;

        // mover plataforma
        Vector2 newPos = Vector2.MoveTowards(oldPos, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // delta de movimiento
        Vector2 delta = newPos - oldPos;

        // arrastrar pasajeros
        if (delta != Vector2.zero)
        {
            foreach (var p in passengers)
            {
                if (p == null) continue;
                p.MovePosition(p.position + delta);
            }
        }

        // cambiar destino
        if (Vector2.Distance(newPos, target) <= arriveDistance)
        {
            goingToB = !goingToB;
            target = goingToB ? (Vector2)pointB.position : (Vector2)pointA.position;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Detectar al player por el Rigidbody2D (más fiable que por el collider)
        Rigidbody2D prb = collision.rigidbody;
        if (prb == null) return;
        if (!prb.CompareTag("Player")) return;

        // Comprobar si está encima (centro del player por encima del centro de la plataforma)
        bool playerAbove = prb.position.y > rb.position.y + topTolerance;

        if (playerAbove)
            passengers.Add(prb);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D prb = collision.rigidbody;
        if (prb == null) return;
        if (!prb.CompareTag("Player")) return;

        passengers.Remove(prb);
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.1f);
            Gizmos.DrawWireSphere(pointB.position, 0.1f);
        }
    }
}