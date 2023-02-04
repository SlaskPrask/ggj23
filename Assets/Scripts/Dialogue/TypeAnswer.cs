using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Type")]
    public class TypeAnswer : DialogueBase
    {
        TypeAnswer()
        {
            dialogueType = DialogueType.TYPE;
        }

        public override void Invoke(DialogueReader reader)
        {
            throw NotImplementedException(
        }
    }
}