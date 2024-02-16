using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class IK_Basic_1 : MonoBehaviour
{
    private Vector3 IKpoint;
    private Vector3 nextPoint;

    [SerializeField]
    private float 
        limbLength = 1f,
        strideLength = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        var hit = CheckForNextIKPoint();
        if(hit.point != null)
        {
            IKpoint = hit.point;
            nextPoint = hit.point;
        }
    }

    private void InitialiseLimbs()
    {

    }

    private RaycastHit CheckForNextIKPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.parent.up);
        Physics.Raycast(ray, out hit, limbLength * 1.2f);
        return hit;
    }

    [Header("Debug Options")]
    [SerializeField] 
    private float wireSphereSize = .2f;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (nextPoint != Vector3.zero)
        {
            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawWireSphere(nextPoint, wireSphereSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, IKpoint);
            Gizmos.DrawWireSphere(IKpoint, wireSphereSize);
        }
        else
        {
            var point = transform.position + -transform.parent.up * limbLength;
            Gizmos.DrawLine(transform.position, point);
            Gizmos.DrawWireSphere(point, wireSphereSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, point);
            Gizmos.DrawWireSphere(point, wireSphereSize);
        }        
    }
}
