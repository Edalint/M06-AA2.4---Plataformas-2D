using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CheckpointManager : MonoBehaviour
{
    public Vector2 initialSpawnPosition { get; private set; }
    public Vector2 lastCheckpointPosition { get; private set; }
    public bool hasCheckpoint { get; private set; }

    Rigidbody2D rb;
    Checkpoint currentCheckpoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        initialSpawnPosition = transform.position;
        lastCheckpointPosition = initialSpawnPosition;
        hasCheckpoint = false;
    }

    public void ReachCheckpoint(Checkpoint checkpoint)
    {
        if (checkpoint == null) return;

        // Apaga el anterior (opcional pero recomendado)
        if (currentCheckpoint != null && currentCheckpoint != checkpoint)
            currentCheckpoint.SetActive(false);

        currentCheckpoint = checkpoint;
        currentCheckpoint.SetActive(true);

        lastCheckpointPosition = checkpoint.transform.position;
        hasCheckpoint = true;
    }

    public void Respawn()
    {
        Vector2 target = hasCheckpoint ? lastCheckpointPosition : initialSpawnPosition;

        // Reset de movimiento para que no siga “volando”
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.position = target; // mover rigidbody de forma segura
        Physics2D.SyncTransforms();
    }
}