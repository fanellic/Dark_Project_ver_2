using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class FootCorrector : MonoBehaviour
{
    public bool Active = true;
    [SerializeField]
    private Transform 
        jennyOriginBase,
        jennySpine,
        jennyRefSpine,
        BlenderLeftFootIK, 
        BlenderRightFootIK, 
        BlenderLeftKneeIK, 
        BlenderRightKneeIK, 
        unityLeftFootIK, 
        unityRightFootIK, 
        unityLeftKneeIK, 
        unityRightKneeIK, 
        leftFoot, 
        rightFoot, 
        leftToeEnd, 
        rightToeEnd,
        pelvisRoot;

    [Header("Settings")]
    [Range(-10f, 10f)]
    public float
        spineYAdjust = .05f,
        spineZAdjust = .2f,
        heelHeight = .15f;

    private float hintSpeed = 7f;

    [Range(0f, 100f)]
    public float
        pelvisSpeed = 10f,
        addMovementSpeed = 13f,
        footCorMovementSpeed = 35f,
        dumby = 2f;
    public float smoothTime = .35f;
    [SerializeField] 
        private float footCorRotationSpeed = 10.0f, 
        footCorRevRotSpeedDiv = 2f;
    public LayerMask layerMask;
    [Range(0f, 7f)]
    public float groundAdditive = 4.0f;
    [Range(0f, 1f)]
    public float maxDistanceTester = 1.0f;
    public JennyController jennyController;
    public Animator jennyAnimator;

    private Vector3 UpwardsSlopeNormalAverage;
    private Quaternion currentSpineAngle;
    private Vector3 currentSpineNormal = Vector3.up;
    private float maxAdditiveXRotation = 10f;

    //Items needed for Skirt Targeting
    [HideInInspector]
    public Vector3 leftFootHitNormalRef, rightFootHitNormalRef;

    //Private
    private Quaternion 
        currentLeftAngle, 
        currentRightAngle;
    private Vector3
        currentLocalPelvisPosition,
        currentLeftFootPosition,
        currentRightFootPosition;

    private float 
        currentSpineY,
        currentSpineZ,
        currentPelvisYAdditive = 0,
        currentHintXAdd = 0,
        currentHintYRightAdd = 0,
        currentHintYLeftAdd = 0,
        currentLeftFootYAdditive = 0, 
        currentRightFootYAdditive = 0,
        currentSpineAdditiveRotation = 0f;

    void Start()
    {
        currentLeftAngle = unityLeftFootIK.rotation;
        currentRightAngle = unityRightFootIK.rotation;
        currentLeftFootPosition = unityLeftFootIK.position;
        currentRightFootPosition = unityRightFootIK.position;
        currentLocalPelvisPosition = pelvisRoot.localPosition;
    }

    private void LateUpdate()
    {
        pleaseFootCorrector(jennyController.isGrounded, jennyController.movementVectorRef);
    }
    
    private float calculateYHintAdditive(float additive, RaycastHit footHit)
    {
        float dotProduct = Vector3.Dot(transform.forward, footHit.normal);

        if (dotProduct < -0.1f)
        {
            // The character is moving uphill.
            return (Mathf.Pow(dumby, additive));
        }
        else if (dotProduct > 0.1f)
        {
            // The character is moving downhill.
            return (-Mathf.Pow(dumby, additive));
        }
        else
        {
            // Flat or no significant slope relative to movement.
            return (0);
        }
    }

    private void pleaseFootCorrector(bool isGrounded, Vector2 movementVector)
    {
        //Set up ray cast variables
        Vector3 leftRayDirection = Vector3.down;
        Vector3 rightRayDirection = Vector3.down;
        float maxDistance = .5f + groundAdditive;

        //Left Foot
        RaycastHit leftFootHit;
        Vector3 leftFootOrigin = new Vector3(BlenderLeftFootIK.position.x, BlenderLeftFootIK.position.y + groundAdditive, BlenderLeftFootIK.position.z);
        float leftFootHitMaxDistance = maxDistanceTester + groundAdditive;

        // Right Foot
        RaycastHit rightFootHit;
        Vector3 rightFootOrigin = new Vector3(BlenderRightFootIK.position.x, BlenderRightFootIK.position.y + groundAdditive, BlenderRightFootIK.position.z);
        float rightFootHitMaxDistance = maxDistanceTester + groundAdditive;



        //Case 1: Both feet are touching the ground (not necessarily the same ground with the same slope)
        if (Active && isGrounded && Physics.Raycast(leftFootOrigin, leftRayDirection, out leftFootHit, leftFootHitMaxDistance, layerMask) && Physics.Raycast(rightFootOrigin, rightRayDirection, out rightFootHit, rightFootHitMaxDistance, layerMask) && leftFootHit.transform.tag == "Walkable" && rightFootHit.transform.tag == "Walkable")
        {
            //Skirt Targeting needs this
            leftFootHitNormalRef = leftFootHit.normal;
            rightFootHitNormalRef = rightFootHit.normal;


            if (jennyAnimator.GetFloat("Speed") != 0)
            {
                Debug.DrawLine(leftFootOrigin, leftFootHit.point, Color.red);
                Debug.DrawLine(rightFootOrigin, rightFootHit.point, Color.red);
            }
            else
            {
                Debug.DrawLine(leftFootOrigin, leftFootHit.point, Color.red);
                Debug.DrawLine(rightFootOrigin, rightFootHit.point, Color.red);
            }
            //adjust left and right foot IK target angles
            Quaternion leftFootNewAngle = Quaternion.FromToRotation(transform.up, leftFootHit.normal) * BlenderLeftFootIK.rotation;
            unityLeftFootIK.rotation = Quaternion.Lerp(currentLeftAngle, leftFootNewAngle, Time.deltaTime * footCorRotationSpeed);
            currentLeftAngle = unityLeftFootIK.rotation;
            Quaternion rightFootNewAngle = Quaternion.FromToRotation(transform.up, rightFootHit.normal) * BlenderRightFootIK.rotation;
            unityRightFootIK.rotation = Quaternion.Lerp(currentRightAngle, rightFootNewAngle, Time.deltaTime * footCorRotationSpeed);
            currentRightAngle = unityRightFootIK.rotation;


            //set additive variables
            float newLeftFootYAdditive = 0f;
            float newRightFootYAdditive = 0f;
            float newLeftHintYAdditive = 0f;
            float newRightHintYAdditive = 0f;
            float leftFootYAdditive = 0;
            float rightFootYAdditive = 0;
            float newPelvisYAdditive = 0f;
            float hintOffsetX = 0;

            float right_foot_y_delta = rightFootHit.point.y - jennyOriginBase.transform.position.y;
            float left_foot_y_delta = leftFootHit.point.y - jennyOriginBase.transform.position.y;

            if (right_foot_y_delta < left_foot_y_delta)
            {
                //print("right lower"); print(rightFootHeight);
                float slopeAngleLeft = Vector3.Angle(leftFootHit.normal, Vector3.up);
                var footSlopeOffsetLeft = 0.001f * slopeAngleLeft - 0.025f;

                newPelvisYAdditive = right_foot_y_delta + footSlopeOffsetLeft; // + rightFootHeight;
                newLeftFootYAdditive = left_foot_y_delta - right_foot_y_delta;
                newLeftHintYAdditive = calculateYHintAdditive(newLeftFootYAdditive, leftFootHit);
                newRightHintYAdditive = calculateYHintAdditive(newRightFootYAdditive, rightFootHit);
            }
            else if (right_foot_y_delta > left_foot_y_delta)
            {
                //print("left lower");
                float slopeAngleRight = Vector3.Angle(rightFootHit.normal, Vector3.up);
                var footSlopeOffsetRight = 0.001f * slopeAngleRight - 0.025f;
                newPelvisYAdditive = left_foot_y_delta + footSlopeOffsetRight; // + leftFootHeight;
                newRightFootYAdditive = right_foot_y_delta - left_foot_y_delta;
                newLeftHintYAdditive = calculateYHintAdditive(newLeftFootYAdditive, leftFootHit);
                newRightHintYAdditive = calculateYHintAdditive(newRightFootYAdditive, rightFootHit);
            }
            else
            {
                //print(rightFootHeight);
                if (leftFootHit.normal == new Vector3(0f, 1f, 0))
                {
                    newPelvisYAdditive = -.02f;
                }
                else
                {
                    //print(rightFootHeight);
                    float slopeAngleRight = Vector3.Angle(rightFootHit.normal, Vector3.up);
                    var footSlopeOffsetRight = 0.001f * slopeAngleRight - 0.025f;
                    newPelvisYAdditive = right_foot_y_delta + footSlopeOffsetRight; // + rightFootHeight;
                    newLeftFootYAdditive = 0;
                    newRightFootYAdditive = 0;
                }
            }


            if (jennyAnimator.GetFloat("Speed") > .0001)
            {
                hintOffsetX = .4f;
            }

            else
            {
                hintOffsetX = 0f;
            }

            float dotRightProduct = Vector3.Dot(transform.forward, rightFootHit.normal);
            float dotLeftProduct = Vector3.Dot(transform.forward, leftFootHit.normal);
            float speedAdjustSpine = 4;

            Vector3 targetNormal = Quaternion.AngleAxis(-12f, transform.right) * Vector3.up;

            bool isUphill = false;

            if ((dotRightProduct+ dotLeftProduct) < -0.1f && (jennyAnimator.GetFloat("Speed") > .0001))
            {
                float spineY = Mathf.Lerp(currentSpineY, spineYAdjust, speedAdjustSpine * Time.deltaTime);
                float spineZ = Mathf.Lerp(currentSpineZ, spineZAdjust, speedAdjustSpine * Time.deltaTime);
                jennySpine.localPosition = new Vector3(jennySpine.localPosition.x, jennySpine.localPosition.y - spineY, jennySpine.localPosition.z - spineZ);
                currentSpineY = spineY;
                currentSpineZ = spineZ;
                targetNormal = Quaternion.AngleAxis(40f, transform.right) * (rightFootHit.normal + leftFootHit.normal).normalized;
                isUphill = true;
            }
            else
            {
                float spineY = Mathf.Lerp(currentSpineY, 0, speedAdjustSpine * Time.deltaTime);
                float spineZ = Mathf.Lerp(currentSpineZ, 0, speedAdjustSpine * Time.deltaTime);
                jennySpine.localPosition = new Vector3(jennySpine.localPosition.x, jennySpine.localPosition.y - spineY, jennySpine.localPosition.z - spineZ);
                currentSpineY = spineY;
                currentSpineZ = spineZ;
            }
            Vector3 averageNormal = (leftFootHit.normal + rightFootHit.normal).normalized;
            float slopeAngle = Vector3.Angle(averageNormal, Vector3.up);
            maxAdditiveXRotation = .15f * slopeAngle + 2.25f; 

            float targetRotation = isUphill ? maxAdditiveXRotation : 0f;
            currentSpineAdditiveRotation = Mathf.Lerp(currentSpineAdditiveRotation, targetRotation, Time.deltaTime * speedAdjustSpine*.7f);

            // 3. Apply the rotation additively to the bone's local space
            Quaternion additiveQuat = Quaternion.Euler(currentSpineAdditiveRotation, 0f, 0f);
            jennySpine.localRotation = jennySpine.localRotation * additiveQuat;

            currentHintXAdd = Mathf.Lerp(currentHintXAdd, hintOffsetX, hintSpeed * Time.deltaTime);
            currentHintYRightAdd = Mathf.Lerp(currentHintYRightAdd, newRightHintYAdditive, hintSpeed * Time.deltaTime);
            currentHintYLeftAdd = Mathf.Lerp(currentHintYLeftAdd, newLeftHintYAdditive, hintSpeed * Time.deltaTime);

            unityLeftKneeIK.position = new Vector3(BlenderLeftKneeIK.position.x, BlenderLeftKneeIK.position.y + currentHintYLeftAdd, BlenderLeftKneeIK.position.z);
            unityLeftKneeIK.localPosition = new Vector3(unityLeftKneeIK.localPosition.x - currentHintXAdd, unityLeftKneeIK.localPosition.y, unityLeftKneeIK.localPosition.z);

            unityRightKneeIK.position = new Vector3(BlenderRightKneeIK.position.x, BlenderRightKneeIK.position.y + currentHintYRightAdd, BlenderRightKneeIK.position.z);
            unityRightKneeIK.localPosition = new Vector3(unityRightKneeIK.localPosition.x + currentHintXAdd, unityRightKneeIK.localPosition.y, unityRightKneeIK.localPosition.z);

            currentPelvisYAdditive = Mathf.Lerp(currentPelvisYAdditive, newPelvisYAdditive, pelvisSpeed * Time.deltaTime);
            pelvisRoot.localPosition = new Vector3(0, currentPelvisYAdditive, 0);

            leftFootYAdditive = Mathf.Lerp(currentLeftFootYAdditive, newLeftFootYAdditive, addMovementSpeed * Time.deltaTime);
            rightFootYAdditive = Mathf.Lerp(currentRightFootYAdditive, newRightFootYAdditive, addMovementSpeed * Time.deltaTime);
            Vector3 leftFootNewPosition = new Vector3(BlenderLeftFootIK.position.x, BlenderLeftFootIK.position.y + leftFootYAdditive, BlenderLeftFootIK.position.z);
            Vector3 rightFootNewPosition = new Vector3(BlenderRightFootIK.position.x, BlenderRightFootIK.position.y + rightFootYAdditive, BlenderRightFootIK.position.z);

            unityLeftFootIK.position = Vector3.MoveTowards(currentLeftFootPosition, leftFootNewPosition, footCorMovementSpeed * Time.deltaTime);
            unityRightFootIK.position = Vector3.MoveTowards(currentRightFootPosition, rightFootNewPosition, footCorMovementSpeed * Time.deltaTime);

            //set the current controllerCenter, feet IK target positions and Foot additives
            currentLeftFootYAdditive = leftFootYAdditive;
            currentRightFootYAdditive = rightFootYAdditive;
            currentLeftFootPosition = unityLeftFootIK.position;
            currentRightFootPosition = unityRightFootIK.position;

            //currentPelvisYAdditive = pelvisYAdditive;
            currentLocalPelvisPosition = pelvisRoot.localPosition;
        }
        else
        {
            //Skirt Targeting needs this
            leftFootHitNormalRef = new Vector3(0f, 0f, 0f);
            rightFootHitNormalRef = new Vector3(0f, 0f, 0f);
            //adjust center and feet position when falling
            unityLeftFootIK.position = BlenderLeftFootIK.position;
            unityRightFootIK.position = BlenderRightFootIK.position;
            currentLeftFootPosition = unityLeftFootIK.position;
            currentRightFootPosition = unityRightFootIK.position;
            currentLeftFootYAdditive = 0f;
            currentRightFootYAdditive = 0f;

            jennySpine.position = jennyRefSpine.position;
            jennySpine.rotation = jennyRefSpine.rotation;

            currentPelvisYAdditive = 0f;
            currentLocalPelvisPosition = new Vector3(0,0,0);
            pelvisRoot.localPosition = new Vector3(0, 0, 0);
            float hintOffsetX = 0f;

            if (jennyAnimator.GetFloat("Speed") > .0001)
            {
                hintOffsetX = .4f;
            }

            currentHintXAdd = 0;
            currentHintYRightAdd = 0;
            currentHintYLeftAdd = 0;

            unityLeftKneeIK.position = new Vector3(BlenderLeftKneeIK.position.x, BlenderLeftKneeIK.position.y, BlenderLeftKneeIK.position.z);
            unityRightKneeIK.position = new Vector3(BlenderRightKneeIK.position.x, BlenderRightKneeIK.position.y, BlenderRightKneeIK.position.z);

            unityLeftKneeIK.localPosition = new Vector3(unityLeftKneeIK.localPosition.x - hintOffsetX, unityLeftKneeIK.localPosition.y, unityLeftKneeIK.localPosition.z);
            unityRightKneeIK.localPosition = new Vector3(unityRightKneeIK.localPosition.x + hintOffsetX, unityRightKneeIK.localPosition.y, unityRightKneeIK.localPosition.z);

            //adjust both feet angles for smooth transitions for falling
            unityLeftFootIK.rotation = Quaternion.Lerp(currentLeftAngle, BlenderLeftFootIK.rotation, Time.deltaTime * footCorRotationSpeed / footCorRevRotSpeedDiv);
            currentLeftAngle = unityLeftFootIK.rotation;
            unityRightFootIK.rotation = Quaternion.Lerp(currentRightAngle, BlenderRightFootIK.rotation, Time.deltaTime * footCorRotationSpeed / footCorRevRotSpeedDiv);
            currentRightAngle = unityRightFootIK.rotation;
        }
    }

}
