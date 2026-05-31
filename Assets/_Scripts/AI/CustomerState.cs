using UnityEngine;
// NPC states 
public enum CustomerState 
{
    Idle,
    WalkingToTable,
    PlayingActivity, // changed state here (PlayingBlack to PlayingActivity)
    Cheating,
    Suspicious,
    Interrogating,
    EscortedToBackRoom,
    Leaving,
    Gone
}

