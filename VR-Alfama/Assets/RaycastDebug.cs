using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaycastDebug : MonoBehaviour
{
    public RectTransform Cursor;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow);

            Vector3 localPoint = hit.transform.InverseTransformPoint(hit.point);

            Debug.Log(localPoint);

            Cursor.anchoredPosition = localPoint;
        }
    }
}
