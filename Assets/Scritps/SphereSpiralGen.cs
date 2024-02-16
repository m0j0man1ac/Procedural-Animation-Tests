using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpiralGen : MonoBehaviour
{
    [SerializeField]
    [Range(10, 1000)]
    private int numPoints = 10;

    [SerializeField]
    private bool useGoldRatio = false;

    [SerializeField]
    [Range(-100f, 100f)]
    private float turnFract;

    //[SerializeField]
    //[Range(-1f, 1f)]
    //private float pow = 1f;

    [SerializeField]
    private Transform spiralObjsParent;

    [SerializeField]
    private GameObject refPointObj;
    private List<Transform> points = new List<Transform>();
    public List<Transform> SpiralPoints { get { return points; } }

    [SerializeField]
    private Color defaultCol = Color.white;

    private void Start()
    {
        _OnValidate();
    }

#if UNITY_EDITOR
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    private void _OnValidate()
    {
        //parent for object pooling
        if(spiralObjsParent == null) {
            spiralObjsParent = new GameObject("Spiral Parent").transform;
            spiralObjsParent.transform.parent = transform;
            spiralObjsParent.localPosition = Vector3.zero;
            spiralObjsParent.localRotation = Quaternion.identity;
        }

        refPointObj.gameObject.SetActive(true);

        //checking if ref object exists
        if (refPointObj == null)
        {
            Debug.LogError("no point reference object to instantiate from");
            return;
        }

        //object pooling
        if (points.Count != numPoints)
        {
            for (int i = points.Count - 1; i >= 0; i--)
                if (points[i] != null)
                    GameObject.DestroyImmediate(points[i].gameObject);

            points.Clear();

            for (int i = 0; i < numPoints; i++)
            {
                points.Add(GameObject.Instantiate(refPointObj, transform.position, Quaternion.identity, spiralObjsParent).transform);
                points[i].name = "point_" + i;
            }
        }

        //setting spiral angle
        float _turnFract;
        float goldRatio = Mathf.PI * (1 + Mathf.Sqrt(5));
        if (useGoldRatio) turnFract = goldRatio;
        _turnFract = turnFract;

        //distributing points
        for (int i = 0; i < numPoints; i++)
        {
            float index = i + .5f;
            float phi = Mathf.Acos(1 - 2 * index / numPoints);
            float theta = _turnFract * index;

            float x = Mathf.Cos(theta) * Mathf.Sin(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(phi);

            //Vector3 pointPosition = new Vector3(x, y, z);
            PlotPoint(i, x, y, z, defaultCol);
        }

        refPointObj.gameObject.SetActive(false);
    }
    #endif

    private void PlotPoint(int pointId, float x, float y, float z, Color col)
    {
        //points[pointId].color = col;
        points[pointId].localPosition = new Vector3(x, y, z);
    }

    private void OnDrawGizmosSelected()
    {
        var here = transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(here, here + transform.forward * 2);

        Gizmos.color = Color.green;
        int i = 0;
        foreach (var p in points)
        {
            if (i == numPoints / 3) Gizmos.color = Color.black;
            Gizmos.DrawLine(here, here + (p.position - here)*.5f);
            i++;
        }
    }
}
