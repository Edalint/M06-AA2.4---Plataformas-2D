using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 1;
    public int currentHealth = 1;

    [Header("Damage Flash")]
    public Color damageTint = new Color(1f, 0.4f, 0.4f, 1f);
    public float flashDuration = 0.15f;

    SpriteRenderer[] renderers;
    Color[] baseColors;
    Coroutine flashRoutine;

    void Awake()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Soporta enemigo con 1 SpriteRenderer o varios (hijos)
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        baseColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            baseColors[i] = renderers[i].color;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        FlashRed();

        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    void FlashRed()
    {
        if (renderers == null || renderers.Length == 0) return;

        // Si ya estaba flasheando, reiniciamos el flash
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Poner rojo
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = damageTint;

        yield return new WaitForSeconds(flashDuration);

        // Volver al color original
        for (int i = 0; i < renderers.Length; i++)
            if (renderers[i] != null) renderers[i].color = baseColors[i];

        flashRoutine = null;
    }
}