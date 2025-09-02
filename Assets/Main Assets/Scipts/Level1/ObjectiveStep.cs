using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectiveStep
{
    [TextArea] public string[] introDialogues;
    [TextArea] public string objectiveText;
    public OnScreenPointerPlugin.OnScreenPointerObject[] markers;

    // Instead of List<List<string>>, use a list of a serializable class
    public List<MarkerDialogueSet> markerDialogues = new List<MarkerDialogueSet>();
    public GameObject[] barriers;
}