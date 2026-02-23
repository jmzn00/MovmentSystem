using UnityEngine;
using UnityEngine.InputSystem.XR;

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
        if (!m_controller.MovmentState.IsGrounded) return;

        Vector2 input = m_controller.InputState.InputDir;
        if (input.sqrMagnitude > 0) 
        {
            Vector3 moveDir = m_controller.Player.forward * input.y + m_controller.Player.right * input.x;
            moveDir.Normalize();

            Vector3 vel = m_controller.Velocity;
            vel.x = moveDir.x * WalkSpeed;
            vel.z = moveDir.z * WalkSpeed;  
            m_controller.Velocity = vel;
        }
        else
        {
            Vector3 velocity = m_controller.Velocity;
            velocity.x = 0;
            velocity.z = 0;
            m_controller.Velocity = velocity;
        }
    }
}
