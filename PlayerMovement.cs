using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float runSpeed = 6;

    [Header("Stamina")]
    [SerializeField] private float maxStamina = 5f;
    [SerializeField] private float staminaDrainTime = 5f;
    [SerializeField] private float staminaRegenTime = 5f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("Audio")]
    private AudioSource footstepSource;
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.3f;

    private float currentStamina;
    private float staminaRegenTimer;
    private bool isRunning;

    private float footstepTimer;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private EntityHolder entityHolder;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public bool IsRunning => isRunning;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        currentStamina = maxStamina;
        entityHolder = GetComponent<EntityHolder>();

        if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }
    }

    public void Update()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        GetInput();
        UpdateStamina();
        UpdateFootsteps();
    }

    public void FixedUpdate()
    {
        if (!GameManager.Instance.IsPlaying)
        {
            return;
        }

        Move();
    }

    private void GetInput()
    {
        moveDirection = Vector3.zero;

        if (Keyboard.current.aKey.isPressed)
        {
            moveDirection -= transform.right;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveDirection += transform.right;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            moveDirection += transform.forward;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveDirection -= transform.forward;
        }

        moveDirection = moveDirection.normalized;

        bool wantsToRun = Keyboard.current.leftShiftKey.isPressed;
        bool canRun = !entityHolder.IsHoldingEntity();
        isRunning = wantsToRun && canRun && currentStamina > 0f && moveDirection != Vector3.zero;
    }

    private void UpdateStamina()
    {
        if (isRunning)
        {
            currentStamina -= (maxStamina / staminaDrainTime) * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);
            staminaRegenTimer = staminaRegenDelay;
        }
        else
        {
            if (staminaRegenTimer > 0f)
            {
                staminaRegenTimer -= Time.deltaTime;
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += (maxStamina / staminaRegenTime) * Time.deltaTime;
                currentStamina = Mathf.Min(currentStamina, maxStamina);
            }
        }
    }

    private void UpdateFootsteps()
    {
        if (footstepSource == null || footstepClip == null)
        {
            return;
        }

        if (moveDirection == Vector3.zero)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0f)
        {
            footstepSource.PlayOneShot(footstepClip);

            footstepTimer = isRunning ? runStepInterval : walkStepInterval;
        }
    }

    private void Move()
    {
        float currentSpeed = isRunning ? runSpeed : speed;
        Vector3 movement = moveDirection * currentSpeed * Time.fixedDeltaTime;

        if (movement == Vector3.zero) return;

        RaycastHit hit;
        if (rb.SweepTest(movement.normalized, out hit, movement.magnitude, QueryTriggerInteraction.Ignore))
        {
            rb.MovePosition(rb.position + movement.normalized * (hit.distance - 0.01f));
        }
        else
        {
            rb.MovePosition(rb.position + movement);
        }
    }
}
