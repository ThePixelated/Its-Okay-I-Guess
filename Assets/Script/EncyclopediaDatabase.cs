using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Master list of all Encyclopedia entries (the "database" Tablet Mode UI reads from).
/// Create exactly one of these and assign it to the Encyclopedia UI controller.
/// </summary>
[CreateAssetMenu(fileName = "EncyclopediaDatabase", menuName = "Scriptable Objects/EncyclopediaDatabase")]
public class EncyclopediaDatabase : ScriptableObject
{
    public List<EncyclopediaEntry> entries = new List<EncyclopediaEntry>();

    public List<EncyclopediaEntry> GetByTag(CopingTag tag)
    {
        return entries.FindAll(e => e.tag == tag);
    }

    public EncyclopediaEntry GetByID(string entryID)
    {
        return entries.Find(e => e.entryID == entryID);
    }
}