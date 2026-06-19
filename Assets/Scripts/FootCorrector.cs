using UnityEngine;

public class FootCorrector : MonoBehaviour
{
    [SerializeField]
    private Transform 
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
    [Range(0f, 100f)]
    public float
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


    //Items needed for Skirt Targeting
    [HideInInspector]
    public Vector3 leftFootHitNormalRef, rightFootHitNormalRef;

    //Private
    private Quaternion 
        currentLeftAngle, 
        currentRightAngle;
    private Vector3
        currentLeftFootPosition,
        currentRightFootPosition,
        currentPelvisRootPosition,
        pelvisRootInitialPosition;

    private float 
        currentLeftFootYAdditive = 0, 
        currentRightFootYAdditive = 0;

    void Start()
    {
        currentLeftAngle = unityLeftFootIK.rotation;
        currentRightAngle = unityRightFootIK.rotation;
        currentLeftFootPosition = unityLeftFootIK.position;
        currentRightFootPosition = unityRightFootIK.position;
    }

    private void LateUpdate()
    {
        //mainFootCorrector(jennyController.isGrounded, jennyController.movementVectorRef);
        revampedFootCorrector(jennyController.isGrounded, jennyController.movementVectorRef);
    }

    private float calculateFootDisToHit(RaycastHit footHit, Vector3 blenderFootPosition)
    {
        float disToHit = (footHit.point.y - blenderFootPosition.y);
        return (disToHit);
    }

    private float calculateFootHeight(RaycastHit footHit, Transform toeEnd, Transform foot, Vector3 blenderFootPosition)
    {
        Vector3 sum = (foot.transform.forward + foot.transform.up);
        Vector3 heelPosition = foot.position + sum * 1.15f;
        float lowestPointOnFootY = Mathf.Min(heelPosition.y, (toeEnd.position.y));
        float shoeSize = (foot.transform.position.y - lowestPointOnFootY);
        float footHeight = shoeSize - (lowestPointOnFootY - footHit.point.y);
        return (footHeight);
    }

    
    private float calculateYHintAdditive(float additive, RaycastHit footHit)
    {
        float dotProduct = Vector3.Dot(transform.forward, footHit.normal);

        if (dotProduct < -0.1f)
        {
            // The character is moving uphill.
            addMovementSpeed = 15f;
            return (Mathf.Pow(dumby, additive));
        }
        else if (dotProduct > 0.1f)
        {
            // The character is moving downhill.
            addMovementSpeed = 2.25f;
            return (0);
        }
        else
        {
            // Flat or no significant slope relative to movement.
            addMovementSpeed = 15f;
            return (0);
        }
    }


