using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    public enum DialogueType
    {
        NULL = 0,
        MAIN = 1,
        EVENT = 2,
        PICK = 3,
        TYPE = 4
    }
    public abstract class DialogueBase : ScriptableObject
    {
        public DialogueType dialogueType { get; protected set; }
        [SerializeField]
        private DialogueBase leadsTo;

        protected void SetLeads(DialogueBase dialogue)
        {
            leadsTo = dialogue;
        }

        public virtual void Invoke(DialogueReader reader) 
        { 
            Debug.LogError("Faulty invoke: " + name); 
        }
        public DialogueBase GetLeadsTo()
        {
            return leadsTo;
        }
    }
}