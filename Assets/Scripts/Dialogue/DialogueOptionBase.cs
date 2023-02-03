using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOptionBase : DialogueBase
{
    [SerializeField]
    private DialogueBase leadsTo;

    public DialogueBase GetLeadsTo()
    {
        if (leadsTo == null)
        {
            Debug.LogError("Option: " + name + "\n contains no leadsTo!");
        }
        return leadsTo;
    }
}