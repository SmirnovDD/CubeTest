using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public bool CursorLocked { get; private set; } = true;
    public Vector2 MouseDelta { get; private set; }
    public Vector2 MovementDirection { get; private set; }
    public bool Jump { get; set; }
    public bool Sprint { get; private set; }
    public bool AnalogMovement { get; private set; }

    private void Update()
    {
        MouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y")) * 150;
        MovementDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Jump = Input.GetKeyDown(KeyCode.Space);
        Sprint = Input.GetKey(KeyCode.LeftShift);
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(CursorLocked);
    }
    
    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
