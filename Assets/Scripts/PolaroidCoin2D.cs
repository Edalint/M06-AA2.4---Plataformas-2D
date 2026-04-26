using UnityEngine;

public class PolaroidCoin2D : MonoBehaviour
{
    [SerializeField] private PolaroidCounterUI2D counter;
    [SerializeField] private int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger con: " + other.name);

        if (!other.CompareTag("Player")) return;

        if (counter == null)
        {
            Debug.LogError("Counter no asignado en la moneda (campo counter).");
            return;
        }

        counter.Add(value);
        Destroy(gameObject);
    }
}