using UnityEngine;

[ExecuteInEditMode]
public class HumanLegIK : MonoBehaviour
{
    [Header("Bones")]
    public Transform thigh;
    public Transform knee;
    public Transform calf;
    public Transform foot;

    [Header("Targets")]
    public Transform footTarget;
    public Transform kneeHint;

    [Header("Settings")]
    [Range(0f, 1f)] public float ikWeight = 1f;

    private float lengthUpper;
    private float lengthLower;
    private float totalLength;

    void OnEnable()
    {
        InitializeLengths();
    }

    void InitializeLengths()
    {
        if (!thigh || !calf || !foot) return;

        // Calculate lengths while ignoring the knee joint's position
        lengthUpper = Vector3.Distance(thigh.position, calf.position);
        lengthLower = Vector3.Distance(calf.position, foot.position);
        totalLength = lengthUpper + lengthLower;
    }

    void LateUpdate()
    {
        if (!thigh || !knee || !calf || !foot || !footTarget || !kneeHint) return;
        if (ikWeight <= 0f) return;

        // 1. Core Trigonometry (Two-Bone Math skipping the Knee)
        Vector3 rootPos = thigh.position;
        Vector3 targetPos = footTarget.position;
        Vector3 hintPos = kneeHint.position;

        Vector3 toTarget = targetPos - rootPos;
        float distToTarget = toTarget.magnitude;

        // Clamp target distance to prevent bone stretching/snapping
        if (distToTarget > totalLength)
        {
            targetPos = rootPos + (toTarget.normalized * totalLength);
            distToTarget = totalLength;
        }
        else if (distToTarget < 0.01f)
        {
            distToTarget = 0.01f;
        }

        // Calculate the bending plane using the Knee Hint
        Vector3 targetDir = toTarget.normalized;
        Vector3 hintDir = (hintPos - rootPos).normalized;
        Vector3 planeNormal = Vector3.Cross(targetDir, hintDir).normalized;
        Vector3 bendDir = Vector3.Cross(planeNormal, targetDir).normalized;

        // Law of Cosines to find interior angle at the Thigh root
        float cosAlpha = (distToTarget * distToTarget + lengthUpper * lengthUpper - lengthLower * lengthLower) / (2f * distToTarget * lengthUpper);
        cosAlpha = Mathf.Clamp(cosAlpha, -1f, 1f);
        float alpha = Mathf.Acos(cosAlpha);

        // Law of Cosines to find interior angle at the Calf joint
        float cosBeta = (lengthUpper * lengthUpper + lengthLower * lengthLower - distToTarget * distToTarget) / (2f * lengthUpper * lengthLower);
        cosBeta = Mathf.Clamp(cosBeta, -1f, 1f);
        float beta = Mathf.Acos(cosBeta);

        // 2. Position the Calf position via math
        Vector3 calculatedCalfPos = rootPos + (Quaternion.AngleAxis(alpha * Mathf.Rad2Deg, planeNormal) * targetDir) * lengthUpper;

        // 3. Apply Rotations to the Main Chain
        Quaternion targetThighRot = Quaternion.LookRotation(calculatedCalfPos - rootPos, bendDir);
        Quaternion targetCalfRot = Quaternion.LookRotation(targetPos - calculatedCalfPos, calculatedCalfPos - rootPos);

        thigh.rotation = Quaternion.Slerp(thigh.rotation, targetThighRot, ikWeight);
        calf.rotation = Quaternion.Slerp(calf.rotation, targetCalfRot, ikWeight);
        foot.rotation = Quaternion.Slerp(foot.rotation, footTarget.rotation, ikWeight);

        // 4. The Knee Fix: Position and blend the extra knee joint
        // Keeps the knee bone perfectly centered between thigh and calf
        knee.position = Vector3.Lerp(thigh.position, calf.position, 0.5f);

        // Averages the rotation between thigh and calf for smooth skin deformation
        Quaternion averageKneeRot = Quaternion.Slerp(thigh.rotation, calf.rotation, 0.5f);
        knee.rotation = Quaternion.Slerp(knee.rotation, averageKneeRot, ikWeight);
    }

    // Dynamic length recalculation if you scale the rig in the editor
    void OnValidate()
    {
        InitializeLengths();
    }
}