using System;
using System.Collections.Generic;


[Serializable]
public class VideoTapeList
{
    public string base_url;
    public List<OnlineVideoTape> videos;
}

[Serializable]
public class OnlineVideoTape
{
    public string description;
    public string file_name;
    public bool is_local;
    public string external_url;

    public OnlineVideoTape(string description, string file_name, bool is_local, string external_url)
    {
        this.description = description;
        this.file_name = file_name;
        this.is_local = is_local;
        this.external_url = external_url;
    }
}
