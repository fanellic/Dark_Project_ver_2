using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SkirtTargeting : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer skirtTargetSkinnedMesh;

    [SerializeField] private Transform skirtTargetTransform;

    [SerializeField] private Transform parentVeryFront, parentFrontLeft, parentFrontRight, parentFrontSideLeft, ParentFrontSideRight;

    private List<GameObject> skirtTargets = new List<GameObject>();

    private Mesh bakedSkirtTargetMesh;

    public float scaleFactor = .1f;

    public float dumb = 1f;

    [SerializeField] private float moveSpeed = 2f, scaleMoveSpeed = 2f;

    [SerializeField] private Transform leftShinBone, rightShinBone, leftFootIKTarget, rightFootIKTarget;

    private Vector3 targetOGLocalPosition0, targetOGLocalPosition1, targetOGLocalPosition2, targetOGLocalPosition3, targetOGLocalPosition4, targetOGLocalPosition5, targetOGLocalPosition6, targetOGLocalPosition7, targetOGLocalPosition8, targetOGLocalPosition9;

    private float xSlide0, xSlide1, xSlide2, xSlide3, xSlide4, xSlide5, xSlide6, xSlide7, xSlide8, xSlide9;
    private float ySlide0, ySlide1, ySlide2, ySlide3, ySlide4, ySlide5, ySlide6, ySlide7, ySlide8, ySlide9;
    private float zSlide0, zSlide1, zSlide2, zSlide3, zSlide4, zSlide5, zSlide6, zSlide7, zSlide8, zSlide9;

    [SerializeField] private JennyController JennyController;

    [SerializeField] private FootCorrector JennyFootCorrector;

    private bool leftLegUphillGoing = false, rightLegUphillGoing = false;

    private int leftInitialCount = 0, rightInitialCount = 0;

    private Stack<bool> goingUphill = new Stack<bool>();

    private float parentVeryFrontCurScaleY = 1f, parentFrontLeftCurScaleY = 1f, parentFrontRightCurScaleY = 1f, parentFrontSideLeftCurScaleY = 1f, parentFrontSideRightCurScaleY = 1f;

    void Start()
    {

        //Get all Transform components in the hierarchy, including the parent itself
        Transform[] allDescendantTransforms = GetComponentsInChildren<Transform>();

        //Iterate through the collected transforms and add them to the skirtTargets list, excluding the parent's own transform
        foreach (Transform childTransform in allDescendantTransforms)
        {
            // Ensure we don't add the parent GameObject itself
            if (childTransform != this.transform)
            {
                skirtTargets.Add(childTransform.gameObject);
            }
        }

        //Check to make sure skirtTargetSkinnedMesh has been assigned
        if (skirtTargetSkinnedMesh == null)
        {
            return;
        }

        //Create a new mesh to bake the vertex data into.
        bakedSkirtTargetMesh = new Mesh();

        // Bake the current state of the skinned mesh into a temporary mesh.
        skirtTargetSkinnedMesh.BakeMesh(bakedSkirtTargetMesh);

        // Get the list of vertices from the baked mesh.
        Vector3[] skirtTargetMeshVertices = bakedSkirtTargetMesh.vertices;

        skirtTargets[0].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[2]);
        targetOGLocalPosition0 = skirtTargets[0].transform.localPosition;
        skirtTargets[1].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[28]);
        targetOGLocalPosition1 = skirtTargets[1].transform.localPosition;
        skirtTargets[2].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[5]);
        targetOGLocalPosition2 = skirtTargets[2].transform.localPosition;
        skirtTargets[3].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[7]);
        targetOGLocalPosition3 = skirtTargets[3].transform.localPosition;
        skirtTargets[4].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[8]);
        targetOGLocalPosition4 = skirtTargets[4].transform.localPosition;
        skirtTargets[5].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[10]);
        targetOGLocalPosition5 = skirtTargets[5].transform.localPosition;
        skirtTargets[6].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[16]);
        targetOGLocalPosition6 = skirtTargets[6].transform.localPosition;
        skirtTargets[7].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[18]);
        targetOGLocalPosition7 = skirtTargets[7].transform.localPosition;
        skirtTargets[8].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[19]);
        targetOGLocalPosition8 = skirtTargets[8].transform.localPosition;
        skirtTargets[9].transform.position = WorldPositionCalculator(skirtTargetMeshVertices[25]);
        targetOGLocalPosition9 = skirtTargets[9].transform.localPosition;

        leftLegUphillGoing = false;

        goingUphill.Push(false);
        /*

        physicsRotater3intBone0Weight = physicsRotater3.bone0Weight;
        physicsRotater3intBone1Weight = physicsRotater3.bone1Weight;
        physicsRotater3intBone2Weight = physicsRotater3.bone2Weight;

        physicsRotater4intBone0Weight = physicsRotater4.bone0Weight;
        physicsRotater4intBone1Weight = physicsRotater4.bone1Weight;
        physicsRotater4intBone2Weight = physicsRotater4.bone2Weight;

        physicsRotater5intBone0Weight = physicsRotater5.bone0Weight;
        physicsRotater5intBone1Weight = physicsRotater5.bone1Weight;
        physicsRotater5intBone2Weight = physicsRotater5.bone2Weight;

        physicsRotater3CurBone0Weight = physicsRotater3.bone0Weight;
        physicsRotater3CurBone1Weight = physicsRotater3.bone1Weight;
        physicsRotater3CurBone2Weight = physicsRotater3.bone2Weight;

        physicsRotater4CurBone0Weight = physicsRotater4.bone0Weight;
        physicsRotater4CurBone1Weight = physicsRotater4.bone1Weight;
        physicsRotater4CurBone2Weight = physicsRotater4.bone2Weight;

        physicsRotater5CurBone0Weight = physicsRotater5.bone0Weight;
        physicsRotater5CurBone1Weight = physicsRotater5.bone1Weight;
        physicsRotater5CurBone2Weight = physicsRotater5.bone2Weight;*/

    }

    private Vector3 WorldPositionCalculator(Vector3 localPos)
    {

        Vector3 worldPos = skirtTargetTransform.transform.TransformPoint(localPos);
        return worldPos;
    }

    private float ValueToMoveCalculator (float skirtTargetMeshVertexValue, float globalPointValue)
    {
        return (globalPointValue-skirtTargetMeshVertexValue);
    }

    private void targetSetter(Vector3[] skirtTargetMeshVertices)
    {
        float dotProductLeft = Vector3.Dot(JennyController.refTransformForward, JennyFootCorrector.leftFootHitNormalRef);
        float dotProductRight = Vector3.Dot(JennyController.refTransformForward, JennyFootCorrector.rightFootHitNormalRef);
        bool priorUphill = true;
        if (goingUphill.Count > 0)
        {
            priorUphill = goingUphill.Pop();
        }

        if (JennyController.isGrounded)
        {
            if ((dotProductLeft < -0.1f || dotProductRight < -0.1f) && JennyController.movementVectorRef != new Vector2(0f, 0f))
            {
                
                // Jenny is moving uphill.
                goingUphill.Push(true);
                if ((priorUphill == false && dotProductLeft > -0.1f && dotProductRight < -0.1f) || rightLegUphillGoing)
                {
                    rightLegUphillGoing = true;
                }
                else if (priorUphill == false|| leftLegUphillGoing)
                {
                    leftLegUphillGoing = true;
                }

                //lower front skirt physics
                /*
                physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);
                */

                //left vs right
                float parentVeryFrontScaleVal = .8f;
                float parentFrontLeftScaleVal = .8f;
                float parentFrontRightScaleVal = .8f;
                float parentFrontSideLeftScaleVal = .9f;
                float parentFrontSideRightScaleVal = .9f;

                /*
                float rightXPush = 0;
                float leftXPush = 0;
                float rightUpPush = 0;
                float leftUpPush = 0;
                float target6Push = 0;*/
                float midZSlide = .5f;
                float midXSlide = .5f;

                float leftSide = transform.root.InverseTransformPoint(leftFootIKTarget.transform.position).z;
                float rightSide = transform.root.InverseTransformPoint(rightFootIKTarget.transform.position).z;


                Vector3 upHillLocalPoint3;
                Vector3 upHillLocalPoint5;

                Vector3 globalizedPoint3 = transform.TransformPoint(targetOGLocalPosition3);
                Vector3 idealGoalPoint3 = (skirtTargetMeshVertices[7]); // skirtTargetTransform.transform.InverseTransformPoint(WorldPositionCalculator(skirtTargetMeshVertices[7]));

                Vector3 globalizedPoint5 = transform.TransformPoint(targetOGLocalPosition4);
                Vector3 idealGoalPoint5 = (skirtTargetMeshVertices[10]); // skirtTargetTransform.transform.InverseTransformPoint(WorldPositionCalculator(skirtTargetMeshVertices[10]));


                if (leftSide > rightSide)
                {
                    //rightXPush = -.25f * scaleFactor;
                    //leftXPush = (1f + moreStupid) * scaleFactor;
                    //rightUpPush = ((transform.root.InverseTransformPoint(leftShinBone.transform.position).y) / stupid ) * scaleFactor;
                    //leftUpPush = ((transform.root.InverseTransformPoint(leftShinBone.transform.position).y) / 7f + leftAdditive) * scaleFactor;
                    float slopeAngle = Vector3.Angle(JennyFootCorrector.leftFootHitNormalRef, Vector3.up);
                    scaleFactor = 0.011f * slopeAngle + 0.165f;

                    upHillLocalPoint3 = new Vector3(targetOGLocalPosition5.x - .4f * scaleFactor, targetOGLocalPosition5.y + .93f * scaleFactor, targetOGLocalPosition5.z + .36f * scaleFactor);
                    globalizedPoint3 = transform.TransformPoint(upHillLocalPoint3);
                    idealGoalPoint3 = skirtTargetTransform.transform.InverseTransformPoint(globalizedPoint3);

                    upHillLocalPoint5 = new Vector3(targetOGLocalPosition3.x + .4f * scaleFactor, targetOGLocalPosition3.y + .4f * scaleFactor, targetOGLocalPosition3.z + .34f * scaleFactor);
                    globalizedPoint5 = transform.TransformPoint(upHillLocalPoint5);
                    idealGoalPoint5 = skirtTargetTransform.transform.InverseTransformPoint(globalizedPoint5);


                    parentVeryFrontScaleVal = .6f;
                    parentFrontLeftScaleVal = .6f;
                    parentFrontRightScaleVal = .8f;
                    parentFrontSideLeftScaleVal = 1f;
                    parentFrontSideRightScaleVal = 1f;
                    midZSlide = .1f;
                    midXSlide = .7f;

                    if (leftLegUphillGoing)
                    {
                        leftInitialCount++;
                    }
                    
                    if (rightInitialCount > 3)
                    {
                        rightLegUphillGoing = false;
                    }
                }
                else if (leftSide < rightSide)
                {
                    //rightXPush = (1f + moreStupid) * scaleFactor;
                    //leftXPush = -.25f * scaleFactor;
                    //rightUpPush = ((transform.root.InverseTransformPoint(rightShinBone.transform.position).y) / 10f + rightAdditive) * scaleFactor;
                    //leftUpPush = ((transform.root.InverseTransformPoint(rightShinBone.transform.position).y) / stupid) * scaleFactor;
                    //target6Push = .4f * scaleFactor;
                    //midXSlide = .35f;
                    float slopeAngle = Vector3.Angle(JennyFootCorrector.rightFootHitNormalRef, Vector3.up);
                    scaleFactor =  0.011f * slopeAngle + 0.165f;

                    upHillLocalPoint3 = new Vector3(targetOGLocalPosition3.x - .18f * scaleFactor, targetOGLocalPosition3.y + .4f * scaleFactor, targetOGLocalPosition3.z + .32f * scaleFactor);
                    globalizedPoint3 = transform.TransformPoint(upHillLocalPoint3);
                    idealGoalPoint3 = skirtTargetTransform.transform.InverseTransformPoint(globalizedPoint3);

                    upHillLocalPoint5 = new Vector3(targetOGLocalPosition5.x + .08f * scaleFactor, targetOGLocalPosition5.y + .97f * scaleFactor, targetOGLocalPosition5.z + .33f * scaleFactor);
                    globalizedPoint5 = transform.TransformPoint(upHillLocalPoint5);
                    idealGoalPoint5 = skirtTargetTransform.transform.InverseTransformPoint(globalizedPoint5);

                    parentVeryFrontScaleVal = .6f;
                    parentFrontLeftScaleVal = .8f;
                    parentFrontRightScaleVal = .6f;
                    parentFrontSideLeftScaleVal = 1f;
                    parentFrontSideRightScaleVal = 1f;
                    midXSlide = .35f;
                    midZSlide = .8f;

                    if ( rightLegUphillGoing)
                    {
                        rightInitialCount++;
                    }
                    if (leftInitialCount > 3)
                    {
                        leftLegUphillGoing = false;
                    }
                }

                //set parent scale
                Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
                Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
                Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
                Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
                Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

                parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, parentVeryFrontScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, parentFrontLeftScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, parentFrontRightScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, parentFrontSideLeftScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, parentFrontSideRightScaleVal, Time.deltaTime * scaleMoveSpeed);

                
                parentVeryFront.transform.localScale = parentVeryFrontScale;
                parentFrontLeft.transform.localScale = parentFrontLeftScale;
                parentFrontRight.transform.localScale = parentFrontRightScale;
                parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
                ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

                parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
                parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
                parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
                parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
                parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;
                
               //left back target
               Vector3 globalizedPoint0 = transform.TransformPoint(targetOGLocalPosition0);
               Vector3 goalPoint0 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[2]), globalizedPoint0, .8f));
               float xValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].x, goalPoint0.x);
               float yValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].y, goalPoint0.y);
               float zValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].z, goalPoint0.z);
               xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
               ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
               zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

               //left side target
               Vector3 globalizedPoint1 = transform.TransformPoint(targetOGLocalPosition1);
               Vector3 goalPoint1 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[28]), globalizedPoint1, .5f));
               float xValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].x, goalPoint1.x);
               float yValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].y, goalPoint1.y);
               float zValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].z, goalPoint1.z);
               xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
               ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
               zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

               //left side-front target
               Vector3 globalizedPoint2 = transform.TransformPoint(targetOGLocalPosition2);
               Vector3 goalPoint2 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[5]), globalizedPoint2, .2f));
               float xValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].x, goalPoint2.x);
               float yValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].y, goalPoint2.y);
               float zValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].z, goalPoint2.z);
               xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
               ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
               zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

                // #########################################################################################


                //left front target goal point
                /*
                float amountToPushIn3 = leftXPush;
                float amountToPushUp3 = leftUpPush;
               Vector3 idealGoalPoint3 = new Vector3(skirtTargetMeshVertices[7].x + amountToPushIn3, skirtTargetMeshVertices[7].y, skirtTargetMeshVertices[7].z + amountToPushUp3); //skirtTargetMeshVertices[7];



                //right front target goal point
                float amountToPushIn5 = rightXPush;
                float amountToPushUp5 = rightUpPush;
               Vector3 idealGoalPoint5 = new Vector3(skirtTargetMeshVertices[10].x - amountToPushIn5, skirtTargetMeshVertices[10].y, skirtTargetMeshVertices[10].z + amountToPushUp5); //skirtTargetMeshVertices[10];*/


                

                //very front target
                Vector3 goalPoint4 = new Vector3(Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, midXSlide).x, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).y, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, midZSlide).z);
               float xValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].x, goalPoint4.x);
               float yValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].y, goalPoint4.y);
               float zValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].z, goalPoint4.z);
               xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
               ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
               zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

               //left front target
               Vector3 goalPoint3 = new Vector3(idealGoalPoint3.x, idealGoalPoint3.y, idealGoalPoint3.z);
               float xValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].x, goalPoint3.x);
               float yValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].y, goalPoint3.y);
               float zValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].z, goalPoint3.z);
               xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
               ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
               zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

               //right front target
               Vector3 goalPoint5 = new Vector3(idealGoalPoint5.x, idealGoalPoint5.y, idealGoalPoint5.z);
               float xValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].x, goalPoint5.x);
               float yValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].y, goalPoint5.y);
               float zValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].z, goalPoint5.z);
               xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
               ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
               zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

               // #########################################################################################
               
               //right side-front target
               
               Vector3 globalizedPoint6 = transform.TransformPoint(targetOGLocalPosition6);
               Vector3 goalPoint6 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[16]), globalizedPoint6, .2f));
               //goalPoint6 = new Vector3(goalPoint6.x - target6Push, goalPoint6.y, goalPoint6.z);
               float xValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].x, goalPoint6.x);
               float yValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].y, goalPoint6.y);
               float zValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].z, goalPoint6.z);
               xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
               ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
               zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);

                
               //right side target
               Vector3 globalizedPoint7 = transform.TransformPoint(targetOGLocalPosition7);
               Vector3 goalPoint7 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[18]), globalizedPoint7, .5f));
               float xValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].x, goalPoint7.x);
               float yValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].y, goalPoint7.y);
               float zValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].z, goalPoint7.z);
               xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
               ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
               zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

               //right back target
               Vector3 globalizedPoint8 = transform.TransformPoint(targetOGLocalPosition8);
               Vector3 goalPoint8 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[19]), globalizedPoint8, .8f));
               float xValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].x, goalPoint8.x);
               float yValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].y, goalPoint8.y);
               float zValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].z, goalPoint8.z);
               xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
               ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
               zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

               //very back target
               Vector3 globalizedPoint9 = transform.TransformPoint(targetOGLocalPosition9);
               Vector3 goalPoint9 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[25]), globalizedPoint9, .8f));//Vector3.Lerp(goalPoint0, goalPoint8, .5f); 
               float xValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].x, goalPoint9.x);
               float yValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].y, goalPoint9.y);
               float zValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].z, goalPoint9.z);
               xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
               ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
               zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);

            }
            else
            {
                // Jenny is going downhill, staying flat or no significant slope relative to movement.
                goingUphill.Push(false);
                leftInitialCount = 0;
                rightInitialCount = 0;
                leftLegUphillGoing = false;
                rightLegUphillGoing = false;

                //set parent scale
                Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
                Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
                Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
                Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
                Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

                parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);

                parentVeryFront.transform.localScale = parentVeryFrontScale;
                parentFrontLeft.transform.localScale = parentFrontLeftScale;
                parentFrontRight.transform.localScale = parentFrontRightScale;
                parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
                ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

                parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
                parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
                parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
                parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
                parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;

                //enable physics on front of skirt
                /*
                physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);*/
                /*
                if (parentFrontSideLeftCurScaleY > .999f)
                {
                }
                if (parentFrontSideRightCurScaleY > .999f)
                {
                }*/

                float xValueToMove = 0f;
                float yValueToMove = 0f;
                float zValueToMove = 0f;

                //left back target
                Vector3 globalizedPoint0 = transform.TransformPoint(targetOGLocalPosition0);
                Vector3 goalPoint0 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[2]), globalizedPoint0, .4f));
                float xValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].x, goalPoint0.x);
                float yValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].y, goalPoint0.y);
                float zValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].z, goalPoint0.z);
                xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
                ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
                zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

                //left side target
                Vector3 globalizedPoint1 = transform.TransformPoint(targetOGLocalPosition1);
                Vector3 goalPoint1 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[28]), globalizedPoint1, .4f));
                float xValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].x, goalPoint1.x);
                float yValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].y, goalPoint1.y);
                float zValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].z, goalPoint1.z);
                xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
                ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
                zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

                //left side-front target
                Vector3 globalizedPoint2 = transform.TransformPoint(targetOGLocalPosition2);
                Vector3 goalPoint2 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[5]), globalizedPoint2, .4f));
                float xValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].x, goalPoint2.x);
                float yValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].y, goalPoint2.y);
                float zValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].z, goalPoint2.z);
                xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
                ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
                zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

                //left front target
                float xValueToMove3 = xValueToMove;
                float yValueToMove3 = yValueToMove;
                float zValueToMove3 = zValueToMove;
                xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
                ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
                zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

                //very front target
                Vector3 idealGoalPoint3 = skirtTargetMeshVertices[7];
                Vector3 idealGoalPoint5 = skirtTargetMeshVertices[10];
                Vector3 goalPoint4 = new Vector3(Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).x, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).y, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).z);
                float xValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].x, goalPoint4.x);
                //float yValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].y, goalPoint4.y);
                //float zValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].z, goalPoint4.z);
                //float xValueToMove4 = xValueToMove;
                float yValueToMove4 = yValueToMove;
                float zValueToMove4 = zValueToMove;
                xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
                ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
                zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

                //right front target
                float xValueToMove5 = xValueToMove;
                float yValueToMove5 = yValueToMove;
                float zValueToMove5 = zValueToMove;
                xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
                ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
                zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

                //right side-front target
                Vector3 globalizedPoint6 = transform.TransformPoint(targetOGLocalPosition6);
                Vector3 goalPoint6 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[16]), globalizedPoint6, .4f));
                float xValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].x, goalPoint6.x);
                float yValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].y, goalPoint6.y);
                float zValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].z, goalPoint6.z);
                xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
                ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
                zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);

                //right side target
                Vector3 globalizedPoint7 = transform.TransformPoint(targetOGLocalPosition7);
                Vector3 goalPoint7 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[18]), globalizedPoint7, .4f));
                float xValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].x, goalPoint7.x);
                float yValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].y, goalPoint7.y);
                float zValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].z, goalPoint7.z);
                xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
                ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
                zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

                //right back target
                Vector3 globalizedPoint8 = transform.TransformPoint(targetOGLocalPosition8);
                Vector3 goalPoint8 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[19]), globalizedPoint8, .4f));
                float xValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].x, goalPoint8.x);
                float yValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].y, goalPoint8.y);
                float zValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].z, goalPoint8.z);
                xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
                ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
                zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

                //very back target
                Vector3 globalizedPoint9 = transform.TransformPoint(targetOGLocalPosition9);
                Vector3 goalPoint9 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[25]), globalizedPoint9, .7f));//Vector3.Lerp(goalPoint0, goalPoint8, .5f); 
                float xValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].x, goalPoint9.x);
                float yValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].y, goalPoint9.y);
                float zValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].z, goalPoint9.z);
                xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
                ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
                zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            // Jenny is falling
            goingUphill.Push(false);
            leftInitialCount = 0;
            rightInitialCount = 0;
            leftLegUphillGoing = false;
            rightLegUphillGoing = false;

            //set parent scale
            Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
            Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
            Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
            Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
            Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

            parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);

            parentVeryFront.transform.localScale = parentVeryFrontScale;
            parentFrontLeft.transform.localScale = parentFrontLeftScale;
            parentFrontRight.transform.localScale = parentFrontRightScale;
            parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
            ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

            parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
            parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
            parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
            parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
            parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;

            //enable physics on front of skirt
            /*
            physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);

            physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);

            physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);*/

            /*
            if (parentVeryFrontCurScaleY > .999f)
            {
                physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }
            if (parentFrontLeftCurScaleY > .999f)
            {
                physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }
            if (parentFrontRightCurScaleY > .999f)
            {
                physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }*/

            /*
            if (parentFrontSideLeftCurScaleY > .999f)
            {
            }
            if (parentFrontSideRightCurScaleY > .999f)
            {
            }*/

            float xValueToMove = 0f;
            float yValueToMove = 0f;
            float zValueToMove = .32f;

            //left back target
            float xValueToMove0 = xValueToMove;
            float yValueToMove0 = yValueToMove;
            float zValueToMove0 = zValueToMove;
            xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
            ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
            zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

            //left side target
            float xValueToMove1 = xValueToMove;
            float yValueToMove1 = yValueToMove;
            float zValueToMove1 = zValueToMove;
            xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
            ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
            zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

            //left side-front target
            float xValueToMove2 = xValueToMove;
            float yValueToMove2 = yValueToMove;
            float zValueToMove2 = zValueToMove;
            xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
            ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
            zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

            //left front target
            float xValueToMove3 = xValueToMove;
            float yValueToMove3 = yValueToMove;
            float zValueToMove3 = zValueToMove;
            xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
            ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
            zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

            //very front target
            float xValueToMove4 = xValueToMove;
            float yValueToMove4 = yValueToMove;
            float zValueToMove4 = zValueToMove;
            xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
            ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
            zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

            //right front target
            float xValueToMove5 = xValueToMove;
            float yValueToMove5 = yValueToMove;
            float zValueToMove5 = zValueToMove;
            xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
            ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
            zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

            //right side-front target
            float xValueToMove6 = xValueToMove;
            float yValueToMove6 = yValueToMove;
            float zValueToMove6 = zValueToMove;
            xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
            ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
            zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);

            //right side target
            float xValueToMove7 = xValueToMove;
            float yValueToMove7 = yValueToMove;
            float zValueToMove7 = zValueToMove;
            xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
            ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
            zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

            //right back target
            float xValueToMove8 = xValueToMove;
            float yValueToMove8 = yValueToMove;
            float zValueToMove8 = zValueToMove;
            xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
            ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
            zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

            //very back target
            float xValueToMove9 = xValueToMove;
            float yValueToMove9 = yValueToMove;
            float zValueToMove9 = zValueToMove;
            xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
            ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
            zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);
        }
    }


    private void targetSetterBetter(Vector3[] skirtTargetMeshVertices) {

        float dotProductLeft = Vector3.Dot(JennyController.refTransformForward, JennyFootCorrector.leftFootHitNormalRef);
        float dotProductRight = Vector3.Dot(JennyController.refTransformForward, JennyFootCorrector.rightFootHitNormalRef);
        bool priorUphill = true;
        if (goingUphill.Count > 0)
        {
            priorUphill = goingUphill.Pop();
        }

        if (JennyController.isGrounded)
        {
            if ((dotProductLeft < -0.1f || dotProductRight < -0.1f) && JennyController.movementVectorRef != new Vector2(0f, 0f))
            {
                /*
                // Jenny is moving uphill.
                goingUphill.Push(true);
                float leftAdditive = 0;
                float rightAdditive = 0;
                if ((priorUphill == false && dotProductLeft > -0.1f && dotProductRight < -0.1f) || rightLegUphillGoing)
                {
                    rightAdditive = 1f; // 1f;
                    rightLegUphillGoing = true;
                }
                else if (priorUphill == false || leftLegUphillGoing)
                {
                    leftAdditive = 1f;// 2f;
                    leftLegUphillGoing = true;
                }

                //lower front skirt physics
                
                //physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);

                //physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);

                //physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, lowerBone0Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, lowerBone1Weight, Time.deltaTime * physicsWeightSpeed);
                //physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, lowerBone2Weight, Time.deltaTime * physicsWeightSpeed);
                

                //left vs right
                float parentVeryFrontScaleVal = .8f;
                float parentFrontLeftScaleVal = .8f;
                float parentFrontRightScaleVal = .8f;
                float parentFrontSideLeftScaleVal = .9f;
                float parentFrontSideRightScaleVal = .9f;

                float rightXPush = 0;
                float leftXPush = 0;
                float rightUpPush = 0;
                float leftUpPush = 0;
                float target6Push = 0;
                float midZSlide = .5f;
                float midXSlide = .5f;

                float leftSide = transform.root.InverseTransformPoint(leftFootIKTarget.transform.position).z;
                float rightSide = transform.root.InverseTransformPoint(rightFootIKTarget.transform.position).z;

                if (leftSide > rightSide)
                {
                    rightXPush = -.25f * scaleFactor;
                    leftXPush = (1f + moreStupid) * scaleFactor;
                    rightUpPush = ((transform.root.InverseTransformPoint(leftShinBone.transform.position).y) / stupid) * scaleFactor;
                    leftUpPush = ((transform.root.InverseTransformPoint(leftShinBone.transform.position).y) / 7f + leftAdditive) * scaleFactor;
                    parentVeryFrontScaleVal = .7f;
                    parentFrontLeftScaleVal = .6f;
                    parentFrontRightScaleVal = .9f;
                    parentFrontSideLeftScaleVal = .8f;
                    parentFrontSideRightScaleVal = 1f;
                    midZSlide = .1f;
                    midXSlide = .7f;

                    if (leftLegUphillGoing)
                    {
                        leftInitialCount++;
                    }

                    if (rightInitialCount > 3)
                    {
                        rightLegUphillGoing = false;
                    }
                }
                else if (leftSide < rightSide)
                {
                    rightXPush = (1f + moreStupid) * scaleFactor;
                    leftXPush = -.25f * scaleFactor;
                    rightUpPush = ((transform.root.InverseTransformPoint(rightShinBone.transform.position).y) / 10f + rightAdditive) * scaleFactor;
                    leftUpPush = ((transform.root.InverseTransformPoint(rightShinBone.transform.position).y) / stupid) * scaleFactor;
                    target6Push = .4f * scaleFactor;
                    midXSlide = .35f;
                    parentVeryFrontScaleVal = .7f;
                    parentFrontLeftScaleVal = .9f;
                    parentFrontRightScaleVal = .6f;
                    parentFrontSideLeftScaleVal = 1f;
                    parentFrontSideRightScaleVal = .8f;
                    midZSlide = .8f;

                    if (rightLegUphillGoing)
                    {
                        rightInitialCount++;
                    }
                    if (leftInitialCount > 3)
                    {
                        leftLegUphillGoing = false;
                    }
                }

                //set parent scale
                Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
                Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
                Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
                Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
                Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

                parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, parentVeryFrontScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, parentFrontLeftScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, parentFrontRightScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, parentFrontSideLeftScaleVal, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, parentFrontSideRightScaleVal, Time.deltaTime * scaleMoveSpeed);


                parentVeryFront.transform.localScale = parentVeryFrontScale;
                parentFrontLeft.transform.localScale = parentFrontLeftScale;
                parentFrontRight.transform.localScale = parentFrontRightScale;
                parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
                ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

                parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
                parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
                parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
                parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
                parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;

                //left back target
                Vector3 globalizedPoint0 = transform.TransformPoint(targetOGLocalPosition0);
                Vector3 goalPoint0 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[2]), globalizedPoint0, .8f));
                float xValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].x, goalPoint0.x);
                float yValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].y, goalPoint0.y);
                float zValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].z, goalPoint0.z);
                xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
                ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
                zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

                //left side target
                Vector3 globalizedPoint1 = transform.TransformPoint(targetOGLocalPosition1);
                Vector3 goalPoint1 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[28]), globalizedPoint1, .5f));
                float xValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].x, goalPoint1.x);
                float yValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].y, goalPoint1.y);
                float zValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].z, goalPoint1.z);
                xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
                ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
                zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

                //left side-front target
                Vector3 globalizedPoint2 = transform.TransformPoint(targetOGLocalPosition2);
                Vector3 goalPoint2 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[5]), globalizedPoint2, .2f));
                float xValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].x, goalPoint2.x);
                float yValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].y, goalPoint2.y);
                float zValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].z, goalPoint2.z);
                xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
                ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
                zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

                // #########################################################################################


                //left front target goal point
                float amountToPushIn3 = leftXPush;
                float amountToPushUp3 = leftUpPush;
                Vector3 idealGoalPoint3 = new Vector3(skirtTargetMeshVertices[7].x + amountToPushIn3, skirtTargetMeshVertices[7].y, skirtTargetMeshVertices[7].z + amountToPushUp3); //skirtTargetMeshVertices[7];

                //right front target goal point
                float amountToPushIn5 = rightXPush;
                float amountToPushUp5 = rightUpPush;
                Vector3 idealGoalPoint5 = new Vector3(skirtTargetMeshVertices[10].x - amountToPushIn5, skirtTargetMeshVertices[10].y, skirtTargetMeshVertices[10].z + amountToPushUp5); //skirtTargetMeshVertices[10];


                //very front target
                Vector3 goalPoint4 = new Vector3(Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, midXSlide).x, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).y, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, midZSlide).z);
                float xValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].x, goalPoint4.x);
                float yValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].y, goalPoint4.y);
                float zValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].z, goalPoint4.z);
                xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
                ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
                zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

                //left front target
                Vector3 goalPoint3 = new Vector3(idealGoalPoint3.x, idealGoalPoint3.y, idealGoalPoint3.z);
                float xValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].x, goalPoint3.x);
                float yValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].y, goalPoint3.y);
                float zValueToMove3 = ValueToMoveCalculator(skirtTargetMeshVertices[7].z, goalPoint3.z);
                xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
                ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
                zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

                //right front target
                Vector3 goalPoint5 = new Vector3(idealGoalPoint5.x, idealGoalPoint5.y, idealGoalPoint5.z);
                float xValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].x, goalPoint5.x);
                float yValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].y, goalPoint5.y);
                float zValueToMove5 = ValueToMoveCalculator(skirtTargetMeshVertices[10].z, goalPoint5.z);
                xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
                ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
                zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

                // #########################################################################################

                //right side-front target

                Vector3 globalizedPoint6 = transform.TransformPoint(targetOGLocalPosition6);
                Vector3 goalPoint6 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[16]), globalizedPoint6, .2f));
                goalPoint6 = new Vector3(goalPoint6.x - target6Push, goalPoint6.y, goalPoint6.z);
                float xValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].x, goalPoint6.x);
                float yValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].y, goalPoint6.y);
                float zValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].z, goalPoint6.z);
                xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
                ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
                zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);


                //right side target
                Vector3 globalizedPoint7 = transform.TransformPoint(targetOGLocalPosition7);
                Vector3 goalPoint7 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[18]), globalizedPoint7, .5f));
                float xValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].x, goalPoint7.x);
                float yValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].y, goalPoint7.y);
                float zValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].z, goalPoint7.z);
                xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
                ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
                zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

                //right back target
                Vector3 globalizedPoint8 = transform.TransformPoint(targetOGLocalPosition8);
                Vector3 goalPoint8 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[19]), globalizedPoint8, .8f));
                float xValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].x, goalPoint8.x);
                float yValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].y, goalPoint8.y);
                float zValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].z, goalPoint8.z);
                xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
                ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
                zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

                //very back target
                Vector3 globalizedPoint9 = transform.TransformPoint(targetOGLocalPosition9);
                Vector3 goalPoint9 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[25]), globalizedPoint9, .8f));//Vector3.Lerp(goalPoint0, goalPoint8, .5f); 
                float xValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].x, goalPoint9.x);
                float yValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].y, goalPoint9.y);
                float zValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].z, goalPoint9.z);
                xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
                ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
                zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);
                */
            }
            else
            {
                // Jenny is going downhill, staying flat or no significant slope relative to movement.
                goingUphill.Push(false);
                leftInitialCount = 0;
                rightInitialCount = 0;
                leftLegUphillGoing = false;
                rightLegUphillGoing = false;

                //set parent scale
                Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
                Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
                Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
                Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
                Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

                parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
                parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);

                parentVeryFront.transform.localScale = parentVeryFrontScale;
                parentFrontLeft.transform.localScale = parentFrontLeftScale;
                parentFrontRight.transform.localScale = parentFrontRightScale;
                parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
                ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

                parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
                parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
                parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
                parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
                parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;

                //enable physics on front of skirt
                /*
                physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);

                physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);*/
                /*
                if (parentFrontSideLeftCurScaleY > .999f)
                {
                }
                if (parentFrontSideRightCurScaleY > .999f)
                {
                }*/

                float xValueToMove = 0f;
                float yValueToMove = 0f;
                float zValueToMove = 0f;




                Vector3 target0 = new Vector3(skirtTargetMeshVertices[2].x, skirtTargetMeshVertices[2].y, skirtTargetMeshVertices[2].z);
                skirtTargets[0].transform.position = WorldPositionCalculator(target0);

                Vector3 target1 = new Vector3(skirtTargetMeshVertices[28].x, skirtTargetMeshVertices[28].y, skirtTargetMeshVertices[28].z);
                skirtTargets[1].transform.position = WorldPositionCalculator(target1);

                Vector3 target2 = new Vector3(skirtTargetMeshVertices[5].x, skirtTargetMeshVertices[5].y, skirtTargetMeshVertices[5].z);
                skirtTargets[2].transform.position = WorldPositionCalculator(target2);

                Vector3 target3 = new Vector3(skirtTargetMeshVertices[7].x, skirtTargetMeshVertices[7].y, skirtTargetMeshVertices[7].z);
                skirtTargets[3].transform.position = WorldPositionCalculator(target3);

                Vector3 target5 = new Vector3(skirtTargetMeshVertices[10].x, skirtTargetMeshVertices[10].y, skirtTargetMeshVertices[10].z);
                skirtTargets[5].transform.position = WorldPositionCalculator(target5);

                Vector3 target6 = new Vector3(skirtTargetMeshVertices[16].x, skirtTargetMeshVertices[16].y, skirtTargetMeshVertices[16].z);
                skirtTargets[6].transform.position = WorldPositionCalculator(target6);

                Vector3 target7 = new Vector3(skirtTargetMeshVertices[18].x, skirtTargetMeshVertices[18].y, skirtTargetMeshVertices[18].z);
                skirtTargets[7].transform.position = WorldPositionCalculator(target7);

                Vector3 target8 = new Vector3(skirtTargetMeshVertices[19].x, skirtTargetMeshVertices[19].y, skirtTargetMeshVertices[19].z);
                skirtTargets[8].transform.position = WorldPositionCalculator(target8);

                Vector3 target9 = new Vector3(skirtTargetMeshVertices[25].x, skirtTargetMeshVertices[25].y, skirtTargetMeshVertices[25].z);
                skirtTargets[9].transform.position = WorldPositionCalculator(target9);

                Vector3 target4 = new Vector3(skirtTargetMeshVertices[8].x, skirtTargetMeshVertices[8].y, skirtTargetMeshVertices[8].z);
                skirtTargets[4].transform.position = WorldPositionCalculator(target4);




                //left back target
                Vector3 globalizedPoint0 = transform.TransformPoint(targetOGLocalPosition0);
                Vector3 goalPoint0 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[2]), globalizedPoint0, .4f));
                float xValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].x, goalPoint0.x);
                float yValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].y, goalPoint0.y);
                float zValueToMove0 = ValueToMoveCalculator(skirtTargetMeshVertices[2].z, goalPoint0.z);
                xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
                ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
                zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

                //left side target
                Vector3 globalizedPoint1 = transform.TransformPoint(targetOGLocalPosition1);
                Vector3 goalPoint1 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[28]), globalizedPoint1, .4f));
                float xValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].x, goalPoint1.x);
                float yValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].y, goalPoint1.y);
                float zValueToMove1 = ValueToMoveCalculator(skirtTargetMeshVertices[28].z, goalPoint1.z);
                xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
                ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
                zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

                //left side-front target
                Vector3 globalizedPoint2 = transform.TransformPoint(targetOGLocalPosition2);
                Vector3 goalPoint2 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[5]), globalizedPoint2, .4f));
                float xValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].x, goalPoint2.x);
                float yValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].y, goalPoint2.y);
                float zValueToMove2 = ValueToMoveCalculator(skirtTargetMeshVertices[5].z, goalPoint2.z);
                xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
                ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
                zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

                //left front target
                float xValueToMove3 = xValueToMove;
                float yValueToMove3 = yValueToMove;
                float zValueToMove3 = zValueToMove;
                xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
                ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
                zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

                //very front target
                Vector3 idealGoalPoint3 = skirtTargetMeshVertices[7];
                Vector3 idealGoalPoint5 = skirtTargetMeshVertices[10];
                Vector3 goalPoint4 = new Vector3(Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).x, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).y, Vector3.Lerp(idealGoalPoint3, idealGoalPoint5, .5f).z);
                float xValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].x, goalPoint4.x);
                //float yValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].y, goalPoint4.y);
                //float zValueToMove4 = ValueToMoveCalculator(skirtTargetMeshVertices[8].z, goalPoint4.z);
                //float xValueToMove4 = xValueToMove;
                float yValueToMove4 = yValueToMove;
                float zValueToMove4 = zValueToMove;
                xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
                ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
                zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

                //right front target
                float xValueToMove5 = xValueToMove;
                float yValueToMove5 = yValueToMove;
                float zValueToMove5 = zValueToMove;
                xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
                ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
                zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

                //right side-front target
                Vector3 globalizedPoint6 = transform.TransformPoint(targetOGLocalPosition6);
                Vector3 goalPoint6 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[16]), globalizedPoint6, .4f));
                float xValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].x, goalPoint6.x);
                float yValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].y, goalPoint6.y);
                float zValueToMove6 = ValueToMoveCalculator(skirtTargetMeshVertices[16].z, goalPoint6.z);
                xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
                ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
                zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);

                //right side target
                Vector3 globalizedPoint7 = transform.TransformPoint(targetOGLocalPosition7);
                Vector3 goalPoint7 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[18]), globalizedPoint7, .4f));
                float xValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].x, goalPoint7.x);
                float yValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].y, goalPoint7.y);
                float zValueToMove7 = ValueToMoveCalculator(skirtTargetMeshVertices[18].z, goalPoint7.z);
                xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
                ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
                zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

                //right back target
                Vector3 globalizedPoint8 = transform.TransformPoint(targetOGLocalPosition8);
                Vector3 goalPoint8 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[19]), globalizedPoint8, .4f));
                float xValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].x, goalPoint8.x);
                float yValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].y, goalPoint8.y);
                float zValueToMove8 = ValueToMoveCalculator(skirtTargetMeshVertices[19].z, goalPoint8.z);
                xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
                ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
                zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

                //very back target
                Vector3 globalizedPoint9 = transform.TransformPoint(targetOGLocalPosition9);
                Vector3 goalPoint9 = skirtTargetTransform.transform.InverseTransformPoint(Vector3.Lerp(WorldPositionCalculator(skirtTargetMeshVertices[25]), globalizedPoint9, .7f));//Vector3.Lerp(goalPoint0, goalPoint8, .5f); 
                float xValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].x, goalPoint9.x);
                float yValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].y, goalPoint9.y);
                float zValueToMove9 = ValueToMoveCalculator(skirtTargetMeshVertices[25].z, goalPoint9.z);
                xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
                ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
                zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);
            }
        }
        else
        {
            // Jenny is falling
            goingUphill.Push(false);
            leftInitialCount = 0;
            rightInitialCount = 0;
            leftLegUphillGoing = false;
            rightLegUphillGoing = false;

            //set parent scale
            Vector3 parentVeryFrontScale = parentVeryFront.transform.localScale;
            Vector3 parentFrontLeftScale = parentFrontLeft.transform.localScale;
            Vector3 parentFrontRightScale = parentFrontRight.transform.localScale;
            Vector3 parentFrontSideLeftScale = parentFrontSideLeft.transform.localScale;
            Vector3 parentFrontSideRightScale = ParentFrontSideRight.transform.localScale;

            parentVeryFrontScale.y = Mathf.Lerp(parentVeryFrontCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontLeftScale.y = Mathf.Lerp(parentFrontLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontRightScale.y = Mathf.Lerp(parentFrontRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontSideLeftScale.y = Mathf.Lerp(parentFrontSideLeftCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);
            parentFrontSideRightScale.y = Mathf.Lerp(parentFrontSideRightCurScaleY, 1f, Time.deltaTime * scaleMoveSpeed);

            parentVeryFront.transform.localScale = parentVeryFrontScale;
            parentFrontLeft.transform.localScale = parentFrontLeftScale;
            parentFrontRight.transform.localScale = parentFrontRightScale;
            parentFrontSideLeft.transform.localScale = parentFrontSideLeftScale;
            ParentFrontSideRight.transform.localScale = parentFrontSideRightScale;

            parentVeryFrontCurScaleY = parentVeryFront.transform.localScale.y;
            parentFrontLeftCurScaleY = parentFrontLeft.transform.localScale.y;
            parentFrontRightCurScaleY = parentFrontRight.transform.localScale.y;
            parentFrontSideLeftCurScaleY = parentFrontSideLeft.transform.localScale.y;
            parentFrontSideRightCurScaleY = ParentFrontSideRight.transform.localScale.y;

            //enable physics on front of skirt
            /*
            physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);

            physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);

            physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
            physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);*/

            /*
            if (parentVeryFrontCurScaleY > .999f)
            {
                physicsRotater4.bone0Weight = Mathf.Lerp(physicsRotater4CurBone0Weight, physicsRotater4intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone1Weight = Mathf.Lerp(physicsRotater4CurBone1Weight, physicsRotater4intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater4.bone2Weight = Mathf.Lerp(physicsRotater4CurBone2Weight, physicsRotater4intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }
            if (parentFrontLeftCurScaleY > .999f)
            {
                physicsRotater3.bone0Weight = Mathf.Lerp(physicsRotater3CurBone0Weight, physicsRotater3intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone1Weight = Mathf.Lerp(physicsRotater3CurBone1Weight, physicsRotater3intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater3.bone2Weight = Mathf.Lerp(physicsRotater3CurBone2Weight, physicsRotater3intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }
            if (parentFrontRightCurScaleY > .999f)
            {
                physicsRotater5.bone0Weight = Mathf.Lerp(physicsRotater5CurBone0Weight, physicsRotater5intBone0Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone1Weight = Mathf.Lerp(physicsRotater5CurBone1Weight, physicsRotater5intBone1Weight, Time.deltaTime * physicsWeightSpeed);
                physicsRotater5.bone2Weight = Mathf.Lerp(physicsRotater5CurBone2Weight, physicsRotater5intBone2Weight, Time.deltaTime * physicsWeightSpeed);
            }*/

            /*
            if (parentFrontSideLeftCurScaleY > .999f)
            {
            }
            if (parentFrontSideRightCurScaleY > .999f)
            {
            }*/

            float xValueToMove = 0f;
            float yValueToMove = 0f;
            float zValueToMove = .32f;

            //left back target
            float xValueToMove0 = xValueToMove;
            float yValueToMove0 = yValueToMove;
            float zValueToMove0 = zValueToMove;
            xSlide0 = Mathf.Lerp(xSlide0, xValueToMove0, Time.deltaTime * moveSpeed);
            ySlide0 = Mathf.Lerp(ySlide0, yValueToMove0, Time.deltaTime * moveSpeed);
            zSlide0 = Mathf.Lerp(zSlide0, zValueToMove0, Time.deltaTime * moveSpeed);

            //left side target
            float xValueToMove1 = xValueToMove;
            float yValueToMove1 = yValueToMove;
            float zValueToMove1 = zValueToMove;
            xSlide1 = Mathf.Lerp(xSlide1, xValueToMove1, Time.deltaTime * moveSpeed);
            ySlide1 = Mathf.Lerp(ySlide1, yValueToMove1, Time.deltaTime * moveSpeed);
            zSlide1 = Mathf.Lerp(zSlide1, zValueToMove1, Time.deltaTime * moveSpeed);

            //left side-front target
            float xValueToMove2 = xValueToMove;
            float yValueToMove2 = yValueToMove;
            float zValueToMove2 = zValueToMove;
            xSlide2 = Mathf.Lerp(xSlide2, xValueToMove2, Time.deltaTime * moveSpeed);
            ySlide2 = Mathf.Lerp(ySlide2, yValueToMove2, Time.deltaTime * moveSpeed);
            zSlide2 = Mathf.Lerp(zSlide2, zValueToMove2, Time.deltaTime * moveSpeed);

            //left front target
            float xValueToMove3 = xValueToMove;
            float yValueToMove3 = yValueToMove;
            float zValueToMove3 = zValueToMove;
            xSlide3 = Mathf.Lerp(xSlide3, xValueToMove3, Time.deltaTime * moveSpeed);
            ySlide3 = Mathf.Lerp(ySlide3, yValueToMove3, Time.deltaTime * moveSpeed);
            zSlide3 = Mathf.Lerp(zSlide3, zValueToMove3, Time.deltaTime * moveSpeed);

            //very front target
            float xValueToMove4 = xValueToMove;
            float yValueToMove4 = yValueToMove;
            float zValueToMove4 = zValueToMove;
            xSlide4 = Mathf.Lerp(xSlide4, xValueToMove4, Time.deltaTime * moveSpeed);
            ySlide4 = Mathf.Lerp(ySlide4, yValueToMove4, Time.deltaTime * moveSpeed);
            zSlide4 = Mathf.Lerp(zSlide4, zValueToMove4, Time.deltaTime * moveSpeed);

            //right front target
            float xValueToMove5 = xValueToMove;
            float yValueToMove5 = yValueToMove;
            float zValueToMove5 = zValueToMove;
            xSlide5 = Mathf.Lerp(xSlide5, xValueToMove5, Time.deltaTime * moveSpeed);
            ySlide5 = Mathf.Lerp(ySlide5, yValueToMove5, Time.deltaTime * moveSpeed);
            zSlide5 = Mathf.Lerp(zSlide5, zValueToMove5, Time.deltaTime * moveSpeed);

            //right side-front target
            float xValueToMove6 = xValueToMove;
            float yValueToMove6 = yValueToMove;
            float zValueToMove6 = zValueToMove;
            xSlide6 = Mathf.Lerp(xSlide6, xValueToMove6, Time.deltaTime * moveSpeed);
            ySlide6 = Mathf.Lerp(ySlide6, yValueToMove6, Time.deltaTime * moveSpeed);
            zSlide6 = Mathf.Lerp(zSlide6, zValueToMove6, Time.deltaTime * moveSpeed);

            //right side target
            float xValueToMove7 = xValueToMove;
            float yValueToMove7 = yValueToMove;
            float zValueToMove7 = zValueToMove;
            xSlide7 = Mathf.Lerp(xSlide7, xValueToMove7, Time.deltaTime * moveSpeed);
            ySlide7 = Mathf.Lerp(ySlide7, yValueToMove7, Time.deltaTime * moveSpeed);
            zSlide7 = Mathf.Lerp(zSlide7, zValueToMove7, Time.deltaTime * moveSpeed);

            //right back target
            float xValueToMove8 = xValueToMove;
            float yValueToMove8 = yValueToMove;
            float zValueToMove8 = zValueToMove;
            xSlide8 = Mathf.Lerp(xSlide8, xValueToMove8, Time.deltaTime * moveSpeed);
            ySlide8 = Mathf.Lerp(ySlide8, yValueToMove8, Time.deltaTime * moveSpeed);
            zSlide8 = Mathf.Lerp(zSlide8, zValueToMove8, Time.deltaTime * moveSpeed);

            //very back target
            float xValueToMove9 = xValueToMove;
            float yValueToMove9 = yValueToMove;
            float zValueToMove9 = zValueToMove;
            xSlide9 = Mathf.Lerp(xSlide9, xValueToMove9, Time.deltaTime * moveSpeed);
            ySlide9 = Mathf.Lerp(ySlide9, yValueToMove9, Time.deltaTime * moveSpeed);
            zSlide9 = Mathf.Lerp(zSlide9, zValueToMove9, Time.deltaTime * moveSpeed);
        }

    }

    void Update()
    {
        //Create a new mesh to bake the vertex data into.
        bakedSkirtTargetMesh = new Mesh();

        // Bake the current state of the skinned mesh into a temporary mesh.
        skirtTargetSkinnedMesh.BakeMesh(bakedSkirtTargetMesh);

        // Get the list of vertices from the baked mesh.
        Vector3[] skirtTargetMeshVertices = bakedSkirtTargetMesh.vertices;

        targetSetter(skirtTargetMeshVertices);

        /*
        physicsRotater3CurBone0Weight = physicsRotater3.bone0Weight;
        physicsRotater3CurBone1Weight = physicsRotater3.bone1Weight;
        physicsRotater3CurBone2Weight = physicsRotater3.bone2Weight;

        physicsRotater4CurBone0Weight = physicsRotater4.bone0Weight;
        physicsRotater4CurBone1Weight = physicsRotater4.bone1Weight;
        physicsRotater4CurBone2Weight = physicsRotater4.bone2Weight;

        physicsRotater5CurBone0Weight = physicsRotater5.bone0Weight;
        physicsRotater5CurBone1Weight = physicsRotater5.bone1Weight;
        physicsRotater5CurBone2Weight = physicsRotater5.bone2Weight;*/

        /*
        Vector3 target0 = new Vector3(skirtTargetMeshVertices[2].x, skirtTargetMeshVertices[2].y, skirtTargetMeshVertices[2].z);
        skirtTargets[0].transform.position = WorldPositionCalculator(target0);

        Vector3 target1 = new Vector3(skirtTargetMeshVertices[28].x, skirtTargetMeshVertices[28].y, skirtTargetMeshVertices[28].z);
        skirtTargets[1].transform.position = WorldPositionCalculator(target1);

        Vector3 target2 = new Vector3(skirtTargetMeshVertices[5].x, skirtTargetMeshVertices[5].y, skirtTargetMeshVertices[5].z);
        skirtTargets[2].transform.position = WorldPositionCalculator(target2);

        Vector3 target3 = new Vector3(skirtTargetMeshVertices[7].x, skirtTargetMeshVertices[7].y, skirtTargetMeshVertices[7].z);
        skirtTargets[3].transform.position = WorldPositionCalculator(target3);

        Vector3 target5 = new Vector3(skirtTargetMeshVertices[10].x, skirtTargetMeshVertices[10].y, skirtTargetMeshVertices[10].z);
        skirtTargets[5].transform.position = WorldPositionCalculator(target5);

        Vector3 target6 = new Vector3(skirtTargetMeshVertices[16].x, skirtTargetMeshVertices[16].y, skirtTargetMeshVertices[16].z);
        skirtTargets[6].transform.position = WorldPositionCalculator(target6);

        Vector3 target7 = new Vector3(skirtTargetMeshVertices[18].x , skirtTargetMeshVertices[18].y, skirtTargetMeshVertices[18].z);
        skirtTargets[7].transform.position = WorldPositionCalculator(target7);

        Vector3 target8 = new Vector3(skirtTargetMeshVertices[19].x, skirtTargetMeshVertices[19].y, skirtTargetMeshVertices[19].z);
        skirtTargets[8].transform.position = WorldPositionCalculator(target8);

        Vector3 target9 = new Vector3(skirtTargetMeshVertices[25].x, skirtTargetMeshVertices[25].y, skirtTargetMeshVertices[25].z);
        skirtTargets[9].transform.position = WorldPositionCalculator(target9);

        Vector3 target4 = new Vector3(skirtTargetMeshVertices[8].x, skirtTargetMeshVertices[8].y, skirtTargetMeshVertices[8].z);
        skirtTargets[4].transform.position = WorldPositionCalculator(target4);*/

        
        
        Vector3 target0 = new Vector3(skirtTargetMeshVertices[2].x + xSlide0, skirtTargetMeshVertices[2].y + ySlide0, skirtTargetMeshVertices[2].z + zSlide0);
        skirtTargets[0].transform.position = WorldPositionCalculator(target0);

        Vector3 target1 = new Vector3(skirtTargetMeshVertices[28].x + xSlide1, skirtTargetMeshVertices[28].y + ySlide1, skirtTargetMeshVertices[28].z + zSlide1);
        skirtTargets[1].transform.position = WorldPositionCalculator(target1);

        Vector3 target2 = new Vector3(skirtTargetMeshVertices[5].x + xSlide2, skirtTargetMeshVertices[5].y + ySlide2, skirtTargetMeshVertices[5].z + zSlide2);
        skirtTargets[2].transform.position = WorldPositionCalculator(target2);

        Vector3 target3 = new Vector3(skirtTargetMeshVertices[7].x + xSlide3, skirtTargetMeshVertices[7].y + ySlide3, skirtTargetMeshVertices[7].z + zSlide3);
        skirtTargets[3].transform.position = WorldPositionCalculator(target3);

        Vector3 target5 = new Vector3(skirtTargetMeshVertices[10].x + xSlide5, skirtTargetMeshVertices[10].y + ySlide5, skirtTargetMeshVertices[10].z + zSlide5);
        skirtTargets[5].transform.position = WorldPositionCalculator(target5);

        Vector3 target6 = new Vector3(skirtTargetMeshVertices[16].x + xSlide6, skirtTargetMeshVertices[16].y + ySlide6, skirtTargetMeshVertices[16].z + zSlide6);
        skirtTargets[6].transform.position = WorldPositionCalculator(target6);

        Vector3 target7 = new Vector3(skirtTargetMeshVertices[18].x + xSlide7, skirtTargetMeshVertices[18].y + ySlide7, skirtTargetMeshVertices[18].z + zSlide7);
        skirtTargets[7].transform.position = WorldPositionCalculator(target7);

        Vector3 target8 = new Vector3(skirtTargetMeshVertices[19].x + xSlide8, skirtTargetMeshVertices[19].y + ySlide8, skirtTargetMeshVertices[19].z + zSlide8);
        skirtTargets[8].transform.position = WorldPositionCalculator(target8);

        Vector3 target9 = new Vector3(skirtTargetMeshVertices[25].x + xSlide9, skirtTargetMeshVertices[25].y + ySlide9, skirtTargetMeshVertices[25].z + zSlide9);
        skirtTargets[9].transform.position = WorldPositionCalculator(target9);

        Vector3 target4 = new Vector3(skirtTargetMeshVertices[8].x + xSlide4, skirtTargetMeshVertices[8].y - .005f + ySlide4, skirtTargetMeshVertices[8].z + zSlide4);
        skirtTargets[4].transform.position = WorldPositionCalculator(target4);
    }

}