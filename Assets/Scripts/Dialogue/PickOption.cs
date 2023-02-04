using System;
using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Option")]
    public class PickOption : DialogueBase
    {
        [Multiline]
        [SerializeField]
        private string optionText;

        PickOption()
        {
            dialogueType = DialogueType.PICK;
        }

        public string GetParsedOption()
        {
            // TODO: Parse Option
            return optionText;
        }
    }
}
