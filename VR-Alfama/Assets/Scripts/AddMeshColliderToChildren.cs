using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshColliderToChildren : MonoBehaviour
{

    public PhysicMaterial ColliderMaterial;
    void Start()
    {
        var meshes = this.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
        {
            var collider = mesh.gameObject.AddComponent<MeshCollider>();
            collider.material = ColliderMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}