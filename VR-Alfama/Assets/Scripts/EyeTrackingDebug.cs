using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HP.Omnicept;
using HP.Omnicept.Messaging;
using HP.Omnicept.Messaging.Messages;

public class EyeTrackingDebug : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public GameObject trackCube;
    public Vector3 offset;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void HandleEyeTracking(EyeTracking eyeTracking) {
        Vector3 trackedGazeDirection = new Vector3(-eyeTracking.CombinedGaze.X, eyeTracking.CombinedGaze.Y, eyeTracking.CombinedGaze.Z);
        Vector3 newPosition = transform.position + (transform.rotation * trackedGazeDirection + offset) * 5f;
        trackCube.transform.position = newPosition;

        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, (transform.rotation * trackedGazeDirection)), out hit))
        {
            //Debug.Log(hit.transform.gameObject);
        }
    }
}
