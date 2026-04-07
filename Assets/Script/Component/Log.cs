using System.Collections.Generic;

[System.Serializable] // WAJIB agar bisa dibaca Unity & List
public class LogEntry
{
    public string source;
    public string actionName;
    public CopingTag tag;
    public string statImpact;
    public bool isSignificant;

    // Constructor untuk mempermudah pembuatan log baru
    public LogEntry(string source, string action, CopingTag tag, string impact, bool significant)
    {
        this.source = source;
        this.actionName = action;
        this.tag = tag;
        this.statImpact = impact;
        this.isSignificant = significant;
    }
}

[System.Serializable]
public class LogWrapper
{
    public List<LogEntry> logs;
}