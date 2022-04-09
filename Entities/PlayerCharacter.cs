using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts
{
    public class PlayerCharacter : MonoBehaviour
    {
        #region Declarations
        CharacterController controller;

        [HideInInspector]
        public float baseMoveSpeed = 1f;

        public float moveSpeed = 1f;
        public float moveSpeedMultiplier = 12f;
        public float slowAmount = 0.33f;
        public float hitEffectsDuration = 1f;

        public Vector3 velocity;
        public float gravity;
        public bool isGrounded;
        public GameObject groundChecker;
        public LayerMask obstacleLayer;
        public float groundDistance;
        public float maxJumpVelocity;

        public GameObject parentObject;

        public SkinnedMeshRenderer renderer;
        [HideInInspector]
        public AttackController attackController;
        public Animator animator;

        public float dashDistance = 5f;
        public float dashDuration = 0.5f;
        public float dashCooldown = 3f;
        bool dashOnCooldown;
        float dashCooldownTimer;
        public bool dashing;

        int jumpState;
        public float doubleJumpReduction = 2f;

        public float raycastOffset = 0.5f;

        Vector3 input;
        public Transform collisionSensorLow, collisionSensorHigh;

        float colliderRadius;

        public float currentHP;
        public float maxHP = 200f;

        public bool inputSuspended, isSlowed, canAttack, canMove, canDoubleJump, isDead, invulnerable, wasAirborneLastFrame;


        bool flickering;
        float flickerTimer;
        #endregion

        void Start()
        {
            attackController = GetComponent<AttackController>();
            controller = GetComponentInParent<CharacterController>();
            colliderRadius = GetComponentInParent<CapsuleCollider>().radius;
            animator = GetComponent<Animator>();

        }

        public void InitializeStats()
        {
            currentHP = maxHP;
            moveSpeed = baseMoveSpeed;

            inputSuspended = false;
            canAttack = true;
            canMove = true;
            isDead = false;
            isSlowed = false;
            invulnerable = false;


            dashCooldownTimer = dashCooldown;
            dashOnCooldown = false;

            HUD.Instance.UpdatePlayerHealthBar();
        }

        void Update()
        {
            HandleDash();

            input = GetInput();

            GroundCheck();

            HandleJump();
            HandleGravity();
            HandleMovement();

            if (flickering && !GameManager.Instance.gamePaused && renderer)
            {
                renderer.enabled = !renderer.enabled;
                HandleFlickerTimer();
            }

            Animate();
        }

        void GroundCheck()
        {
            RaycastHit groundHit;
            if (Physics.Raycast(groundChecker.transform.position, -transform.up * groundDistance, out groundHit, groundDistance, obstacleLayer))
            {
                isGrounded = true;
                jumpState = 0;
            }
            else
            {
                isGrounded = false;
            }

            animator.SetBool("isGrounded", isGrounded);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }


        }

        void HandleMovement()
        {
            if (input == Vector3.zero || !canMove || GameManager.Instance.gamePaused) return;

            var rot = Quaternion.LookRotation(input, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 360f);

            controller.Move(input * (moveSpeed * moveSpeedMultiplier) * Time.deltaTime);
        }

        void HandleJump()
        {
            jumpState = 0;
            canDoubleJump = false;

            // State 1                     
            if ((Input.GetKeyDown(InputManager.Instance.jump_keyboard) || Input.GetKeyDown(InputManager.Instance.jump_gamepad)))
            {
                if (isGrounded)
                {
                      velocity.y = maxJumpVelocity;

                      StartCoroutine(DoubleJumpDelay(0.1f));   
                      wasAirborneLastFrame = true;
                }

                // Double Jump    
                // State 2               
                else
                {
                    if (!isGrounded && canDoubleJump && jumpState == 1)
                    {
                        jumpState = 2;
                        velocity.y = maxJumpVelocity - doubleJumpReduction;
                        canDoubleJump = false;
                        animator.SetBool("DoubleJump", true);
                    }
                }

            }

            // Stop double jump animation once descending again
            if (jumpState == 2 && velocity.y < 0)
                animator.SetBool("DoubleJump", false);

            // Jump cancel
            if (Input.GetButtonUp("Jump"))
            {
                if (velocity.y > 0.01f)
                {
                    velocity.y = 0.01f;
                }
            }

        }

        IEnumerator DoubleJumpDelay(float duration)
        {
            yield return new WaitForSeconds(duration);
            jumpState = 1;
            canDoubleJump = true;
            animator.SetInteger("JumpState", jumpState);
        }

        void HandleGravity()
        {
            if (dashing) return;
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        void HandleDash()
        {
            if (isSlowed) return;

            // Dash cooldown
            //if (dashOnCooldown)
            //{
            //    dashCooldownTimer -= Time.deltaTime;

            //    if (dashCooldownTimer <= 0)
            //    {
            //        dashOnCooldown = false;
            //        dashCooldownTimer = dashCooldown;
            //    }

            //    return;
            //}

            Vector3 dashDest;
            var tempDashDuration = dashDuration;
            var velocityY = velocity.y;

            if ((Input.GetKeyDown(InputManager.Instance.dash_keyboard) || 
                Input.GetKeyDown(InputManager.Instance.dash_gamepad)))
            {
                //print("dash input");

                RaycastHit lowHit, highHit;
                if (Physics.Raycast(collisionSensorLow.position, transform.forward * dashDistance, out lowHit, dashDistance, obstacleLayer))
                {
                    dashDest = transform.position + transform.forward * (lowHit.distance - colliderRadius);
                    var difference = lowHit.distance / dashDistance;
                    tempDashDuration *= difference;

                }
                else if (Physics.Raycast(collisionSensorHigh.position, transform.forward * dashDistance, out highHit, dashDistance, obstacleLayer))
                {
                    dashDest = transform.position + transform.forward * (highHit.distance - colliderRadius);

                    var difference = lowHit.distance / dashDistance;
                    tempDashDuration *= difference;
                }
                else
                {
                    dashDest = transform.position + transform.forward * dashDistance;
                }

                var dashTarget = new Vector3(dashDest.x, transform.position.y, dashDest.z);

                StartDash(dashTarget, tempDashDuration);

                dashOnCooldown = true;
                velocity.y = velocityY;
            }
        }

        void StartDash(Vector3 destination, float duration)
        {
            print($"DashDest: {destination.ToString()} -- Duration: {duration.ToString()}");
            parentObject.transform.DOMove(destination, duration).SetEase(Ease.Linear).OnComplete(DashCompleted);
            dashing = true;
        }

        void DashCompleted()
        {
            print("Dash completed");
            dashing = false;
        }

        void Animate()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, 0f, vertical);

            float velocityZ = Vector3.Dot(movement.normalized, transform.forward);
            float velocityX = Vector3.Dot(movement.normalized, transform.right);

            if (velocityX < 0) velocityX = 0;
            if (velocityZ < 0) velocityZ = 0;

            animator.SetFloat("VelocityZ", velocityZ, 0f, Time.deltaTime);
            animator.SetFloat("VelocityX", velocityX, 0f, Time.deltaTime);
            animator.SetFloat("VelocityY", velocity.y, 0f, Time.deltaTime);

            animator.SetFloat("MoveSpeed", moveSpeed, 0f, Time.deltaTime);

            var attackAnimationSpeed = (1 - attackController.attackDuration) * 2f;

            animator.SetFloat("AttackSpeed", attackAnimationSpeed, 0f, Time.deltaTime);
            animator.SetInteger("JumpState", jumpState);
            animator.SetBool("Dashing", dashing);
        }


        public Vector3 GetInput()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 315, 0));
            return matrix.MultiplyPoint3x4(input);
        }


        public void TakeDamage(float amount, bool shouldFlicker)
        {
            if (currentHP <= 0 || isDead)
            {
                currentHP = 0;
                HUD.Instance.UpdatePlayerHealthBar();
                return;
            }

            if (invulnerable) return;

            currentHP -= (int)(amount);

            if (shouldFlicker)
            {
                StartFlickering();
            }

            HUD.Instance.UpdatePlayerHealthBar();

            if (currentHP <= 0 && !isDead)
            {
                StartCoroutine(PlayerDeath());
            }
        }

        public void Heal(float amount)
        {
            currentHP += (int)(amount);
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }

            HUD.Instance.UpdatePlayerHealthBar();
        }

        IEnumerator PlayerDeath()
        {
            isDead = true;
            canMove = false;
            canAttack = false;

            for (int i = 0; i < 500f; i++)
            {
                var chunk = Instantiate(CrystalController.Instance.crystalChunkPrefab, transform.position, Quaternion.identity);
                var rb = chunk.GetComponent<Rigidbody>();
                VFX.Instance.Explode(rb, 0.1f, 0.1f, 50f);
            }

            AudioManager.Instance.Play("Shatter-1");

            Destroy(renderer.gameObject);
            GameManager.Instance.StartGameOver();

            yield return new WaitForSeconds(2f);

            Destroy(parentObject);

        }

        public void StartFlickering()
        {
            flickerTimer = hitEffectsDuration;
            flickering = true;
        }
        
        void HandleFlickerTimer()
        {
            if (renderer == null) return;

            flickerTimer -= Time.deltaTime;

            if (flickerTimer <= 0)
            {
                flickering = false;
                renderer.enabled = true;
            }
        }

    }
}
