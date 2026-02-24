using UnityEngine;

public class GravityModule : IEnvironmentModule
{
    private readonly MovementController m_controller;

    public GravityModule(MovementController controller)
    {
        m_controller = controller;
    }

    public void UpdateEnviroment() 
    {    
        PlayerStats stats = m_controller.RuntimeStats;
        Vector3 velocity = m_controller.Velocity;

        if (!m_controller.MovmentState.IsGrounded)
        {            
            velocity.y += stats.Gravity * Time.deltaTime;
            m_controller.Velocity = velocity;
        }
        else
        {
            if (m_controller.Velocity.y < 0)
                m_controller.Velocity = new Vector3(velocity.x, 0, velocity.z);
        }
    
    }
}
