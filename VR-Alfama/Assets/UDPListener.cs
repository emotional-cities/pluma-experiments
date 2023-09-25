using Bonsai.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
    BonsaiWorkflow Workflow;
    private void Awake()
    {
        Workflow = GetComponent<BonsaiWorkflow>();
        Workflow.InjectSubscription("ReceiveTimer", typeof(PublishSubjectBuilder), val =>
        {
            Debug.Log(val);
        });
    }
}
