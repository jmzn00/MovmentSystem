using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class GravityModule : IEnvironmentModule
{
    private readonly MovementController _controller;
    private readonly float _gravity = -9.81f;

    public GravityModule(MovementController controller)
    {
        _controller = controller;
    }

    public void UpdateEnviroment() 
    {    
        if (!_controller.MovmentState.IsGrounded)
        {
            Vector3 velocity = _controller.Velocity;
            velocity.y += _gravity * Time.deltaTime;
            _controller.Velocity = velocity;
        }
        else
        {
            if (_controller.Velocity.y < 0)
                _controller.Velocity = new Vector3(_controller.Velocity.x, 0, _controller.Velocity.z);
        }
    
    }
}
