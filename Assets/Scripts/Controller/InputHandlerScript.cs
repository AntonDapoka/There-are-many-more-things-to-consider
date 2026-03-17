using UnityEngine;

public abstract class InputHandlerScript : MonoBehaviour
{
        public abstract void HandleInput(KeyCode key, bool isPressed);
}