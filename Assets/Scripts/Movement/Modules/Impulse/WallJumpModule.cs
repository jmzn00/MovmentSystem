using UnityEngine;

public class WallJumpModule : IImpulseModule
{
    private MovementController m_controller;
    private int m_wallJumpsPerformed = 0;

    public WallJumpModule(MovementController controller)
    {
        m_controller = controller;
    }
    public void UpdateImpulse() 
    {        
        if (m_controller.MovmentState.IsGrounded)
            m_wallJumpsPerformed = 0;
        if (!CanWallJump()) 
        {            
            return;
        }      
        
        PlayerStats stats = m_controller.RuntimeStats;
        float staminaNeeded;

        if (m_wallJumpsPerformed == 0)
            staminaNeeded = 0f;
        else 
        {
            staminaNeeded = stats.WallJumpStaminaCost
            + Mathf.Pow(stats.WallJumpStaminaCost,
            m_wallJumpsPerformed);
        }
            
        if (staminaNeeded > stats.Stamina) return;

        m_wallJumpsPerformed++;
        m_controller.RuntimeStats.Stamina -= staminaNeeded;

        Vector3 wallNormal = m_controller.MovmentState.ContactNormal;
        Vector3 playerForward = m_controller.Player.forward;

        Vector3 jumpDir = (wallNormal * stats.WallJumpDistance)
            + (playerForward * stats.WallJumpForwardBoost) 
            + (Vector3.up * stats.WallJumpHeight);

        m_controller.Velocity = jumpDir;         
    }
    private bool CanWallJump() 
    {
        return m_controller.MovmentState.OnWall
            && m_controller.InputState.JumpJustPressed;
    }
}
