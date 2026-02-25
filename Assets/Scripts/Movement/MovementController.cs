using System.Collections.Generic;
using TMPro;
using UnityEngine;
public struct InputState
{
    public Vector2 InputDir;
    public bool JumpPressed;
    public bool JumpJustPressed;
    public bool DashPressed;
}
public struct MovmentState
{
    public bool IsGrounded;
    public Vector3 ContactNormal;
    public bool OnWall;
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

    [Header("Stats")]
    [SerializeField] private MovementStats m_stats;
    private PlayerStats m_runtimeStats;
    public MovementStats Stats => m_stats;
    public PlayerStats RuntimeStats => m_runtimeStats;

    private List<IEnvironmentModule> environmentModules = new();
    private List<IIntentModule> intentModules = new();
    private List<IImpulseModule> impulseModules = new();
    private List<IForceModule> forceModules = new();
    private List<IPostProcessModule> postProcessModules = new();

    [Header("DebugUI")]
    [SerializeField] private TMP_Text m_posText;
    [SerializeField] private TMP_Text m_rotText;
    [SerializeField] private TMP_Text m_velText;
    [SerializeField] private TMP_Text m_staminaText;
    private float m_maxMagnitude = 0f;
    private float m_maxMagnitudeResetTimer = 0f;   
    bool m_resetMaxMagnitude = false;
    private void Awake()
    {
        m_runtimeStats = new PlayerStats();
        m_runtimeStats.Initialize(m_stats);

        m_controller = GetComponent<CharacterController>();
        m_inputActions = InputManager.Instance.InputActions;
        m_cameraManager = Camera.main.GetComponent<CameraManager>();     
        BindInputs();

        AddModule(new RotationModule(this));
      
        AddModule(new WalkRunModule(this));
        AddModule(new AirStrafeModule(this));

        AddModule(new DashModule(this));
        AddModule(new JumpModule(this));
        AddModule(new WallJumpModule(this));

        AddModule(new StaminaRegenModule(this));
        AddModule(new GroundFrictionModule(this));
        AddModule(new GravityModule(this));
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
        m_movmentState.OnWall = false;
        CollisionFlags flags = m_controller.Move(Velocity * Time.deltaTime);
        m_movmentState.IsGrounded = (flags & CollisionFlags.Below) != 0;        

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
        m_inputState.JumpJustPressed = false;
        #region DebugUI
        if (Velocity.magnitude > m_maxMagnitude)
        {
            m_maxMagnitude = Velocity.magnitude;

            m_resetMaxMagnitude = true;
            m_maxMagnitudeResetTimer = 0f;
        }

        if (m_resetMaxMagnitude)
        {
            m_maxMagnitudeResetTimer += Time.deltaTime;

            if (m_maxMagnitudeResetTimer >= 5f)
            {
                m_maxMagnitude = 0f;
                m_resetMaxMagnitude = false;
                m_maxMagnitudeResetTimer = 0f;
            }
        }
        m_posText.text = $"Pos: {transform.position}";
        m_rotText.text = $"Rot: {transform.rotation.eulerAngles}";
        m_velText.text = $"Vel: {Velocity.magnitude:F1} || Max: {m_maxMagnitude:F1}";
        m_staminaText.text = $"Stamina: {RuntimeStats.Stamina:F1}";
        #endregion
    }
    void BindInputs()
    {
        m_inputActions.Player.Move.performed += ctx =>
            m_inputState.InputDir = ctx.ReadValue<Vector2>();
        m_inputActions.Player.Move.canceled += ctx =>
            m_inputState.InputDir = Vector2.zero;

        m_inputActions.Player.Jump.performed += ctx =>
        {
            m_inputState.JumpPressed = true;
            m_inputState.JumpJustPressed = true;
        };        
        m_inputActions.Player.Jump.canceled += ctx =>
            m_inputState.JumpPressed = false;

        m_inputActions.Player.Dash.performed += ctx =>
            m_inputState.DashPressed = true;
        m_inputActions.Player.Dash.canceled += ctx =>
            m_inputState.DashPressed = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_movmentState.ContactNormal = hit.normal;

        // Is it a steep surface? (wall)
        float slope = Vector3.Angle(hit.normal, Vector3.up);
        bool isSteep = slope > m_stats.MaxSlopeAngle; // use MaxSlopeAngle from your stats

        // Are we not grounded?
        bool notGrounded = !m_movmentState.IsGrounded;

        // Are we moving roughly towards the wall?
        bool movingIntoWall = Vector3.Dot(Velocity, -hit.normal) > 0f;

        m_movmentState.OnWall = isSteep && notGrounded && movingIntoWall;
    }
}
