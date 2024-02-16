using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBasicConstant : MonoBehaviour
{
    [SerializeField]
    private float speed = 1;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * speed);   
    }
}
