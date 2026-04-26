using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Mode requirement")]
    public PlayerModeToggle modeToggle;
    public bool attackOnlyInAltMode = false;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRadius = 0.6f;
    public LayerMask enemyMask;
    public int damage = 1;

    [Header("Animation")]
    public Animator animator;
    public string attackTriggerName = "attack";

    [Header("Optional: follow facing")]
    public SpriteRenderer playerRenderer;
    public Vector2 attackLocalOffset = new Vector2(0.6f, 0f);

    void Awake()
    {
        if (modeToggle == null) modeToggle = GetComponent<PlayerModeToggle>();
        if (playerRenderer == null) playerRenderer = GetComponent<SpriteRenderer>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Update()
    {
        // mover AttackPoint según hacia dónde mira
        if (attackPoint != null && playerRenderer != null)
        {
            float sign = playerRenderer.flipX ? -1f : 1f;
            attackPoint.localPosition = new Vector3(attackLocalOffset.x * sign, attackLocalOffset.y, 0f);
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!CanAttackInThisMode()) return;

            // ✅ Animación
            if (animator != null)
                animator.SetTrigger(attackTriggerName);

            // ✅ Daño (ahora mismo se aplica inmediatamente)
            DoAttack();
        }
    }

    bool CanAttackInThisMode()
    {
        if (modeToggle == null) return true;
        return modeToggle.IsAltMode == attackOnlyInAltMode;
    }

    void DoAttack()
    {
        if (attackPoint == null) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            EnemyHealth eh = hits[i].GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}