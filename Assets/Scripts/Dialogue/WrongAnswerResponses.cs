using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public static class AnswerResponses
{
    public static string[] wrongResponses = new string[]
    {
        "No.",
        "Try again.",
        "Not correct!",
        "Think harder.",
        "Are you even trying?"
    };

    public static string[] correctResponses = new string[]
    {
        "Right!",
        "Yes!",
        "Okay.",
        "Next!",
        "Hmm, I see..."
    };

    public const string noAnswer = "Why are you so quiet?";
}
