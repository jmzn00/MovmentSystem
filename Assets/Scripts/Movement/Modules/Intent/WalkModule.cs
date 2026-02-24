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
        if (!m_controller.MovmentState.IsGrounded)
            return;

        Vector2 input = m_controller.InputState.InputDir;
        PlayerStats stats = m_controller.RuntimeStats;

        Vector3 vel = m_controller.Velocity;
        if (input.sqrMagnitude > 0f)
        {
            Vector3 wishDir =
                m_controller.Player.forward * input.y +
                m_controller.Player.right * input.x;

            wishDir = Vector3.ProjectOnPlane(
                wishDir,
                m_controller.MovmentState.GroundNormal
            ).normalized;            
            

            Vector3 horizontalVel = wishDir * stats.WalkSpeed;

            vel.x = horizontalVel.x;
            vel.z = horizontalVel.z;

            m_controller.Velocity = vel;
        }
        else
        {
            vel.x = 0f;
            vel.z = 0f;
            m_controller.Velocity = vel;
        }
    }
}