    private void revampedFootCorrector(bool isGrounded, Vector2 movementVector)
    {
        //Set up ray cast variables
        Vector3 leftRayDirection = Vector3.down;
        Vector3 rightRayDirection = Vector3.down;

        //Left Foot
        RaycastHit leftFootHit;
        Vector3 leftFootOrigin = new Vector3(leftFoot.position.x, leftFoot.position.y, leftFoot.position.z);

        // Right Foot
        RaycastHit rightFootHit;
        Vector3 rightFootOrigin = new Vector3(rightFoot.position.x, rightFoot.position.y, rightFoot.position.z);


        //Case 1: Both feet are touching the ground (not necessarily the same ground with the same slope)
        if (isGrounded && Physics.Raycast(leftFootOrigin, leftRayDirection, out leftFootHit, maxDistanceTester, layerMask) && Physics.Raycast(rightFootOrigin, rightRayDirection, out rightFootHit, maxDistanceTester, layerMask))
        {
            //Skirt Targeting needs this
            leftFootHitNormalRef = leftFootHit.normal;
            rightFootHitNormalRef = rightFootHit.normal;

            print("hit"); 
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

            //get left and right calculateFootDisToHit and footHeight
            float leftFootDisToHit = calculateFootDisToHit(leftFootHit, BlenderLeftFootIK.transform.position);
            float rightFootDisToHit = calculateFootDisToHit(rightFootHit, BlenderRightFootIK.transform.position);
            float leftFootHeight = calculateFootHeight(leftFootHit, leftToeEnd, leftFoot, BlenderLeftFootIK.transform.position);
            float rightFootHeight = calculateFootHeight(rightFootHit, rightToeEnd, rightFoot, BlenderRightFootIK.transform.position);

            //determine controllerCenterAdditive and which foot needs an offset
            float controllerCenterAdditive = 0;
            if (leftFootHit.point.y > rightFootHit.point.y)
            {
                controllerCenterAdditive = -rightFootDisToHit - rightFootHeight;
                newLeftFootYAdditive = leftFootDisToHit + leftFootHeight + controllerCenterAdditive;
                newLeftHintYAdditive = calculateYHintAdditive(newLeftFootYAdditive, leftFootHit);
            }
            else if (rightFootHit.point.y > leftFootHit.point.y)
            {
                controllerCenterAdditive = -leftFootDisToHit - leftFootHeight;
                newRightFootYAdditive = rightFootDisToHit + rightFootHeight + controllerCenterAdditive;
                newRightHintYAdditive = calculateYHintAdditive(newRightFootYAdditive, rightFootHit);
            }
            else
            {

                addMovementSpeed = 15f;
                if (leftFootHit.normal == new Vector3(0f, 1f, 0))
                {
                    controllerCenterAdditive = 0;
                }
                else
                {
                    controllerCenterAdditive = Mathf.Abs(leftFootDisToHit) - leftFootHeight;
                }
            }

        }
    }

