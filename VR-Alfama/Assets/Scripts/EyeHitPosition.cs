using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HP.Omnicept.Unity;

public class EyeHitPosition : MonoBehaviour
{
    public GliaDataPublisher gliaDataPublisher;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        transform.position = gliaDataPublisher.hitPoint;
        
    }
}
