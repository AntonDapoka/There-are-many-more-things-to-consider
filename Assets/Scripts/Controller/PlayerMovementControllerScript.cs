using UnityEngine;

public class PlayerMovementControllerScript : InputHandlerScript
{
    [SerializeField] private PlayerMovementInteractorScript interactor;

    private bool wPressed;
    private bool aPressed;
    private bool sPressed;
    private bool dPressed;
    private bool shiftPressed;

    public override void HandleInput(KeyCode key, bool isPressed)
    {
        switch (key)
        {
            case KeyCode.W: wPressed = isPressed; break;
            case KeyCode.A: aPressed = isPressed; break;
            case KeyCode.S: sPressed = isPressed; break;
            case KeyCode.D: dPressed = isPressed; break;

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                shiftPressed = isPressed;
                break;
        }

        UpdateMovement();
    }

    private void UpdateMovement()
    {
        if (!shiftPressed)
        {
            interactor.SetInput(Vector2.zero);
            return;
        }

        Vector2 input = Vector2.zero;

        if (wPressed) input += Vector2.up;
        if (sPressed) input += Vector2.down;
        if (aPressed) input += Vector2.left;
        if (dPressed) input += Vector2.right;

        interactor.SetInput(input);
    }
}