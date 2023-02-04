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

        public override void Invoke(DialogueReader reader)
        {
            throw new NotImplementedException();
        }

        public DialogueBase ValidateAnswer(string answer, ref string response)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                // Quiet
                response = WrongAnswerResponses.noAnswer;
                return null;
            }
            if (correctAnswer.ToLower() == answer.ToLower())
            {
                return GetLeadsTo()[0];
            }

            //Wrong answer
            response = WrongAnswerResponses.responses[UnityEngine.Random.Range(0, WrongAnswerResponses.responses.Length)];
            return null;
        }

        public string GetFirstLetterOfAnswer()
        {
            return correctAnswer.Substring(0, 1);
        }
    }
}