using UnityEngine;

public class JennyController : MonoBehaviour
{
    private CharacterController jennyController;
    private Animator jennyAnimator;
    public float smoothTime = .35f;

    private float _currentVelocity;
    private float gravity = -9.81f;
    private float gravityVelocity;
    [SerializeField] private float gravityMultiplier = 3.0f;
    public LayerMask layerMask;

    //skirt target will access this:
    public Vector3 refTransformForward;

    [HideInInspector]
    public Vector2 movementVectorRef;

    public bool isGrounded;

    public int cameraViewY = -1, cameraViewX = -1;

    [Range(0f, 100f)]
    public float MovementSpeed = 13f;

    [Range(0f, 1f)]
    public float groundSphereCheck = .5f;

    void Start()
    {
        jennyController = GetComponent<CharacterController>();
        jennyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    public void Move(Vector2 movementVector)
    {
        refTransformForward = transform.forward;
        movementVectorRef = movementVector;
        //Check to see if player is grounded and set gravity velocity
        isGrounded = Physics.CheckSphere(transform.position, groundSphereCheck, layerMask);


        gravityVelocity += gravity * gravityMultiplier * Time.deltaTime;
        if (isGrounded && gravityVelocity <= 0.0f)
        {
            gravityVelocity = -2.0f;
        }

        //Check to see if there's any movement to reset animator
        if (movementVector.sqrMagnitude == 0)
        {
            jennyAnimator.SetFloat("Speed", 0f);
            if (!isGrounded)
            {
                Vector3 freeFallVelocity = new Vector3(0, gravityVelocity, 0).normalized;
                freeFallVelocity = freeFallVelocity * MovementSpeed * Time.deltaTime;
                jennyController.Move(freeFallVelocity);
            }
            return;
        }
        //Rotate Character using inputs and camera view
        var targetAngle = Mathf.Atan2(movementVector.x * cameraViewX, movementVector.y * cameraViewY) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, smoothTime);
        transform.localRotation = Quaternion.Euler(0, angle, 0);

        //Set velocity using inputs, camera view and gravity velocity
        Vector3 velocity = new Vector3(movementVector.x * cameraViewX, 0, movementVector.y * cameraViewY).normalized;
        velocity.y = gravityVelocity;
        velocity = velocity * MovementSpeed * Time.deltaTime;

        //Move Character based on velocity and set animator speed
        jennyController.Move(velocity);
        jennyAnimator.SetFloat("Speed", velocity.magnitude);
    }
}
