using UnityEngine;

public class FPSCameraModule : ICameraModule
{
    public float Sensitivity = 100f;
    public float PitchClamp = 85f;

    private CameraManager m_manager;
    private Camera m_camera;
    private Transform m_head;

    private float m_yaw;
    private float m_pitch;


    public FPSCameraModule(CameraManager manager) 
    {
        m_manager = manager;
        m_camera = m_manager.Camera;
        m_head = m_manager.m_playerHead;
    }
    public void Tick()
    {
        Vector2 lookInput = m_manager.InputState.InputDir;

        m_yaw += lookInput.x * Sensitivity * Time.deltaTime;
        m_pitch -= lookInput.y * Sensitivity * Time.deltaTime;
        m_pitch = Mathf.Clamp(m_pitch, -PitchClamp, PitchClamp);

        m_camera.transform.localRotation = Quaternion.Euler(m_pitch, m_yaw, 0f);
        m_camera.transform.position = m_head.position;
    }
    public bool IsActive { get; set; }
}
