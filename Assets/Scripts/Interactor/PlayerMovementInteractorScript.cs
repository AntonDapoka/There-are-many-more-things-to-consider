using UnityEngine;

public class PlayerMovementInteractorScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private Rigidbody2D rb; 
    [SerializeField] private Vector2 MovementSpeed = new Vector2(100.0f, 100.0f);

    private Vector2 inputVector = Vector2.zero;

    private void Awake()
    {
        rb = player.GetComponent<Rigidbody2D>();

        rb.angularDrag = 0.0f;
        rb.gravityScale = 0.0f;
    }

    public void SetInput(Vector2 input)
    {
        inputVector = input.normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (inputVector * MovementSpeed * Time.fixedDeltaTime));
    }
}