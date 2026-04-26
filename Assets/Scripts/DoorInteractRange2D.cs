using UnityEngine;

public class DoorInteractRange2D : MonoBehaviour
{
    [SerializeField] private DoorToScene2D door;

    private void Reset()
    {
        door = GetComponentInParent<DoorToScene2D>();
    }

    private void Awake()
    {
        if (door == null)
            door = GetComponentInParent<DoorToScene2D>();

        if (door == null)
            Debug.LogError("DoorInteractRange2D: No encuentro DoorToScene2D en el padre. Asigna 'door' en el Inspector.", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (door == null) return;

        if (!IsPlayer(other)) return;
        door.SetPlayerInRange(true);

    Debug.Log("ENTER rango con: " + other.name);
    if (!other.CompareTag("Player")) return;
    door.SetPlayerInRange(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (door == null) return;

        if (!IsPlayer(other)) return;
        door.SetPlayerInRange(false);
    }

    private bool IsPlayer(Collider2D other)
    {
        // Importante: si el collider está en un HIJO del player, el tag puede estar en el padre.
        return other.CompareTag("Player") || other.transform.root.CompareTag("Player");
    }
    
}