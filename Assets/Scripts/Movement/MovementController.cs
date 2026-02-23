using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public struct InputState 
{
    public Vector2 InputDir;
    public bool JumpPressed;
}

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    private CharacterController m_controller;
    private InputSystem_Actions m_inputActions;

    private InputState m_inputState;
    public InputState InputState => m_inputState;

    public Vector3 Velocity { get; set; }

    private List<IEnvironmentModule> environmentModules = new();
    private List<IIntentModule> intentModules = new();
    private List<IImpulseModule> impulseModules = new();
    private List<IForceModule> forceModules = new();
    private List<IPostProcessModule> postProcessModules = new();

    private void OnEnable()
    {
        m_inputActions.Enable();
    }
    private void OnDisable()
    {
        m_inputActions.Disable();
    }    
    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_inputActions = new InputSystem_Actions();
        BindInputs();

        AddModule(new WalkModule(this));       
    }
    private void AddModule(object module) 
    {
        if (module is IEnvironmentModule env) environmentModules.Add(env);
        if (module is IIntentModule intent) intentModules.Add(intent);
        if (module is IImpulseModule imp) impulseModules.Add(imp);
        if (module is IForceModule force) forceModules.Add(force);
        if (module is IPostProcessModule post) postProcessModules.Add(post);
    }    
    private void Update()
    {
        for (int i = 0; i < environmentModules.Count; i++) {
            environmentModules[i].UpdateEnviroment();
        }
        for (int i = 0; i < intentModules.Count; i++) { 
            intentModules[i].UpdateIntent();
        }
        for (int i = 0; i < impulseModules.Count; i++) { 
            impulseModules[i].UpdateImpulse();
        }
        for (int i = 0; i < forceModules.Count; i++) { 
            forceModules[i].UpdateForce();
        }
        for (int i = 0; i < postProcessModules.Count; i++) { 
            postProcessModules[i].UpdatePostProcess();
        }

        m_inputState.JumpPressed = false;
        m_controller.Move(Velocity * Time.deltaTime);
    }
    void BindInputs()
    {
        m_inputActions.Player.Move.performed += ctx =>
            m_inputState.InputDir = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Move.canceled += ctx =>
            m_inputState.InputDir = Vector2.zero;
        m_inputActions.Player.Jump.performed += ctx =>
            m_inputState.JumpPressed = true;
    }
}
