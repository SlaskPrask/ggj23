using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Option")]
    public class PickOption : DialogueBase
    {
        [SerializeField]
        private string correctAnswer;
        [SerializeField]
        private string[] wrongAnswers;
        private int correctChoise = 0;

        PickOption()
        {
            dialogueType = DialogueType.PICK;
        }

        public bool CheckCorrectAnswer(int pickedOption)
        {
            return pickedOption == correctChoise;
        }

        public string[] GetShuffeledOption()
        {
            return ShuffleOptions();
        }

        private string[] ShuffleOptions()
        {
            List<int> options = new List<int> { 0, 1, 2, 3 };
            string[] shuffled = new string[4];
            int index;
            int rand;

            for (int i = 0; i < shuffled.Length; i++)
            {
                rand = UnityEngine.Random.Range(0, options.Count); 
                index = options[rand];
                options.RemoveAt(rand);

                if (i == 3)
                {
                    shuffled[index] = correctAnswer;
                    correctChoise = index;
                }
                else
                {
                    shuffled[index] = wrongAnswers[i];

                }
            }
            return shuffled;
        }
    }
}
