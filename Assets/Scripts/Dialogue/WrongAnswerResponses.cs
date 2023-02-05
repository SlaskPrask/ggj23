using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public static class AnswerResponses
{
    private static string[] wrongResponses = new string[]
    {
        "No.",
        "Try again.",
        "Not correct!",
        "Think harder.",
        "Are you even trying?"
    };

    private static string[] correctResponses = new string[]
    {
        "Right!",
        "Yes!",
        "Okay.",
        "Next!",
        "Hmm, I see..."
    };

    public const string noAnswer = "Why are you so quiet?";
    public static string wrongAnswer 
    { 
        get 
        {
            return wrongResponses[UnityEngine.Random.Range(0, wrongResponses.Length)];
        } 
    }

    public static string correctAnswer
    {
        get
        {
            return correctResponses[UnityEngine.Random.Range(0, correctResponses.Length)];
        }
    }
    

}
