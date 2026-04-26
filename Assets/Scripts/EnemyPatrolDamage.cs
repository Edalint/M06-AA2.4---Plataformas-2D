using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class EnemyPatrolDamage : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float arriveDistance = 0.05f;

    [Header("Damage")]
    public int damage = 1;
    public HealthSystemUI healthSystem; // arrástralo si quieres (recomendado)

    [Header("Enemy Animations Toggle (E)")]
    public AnimatorOverrideController altAnimations; // Enemy_AltAnimations

    SpriteRenderer sr;
    Rigidbody2D rb;
    Animator anim;

    RuntimeAnimatorController baseController;
    Vector2 target;
    bool goingToB = true;
    bool altMode = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        // Guardamos el controller original
        baseController = anim.runtimeAnimatorController;

        // Por si no lo asignas en Inspector
        if (healthSystem == null)
            healthSystem = FindObjectOfType<HealthSystemUI>();

        if (pointA != null && pointB != null)
            target = pointB.position;
    }

    void Update()
    {
        // Toggle animaciones con E (se sincroniza con tu E global)
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            altMode = !altMode;

            if (altMode && altAnimations != null)
                anim.runtimeAnimatorController = altAnimations;
            else
                anim.runtimeAnimatorController = baseController;
        }
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        Vector2 pos = rb.position;
        Vector2 newPos = Vector2.MoveTowards(pos, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(newPos, target) <= arriveDistance)
        {
            goingToB = !goingToB;
            target = goingToB ? (Vector2)pointB.position : (Vector2)pointA.position;
        }

        // Flip según dirección (para que mire al lado correcto)
        float dirX = target.x - newPos.x;
        if (Mathf.Abs(dirX) > 0.001f)
            sr.flipX = dirX < 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (healthSystem != null)
            healthSystem.RemoveHealth(damage); // tu cooldown de 3s evita restar repetido
    }
}