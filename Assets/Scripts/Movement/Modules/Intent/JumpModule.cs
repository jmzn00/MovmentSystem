using UnityEngine;

public class JumpModule : IImpulseModule
{
    private readonly MovementController _controller;
    private readonly float _jumpForce = 5f;

    public JumpModule(MovementController controller)
    {
        _controller = controller;
    }

    public void UpdateImpulse()
    {
        if (_controller.InputState.JumpPressed && _controller.MovmentState.IsGrounded)
        {
            Vector3 velocity = _controller.Velocity;
            velocity.y = _jumpForce;
            _controller.Velocity = velocity;
        }
    }
}