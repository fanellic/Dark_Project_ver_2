using UnityEngine;

public class targetDampTrackDress : MonoBehaviour
{
    public Transform target;

    Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.rotation;
        /*
        Vector3 directionVector = target.transform.position - transform.position;
        Quaternion rotationToAlign = Quaternion.FromToRotation(transform.up, directionVector);
        transform.rotation = rotationToAlign * transform.rotation;*/
    }

    void LateUpdate()
    {
        //Vector3 direction = (target.position - transform.position).normalized;

        //Quaternion delta = Quaternion.FromToRotation(transform.up, direction);

        //transform.rotation = delta * initialRotation;
        
        Vector3 directionVector = target.transform.position - transform.position;
        Quaternion rotationToAlign = Quaternion.FromToRotation(transform.up, directionVector);
        transform.rotation = rotationToAlign * initialRotation;
    }
}
