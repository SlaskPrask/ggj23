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
        [SerializeField]
        private string correctAnswer;

        TypeAnswer()
        {
            dialogueType = DialogueType.TYPE;
        }

        public DialogueBase ValidateAnswer(string answer, ref string response)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                // Quiet
                response = AnswerResponses.noAnswer;
                return null;
            }
            if (correctAnswer.ToLower() == answer.ToLower())
            {
                return GetLeadsTo();
            }

            //Wrong answer
            response = AnswerResponses.wrongResponses[UnityEngine.Random.Range(0, AnswerResponses.wrongResponses.Length)];
            return null;
        }

        public string GetFirstLetterOfAnswer()
        {
            return correctAnswer.Substring(0, 1);
        }
    }
}