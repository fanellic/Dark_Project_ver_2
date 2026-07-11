using UnityEngine;

public class footPrintStamp : MonoBehaviour
{

    [Header("References")]
    public MeshRenderer footprintRenderer;
    public Transform footprintQuad;

    [Header("Raycast")]
    public LayerMask snowLayer;

    [Header("End of Toe")]
    public Transform toe;

    [Header("Settings")]
    //public float plantVelocityThreshold = 0.02f;

    //public float stampCooldown = 0.25f;

    Vector3 lastPosition;

    float cooldownTimer;

    bool planted;

    void Start()
    {
        lastPosition = transform.position;

        footprintRenderer.enabled = false;
    }


    private void FixedUpdate()
    {
        cooldownTimer -= Time.deltaTime;

        //Foot Velocity
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;

        lastPosition = transform.position;

        float verticalVelocity =
            Mathf.Abs(velocity.y);

        //Foot is planted stably
        //bool stablePlant = verticalVelocity < plantVelocityThreshold;


        //Set up ray cast variables
        Vector3 rayDirection = Vector3.down;
        RaycastHit footHit;
        Vector3 footOrigin = transform.position;
        float footHitMaxDistance = 2f;

        RaycastHit toeHit;
        Vector3 toeOrigin = toe.transform.position;
        float toeHitMaxDistance = 1f;

        bool hitSnow = false;
        if (Physics.Raycast(footOrigin, rayDirection, out footHit, footHitMaxDistance, snowLayer) && Physics.Raycast(toeOrigin, rayDirection, out toeHit, toeHitMaxDistance, snowLayer))
        {
            if (toeHit.distance < .25f && footHit.distance < 1.52f)
            {
                hitSnow = true;
            }

            SnowFootPrintManager.Instance.AddFootprint(
               footprintQuad.position,
               footprintQuad.localScale,
               footprintQuad.rotation
           );
        }

        if (hitSnow && !planted && cooldownTimer <= 0f)
        {
            planted = true;
            

            //cooldownTimer = stampCooldown;

            //footprintRenderer.enabled = true;

            
            SnowFootPrintManager.Instance.AddFootprint(
                footprintQuad.position,
                footprintQuad.localScale,
                footprintQuad.rotation
            );
        }

        // FOOT LIFTED
        if (!hitSnow)
        {
            planted = false;
            //footprintRenderer.enabled = false;
        }
    }
}
 
