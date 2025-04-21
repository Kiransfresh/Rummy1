using System;

[Serializable]
public class ReadOnlyPanelModel
{

    public string title;
    public string message;
    public content html_content;

}
[Serializable]
public class content
{
    public string title;
    public string description;
    public string plain_description;

}
