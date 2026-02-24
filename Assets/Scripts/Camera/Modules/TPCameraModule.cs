using UnityEngine;

public class TPCameraModule : ICameraModule
{
    private CameraManager m_cameraManager;
    public bool IsActive { get; set; }
    private Transform m_head;
    private Camera m_camera;

    public float DistanceFromPlayer = 5f;
    public float PitchClamp = 85f;
    public float Sensitivity = 100f;

    private float m_yaw;
    private float m_pitch;

    public float CollisionRadius = 0.3f;
    public float CollisionOffset = 0.05f;
    public LayerMask CollisionMask;

    public Vector3 cameraOffset = new Vector3(1f, 0f, 0f);


    public TPCameraModule(CameraManager cameraManager)
    {
        m_cameraManager = cameraManager;
        m_head = m_cameraManager.m_playerHead;
        m_camera = m_cameraManager.Camera;

        Vector3 angles = m_camera.transform.eulerAngles;
        m_yaw = angles.y;
        m_pitch = angles.x;
    }
    public void Tick() 
    {
        if (!IsActive) return;

        Vector2 lookInput = m_cameraManager.InputState.InputDir;

        m_yaw += lookInput.x * Sensitivity * Time.deltaTime;
        m_pitch -= lookInput.y * Sensitivity * Time.deltaTime;
        m_pitch = Mathf.Clamp(m_pitch, -PitchClamp, PitchClamp);

        Quaternion rotation = Quaternion.Euler(m_pitch, m_yaw, 0f);

        Vector3 desiredPosition = 
            m_head.position
            - rotation * Vector3.forward * DistanceFromPlayer
            + rotation * cameraOffset;

        Vector3 direction = desiredPosition - m_head.position;
        float distance = direction.magnitude;

        if (Physics.SphereCast(
            m_head.position,
            CollisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance)) 
        {
            desiredPosition = hit.point - direction.normalized * CollisionOffset;
        }

        m_camera.transform.position = desiredPosition;
        m_camera.transform.rotation = rotation;
    }
}
