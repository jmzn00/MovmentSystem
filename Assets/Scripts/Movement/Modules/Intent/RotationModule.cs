using UnityEngine;

public class RotationModule : IIntentModule
{
    MovementController m_controller;
    public RotationModule(MovementController controller) 
    {
        m_controller = controller;
    }
    public void UpdateIntent()
    {
        m_controller.Player.rotation = Quaternion.Euler(0, 
            m_controller.CameraManager.Camera.transform.rotation.eulerAngles.y, 0);
    }
}
