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

        public bool ValidateAnswer(string answer, ref string response)
        {
            if (correctAnswer.ToLower() == answer.ToLower())
            {
                response = AnswerResponses.correctAnswer;
                return true;
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                // Quiet
                response = AnswerResponses.noAnswer;
            }
            else
            {
                // Wrong answer
                response = AnswerResponses.correctAnswer;
            }
            return false;
        }

        public string GetFirstLetterOfAnswer()
        {
            return correctAnswer.Substring(0, 1);
        }
    }
}