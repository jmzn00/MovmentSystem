using System.Collections.Generic;
using TMPro;
using UnityEngine;
public struct InputState
{
    public Vector2 InputDir;
    public bool JumpPressed;
    public bool JumpJustPressed;
    public bool DashPressed;
    public bool CrouchPressed;
}
public struct MovmentState
{
    public bool IsGrounded;
    public Vector3 ContactNormal;
    public bool OnWall;
    public bool IsSliding;
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

    [Header("RayCast")]
    [SerializeField] private LayerMask m_defaultLayer;

    [Header("DebugUI")]
    [SerializeField] private TMP_Text m_posText;
    [SerializeField] private TMP_Text m_rotText;
    [SerializeField] private TMP_Text m_velText;
    [SerializeField] private TMP_Text m_staminaText;

    [Header("TempVisuals")]
    [SerializeField] private GameObject m_visualCapsule;
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
        //AddModule(new AirStrafeModule(this));

        AddModule(new DashModule(this));
        AddModule(new JumpModule(this));
        AddModule(new WallJumpModule(this));
        AddModule(new SlideModule(this));

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
        UpdateContact();
        CollisionFlags flags = m_controller.Move(Velocity * Time.deltaTime);

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
        m_posText.text = $"Pos: {transform.position}";
        m_rotText.text = $"Rot: {transform.rotation.eulerAngles}";
        LogSpeed();
        m_staminaText.text = $"Stamina: {RuntimeStats.Stamina:F1}";
        #endregion        
    }


    void LogSpeed() 
    {
        Vector3 horizontalVelocity = new Vector3(Velocity.x, 0f, Velocity.z);
        float speed = horizontalVelocity.magnitude;

        m_velText.text = $"Speed: {speed:F1}";
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

        m_inputActions.Player.Crouch.performed += ctx => 
            m_inputState.CrouchPressed = true;
        m_inputActions.Player.Crouch.canceled += ctx =>
            m_inputState.CrouchPressed = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {        
        m_movmentState.ContactNormal = hit.normal;

        float slope = Vector3.Angle(hit.normal, Vector3.up);
        bool isSteep = slope > m_stats.MaxSlopeAngle;

        bool notGrounded = !m_movmentState.IsGrounded;

        bool movingIntoWall = Vector3.Dot(Velocity, -hit.normal) > 0f;

        m_movmentState.OnWall = isSteep && notGrounded && movingIntoWall;        
    }
    public void SetSliding(bool value) 
    {
        m_movmentState.IsSliding = value;    
        
        if (value) 
        {
            m_visualCapsule.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
        }
        else        
        {
            m_visualCapsule.transform.localRotation = Quaternion.identity;
        }
    }

    private void UpdateContact()
    {
        m_movmentState.IsGrounded = m_controller.isGrounded;
        m_movmentState.OnWall = false;

        if (!m_movmentState.IsGrounded)
        {
            GroundedRayCheck();
        }
    }
    private void GroundedRayCheck() 
    {
        float rayLength = 0.1f;
        Vector3 origin = transform.position + m_controller.center;

        if (Physics.SphereCast(origin, m_controller.radius * 0.9f, Vector3.down,
            out RaycastHit hit, m_controller.height / 2 + rayLength)) 
        {
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            if (slope <= m_stats.MaxSlopeAngle) 
            {
                m_movmentState.IsGrounded = true;
                m_movmentState.ContactNormal = hit.normal;
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (m_controller == null || m_cameraManager == null) return;

        Vector3 origin = transform.position + m_controller.center;
        Vector3 normal = m_movmentState.ContactNormal;

        // Contact normal
        if (normal.sqrMagnitude > 0.0001f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(origin, normal * 2f);
        }

        // Input direction (world space)
        Vector3 camForward = m_cameraManager.transform.forward;
        Vector3 camRight = m_cameraManager.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        Vector3 moveDir =
            camForward.normalized * m_inputState.InputDir.y +
            camRight.normalized * m_inputState.InputDir.x;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(origin, moveDir * 2f);
        }
    }
}
