using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TrialSession : DataPublisher
{
    [System.Serializable]
    public class Trial
    {
        public int SecondsDuration;
        public Texture2D Map;
    }

    public float SecondsInterTrialInterval = 3;

    public VrInteractionController InteractionSource;
    public UiManager UiManager;

    public Trial[] TrialList;
    private int CurrentTrialIndex = 0;

    private delegate void UpdateAction();
    private UpdateAction updateAction = () => { };

    private float Timer = 0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        updateAction = InitialState;   
    }

    // Update is called once per frame
    void Update()
    {
        updateAction();
    }

    void InitialState()
    {
        // Open initial menu
        UiManager.OpenMessagePanel(new UiManager.InfoMessage { Title = "AlfamaVR", Body = "Adjust the headset and pick up the controllers. Press the right trigger to continue." });

        updateAction = StartState;
    }

    void StartState()
    {
        if (InteractionSource.RightInteractionState)
        {
            Timer = SecondsInterTrialInterval;
            UiManager.CloseMessagePanel();
            updateAction = InterTrialIntervalState;
        }
    }

    void InterTrialIntervalState()
    {
        UiManager.OpenMessagePanel(new UiManager.InfoMessage { Title = "AlfamaVR", Body = String.Format("Prepare to explore the space in" + System.Environment.NewLine + "{0}", Timer) });

        Timer -= Time.deltaTime;
    }
}
