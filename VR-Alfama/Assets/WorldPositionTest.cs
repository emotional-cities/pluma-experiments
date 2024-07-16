using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPositionTest : MonoBehaviour
{
    public GameObject Object;
    public RawImage RawImage;
    public Camera SnapCamera;

    // Start is called before the first frame update
    void Start()
    {
        SnapCamera.aspect = 1;

        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = SnapCamera.targetTexture;

        SnapCamera.Render();

        Texture2D snapShot = new Texture2D(SnapCamera.targetTexture.width, SnapCamera.targetTexture.height);
        snapShot.ReadPixels(new Rect(0, 0, SnapCamera.targetTexture.width, SnapCamera.targetTexture.height), 0, 0);
        snapShot.Apply();

        RenderTexture.active = currentRenderTexture;

        RawImage.texture = snapShot;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Input.mousePosition;

        // Get UI raycast
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));

            RaycastHit hit;
            Physics.Raycast(worldPoint, Camera.main.transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("UI"));

            if (hit.transform != null)
            {
                Vector3 localHit = RawImage.transform.InverseTransformPoint(hit.point);

                BoxCollider collider = (BoxCollider)hit.collider;
                float fractionX = (localHit.x + (collider.size.x / 2)) / collider.size.x;
                float fractionY = (localHit.y + (collider.size.y / 2)) / collider.size.y;

                Debug.Log(fractionX.ToString() + " - " + fractionY.ToString());

                Vector3 convertedPoint = SnapCamera.ScreenToWorldPoint(new Vector3(Screen.height * fractionX, Screen.height*fractionY, Camera.main.nearClipPlane));

                Debug.Log(convertedPoint);

                Object.transform.position = convertedPoint;
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, cam.nearClipPlane + 2f));
        //    worldPoint.y = 10f;
        //    Debug.Log(worldPoint);
        //    Object.transform.position = worldPoint;
        //}
    }
}
