using UnityEngine;

public class targetDampTrackDress : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        Vector3 directionVector = target.transform.position - transform.position;
        Quaternion rotationToAlign = Quaternion.FromToRotation(transform.up, directionVector);
        transform.rotation = rotationToAlign * transform.rotation;
    }

    void LateUpdate()
    {
        Vector3 directionVector = target.transform.position - transform.position;
        Quaternion rotationToAlign = Quaternion.FromToRotation(transform.up, directionVector);
        transform.rotation = rotationToAlign * transform.rotation;
    }
}
