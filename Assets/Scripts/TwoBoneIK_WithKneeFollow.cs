using UnityEngine;

public class TwoBoneIK_WithKneeFollow : MonoBehaviour
{
    [Header("Bones")]
    public Transform thigh;
    public Transform knee;
    public Transform shin;
    public Transform foot;

    [Header("Target")]
    public Transform target;
    public Transform pole; // knee direction helper

    [Range(0f, 1f)]
    public float weight = 1f;

    void LateUpdate()
    {
        SolveIK();
        UpdateKneeVisual();
    }

    void SolveIK()
    {
        Vector3 aPos = thigh.position;
        Vector3 bPos = shin.position;
        Vector3 cPos = foot.position;
        Vector3 tPos = target.position;

        float aLen = Vector3.Distance(aPos, bPos);
        float bLen = Vector3.Distance(bPos, cPos);

        Vector3 dir = (tPos - aPos).normalized;
        float dist = Vector3.Distance(aPos, tPos);
        dist = Mathf.Min(dist, aLen + bLen - 0.0001f);

        Vector3 poleDir = pole ? (pole.position - aPos).normalized : Vector3.up;
        Vector3 axis = Vector3.Cross(dir, poleDir).normalized;
        if (axis == Vector3.zero)
            axis = Vector3.Cross(dir, Vector3.up).normalized;

        // Law of cosines
        float angleA = Mathf.Acos(
            Mathf.Clamp((aLen * aLen + dist * dist - bLen * bLen) / (2f * aLen * dist), -1f, 1f)
        ) * Mathf.Rad2Deg;

        float angleB = Mathf.Acos(
            Mathf.Clamp((aLen * aLen + bLen * bLen - dist * dist) / (2f * aLen * bLen), -1f, 1f)
        ) * Mathf.Rad2Deg;

        // Root rotation (thigh)
        Quaternion thighRot =
            Quaternion.LookRotation(dir, axis) *
            Quaternion.AngleAxis(angleA, axis);

        thigh.rotation = Quaternion.Slerp(thigh.rotation, thighRot, weight);

        // Reposition shin (important step)
        shin.position = thigh.position + thigh.rotation * (shin.position - thigh.position);

        Vector3 shinDir = (tPos - shin.position).normalized;

        Quaternion shinRot =
            Quaternion.LookRotation(shinDir, axis) *
            Quaternion.AngleAxis(180f - angleB, axis);

        shin.rotation = Quaternion.Slerp(shin.rotation, shinRot, weight);

        // Foot aligns to target
        foot.position = target.position;
        foot.rotation = target.rotation;
    }

    void UpdateKneeVisual()
    {
        if (!knee || !thigh || !shin) return;

        // Knee is NOT part of IK — it just follows orientation visually

        Vector3 thighToShin = (shin.position - thigh.position).normalized;
        Vector3 kneeUp = Vector3.Cross(thighToShin, Vector3.right);

        if (kneeUp == Vector3.zero)
            kneeUp = Vector3.up;

        // Smooth visual alignment
        Quaternion kneeRot =
            Quaternion.LookRotation(thighToShin, kneeUp);

        knee.rotation = Quaternion.Slerp(knee.rotation, kneeRot, weight);
        knee.position = Vector3.Lerp(knee.position, (thigh.position + shin.position) * 0.5f, weight);
    }
}
