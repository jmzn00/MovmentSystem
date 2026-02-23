using UnityEngine;

public class WalkModule : IIntentModule
{
    private MovementController m_controller;
    public float WalkSpeed = 5f;

    public WalkModule(MovementController controller)
    {
        m_controller = controller;
    }
    public void UpdateIntent()
    {
        Vector3 inputDir = new Vector3(m_controller.InputState.InputDir.x, 0, m_controller.InputState.InputDir.y);
        m_controller.Velocity = inputDir.normalized * WalkSpeed;
    }
}
