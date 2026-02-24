using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    private InputState m_inputState;
    public InputState InputState => m_inputState;
    private Camera m_camera;
    public Camera Camera => m_camera;
    private List<ICameraModule> m_modules = new List<ICameraModule>();

    private FPSCameraModule fpsModule;
    private TPCameraModule tpModule;

    public Transform m_playerHead;    

    private InputSystem_Actions m_inputActions;
    public InputSystem_Actions InputActions => m_inputActions;
    public void RegisterModule(ICameraModule module)
    {
        m_modules.Add(module);
    }
    public void SetActiveModule(ICameraModule module) 
    {
        foreach (var m in m_modules)
            m.IsActive = m == module;
    }
    private void Awake()
    {
        m_inputActions = InputManager.Instance.InputActions;
        m_inputActions.Player.Look.performed += ctx => m_inputState.InputDir = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Look.canceled += ctx => m_inputState.InputDir = Vector2.zero;        
        m_inputActions.Player.SwitchCamera.performed += ctx => SwitchModule();
        m_camera = GetComponent<Camera>();
        fpsModule = new FPSCameraModule(this);
        tpModule = new TPCameraModule(this);

        RegisterModule(fpsModule);
        RegisterModule(tpModule);

        //SetActiveModule(fpsModule);
        SetActiveModule(tpModule);
    }
    private void SwitchModule()
    {
        var activeIndex = m_modules.FindIndex(m => m.IsActive);

        if (activeIndex == -1)
            return;

        var nextIndex = (activeIndex + 1) % m_modules.Count;

        for (int i = 0; i < m_modules.Count; i++) 
        {
            m_modules[i].IsActive = i == nextIndex;
        }
    }
    private void Update()
    {
        for (int i = 0;  i < m_modules.Count; i++)
        {
            if (m_modules[i].IsActive)
                m_modules[i].Tick();
        }
    }

}
