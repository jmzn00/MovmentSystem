using UnityEngine;

public class GroundFrictionModule : IEnvironmentModule
{
    private MovementController m_controller;
    public GroundFrictionModule(MovementController controller) 
    {
        m_controller = controller;
    }
    public void UpdateEnviroment() 
    {
        if (!m_controller.MovmentState.IsGrounded) return;

        PlayerStats stats = m_controller.RuntimeStats;
        Vector3 vel = m_controller.Velocity;
        Vector3 hor = new Vector3(vel.x, 0, vel.z);
        Vector2 inputDir = m_controller.InputState.InputDir;
        Transform player = m_controller.Player;

        // forward/back friction
        hor = HandleAxisFriction(hor, player.forward, inputDir.y, stats.Acceleration, stats.TimeToStop, stats.SlideFactor, stats.BrakeFactor);

        // strafe friction
        hor = HandleAxisFriction(hor, player.right, inputDir.x, stats.Acceleration, stats.TimeToStop, stats.SlideFactor, stats.BrakeFactor);

        m_controller.Velocity = new Vector3(hor.x, vel.y, hor.z);
    }
    private Vector3 HandleAxisFriction(Vector3 velocity, Vector3 axis, float input, float accel, float stopTime, float slideFactor, float brakeFactor)
    {
        axis.Normalize();
        float currentSpeed = Vector3.Dot(velocity, axis);

        // zero input  slide according to stopTime
        if (Mathf.Abs(input) < 0.01f)
        {
            float decel = Mathf.Abs(currentSpeed) / stopTime * Time.deltaTime;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, decel);
        }
        else
        {
            // opposite input  brake faster
            if (Mathf.Sign(currentSpeed) != Mathf.Sign(input) && Mathf.Abs(currentSpeed) > 0.01f)
            {
                float wishSpeed = input * Mathf.Abs(currentSpeed);
                float decel = Mathf.Abs(currentSpeed - wishSpeed) * brakeFactor * Time.deltaTime;
                currentSpeed = Mathf.MoveTowards(currentSpeed, input * Mathf.Abs(currentSpeed), decel);
            }
        }

        velocity -= axis * Vector3.Dot(velocity, axis);
        velocity += axis * currentSpeed;
        return velocity;
    }
}
