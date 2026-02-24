using UnityEngine;

public class WallJumpModule : IImpulseModule
{
    private MovementController m_controller;
    

    public WallJumpModule(MovementController controller)
    {
        m_controller = controller;
    }
    public void UpdateImpulse() 
    {
        if (!m_controller.MovmentState.OnWall) return;
        if (!m_controller.InputState.JumpJustPressed) return;

        PlayerStats stats = m_controller.RuntimeStats;

        Vector3 wallNormal = m_controller.MovmentState.GroundNormal;
        Vector3 playerForward = m_controller.Player.forward;

        Vector3 jumpDir = (wallNormal * stats.WallJumpDistance)
            + (playerForward * stats.WallJumpForwardBoost) 
            + (Vector3.up * stats.WallJumpHeight);

        m_controller.Velocity = jumpDir;        
    }
}
