using UnityEngine;

public class RotateBetweenTwoPoints : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    // Which local axis should point along the line?
    public Vector3 localForward = Vector3.forward;

    void LateUpdate()
    {
        if (pointA == null || pointB == null)
            return;

        Vector3 direction = pointB.position - pointA.position;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        // Rotate so this object's forward points along the line.
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Adjust if your model uses a different forward axis.
        transform.rotation = targetRotation * Quaternion.FromToRotation(Vector3.forward, localForward);
    }
}
