using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerJump))]


public class PlayerModeToggle : MonoBehaviour
{

    [Header("Tilemaps (enable one at a time)")]

public GameObject tileMap1;
public GameObject tileMap2;
    [Header("Animations")]
    public AnimatorOverrideController altAnimations; // Player_AltAnimations

    [Header("Body Colliders (enable one at a time)")]
    public Collider2D normalBodyCollider;
    public Collider2D altBodyCollider;

    [Header("Optional: Ground sensor sizes")]
    public bool changeGroundSensorToo = true;
    public float normalGroundRadius = 0.4f;
    public Vector2 normalGroundOffset = new Vector2(0, -0.7f);
    public float altGroundRadius = 0.3f;
    public Vector2 altGroundOffset = new Vector2(0, -0.55f);

    [Header("Jumps per mode")]
    [Min(1)] public int normalMaxJumps = 1;
    [Min(1)] public int altMaxJumps = 2;

    // ✅ NUEVO: UI Sprite Toggle
    [Header("UI Sprite Toggle")]
    public Image uiImage;       // Arrastra aquí el Image del Canvas
    public Sprite uiSprite1;    // Sprite 1
    public Sprite uiSprite2;    // Sprite 2
    public bool setNativeSizeOnSwap = false;

    Animator anim;
    PlayerJump jump;
    CircleCollisionDetector groundDetector;

    RuntimeAnimatorController baseController;
    bool altMode;

    // ✅ AÑADIDO: Exponer el modo actual sin cambiar lógica
    public bool IsAltMode => altMode;

    void Awake()
    {
        anim = GetComponent<Animator>();
        jump = GetComponent<PlayerJump>();
        groundDetector = GetComponent<CircleCollisionDetector>();

        // Guardamos el controller original (IDLE/WALKING/JUMP)
        baseController = anim.runtimeAnimatorController;

        // Por seguridad: si no asignaste el normal collider, intenta pillarlo
        if (normalBodyCollider == null)
            normalBodyCollider = GetComponent<Collider2D>();

        ApplyMode(false);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ApplyMode(!altMode);
        }
    }

    void ApplyMode(bool useAlt)
    {
        altMode = useAlt;

        // 1) Cambiar animaciones
        if (useAlt && altAnimations != null)
            anim.runtimeAnimatorController = altAnimations;
        else
            anim.runtimeAnimatorController = baseController;

        // 2) Cambiar doble salto
        jump.maxJumps = useAlt ? altMaxJumps : normalMaxJumps;

        // 3) Cambiar collider del cuerpo
        if (normalBodyCollider != null) normalBodyCollider.enabled = !useAlt;
        if (altBodyCollider != null) altBodyCollider.enabled = useAlt;

        // 5) Cambiar Tilemaps
if (tileMap1 != null) tileMap1.SetActive(!useAlt);
if (tileMap2 != null) tileMap2.SetActive(useAlt);

        // ✅ NUEVO: Cambiar Sprite en la UI
        if (uiImage != null)
        {
            uiImage.sprite = useAlt ? uiSprite2 : uiSprite1;

            if (setNativeSizeOnSwap)
                uiImage.SetNativeSize();
        }

        // A veces ayuda a que física y gizmos se actualicen inmediato
        Physics2D.SyncTransforms();
    }
}