using UnityEngine;

public class skirtParentRotater : MonoBehaviour
{
    public Transform parDressBack, parDressFront, parDressInFrontLeft, parDressInFrontRight, parDressOutBackLeft, parDressOutBackRight, parDressOutFrontLeft, parDressOutFrontRight, parDressSideLeft, parDressSideRight;

    public Transform trackDressBack, trackDressFront, trackDressInFrontLeft, trackDressInFrontRight, trackDressOutBackLeft, trackDressOutBackRight, trackDressOutFrontLeft, trackDressOutFrontRight, trackDressSideLeft, trackDressSideRight;

    //public Transform physObject4;
    void Awake()
    {
        
    }

    void Start()
    {
        
        parDressBack.transform.rotation = trackDressBack.transform.rotation;
        parDressFront.transform.rotation = trackDressFront.transform.rotation;
        parDressInFrontLeft.transform.rotation = trackDressInFrontLeft.transform.rotation;
        parDressInFrontRight.transform.rotation = trackDressInFrontRight.transform.rotation;
        parDressOutBackLeft.transform.rotation = trackDressOutBackLeft.transform.rotation;
        parDressOutBackRight.transform.rotation = trackDressOutBackRight.transform.rotation;
        parDressOutFrontLeft.transform.rotation = trackDressOutFrontLeft.transform.rotation;
        parDressOutFrontRight.transform.rotation = trackDressOutFrontRight.transform.rotation;
        parDressSideLeft.transform.rotation = trackDressSideLeft.transform.rotation;
        parDressSideRight.transform.rotation = trackDressSideRight.transform.rotation;
    }
        void LateUpdate()
    {
        parDressBack.transform.rotation = trackDressBack.transform.rotation;
        parDressFront.transform.rotation = trackDressFront.transform.rotation;
        parDressInFrontLeft.transform.rotation = trackDressInFrontLeft.transform.rotation;
        parDressInFrontRight.transform.rotation = trackDressInFrontRight.transform.rotation;
        parDressOutBackLeft.transform.rotation = trackDressOutBackLeft.transform.rotation;
        parDressOutBackRight.transform.rotation = trackDressOutBackRight.transform.rotation;
        parDressOutFrontLeft.transform.rotation = trackDressOutFrontLeft.transform.rotation;
        parDressOutFrontRight.transform.rotation = trackDressOutFrontRight.transform.rotation;
        parDressSideLeft.transform.rotation = trackDressSideLeft.transform.rotation;
        parDressSideRight.transform.rotation = trackDressSideRight.transform.rotation;
    }
}
