using System;

[Serializable]
public class Response<T> 
{
    public string status;
    public string message;
    public string eventType;
    public string error;
    public T data;
}

[Serializable]
public class SubResponse
{
    public string title;
    public string message;

}



