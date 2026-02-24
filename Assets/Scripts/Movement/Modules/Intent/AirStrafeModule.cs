using UnityEngine;

public class AirStrafeModule : IIntentModule
{
    private MovementController m_controller;
    public AirStrafeModule(MovementController controller)
    {
        m_controller = controller;
    }
    public void UpdateIntent() 
    {
        if (!m_controller.MovmentState.IsGrounded) 
        {
            PlayerStats stats = m_controller.RuntimeStats;

            Vector2 input = m_controller.InputState.InputDir;
            Vector3 playerForward = m_controller.Player.forward;

            Vector3 wishDir = playerForward * input.y + m_controller.Player.right * input.x;

            wishDir.y = 0f;
            wishDir.Normalize();

            Vector3 vel = m_controller.Velocity;

            float currentSpeed = Vector3.Dot(vel, wishDir);

            float addSpeed = stats.MaxAirSpeed - currentSpeed;
            if (addSpeed <= 0f)
                return;
            float accelSpeed = stats.AirAcceleration
            * stats.AirControl * Time.deltaTime;

            if (accelSpeed > addSpeed)
                accelSpeed = addSpeed;

            vel += wishDir * accelSpeed;

            m_controller.Velocity = vel;
        }
    }
}
