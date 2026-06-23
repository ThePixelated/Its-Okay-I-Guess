using System.Collections.Generic;

/// <summary>
/// Single logged player choice from Phase Action (CardMode).
/// Built from ChaptSection.actionSection at the moment "Pilih Aksi" is confirmed.
/// </summary>
[System.Serializable]
public class LogEntry
{
    public string title;        // ChaptSection.Title — situasi/skenario
    public string source;       // ActData.actID — Act mana log ini berasal
    public string actionName;   // text choice yang dipilih (firstChoice/secondChoice)
    public CopingTag tag;       // Adaptive / Maladaptive
    public StatVariable statVariable; // delta stat yang diterapkan

    public LogEntry(string title, string source, string actionName, CopingTag tag, StatVariable statVariable)
    {
        this.title = title;
        this.source = source;
        this.actionName = actionName;
        this.tag = tag;
        this.statVariable = statVariable;
    }
}

[System.Serializable]
public class LogWrapper
{
    public List<LogEntry> logs;
}