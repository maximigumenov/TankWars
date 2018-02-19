using UnityEngine;
using System.Collections;
using Unit;

public class MonitorCamera : MonoBehaviour
{
    [SerializeField] Characteristics characteristics;
    [SerializeField] Transform targetMove;
    [SerializeField] Transform targetLook;
    [SerializeField] Transform cameraObject;
    SimpleMove moveObject = new SimpleMove();
    public static Transform transformCamera;
    public static Vector3 startPosition;
    // Use this for initialization
    void Start()
    {
        moveObject.Initialization(characteristics);
        MonitorCamera.transformCamera = characteristics.transformObject;
        MonitorCamera.startPosition = MonitorCamera.transformCamera.position;
    }

    public static void SetStartPosition()
    {
        MonitorCamera.transformCamera.position = MonitorCamera.startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        moveObject.Move(cameraObject, targetMove);
        moveObject.TurnToTarget(cameraObject, targetLook);
    }
}
