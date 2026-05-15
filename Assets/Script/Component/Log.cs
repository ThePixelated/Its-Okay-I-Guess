using System.Collections.Generic;

[System.Serializable] // WAJIB agar bisa dibaca Unity & List
public class LogEntry
{
    public string title;
    public string source;
    public string actionName;
    public CopingTag tag;
    public string statImpact;
    public bool isSignificant;
    public StatVariable statVariable;

    // Constructor untuk mempermudah pembuatan log baru
    public LogEntry(string title, string source, string action, CopingTag tag, string impact, bool significant, StatVariable statVariable)
    {
        this.title = title;
        this.source = source;
        this.actionName = action;
        this.tag = tag;
        this.statImpact = impact;
        this.isSignificant = significant;
        this.statVariable = statVariable;
    }
}

[System.Serializable]
public class LogWrapper
{
    public List<LogEntry> logs;
}