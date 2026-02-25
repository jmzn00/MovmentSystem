public class PlayerStats
{
    public float WalkSpeed;
    public float RunSpeed;
    public float TimeToRun;

    public float Stamina;
    public float MaxStamina;
    public float StaminaRegenRate;


    public float Acceleration;
    public float Deceleration;
    public float TimeToStop;
    public float SlideFactor;
    public float BrakeFactor;

    public float Gravity;

    public float JumpForce;

    public float MaxAirSpeed;
    public float AirAcceleration;
    public float AirControl;

    public float DashDistance;
    public float DashTime;
    public float DashStaminaCost;
    public float DashCooldown;

    public float WallJumpDistance;
    public float WallJumpForwardBoost;
    public float WallJumpHeight;
    public float WallJumpTime;
    public float FirstWallJumpStaminaCost;
    public float SecondWallJumpStaminaCost;

    public float WallJumpStaminaCost;
    public float ConsecutiveWallJumpStaminaCostMultiplier;
    public void Initialize(MovementStats so) 
    {
        Stamina = 0f;
        MaxStamina = so.MaxStamina;
        StaminaRegenRate = so.StaminaRegenRate;

        WalkSpeed = so.WalkSpeed;
        RunSpeed = so.RunSpeed;    
        TimeToRun = so.TimeToRun;
        

        Acceleration = so.Acceleration;
        Deceleration = so.Deceleration;
        TimeToStop = so.TimeToStop;
        SlideFactor = so.SlideFactor;
        BrakeFactor = so.BrakeFactor;

        JumpForce = so.JumpForce;

        Gravity = so.Gravity;

        AirAcceleration = so.AirAcceleration;
        AirControl = so.AirControl;
        MaxAirSpeed = so.MaxAirSpeed;

        DashDistance = so.DashDistance;
        DashTime = so.DashTime;
        DashStaminaCost = so.DashStaminaCost;
        DashCooldown = so.DashCooldown;

        WallJumpDistance = so.WallJumpDistance;
        WallJumpForwardBoost = so.WallJumpForwardBoost;
        WallJumpHeight = so.WallJumpHeight;
        WallJumpTime = so.WallJumpTime;

        WallJumpStaminaCost = so .WallJumpStaminaCost;
    }
}
