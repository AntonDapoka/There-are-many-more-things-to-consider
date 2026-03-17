using UnityEngine;

public class TextInputControllerScript : MonoBehaviour
{
    [SerializeField] private InputHandlerScript[] inputHandlers;
    public InputHandlerScript[] InputHandler => inputHandlers;

    private static readonly KeyCode[] KeyboardKeys =
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G,
        KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N,
        KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U,
        KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z, 
        KeyCode.Space, KeyCode.LeftShift, KeyCode.RightShift
    };

    private void Update()
    {
        CheckInput();
    }

    public void CheckInput()
    {
        foreach (KeyCode key in KeyboardKeys)
        {
            if (Input.GetKeyDown(key))
            {
                NotifyHandlers(key, true);
            }

            if (Input.GetKeyUp(key))
            {
                NotifyHandlers(key, false);
            }
        }
    }

    private void NotifyHandlers(KeyCode key, bool isPressed)
    {
        foreach (InputHandlerScript handler in inputHandlers)
        {
            handler?.HandleInput(key, isPressed);
        }
    }
}