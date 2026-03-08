using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAnimator : MonoBehaviour
{
    CircleCollisionDetector coll;
    Animator anim;
    SpriteRenderer sr;

    public enum PlayerDirection { Right, Left, Up, Down, RightUp, LeftUp, RightDown, LeftDown }

    // Ahora playerSpeed representa VELOCIDAD (unidades/segundo), no desplazamiento por frame.
    public Vector2 playerSpeed;
    public float playerSpeedAmount;
    public Vector2 playerSpeedNormalized;

    [Min(0.01f)]
    public float playerSpeedAmountThreshold = 0.1f; // Ahora en unidades/segundo

    Vector3 lastPosition;

    public bool allowDiagonals;
    public PlayerDirection direction;

    private void Start()
    {
        coll = GetComponent<CircleCollisionDetector>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // Para que el primer Update no calcule un desplazamiento enorme desde (0,0,0)
        lastPosition = transform.position;
    }

    private void Update()
    {
        // Convertimos "desplazamiento por frame" a "velocidad por segundo"
        float dt = Time.deltaTime;
        if (dt <= 0f) dt = 0.0001f; // seguridad

        Vector2 frameDelta = (Vector2)(transform.position - lastPosition);
        playerSpeed = frameDelta / dt;                 // unidades/segundo
        playerSpeedAmount = playerSpeed.magnitude;     // rapidez (unidades/segundo)

        // Normalizar solo si hay movimiento apreciable
        if (playerSpeedAmount > 0.0001f)
            playerSpeedNormalized = playerSpeed / playerSpeedAmount;
        else
            playerSpeedNormalized = Vector2.zero;

        // Running
        bool isMoving = playerSpeedAmount > playerSpeedAmountThreshold;

        if (isMoving)
        {
            ComputeDirection();
            sr.flipX = playerSpeed.x < 0f;

            if (anim)
                anim.SetBool("isRunning", true);
        }
        else
        {
            if (anim)
                anim.SetBool("isRunning", false);
        }

        // Grounded
        if (anim && coll)
            anim.SetBool("isGrounded", coll.isColliding);

        lastPosition = transform.position;
    }

    void ComputeDirection()
    {
        if (allowDiagonals)
        {
            if (playerSpeedNormalized.x > 0.5f && playerSpeedNormalized.y > 0.5f)
                direction = PlayerDirection.RightUp;
            else if (playerSpeedNormalized.x < -0.5f && playerSpeedNormalized.y > 0.5f)
                direction = PlayerDirection.LeftUp;
            else if (playerSpeedNormalized.x > 0.5f && playerSpeedNormalized.y < -0.5f)
                direction = PlayerDirection.RightDown;
            else if (playerSpeedNormalized.x < -0.5f && playerSpeedNormalized.y < -0.5f)
                direction = PlayerDirection.LeftDown;
            else if (Mathf.Abs(playerSpeedNormalized.x) < Mathf.Abs(playerSpeedNormalized.y))
                direction = (playerSpeedNormalized.y > 0f) ? PlayerDirection.Up : PlayerDirection.Down;
            else
                direction = (playerSpeedNormalized.x > 0f) ? PlayerDirection.Right : PlayerDirection.Left;
        }
        else
        {
            if (playerSpeedNormalized.y > 0f)
            {
                if (playerSpeedNormalized.x > 0f)
                    direction = (playerSpeedNormalized.y > playerSpeedNormalized.x) ? PlayerDirection.Up : PlayerDirection.Right;
                else
                    direction = (playerSpeedNormalized.y > -playerSpeedNormalized.x) ? PlayerDirection.Up : PlayerDirection.Left;
            }
            else
            {
                if (playerSpeedNormalized.x > 0f)
                    direction = (-playerSpeedNormalized.y > playerSpeedNormalized.x) ? PlayerDirection.Down : PlayerDirection.Right;
                else
                    direction = (-playerSpeedNormalized.y > -playerSpeedNormalized.x) ? PlayerDirection.Down : PlayerDirection.Left;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Como playerSpeed ahora es unidades/segundo, este rayo puede ser enorme.
        // Puedes bajar el multiplicador si quieres.
        Debug.DrawRay(transform.position, (Vector3)playerSpeed * 0.2f, Color.red);

        if (allowDiagonals)
        {
            switch (direction)
            {
                case PlayerDirection.Right:     Debug.DrawRay(transform.position, Vector3.right, Color.yellow); break;
                case PlayerDirection.Left:      Debug.DrawRay(transform.position, Vector3.left, Color.yellow); break;
                case PlayerDirection.Up:        Debug.DrawRay(transform.position, Vector3.up, Color.yellow); break;
                case PlayerDirection.Down:      Debug.DrawRay(transform.position, Vector3.down, Color.yellow); break;
                case PlayerDirection.RightUp:   Debug.DrawRay(transform.position, (Vector3.right + Vector3.up).normalized, Color.yellow); break;
                case PlayerDirection.LeftUp:    Debug.DrawRay(transform.position, (Vector3.left + Vector3.up).normalized, Color.yellow); break;
                case PlayerDirection.RightDown: Debug.DrawRay(transform.position, (Vector3.right + Vector3.down).normalized, Color.yellow); break;
                case PlayerDirection.LeftDown:  Debug.DrawRay(transform.position, (Vector3.left + Vector3.down).normalized, Color.yellow); break;
            }
        }
        else
        {
            switch (direction)
            {
                case PlayerDirection.Right: Debug.DrawRay(transform.position, Vector3.right, Color.yellow); break;
                case PlayerDirection.Left:  Debug.DrawRay(transform.position, Vector3.left, Color.yellow); break;
                case PlayerDirection.Up:    Debug.DrawRay(transform.position, Vector3.up, Color.yellow); break;
                case PlayerDirection.Down:  Debug.DrawRay(transform.position, Vector3.down, Color.yellow); break;
            }
        }
    }
}