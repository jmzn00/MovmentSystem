using System.Collections.Generic;
using UnityEngine;
public struct InputState
{
    public Vector2 InputDir;
    public bool JumpPressed;
}
public struct MovmentState
{
    public bool IsGrounded;
    public bool IsJumping;
    public bool IsFalling;
}

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    private CameraManager m_cameraManager;
    private CharacterController m_controller;
    private InputSystem_Actions m_inputActions;

    public CameraManager CameraManager => m_cameraManager;

    private InputState m_inputState;
    public InputState InputState => m_inputState;

    private MovmentState m_movmentState;
    public MovmentState MovmentState => m_movmentState; 

    public Vector3 Velocity { get; set; }

    public Transform Player => transform;

    private List<IEnvironmentModule> environmentModules = new();
    private List<IIntentModule> intentModules = new();
    private List<IImpulseModule> impulseModules = new();
    private List<IForceModule> forceModules = new();
    private List<IPostProcessModule> postProcessModules = new();
    private void Awake()
    {
        m_controller = GetComponent<CharacterController>();
        m_inputActions = InputManager.Instance.InputActions;
        m_cameraManager = Camera.main.GetComponent<CameraManager>();
        BindInputs();

        AddModule(new WalkModule(this));
        AddModule(new RotationModule(this));
        AddModule(new GravityModule(this));
        AddModule(new JumpModule(this));
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
        m_movmentState.IsGrounded = CheckGrounded();
        for (int i = 0; i < environmentModules.Count; i++)
        {
            environmentModules[i].UpdateEnviroment();
        }
        for (int i = 0; i < intentModules.Count; i++)
        {
            intentModules[i].UpdateIntent();
        }
        for (int i = 0; i < impulseModules.Count; i++)
        {
            impulseModules[i].UpdateImpulse();
        }
        for (int i = 0; i < forceModules.Count; i++)
        {
            forceModules[i].UpdateForce();
        }
        for (int i = 0; i < postProcessModules.Count; i++)
        {
            postProcessModules[i].UpdatePostProcess();
        }
        m_controller.Move(Velocity * Time.deltaTime);

        m_inputState.JumpPressed = false;
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
    bool CheckGrounded()
    {
        float distanceToGround = 0.1f;
        return Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, distanceToGround);
    }
}
