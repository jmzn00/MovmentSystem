using UnityEngine;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private InputSystem_Actions m_inputActions;
    public InputSystem_Actions InputActions => m_inputActions;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        m_inputActions = new InputSystem_Actions();

        ToggleCursor(CursorLockMode.Locked);
    }
    public void ToggleCursor(CursorLockMode mode) 
    {
        Cursor.lockState = mode;
        if (mode == CursorLockMode.Locked)
            Cursor.visible = false;
        else
            Cursor.visible = true;
    }
    private void OnEnable()
    {
        m_inputActions.Enable();
    }
    private void OnDisable()
    {
        m_inputActions.Disable();
    }

}
