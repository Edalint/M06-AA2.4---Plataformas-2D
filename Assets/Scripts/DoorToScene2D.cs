using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorToScene2D : MonoBehaviour
{
    [Header("Requisitos")]
    [SerializeField] private PolaroidCounterUI2D counter;
    [SerializeField] private int requiredPolaroids = 3;

    [Header("Escena destino")]
    [SerializeField] private string sceneName;

    [Header("Interacción")]
    [SerializeField] private KeyCode interactKey = KeyCode.Q;

    [Header("Texto de aviso (encima de la puerta)")]
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private float hintDuration = 1.5f;

    private bool playerInRange;
    private bool transitioning;
    private Coroutine hintRoutine;

    private void Awake()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (transitioning) return;

        if (playerInRange && Input.GetKeyDown(interactKey))
            TryEnter();
    }

    // Click izquierdo sobre la puerta (requiere Collider2D en la puerta)
    private void OnMouseDown()
    {
        if (transitioning) return;
        if (!playerInRange) return;

        if (Input.GetMouseButtonDown(0))
            TryEnter();
    }

    private void TryEnter()
    {
        if (counter == null)
        {
            Debug.LogError("DoorToScene2D: Counter no asignado.");
            return;
        }

        if (counter.Count < requiredPolaroids)
        {
            ShowHint($"Necesitas {requiredPolaroids} polaroids");
            return;
        }

        transitioning = true;
        SceneManager.LoadScene(sceneName);
    }

    private void ShowHint(string msg)
    {
        if (hintText == null) return;

        if (hintRoutine != null)
            StopCoroutine(hintRoutine);

        hintRoutine = StartCoroutine(HintRoutine(msg));
    }

    private IEnumerator HintRoutine(string msg)
    {
        hintText.text = msg;
        hintText.gameObject.SetActive(true);

        yield return new WaitForSeconds(hintDuration);

        hintText.gameObject.SetActive(false);
        hintRoutine = null;
    }

    // Llamado por el trigger de rango (objeto hijo con Collider2D IsTrigger)
    public void SetPlayerInRange(bool inRange) => playerInRange = inRange;
}