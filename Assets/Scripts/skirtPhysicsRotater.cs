using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.GraphicsBuffer;

public class skirtPhysicsRotater : MonoBehaviour
{
    [SerializeField] private Transform deformBone1;
    private List<Transform> deformBones;
    private Transform[] physicsTargets;
    private Quaternion bone0CurRot, bone1CurRot, bone2CurRot;
    //skirt targeting needs to access the following:
    public float bone0Weight = .4f, bone1Weight = .8f, bone2Weight = 1f;

    void Start()
    {
        deformBones = GetBonesIncludingEnds(deformBone1.transform);
        physicsTargets = GetComponentsInChildren<Transform>().Skip(1).ToArray();

        bone0CurRot = deformBones[0].transform.rotation;
        bone1CurRot = deformBones[1].transform.rotation;
        bone2CurRot = deformBones[2].transform.rotation;
    }

    void LateUpdate()
    {
        DampTrack(physicsTargets[1], deformBones[0], bone0Weight, bone0CurRot); //blender weight .4f
        DampTrack(physicsTargets[2], deformBones[1], bone1Weight, bone1CurRot); //blender weight .8f
        DampTrack(physicsTargets[3], deformBones[2], bone2Weight, bone2CurRot); //blender weight 1f

        bone0CurRot = deformBones[0].transform.rotation;
        bone1CurRot = deformBones[1].transform.rotation;
        bone2CurRot = deformBones[2].transform.rotation;
    }

    private void DampTrack(Transform target, Transform bone, float weight, Quaternion boneCurRot)
    {
        Vector3 directionVector = target.transform.position - bone.transform.position;
        Quaternion rotationToAlign = Quaternion.FromToRotation(bone.transform.up, directionVector);
        Quaternion fullRotation = rotationToAlign * bone.transform.rotation;
        Quaternion weightedRotation = Quaternion.Slerp(bone.transform.rotation, fullRotation, weight);
        bone.transform.rotation = weightedRotation;
    }

    private List<Transform> GetBonesIncludingEnds(Transform root)
    {
        List<Transform> bones = new List<Transform>();
        RecursiveFindBones(root, bones);
        return bones;
    }

    private void RecursiveFindBones(Transform currentBone, List<Transform> boneList)
    {
        if (currentBone.childCount > 0)
        {
            boneList.Add(currentBone);

            foreach (Transform child in currentBone)
            {
                RecursiveFindBones(child, boneList);
            }
        }
        else
        {
            boneList.Add(currentBone);
        }
    }
}
