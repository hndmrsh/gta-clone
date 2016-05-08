using UnityEngine;
using System.Collections;
 
public class CarController : MonoBehaviour {

    public float speed = 30.0f;
    public float reversingSpeed = 15.0f;
    public float turningSpeed = 40.0f;
    public float reversingTurningSpeed = 25.0f;

    public Transform centerOfMass;

    private Rigidbody rigidBody;
    private BoxCollider leftWheels;
    private BoxCollider rightWheels;

    private float groundedDistance;
    private bool reversing;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass = centerOfMass.position;

        leftWheels = transform.FindChild("leftWheels").GetComponent<BoxCollider>();
        rightWheels = transform.FindChild("rightWheels").GetComponent<BoxCollider>();

        groundedDistance = leftWheels.bounds.extents.y;
    }

    public void FixedUpdate()
    {
        int groundedWheelSets = CountGroundedWheelSets();
        if (groundedWheelSets > 0)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 direction = new Vector3(h, 0, v);

            float turningAngle = Vector3.Angle(transform.forward, direction);

            if (this.reversing && (turningAngle < 90 && turningAngle > -90))
            {
                this.reversing = false;
            } else if(!this.reversing && (turningAngle > 160 && turningAngle < 200)) 
            {
                this.reversing = true;
            }

            Vector3 velocity = transform.forward * direction.magnitude;
            if (reversing)
            {
                velocity *= -reversingSpeed;
            }
            else
            {
                velocity *= speed;
            }

            Vector3 torque = Vector3.Cross(transform.forward, direction);

            torque *= (groundedWheelSets / 2f);

            if (reversing)
            {
                torque *= -reversingTurningSpeed;
            }
            else
            {
                torque *= turningSpeed;
            }

            Debug.DrawRay(transform.position, direction, Color.red);
            Debug.DrawRay(transform.position, torque, Color.blue);

            rigidBody.AddForce(velocity);
            rigidBody.AddTorque(torque);
        }
    }

    private int CountGroundedWheelSets()
    {
        return (IsLeftWheelsGrounded() ? 1 : 0) + (IsRightWheelsGrounded() ? 1 : 0);
    }

    private bool IsLeftWheelsGrounded() 
    {
       return Physics.Raycast(leftWheels.transform.position, -transform.up, groundedDistance + 0.1f);
    }

    private bool IsRightWheelsGrounded()
    {
        return Physics.Raycast(rightWheels.transform.position, -transform.up, groundedDistance + 0.1f);
    }

 }