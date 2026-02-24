using UnityEngine;

public class StaminaRegenModule : IEnvironmentModule
{
    private MovementController m_movementController;

    public StaminaRegenModule(MovementController movementController)
    {
        m_movementController = movementController;
    }
    public void UpdateEnviroment() 
    {
        PlayerStats stats = m_movementController.RuntimeStats;

        if (stats.Stamina >= stats.MaxStamina) return;
        stats.Stamina += stats.StaminaRegenRate * Time.deltaTime;        
    }
}
