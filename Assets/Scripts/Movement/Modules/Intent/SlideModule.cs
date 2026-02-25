using UnityEngine;

public class SlideModule : IIntentModule
{
    private MovementController m_movementController;

    public SlideModule(MovementController movementController)
    {
        m_movementController = movementController;
    }

    public void UpdateIntent() 
    {        
        PlayerStats stats = m_movementController.RuntimeStats;
        Vector3 vel = m_movementController.Velocity;
        Vector3 hor = Vector3.ProjectOnPlane(vel,
            m_movementController.MovmentState.ContactNormal);

        if (!CanSlide(hor, stats.WalkSpeed * 1.1f)) 
        {
            m_movementController.SetSliding(false);
            return;
        }
        m_movementController.SetSliding(true);

        Vector3 groundNormal = m_movementController.MovmentState.ContactNormal;
        float slopeDot = Vector3.Dot(Vector3.down, groundNormal);
        Vector3 slideDir = hor.normalized;
        Debug.Log("Sliding");
        if (slopeDot < 0.01f) 
        {
            
        }

        
    }
    private bool CanSlide(Vector3 horizontalVelocity, float walkSpeed) 
    {
        if (!m_movementController.MovmentState.IsGrounded 
            || !m_movementController.InputState.CrouchPressed
            || horizontalVelocity.magnitude < walkSpeed)
        {
            return false;
        }
        return true;
    }
}
