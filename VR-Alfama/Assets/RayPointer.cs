using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPointer : MonoBehaviour
{
    public float PointerLength = 1.5f;
    private LineRenderer LineRenderer;
    private Vector3[] LinePositions;

    // Start is called before the first frame update
    void Start()
    {
        LineRenderer = GetComponent<LineRenderer>();
        LinePositions = new Vector3[2] { transform.parent.position, transform.parent.position + (transform.parent.forward * 1.5f) };
    }

    // Update is called once per frame
    void Update()
    {
        LinePositions[0] = transform.parent.position;
        LinePositions[1] = transform.parent.position + (transform.parent.forward * PointerLength);

        LineRenderer.SetPositions(LinePositions);
    }
}
