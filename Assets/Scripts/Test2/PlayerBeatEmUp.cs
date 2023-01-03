using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBeatEmUp : MonoBehaviour
{
    // Anim
    [SerializeField]
    Transform spriteTarget;
    Animator animator;
    SpriteRenderer spriteRenderer;

    // Input
    CharacterController characterController;
    PlayerInput playerInput;

    // Anim
    int animIsGroundedHash;
    int animStateHash;
    int animJumpHash;
    int animAirSpeedYHash;
    int animAttack1Hash;
    int animAttack2Hash;
    int animAttack3Hash;

    // Movement
    [SerializeField]
    float moveSpeed;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed = false;

    // Gravity
    float gravity = -9.8f;
    float groundedGravity = -0.5f;

    // Jump
    bool isJumpPressed = false;
    float initialJumpVelocity;
    [SerializeField]
    float maxJumpHeight = 4.0f;
    [SerializeField]
    float maxJumpTime = 0.75f;
    bool isJumping = false;

    // Attack
    bool isAttackPressed = false;
    bool isAttacking = false;
    int attackCount = 0;
    [SerializeField]
    float attack1Duration = 0.0f;
    [SerializeField]
    float attack2Duration = 0.0f;
    [SerializeField]
    float attack3Duration = 0.0f;


    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();

        animator = spriteTarget.GetComponent<Animator>();
        spriteRenderer = spriteTarget.GetComponent<SpriteRenderer>();

        animIsGroundedHash = Animator.StringToHash("Grounded");
        animStateHash = Animator.StringToHash("AnimState");
        animJumpHash = Animator.StringToHash("Jump");
        animAirSpeedYHash = Animator.StringToHash("AirSpeedY");
        animAttack1Hash = Animator.StringToHash("Attack1");
        animAttack2Hash = Animator.StringToHash("Attack2");
        animAttack3Hash = Animator.StringToHash("Attack3");

        playerInput.CharacterControls.Move.started += OnMovementInput;
        playerInput.CharacterControls.Move.performed += OnMovementInput;
        playerInput.CharacterControls.Move.canceled += OnMovementInput;

        playerInput.CharacterControls.Jump.started += OnJump;
        playerInput.CharacterControls.Jump.canceled += OnJump;
        SetupJumpVariable();

        playerInput.CharacterControls.Attack.started += OnAttack;
        playerInput.CharacterControls.Attack.canceled += OnAttack;
    }

    void SetupJumpVariable()
    {
        float timeToApex = maxJumpTime * 0.5f;
        gravity = (-2.0f * maxJumpHeight) / Mathf.Pow(timeToApex, 2.0f);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        isMovementPressed = currentMovementInput.x != 0.0f || currentMovementInput.y != 0.0f;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        isAttackPressed = context.ReadValueAsButton();
    }

    void HandleAnimation()
    {
        bool animIsGrounded = animator.GetBool(animIsGroundedHash);
        int animState = animator.GetInteger(animStateHash);
        bool isRunning = animState == 1;

        animator.SetFloat(animAirSpeedYHash, currentMovement.y);

        if (animIsGrounded != characterController.isGrounded)
        {
            animator.SetBool(animIsGroundedHash, characterController.isGrounded);
        }

        if (isRunning != isMovementPressed)
        {
            animator.SetInteger(animStateHash, isMovementPressed ? 1 : 0);
        }

        if (currentMovementInput.x > 0 && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
        }
        else if (currentMovementInput.x < 0 && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
        }
    }

    void HandleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            isJumping = true;
            currentMovement.y = initialJumpVelocity / 2;
            animator.SetTrigger(animJumpHash);
        }
        else if (isJumping && characterController.isGrounded && !isJumpPressed)
        {
            isJumping = false;
        }
    }

    void HandleMovement()
    {
        if (isMovementPressed)
        {
            currentMovement.x = currentMovementInput.x * moveSpeed;
            currentMovement.z = currentMovementInput.y * moveSpeed;
        }
        else
        {
            currentMovement.x = 0.0f;
            currentMovement.z = 0.0f;
        }
    }

    void HandleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if (characterController.isGrounded)
        {
            currentMovement.y = groundedGravity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * (isFalling ? fallMultiplier : 1.0f) * Time.deltaTime);
            float nextYVelovity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelovity;
        }
    }

    void HandleAttack()
    {

    }

    void Update()
    {

        if (!isAttacking)
        {
            HandleMovement();
        }

        characterController.Move(currentMovement * Time.deltaTime);

        HandleGravity();
        HandleJump();

        HandleAnimation();
    }
}
