using Mono.Cecil;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    private Camera m_camera;
    public Camera Camera => m_camera;
    private List<ICameraModule> m_modules = new List<ICameraModule>();

    private FPSCameraModule fpsModule;

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
        m_camera = GetComponent<Camera>();
        fpsModule = new FPSCameraModule(this);

        RegisterModule(fpsModule);
        SetActiveModule(fpsModule);
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
