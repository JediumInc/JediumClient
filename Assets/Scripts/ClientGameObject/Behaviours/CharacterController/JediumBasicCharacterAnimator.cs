using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jedium.Behaviours.CharacterController
{
    public class JediumBasicCharacterAnimator : JediumCharacterAnimator
    {
        private bool _initialized = false;
        private bool _isOwner;
        

        private int hFloat; // Animator variable related to Horizontal Axis.
        private int vFloat; // Animator variable related to Vertical Axis.
        private int groundedBool;                             // Animator variable related to whether or not the player is on the ground.
        private int jumpBool;                           // Animator variable related to jumping.
        private int speedFloat;                      // Speed parameter on the Animator.


        private Rigidbody rBody;
        private ThirdPersonOrbitCamBasic camScript; // Reference to the third person camera script.
        public Transform playerCamera; // Reference to the camera that focus the player.
        private Animator unityAnim;

        private float h; // Horizontal Axis.
        private float v; // Vertical Axis.

        private Vector3 colExtents;                           // Collider extents for ground test.

        private float speed;

        private Vector3 lastDirection;                        // Last direction the player was moving.

        public float speedDampTime = 0.1f;
        public float turnSmoothing = 0.06f;                   // Speed of turn when moving to match camera facing.
        public float jumpHeight = 1.5f;                 // Default jump height.
        public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.
        public float sprintSpeed = 2.0f;                // Default sprint speed.

        private bool isColliding;                       // Boolean to determine if the player has collided with an obstacle.


        private bool jump;                              // Boolean to determine whether or not the player started a jump.

        public override void SetVH(float V, float H,bool J)
        {
            this.v = V;
            this.h = H;
            this.jump = J;
            unityAnim.SetFloat(hFloat, h);
            unityAnim.SetFloat(vFloat, v);
        }

        public override void Init(bool isOwner)
        {
            _isOwner = isOwner;
            unityAnim = GetComponent<Animator>();
            hFloat = Animator.StringToHash("H");
            vFloat = Animator.StringToHash("V");
            groundedBool = Animator.StringToHash("Grounded");
            jumpBool = Animator.StringToHash("Jump");
            speedFloat = Animator.StringToHash("Speed");
            colExtents = GetComponent<Collider>().bounds.extents;
            if (_isOwner)
            {
                playerCamera = Camera.main.transform;
                rBody = GetComponent<Rigidbody>();
                camScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic>();

                camScript.Setup(this.transform);
            }

            _initialized = true;
        }

        void Update()
        {
            if (_initialized)
            {
                unityAnim.SetBool(groundedBool, IsGrounded());
            }
        }

        void FixedUpdate()
        {
            MovementManagement(h, v);
            // Call the jump manager.
            JumpManagement();
        }

        // Execute the idle and walk/run jump movements.
        void JumpManagement()
        {
            // Start a new jump.
            if (jump && !unityAnim.GetBool(jumpBool) && IsGrounded())
            {

                if (_isOwner)
                {
                    // Is a locomotion jump?
                    if (unityAnim.GetFloat(speedFloat) > 0.1)
                    {
                        // Temporarily change player friction to pass through obstacles.
                        GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
                        GetComponent<CapsuleCollider>().material.staticFriction = 0f;
                        // Set jump vertical impulse velocity.
                        float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                        velocity = Mathf.Sqrt(velocity);
                        rBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
                    }
                }
            }
            // Is already jumping?
            else if (unityAnim.GetBool(jumpBool))
            {
                // Keep forward movement while in the air.
                if (_isOwner)
                {
                    if (!IsGrounded() && !isColliding)
                    {
                        rBody.AddForce(
                            transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed,
                            ForceMode.Acceleration);
                    }
                }

                // Has landed?
                if (IsGrounded()) //(behaviourManager.GetRigidBody.velocity.y < 0)
                {
                    unityAnim.SetBool("Grounded", true);
                    // Change back player friction to default.
                    GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
                    GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
                    // Set jump related parameters.
                    jump = false;

                  
                }
            }
        }

        private void LateUpdate()
        {
        }


        // Deal with the basic player movement
            void MovementManagement(float horizontal, float vertical)
        {
            // On ground, obey gravity.
            if (_isOwner)
            {
                if (IsGrounded())
                    rBody.useGravity = true;
            }

            // Call function that deals with player orientation.
            Rotating(horizontal, vertical);

            // Set proper speed.
            Vector2 dir = new Vector2(horizontal, vertical);
            speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
            // This is for PC only, gamepads control speed via analog stick.
           
           // if (behaviourManager.IsSprinting())
           // {
           //     speed = 0.7f;
           // }

            unityAnim.SetFloat("Speed", speed, speedDampTime, Time.deltaTime);
        }

        public bool IsGrounded()
        {
            Ray ray = new Ray(this.transform.position + Vector3.up * 2 * colExtents.x, Vector3.down);
            return Physics.SphereCast(ray, colExtents.x, colExtents.x + 0.2f);
        }


        // Rotate the player to match correct orientation, according to camera and key pressed.
        Vector3 Rotating(float horizontal, float vertical)
        {
            // Get camera forward direction, without vertical component.
            if (_isOwner)
            {
                Vector3 forward = playerCamera.TransformDirection(Vector3.forward);

                // Player is moving on ground, Y component of camera facing is not relevant.
                forward.y = 0.0f;
                forward = forward.normalized;

                // Calculate target direction based on camera forward and direction key.
                Vector3 right = new Vector3(forward.z, 0, -forward.x);
                Vector3 targetDirection;
                targetDirection = forward * vertical + right * horizontal;

                // Lerp current direction to calculated target direction.
                if ((IsMoving() && targetDirection != Vector3.zero))
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                    Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation,
                       turnSmoothing);
                    rBody.MoveRotation(newRotation);
                    SetLastDirection(targetDirection);
                }

                // If idle, Ignore current camera facing and consider last moving direction.
                if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
                {
                    Repositioning();
                }

                return targetDirection;
            }

            return Vector3.zero;
        }

        // Check if the player is moving.
        public bool IsMoving()
        {
            return (h != 0) || (v != 0);
        }

        // Set the last player direction of facing.
        public void SetLastDirection(Vector3 direction)
        {
            lastDirection = direction;
        }

        // Put the player on a standing up position based on last direction faced.
        public void Repositioning()
        {
            if (_isOwner)
            {
                if (lastDirection != Vector3.zero)
                {
                    lastDirection.y = 0;
                    Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
                    Quaternion newRotation = Quaternion.Slerp(rBody.rotation, targetRotation, turnSmoothing);
                    rBody.MoveRotation(newRotation);
                }
            }
        }

        public override void SetJump(bool Jump)
        {
            this.jump = Jump;
        }

        // Collision detection.
        private void OnCollisionStay(Collision collision)
        {
            isColliding = true;
        }
        private void OnCollisionExit(Collision collision)
        {
            isColliding = false;
        }
    }
}
