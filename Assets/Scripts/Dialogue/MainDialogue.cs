using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Main Dialogue")]
public class MainDialogue : DialogueBase
{
    [Multiline]
    [SerializeField]
    private string dialogue;
    [SerializeField]
    private DialogueBase[] dialogueOptions;

    MainDialogue()
    {
        dialogueType = DialogueType.MAIN;
    }

    public string GetDialogue()
    {
        return dialogue;
    }

    public Tuple<DialogueType, string[]> GetOptions()
    {
        if (dialogueOptions.Length < 1)
        {
            Debug.LogError("Dialogue has no options!");
            return new Tuple<DialogueType, string[]>(DialogueType.NULL, null);
        }

        if (dialogueOptions[0].dialogueType == DialogueType.OPTION_WRITE)
        {
            return new Tuple<DialogueType, string[]>(DialogueType.OPTION_WRITE, null);
        }

        string[] opt = new string[dialogueOptions.Length];
        for (int i = 0; i < opt.Length; i++)
        {
            opt[i] = ((DialogueOptionChoise)dialogueOptions[i]).GetOption();
        }
        return new Tuple<DialogueType, string[]>(DialogueType.OPTION_WRITE, opt);
    }
    
    public DialogueBase GetLeadsToFromOption(int option)
    {
        return ((DialogueOptionBase)dialogueOptions[option]).GetLeadsTo();
    }
}
