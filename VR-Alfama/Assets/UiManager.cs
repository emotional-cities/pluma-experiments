using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public MessagePanel MessagePanel;

    // Start is called before the first frame update
    void Start()
    {
        CloseMessagePanel();
    }

    public void OpenMessagePanel(InfoMessage message)
    {
        MessagePanel.gameObject.SetActive(true);

        MessagePanel.TitleText.text = message.Title;
        MessagePanel.BodyText.text = message.Body;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.gameObject.SetActive(false);
    }

    public struct InfoMessage
    {
        public string Title;
        public string Body;
    }
}
