using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollisionDetector))]
public class PlayerJump : MonoBehaviour
{
    public float force = 20f;

    [Min(1)] public int maxJumps = 1;  // 1 = normal, 2 = doble salto
    public int jumpCount;

    Rigidbody2D rb;
    CircleCollisionDetector coll;
    InputSystem_Actions inputs;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollisionDetector>();
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }

    void Update()
    {
        // Al aterrizar, resetea contador
        if (coll.startedCollidingThisFrame)
            jumpCount = 0;

        if (inputs.Player.Jump.WasPressedThisFrame())
        {
            // Primer salto solo si estás en suelo
            bool canFirstJump = coll.isColliding && jumpCount < maxJumps;

            // Saltos extra (doble salto) solo si YA saltaste al menos una vez
            bool canExtraJump = !coll.isColliding && jumpCount > 0 && jumpCount < maxJumps;

            if (canFirstJump || canExtraJump)
                Jump();
        }
    }

    public void Jump()
    {
        jumpCount++;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
        // Si te da error con linearVelocity, usa rb.velocity
    }
}