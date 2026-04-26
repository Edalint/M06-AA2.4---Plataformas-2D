using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image startImage;
    [SerializeField] private Image creditsImage;
    [SerializeField] private Image exitImage;

    [Header("Sprites")]
    [SerializeField] private Sprite startNormal;
    [SerializeField] private Sprite startSelected;

    [SerializeField] private Sprite creditsNormal;
    [SerializeField] private Sprite creditsSelected;

    [SerializeField] private Sprite exitNormal;
    [SerializeField] private Sprite exitSelected;

    [Header("Credits UI")]
    [SerializeField] private GameObject creditsPanel; // Imagen/Panel de créditos

    private int selectedIndex = 0; // 0=Start, 1=Credits, 2=Exit
    private bool showingCredits = false;

    private void Start()
    {
        selectedIndex = 0;

        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        ApplyVisuals();
    }

    private void Update()
    {
        // Si estamos mostrando créditos: cualquier input los cierra
        if (showingCredits)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                HideCredits();
            }
            return;
        }

        // Movimiento solo con W/S
        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedIndex = Mathf.Max(0, selectedIndex - 1);
            ApplyVisuals();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            selectedIndex = Mathf.Min(2, selectedIndex + 1);
            ApplyVisuals();
        }

        // Confirmar con click izquierdo o con E
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))
        {
            ActivateSelected();
        }
    }

    private void ApplyVisuals()
    {
        startImage.sprite = (selectedIndex == 0) ? startSelected : startNormal;
        creditsImage.sprite = (selectedIndex == 1) ? creditsSelected : creditsNormal;
        exitImage.sprite = (selectedIndex == 2) ? exitSelected : exitNormal;
    }

    private void ActivateSelected()
    {
        switch (selectedIndex)
        {
            case 0:
                SceneManager.LoadScene("Animation");
                break;

            case 1:
                ShowCredits();
                break;

            case 2:
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
        }
    }

    private void ShowCredits()
    {
        showingCredits = true;
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void HideCredits()
    {
        showingCredits = false;
        if (creditsPanel != null)
            creditsPanel.SetActive(false);

        // opcional: al volver, re-aplicamos visuals por si acaso
        ApplyVisuals();
    }
}