using UnityEngine;

public class DashModule : IImpulseModule
{
    private MovementController m_movementController;
    private float m_dashTimer = 0f;
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

        if (m_dashTimer < 0.25f)
        {
            m_dashTimer += Time.deltaTime;
            return;
        }

        if (!m_movementController.InputState.DashPressedThisFrame) return;
        if (stats.Stamina < stats.DashStaminaCost) return;                
        m_dashTimer = 0f;
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

        Vector3 dashVelocity = dashDir * dashSpeed;
        m_movementController.Velocity += dashVelocity;

        stats.Stamina -= stats.DashStaminaCost;
        
    }
}
