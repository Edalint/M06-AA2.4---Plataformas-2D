using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HealthSystemUI : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    public int currentHealth = 5;

    [Header("UI")]
    public Image[] hearts;
    public Sprite heartFullSprite;
    public Sprite heartEmptySprite;

    [Header("Player Feedback (Tint)")]
    public SpriteRenderer playerRenderer; // Arrastra aquí el SpriteRenderer del Player

    // “Un poco rojo/verde” (ajusta a tu gusto)
    public Color damageTint = new Color(1f, 0.6f, 0.6f, 1f);
    public float damageTintDuration = 3f;

    public Color healTint = new Color(0.6f, 1f, 0.6f, 1f);
    public float healTintDuration = 1f;

    // ✅ NUEVO (Paso 3): Respawn en checkpoint al llegar a 0 vida
    public CheckpointManager checkpointManager;
    public bool restoreFullHealthOnRespawn = true;

    // Internos
    Color baseColor = Color.white;
    float damageUntilTime = 0f;
    float healUntilTime = 0f;
    float nextDamageAllowedTime = 0f;

    void Start()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        RefreshHearts();

        if (playerRenderer != null)
            baseColor = playerRenderer.color;
    }

    void Update()
    {
        // Tecla G: sumar vida (sin cooldown)
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
            AddHealth(1);

        // Tecla H: restar vida (con cooldown de 3s)
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
            RemoveHealth(3);

        UpdatePlayerTint();
    }

    public void AddHealth(int amount)
    {
        int before = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        if (currentHealth > before) // solo si realmente sube
        {
            healUntilTime = Time.time + healTintDuration;
            RefreshHearts();
        }
    }

    public void RemoveHealth(int amount)
    {
        // Cooldown activo => no se resta vida, aunque pulses H
        if (Time.time < nextDamageAllowedTime)
            return;

        int before = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth < before) // solo si realmente baja
        {
            // Activamos efecto rojo + cooldown por 3 segundos
            damageUntilTime = Time.time + damageTintDuration;
            nextDamageAllowedTime = damageUntilTime;

            RefreshHearts();

            // ✅ NUEVO (Paso 3): si llega a 0, respawn al último checkpoint (o spawn inicial)
            if (currentHealth == 0)
            {
                if (checkpointManager != null)
                    checkpointManager.Respawn();

                if (restoreFullHealthOnRespawn)
                {
                    currentHealth = maxHealth;
                    RefreshHearts();
                }
            }
        }
    }

    void RefreshHearts()
    {
        if (hearts == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;
            hearts[i].sprite = (i < currentHealth) ? heartFullSprite : heartEmptySprite;
        }
    }

    void UpdatePlayerTint()
    {
        if (playerRenderer == null) return;

        // Prioridad: verde (heal) sobre rojo (damage)
        if (Time.time < healUntilTime)
        {
            playerRenderer.color = healTint;
        }
        else if (Time.time < damageUntilTime)
        {
            playerRenderer.color = damageTint;
        }
        else
        {
            playerRenderer.color = baseColor;
        }
    }
}