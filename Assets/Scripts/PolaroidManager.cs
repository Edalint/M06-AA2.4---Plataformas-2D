using TMPro;
using UnityEngine;

public class PolaroidManager : MonoBehaviour
{
    public static PolaroidManager Instance { get; private set; }

    [SerializeField] private TMP_Text polaroidText;
    public int Count { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        RefreshUI();
    }

    public void Add(int amount)
    {
        Count += amount;
        RefreshUI();
    }

    public bool Spend(int amount)
    {
        if (Count < amount) return false;
        Count -= amount;
        RefreshUI();
        return true;
    }

    public void RefreshUI()
    {
        if (polaroidText != null)
            polaroidText.text = Count.ToString();
    }

    // Si cambias de escena y el texto TMP es distinto en la nueva escena,
    // llama a esto para reasignarlo (opcional).
    public void SetText(TMP_Text newText)
    {
        polaroidText = newText;
        RefreshUI();
    }
}