using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public partial class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float acceleration = 20f;
    public float deceleration = 30f;

    [Header("Signals (Z/X)")]
    [SerializeField] private float crossDuration = 0.6f;
    [SerializeField] private float stopDuration = 0.6f;

    [Header("Hit Reaction (bị xe tông)")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float stunDuration = 1.5f;

    // Animator params
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIsCrossing = Animator.StringToHash("IsCrossing");
    private static readonly int AnimIsStopping = Animator.StringToHash("IsStopping");

    // references
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    // Input
    private InputAction moveAction;
    private InputAction crossAction;
    private InputAction stopAction;

    // coroutines
    private Coroutine crossRoutine;
    private Coroutine stopRoutine;

    // state
    private bool isStunned = false;

    // ================= CORE INIT =================

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        SetupInputActions();
    }

    private void OnEnable()
    {
        moveAction?.Enable();

        if (crossAction != null)
        {
            crossAction.performed += OnCrossPerformed;
            crossAction.Enable();
        }

        if (stopAction != null)
        {
            stopAction.performed += OnStopPerformed;
            stopAction.Enable();
        }
    }

    private void OnDisable()
    {
        moveAction?.Disable();

        if (crossAction != null)
        {
            crossAction.performed -= OnCrossPerformed;
            crossAction.Disable();
        }

        if (stopAction != null)
        {
            stopAction.performed -= OnStopPerformed;
            stopAction.Disable();
        }
    }

    private void SetupInputActions()
    {
        // ------- MOVE -------
        moveAction = new InputAction(name: "Move", type: InputActionType.Value);
        var axis = moveAction.AddCompositeBinding("1DAxis");
        axis.With("Negative", "<Keyboard>/a");
        axis.With("Negative", "<Keyboard>/leftArrow");
        axis.With("Positive", "<Keyboard>/d");
        axis.With("Positive", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick/x");

        // ------- Z – Cross -------
        crossAction = new InputAction("Cross", InputActionType.Button);
        crossAction.AddBinding("<Keyboard>/z");
        crossAction.AddBinding("<Gamepad>/buttonSouth");

        // ------- X – Stop -------
        stopAction = new InputAction("Stop", InputActionType.Button);
        stopAction.AddBinding("<Keyboard>/x");
        stopAction.AddBinding("<Gamepad>/buttonEast");
    }

    protected IEnumerator SetBoolForSeconds(int boolHash, float seconds)
    {
        animator.SetBool(boolHash, true);
        yield return new WaitForSeconds(seconds);
        animator.SetBool(boolHash, false);
    }
}
