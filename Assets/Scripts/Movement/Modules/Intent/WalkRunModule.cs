using UnityEngine;

public class WalkRunModule : IIntentModule
{
    private MovementController m_controller;
    private float m_movementHeldTime;
    public WalkRunModule(MovementController controller) 
    {
        m_controller = controller;
    }
    public void UpdateIntent() 
    {
        PlayerStats stats = m_controller.RuntimeStats;
        Transform player = m_controller.Player;

        Vector2 input = m_controller.InputState.InputDir;
        if (input.sqrMagnitude > 1f) input.Normalize();

        float targetSpeed = (m_movementHeldTime >= stats.TimeToRun)
            ? stats.RunSpeed : stats.WalkSpeed;

        Vector3 vel = m_controller.Velocity;
        Vector3 hor = new Vector3(vel.x, 0, vel.z);

        hor = AccelerateAlong(hor, player.forward, input.y, targetSpeed, stats.Acceleration);
        hor = AccelerateAlong(hor, player.right, input.x, targetSpeed, stats.Acceleration);

        m_controller.Velocity = new Vector3(hor.x, vel.y, hor.z);
    }

    private Vector3 AccelerateAlong(Vector3 velocity, Vector3 axis, float input, float maxSpeed, float accel)
    {
        axis.Normalize();
        float currentSpeed = Vector3.Dot(velocity, axis);
        float wishSpeed = input * maxSpeed;

        float addSpeed = accel * Time.deltaTime;
        float newSpeed = Mathf.MoveTowards(currentSpeed, wishSpeed, addSpeed);

        velocity -= axis * currentSpeed;
        velocity += axis * newSpeed;
        return velocity;
    }

}
