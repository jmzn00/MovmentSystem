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

        float staminaNeeded = GetStaminaCost(stats);            
        if (staminaNeeded > stats.Stamina) return;

        m_wallJumpsPerformed++;
        m_controller.RuntimeStats.Stamina -= staminaNeeded;

        Transform player = m_controller.Player;

        Vector3 wallNormal = m_controller.MovmentState.ContactNormal;
        Vector3 playerForward = m_controller.Player.forward;
        Vector2 input = m_controller.InputState.InputDir;

        Vector3 inputDir = (player.right * input.x) +
            (player.forward * input.y);

        Vector3 jumpDir =
            (wallNormal * stats.WallJumpDistance) +
            (inputDir * stats.WallJumpInputInfluence) +
            (Vector3.up * stats.WallJumpHeight);

        m_controller.Velocity += jumpDir;
    }
    private bool CanWallJump() 
    {
        return m_controller.MovmentState.OnWall
            && m_controller.InputState.JumpJustPressed;
    }
    private float GetStaminaCost(PlayerStats stats) 
    {
        if (m_wallJumpsPerformed == 0)
            return 0f;
        else
        {
            return stats.WallJumpStaminaCost
            + Mathf.Pow(stats.WallJumpStaminaCost,
            m_wallJumpsPerformed);
        }
    }
}
