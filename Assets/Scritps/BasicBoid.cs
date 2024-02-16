using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;

[RequireComponent(typeof(SphereSpiralGen), typeof(Rigidbody))]
public class BasicBoid : MonoBehaviour
{
    //~MACRO
    private Vector3 HERE { get { return transform.position; } set { transform.position = value; } }
    
    private SphereSpiralGen spiralGenScript;
    private Rigidbody rb;

    [SerializeField]
    private int AIBeatPerSecond = 30;

    [SerializeField]
    [Range(0f, 180f)]
    private float maxTurnRadius = 180f;


    [SerializeField]
    private float
        speed = 1f,
        accel = 1f,
        lookAheadTime = 1f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        spiralGenScript = GetComponent<SphereSpiralGen>();

        AIBeat();
    }

    private Vector3 dir2Move;
    private void Update()
    {
        //move in update?
        rb.velocity += dir2Move.normalized * accel * Time.deltaTime;
        //randomness
        //rb.velocity += Random.insideUnitSphere * accel * Time.deltaTime;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);

        //look rotation
        var newRot = Vector3.RotateTowards(transform.forward, rb.velocity.normalized, speed*Time.deltaTime, 0f);
        transform.rotation = Quaternion.LookRotation(newRot);
    }


    private void AIBeat()
    {
        Vector3 dir = CheckForValidDirection();
        //rb.velocity += dir.normalized * accel * (1/(float)AIBeatPerSecond);
        ////randomness
        rb.velocity += Random.insideUnitSphere * accel * (.1f/AIBeatPerSecond);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);

        dir2Move = dir;

        Invoke("AIBeat", 1f/AIBeatPerSecond);
    }

    Vector3 gizmoDir;
    private Vector3 CheckForValidDirection()
    {
        Vector3 dir = Vector3.zero;
        bool hit = true;
        Vector3 checkDir = transform.forward;
        if(rb.velocity!=Vector3.zero)
            checkDir = rb.velocity.normalized;

        foreach (var p in spiralGenScript.SpiralPoints)
        {
            hit = Physics.Raycast(HERE, checkDir, speed * lookAheadTime);

            if (!hit)
            {
                if(Physics.OverlapSphere(HERE + (checkDir.normalized * speed * lookAheadTime), 1.2f).Length == 0 &&
                    Physics.OverlapSphere(HERE + (checkDir.normalized * (speed * lookAheadTime * .2f)), .6f).Length == 0)

                {
                    dir = checkDir;
                    gizmoDir = dir;
                    break;
                }
            }
            checkDir = (p.position - HERE).normalized;
        }
        return dir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        var checkPoint = HERE + transform.forward * speed * lookAheadTime;
        Gizmos.DrawLine(HERE, checkPoint);
        Gizmos.DrawWireSphere(checkPoint, 1.2f);

        Gizmos.color = Color.green;
        checkPoint = HERE + gizmoDir * speed * lookAheadTime;
        Gizmos.DrawLine(HERE, checkPoint);
        Gizmos.DrawWireSphere(checkPoint, 1.2f);
        Gizmos.DrawWireSphere(HERE + gizmoDir * speed * .2f * lookAheadTime, .6f);
    }
}
