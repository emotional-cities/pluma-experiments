using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public MessagePanel MessagePanel;
    public ImagePanel ImagePanel;

    // Start is called before the first frame update
    void Start()
    {
        CloseMessagePanel();
    }

    public void OpenMessagePanel(string title, string body)
    {
        MessagePanel.gameObject.SetActive(true);

        MessagePanel.TitleText.text = title;
        MessagePanel.BodyText.text = body;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.gameObject.SetActive(false);
    }

    public void OpenImagePanel(string title, Texture2D image)
    {
        ImagePanel.gameObject.SetActive(true);

        ImagePanel.TitleText.text = title;
        ImagePanel.Image.texture = image;
    }

    public void CloseImagePanel()
    {
        ImagePanel.gameObject.SetActive(false);
    }
}
