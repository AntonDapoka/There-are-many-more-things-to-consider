using UnityEngine;

public class PlayerTextControllerScript : InputHandlerScript
{
    [SerializeField] private PlayerTextInteractorScript playerTextInteractor;

    public override void HandleInput(KeyCode key, bool isPressed)
    {
        if (!isPressed) return;

        bool shiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (shiftPressed && (key == KeyCode.W || key == KeyCode.A || key == KeyCode.S || key == KeyCode.D)) 
            return;

        if (key >= KeyCode.A && key <= KeyCode.Z)
        {
            char letter = (char)('a' + (key - KeyCode.A));
            playerTextInteractor.ProcessSymbol(char.ToUpper(letter));
        }
    }
}