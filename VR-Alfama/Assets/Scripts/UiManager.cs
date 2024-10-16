using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public MessagePanel MessagePanel;
    public ImagePanel ImagePanel;

    // Start is called before the first frame update
    void Awake()
    {
        CloseMessagePanel();
        CloseImagePanel();
    }

    public void OpenMessagePanel(string title, string body)
    {
        MessagePanel.gameObject.SetActive(true);

        MessagePanel.TitleText.text = title;
        MessagePanel.BodyText.text = body;
        Color color = new Color(0, 0, 0, 1);
        MessagePanel.GetComponent<Image>().color = color;
    }

    public void OpenMessagePanel(string title, string body, float opacity)
    {
        OpenMessagePanel(title, body);

        Color col = new Color(0, 0, 0, opacity);
        MessagePanel.GetComponent<Image>().color = col;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.gameObject.SetActive(false);
    }

    public void OpenImagePanel(string title, Texture2D image, string message)
    {
        ImagePanel.gameObject.SetActive(true);

        ImagePanel.TitleText.text = title;
        ImagePanel.Image.texture = image;
        ImagePanel.Text.text = message;
    }

    public void OpenImagePanel(string title, Texture2D image, string message, bool imageCursorVisible)
    {
        OpenImagePanel(title, image, message);

        ImagePanel.ImageCursor.gameObject.SetActive(imageCursorVisible);
    }

    public void SetImageCursorPosition(Vector3 position)
    {
        ImagePanel.ImageCursor.anchoredPosition = position;
    }

    public void CloseImagePanel()
    {
        ImagePanel.gameObject.SetActive(false);
        ImagePanel.ImageCursor.gameObject.SetActive(false);
    }
}
