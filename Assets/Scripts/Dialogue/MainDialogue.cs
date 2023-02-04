using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Main Dialogue")]
    public class MainDialogue : DialogueBase
    {
        [Multiline]
        [SerializeField]
        private string dialogue;

        MainDialogue()
        {
            dialogueType = DialogueType.MAIN;
        }

        public void SetText(string str)
        {
            dialogue = str;
        }

        public void SetLeadsTo(DialogueBase dialogue)
        {
            SetLeads(dialogue);
        }

        public override void Invoke(DialogueReader reader)
        {
            reader.PrintDialogue(this);
        }

        public string GetParsedDialogue()
        {
            Debug.Log("TODO: parse dialogue!");
            return dialogue;
        }
    }
}