    private void mainFootCorrector(bool isGrounded, Vector2 movementVector)
    {
        //Set up ray cast variables
        Vector3 leftRayDirection = Vector3.down;
        Vector3 rightRayDirection = Vector3.down;
        float maxDistance = 4.0f + groundAdditive;

        //Left Foot
        RaycastHit leftFootHit;
        Vector3 leftFootOrigin = new Vector3(BlenderLeftFootIK.position.x, BlenderLeftFootIK.position.y + groundAdditive, BlenderLeftFootIK.position.z);
        float leftFootHitMaxDistance = maxDistanceTester + groundAdditive;

        // Right Foot
        RaycastHit rightFootHit;
        Vector3 rightFootOrigin = new Vector3(BlenderRightFootIK.position.x, BlenderRightFootIK.position.y + groundAdditive, BlenderRightFootIK.position.z);
        float rightFootHitMaxDistance = maxDistanceTester + groundAdditive;

        //Case 1: Both feet are touching the ground (not necessarily the same ground with the same slope)
        if (isGrounded && Physics.Raycast(leftFootOrigin, leftRayDirection, out leftFootHit, leftFootHitMaxDistance, layerMask) && Physics.Raycast(rightFootOrigin, rightRayDirection, out rightFootHit, rightFootHitMaxDistance, layerMask) && leftFootHit.transform.tag == "Walkable" && rightFootHit.transform.tag == "Walkable")
        {
            print("hit");
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

            //get left and right calculateFootDisToHit and footHeight
            float leftFootDisToHit = calculateFootDisToHit(leftFootHit, BlenderLeftFootIK.transform.position);
            float rightFootDisToHit = calculateFootDisToHit(rightFootHit, BlenderRightFootIK.transform.position);
            float leftFootHeight = calculateFootHeight(leftFootHit, leftToeEnd, leftFoot, BlenderLeftFootIK.transform.position);
            float rightFootHeight = calculateFootHeight(rightFootHit, rightToeEnd, rightFoot, BlenderRightFootIK.transform.position);

            //determine controllerCenterAdditive and which foot needs an offset
            float controllerCenterAdditive = 0;
            if (leftFootHit.point.y > rightFootHit.point.y)
            {
                controllerCenterAdditive = -rightFootDisToHit - rightFootHeight;
                newLeftFootYAdditive = leftFootDisToHit + leftFootHeight + controllerCenterAdditive;
                newLeftHintYAdditive = calculateYHintAdditive(newLeftFootYAdditive, leftFootHit);
            }
            else if (rightFootHit.point.y > leftFootHit.point.y)
            {
                controllerCenterAdditive = -leftFootDisToHit - leftFootHeight;
                newRightFootYAdditive = rightFootDisToHit + rightFootHeight + controllerCenterAdditive;
                newRightHintYAdditive = calculateYHintAdditive(newRightFootYAdditive, rightFootHit);
            }
            else
            {

                addMovementSpeed = 15f;
                if (leftFootHit.normal == new Vector3(0f, 1f, 0))
                {
                    controllerCenterAdditive = 0;
                }
                else
                {
                    controllerCenterAdditive = Mathf.Abs(leftFootDisToHit) - leftFootHeight;
                }
            }


            leftFootYAdditive = Mathf.Lerp(currentLeftFootYAdditive, newLeftFootYAdditive, addMovementSpeed * Time.deltaTime);
            rightFootYAdditive = Mathf.Lerp(currentRightFootYAdditive, newRightFootYAdditive, addMovementSpeed * Time.deltaTime);
            Vector3 leftFootNewPosition = new Vector3(BlenderLeftFootIK.position.x, BlenderLeftFootIK.position.y + leftFootYAdditive, BlenderLeftFootIK.position.z);
            Vector3 rightFootNewPosition = new Vector3(BlenderRightFootIK.position.x, BlenderRightFootIK.position.y + rightFootYAdditive, BlenderRightFootIK.position.z);
            unityLeftFootIK.position = Vector3.MoveTowards(currentLeftFootPosition, leftFootNewPosition, footCorMovementSpeed * Time.deltaTime);
            unityRightFootIK.position = Vector3.MoveTowards(currentRightFootPosition, rightFootNewPosition, footCorMovementSpeed * Time.deltaTime);

            unityLeftKneeIK.position = new Vector3(BlenderLeftKneeIK.position.x, BlenderLeftKneeIK.position.y + newLeftHintYAdditive, BlenderLeftKneeIK.position.z);
            unityLeftKneeIK.localPosition = new Vector3(unityLeftKneeIK.localPosition.x - 4f, unityLeftKneeIK.localPosition.y, unityLeftKneeIK.localPosition.z + 100f);

            unityRightKneeIK.position = new Vector3(BlenderRightKneeIK.position.x, BlenderRightKneeIK.position.y + newRightHintYAdditive, BlenderRightKneeIK.position.z);
            unityRightKneeIK.localPosition = new Vector3(unityRightKneeIK.localPosition.x + 4f, unityRightKneeIK.localPosition.y, unityRightKneeIK.localPosition.z + 100f);

            //set the current controllerCenter, feet IK target positions and Foot additives
            currentLeftFootYAdditive = leftFootYAdditive;
            currentRightFootYAdditive = rightFootYAdditive;
            currentLeftFootPosition = unityLeftFootIK.position;
            currentRightFootPosition = unityRightFootIK.position;
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

            unityLeftKneeIK.position = new Vector3(BlenderLeftKneeIK.position.x, BlenderLeftKneeIK.position.y, BlenderLeftKneeIK.position.z);
            unityRightKneeIK.position = new Vector3(BlenderRightKneeIK.position.x, BlenderRightKneeIK.position.y, BlenderRightKneeIK.position.z);

            //adjust both feet angles for smooth transitions for falling
            unityLeftFootIK.rotation = Quaternion.Lerp(currentLeftAngle, BlenderLeftFootIK.rotation, Time.deltaTime * footCorRotationSpeed / footCorRevRotSpeedDiv);
            currentLeftAngle = unityLeftFootIK.rotation;
            unityRightFootIK.rotation = Quaternion.Lerp(currentRightAngle, BlenderRightFootIK.rotation, Time.deltaTime * footCorRotationSpeed / footCorRevRotSpeedDiv);
            currentRightAngle = unityRightFootIK.rotation;

            addMovementSpeed = 15f;
        }
    }





}
