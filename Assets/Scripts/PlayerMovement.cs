using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : MonoBehaviour
{
    public float Speed = 10.0f;
    public float Gravity = 10.0f;
    public float MaxVelocityChange = 10.0f;
    public bool CanJump = true;
    public float JumpHeight = 2.0f;
    public float LookSensitivity = 1;

    private Rigidbody _rigidbody;
    private Camera _camera;
    private float _distToGround;

    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;
        _rigidbody.useGravity = true;
        _distToGround = GetComponent<Collider>().bounds.extents.y;
    }

    void FixedUpdate()
    {
        if (IsGrounded())
        {
            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            targetVelocity = transform.TransformDirection(targetVelocity);
            targetVelocity *= Speed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocity = _rigidbody.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.x = Mathf.Clamp(velocityChange.x, -MaxVelocityChange, MaxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -MaxVelocityChange, MaxVelocityChange);
            velocityChange.y = 0;
            _rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (CanJump && Input.GetButton("Jump"))
            {
                _rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
            }
        }

        // We apply gravity manually for more tuning control
        _rigidbody.AddForce(new Vector3(0, -Gravity * _rigidbody.mass, 0));
        
        // Handle Cursor for testing.
        if (Input.GetButton("Cancel"))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (Cursor.lockState == CursorLockMode.None)
        {
            return;
        }

        var xRot = Input.GetAxis("Mouse X");
        var yRot = Input.GetAxis("Mouse Y");
        _rigidbody.transform.rotation = Quaternion.Euler(
            new Vector3(0, xRot, 0)
            * LookSensitivity
            + _rigidbody.transform.rotation.eulerAngles);
        _camera.transform.rotation = Quaternion.Euler(
            new Vector3(-yRot, 0, 0)
            * LookSensitivity
            + _camera.transform.rotation.eulerAngles);
    }

    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * JumpHeight * Gravity);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, _distToGround + 0.1f);
    }
}
