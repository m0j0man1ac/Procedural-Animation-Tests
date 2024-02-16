//using Codice.Client.Common;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

using static UnityEngine.ParticleSystem;

public class AnimatedFollow : MonoBehaviour
{
    //original formula
    //y + k(y2) + (k2)(y3) = x + (k3)(x2)
    [SerializeField]
    private Transform target;

    private Rigidbody rb;

    private Vector3 xd; //estimated target velocity
    private Vector3 xp; //previous target location

    //state variables
    private Vector3 vel; //y
    private Vector3 accel; //yd

    //dynamic constraint inputs
    [Range(0f, 10f)]
    [SerializeField] private float f; //frequency
    [Range(0f, 5f)]
    [SerializeField] private float z; //damping 
    [Range(-1f, 1f)]
    [SerializeField] private float r; //responsiveness?

    private float k1, k2, k3; //dynamic constraints

    [Header("Preview Stuff")]
    [Range(20,100)]
    [SerializeField] private int curveResInt = 50;
    private float curveRes { get { return curveResInt * 1.0f; } }
    [SerializeField] private Vector2[] curvePreviewPoints = new Vector2[] {Vector2.zero, new Vector2(.2f, 1), Vector2.one};
    //[SerializeField] private AnimationCurve targetCurve;
    //[SerializeField] private AnimationCurve previewCurve;
    [SerializeField] private MinMaxCurve curve;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        //initilise position
        xp = target.position;
        transform.position = target.position;

        ComputeConstraints();
    }

    void FixedUpdate()
    {
        //estimate target velocity
        xd = (target.position - xp) / Time.fixedDeltaTime;

        rb.position += Time.fixedDeltaTime * vel;
        accel = target.position + k3*xd - rb.position - k1*vel;
        vel += Time.fixedDeltaTime / k2 * accel;

        //update last position
        xp = target.position;
    }

    void ComputeConstraints()
    {
        //compute constraints
        k1 = z / (Mathf.PI * f);
        k2 = 1 / (Mathf.Pow(2 * Mathf.PI * f, 2));
        k3 = r * z / (2 * Mathf.PI * f);
    }

    private void OnValidate()
    {
        ComputeConstraints();
        ComputePreviewCurve();
    }

    private void ComputePreviewCurve()
    {
        //messing with animation curve
        //clear
        //targetCurve.ClearKeys();
        //previewCurve.ClearKeys();
        curve.mode = ParticleSystemCurveMode.TwoCurves;
        curve.curveMin.ClearKeys(); //target curve
        curve.curveMax.ClearKeys(); //follow curve

        //set target points currently moving between
        int previewPointIdx = 0;
        Vector2 curP = curvePreviewPoints[previewPointIdx],
            nextP = curvePreviewPoints[previewPointIdx + 1];

        //time step
        float curveStep = 1 / curveRes;

        //values for preview curve
        float prevTargY = curvePreviewPoints[0].y;
        float targVel = 0;

        float previewVel = 0;
        float previewAccel = 0;

        curve.curveMax.AddKey(curvePreviewPoints[0].x, curvePreviewPoints[0].y);
        curve.curveMin.AddKey(curvePreviewPoints[0].x, curvePreviewPoints[0].y);

        for (int i = 0; i < curveRes; i++)
        {
            float graphTime = i / curveRes;

            //targetCurve
            if (graphTime > nextP.x)
            {
                curP = curvePreviewPoints[++previewPointIdx];
                nextP = curvePreviewPoints[previewPointIdx + 1];
            }
            float targY = Mathf.Lerp(curP.y, nextP.y, (graphTime - curP.x) / (nextP.x - curP.x));
            curve.curveMin.AddKey(graphTime, targY);

            //previewCurve
            //estimate vel
            targVel = (targY - prevTargY) / curveStep;

            float y = curve.curveMax.keys[curve.curveMax.length - 1].value + curveStep * previewVel;
            previewAccel = targY + k3 * targVel - y - k1 * previewVel;
            previewVel += curveStep / k2 * previewAccel;

            //previewCurve.AddKey(graphTime, y);
            //curve.keys[curve.keys.Length - 1].weightedMode = WeightedMode.None;

            curve.curveMax.AddKey(graphTime, y);

            //update last target Y
            prevTargY = targY;

            /*
            //estimate target velocity
            xd = (target.position - xp) / Time.fixedDeltaTime;

            rb.position += Time.fixedDeltaTime * vel;
            accel = target.position + k3*xd - rb.position - k1*vel;
            vel += Time.fixedDeltaTime / k2 * accel;

            //update last position
            xp = target.position; 
            */
        }
    }
}
