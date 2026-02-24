using UnityEngine;

public class DashModule : IImpulseModule
{
    private MovementController m_movementController;
    private bool m_isDashing;
    private float m_dashTimer;
    private Vector3 m_dashVelocity;

    public DashModule(MovementController movementController)
    {
        m_movementController = movementController;

        PlayerStats stats = m_movementController.RuntimeStats;
        stats.Stamina = stats.MaxStamina;
    }
    public void UpdateImpulse() 
    {
        PlayerStats stats = m_movementController.RuntimeStats;
        Vector3 vel = m_movementController.Velocity;

        if (m_isDashing) 
        {
            m_dashTimer -= Time.deltaTime;

            m_movementController.Velocity = new Vector3(
                m_dashVelocity.x,
                vel.y,
                m_dashVelocity.z);

            if (m_dashTimer <= 0f)
                m_isDashing = false;

            return;
        }

        if (!m_movementController.InputState.DashPressed) return;
        if (stats.Stamina < stats.DashStaminaCost) return;

        Vector2 input = m_movementController.InputState.InputDir;
        Vector3 dashDir;

        Transform player = m_movementController.Player;
        if (input.sqrMagnitude > 0.001f) 
        {            
            dashDir = (player.forward * input.y
                + player.right * input.x).normalized;
        }
        else 
        {
            dashDir = player.forward;
        }

        float dashSpeed = stats.DashDistance / stats.DashTime;

        m_dashVelocity = dashDir * dashSpeed;
        m_isDashing = true;

        stats.Stamina -= stats.DashStaminaCost;
        m_dashTimer = stats.DashTime;
    }
}
