using TMPro;
using UnityEngine;

public class PolaroidCounterUI2D : MonoBehaviour
{
    [SerializeField] private TMP_Text polaroidText;
  private int count;
    public int Count => count;

    private void Awake()
    {
        RefreshUI();
    }

    public void Add(int amount)
    {
        count += amount;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (polaroidText != null)
            polaroidText.text = count.ToString();
    }
}