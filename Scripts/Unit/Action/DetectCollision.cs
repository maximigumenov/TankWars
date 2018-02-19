using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class DetectCollision : MonoBehaviour
{
    [SerializeField] CollisionData onCollisionEnter;

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.GetComponent<IDetectCollisionTarget>() != null)
        {
            IDetectCollisionTarget detectCollisionTarget = col.transform.GetComponent<IDetectCollisionTarget>();
            detectCollisionTarget.ActiveEvent();
        }
    }

    [System.Serializable]
    public class CollisionData
    {
        [SerializeField] GameObject parent;
    }
}
