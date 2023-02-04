using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public static class WrongAnswerResponses
{
    public static string[] responses = new string[]
    {
        "No.",
        "Try again.",
        "Not correct!",
        "Think harder.",
        "Are you even trying?"
    };

    public const string noAnswer = "Why are you so quiet?";
}
