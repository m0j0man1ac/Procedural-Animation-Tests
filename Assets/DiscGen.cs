using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscGen : MonoBehaviour
{
    [SerializeField]
    [Range(10, 1000)]
    private int numPoints = 10;

    [SerializeField]
    [Range (-1f, 1f)]
    private float turnFract;

    [SerializeField]
    [Range (-1f, 1f)]
    private float pow = 1f;

    [SerializeField]
    private GameObject refPointObj;
    private List<SpriteRenderer> points = new List<SpriteRenderer>();

    [SerializeField]
    private Color defaultCol = new Color(255,255,255);

    #if UNITY_EDITOR 
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }
    private void _OnValidate()
    {
        refPointObj.gameObject.SetActive(true);

        if (points.Count != numPoints)
        {
            for (int i = points.Count - 1; i >= 0; i--)
                if (points[i] != null)
                    GameObject.DestroyImmediate(points[i].gameObject);

            points.Clear();

            for (int i = 0; i < numPoints; i++)
            {
                points.Add(GameObject.Instantiate(refPointObj, transform.position, Quaternion.identity, transform)
                    .GetComponent<SpriteRenderer>());
                points[i].name = "point_" + i;
            }
        }

        if(refPointObj == null)
        {
            Debug.LogError("no point reference object to instantiate from");
            return;
        }

        for(int i=0; i<numPoints; i++)
        {
            float dst = Mathf.Pow(i / (numPoints-1f), pow);
            float angle = 2 * Mathf.PI * turnFract * i;

            float x = dst * Mathf.Cos(angle);
            float y = dst * Mathf.Sin(angle);

            PlotPoint(i, x, y, defaultCol);
        }

        refPointObj.gameObject.SetActive(false);
    }
    #endif

    private void PlotPoint(int pointId, float x, float y, Color col)
    {
        //points.Add(GameObject.Instantiate(refPointObj, transform.position, Quaternion.identity, transform)
        //    .GetComponent<SpriteRenderer>());

        points[pointId].color = col;
        points[pointId].transform.position = new Vector3(x, y);
    }
}
