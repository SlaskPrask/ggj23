using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Event")]
    public class SpecialEvent : DialogueBase
    {
        public enum GameEvent
        {
            GAME_OVER = 1,
            LOAD_SCENE = 2
        }

        [SerializeField]
        private GameEvent doEvent;

        public SpecialEvent()
        {
            dialogueType = DialogueType.EVENT;
        }

        public void SetEvent(GameEvent e)
        {
            doEvent = e;
        }

        public override void Invoke(DialogueReader reader)
        {
            reader.DoEvent(doEvent);
        }
    }
}
