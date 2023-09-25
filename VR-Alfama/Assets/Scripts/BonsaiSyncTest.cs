using Bonsai.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonsaiSyncTest : MonoBehaviour
{
    public BonsaiWorkflow Workflow;

    private object lastBonsaiValue;

    void Awake()
    {
        Workflow.InjectSubscription("SerialPortOutput", typeof(PublishSubjectBuilder), val =>
        {
            Debug.Log(val);
        });
        //Workflow.InjectSubscription("TimerOutput", typeof(SubscribeSubjectBuilder), val => { 
        //    Debug.Log(val);
        //    lastBonsaiValue = val;
        //});
        //Workflow.InjectSubscription("UnityTimeStamp", typeof(SubscribeSubjectBuilder), val => {
        //    Debug.Log(val);
        //});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
