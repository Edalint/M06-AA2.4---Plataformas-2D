using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public Sprite checkpointOff;
    public Sprite checkpointOn;

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetActive(false); // por defecto OFF
    }

    public void SetActive(bool active)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.sprite = active ? checkpointOn : checkpointOff;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Avisamos al Player para que guarde este checkpoint como el último
        var manager = other.GetComponent<CheckpointManager>();
        if (manager != null)
            manager.ReachCheckpoint(this);
        else
            SetActive(true); // si no existe manager, al menos se ve ON
    }
}