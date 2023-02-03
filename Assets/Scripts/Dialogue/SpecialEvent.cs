using System.Collections;
using UnityEngine;

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

    public void DoEvent()
    {
        switch (doEvent)
        {
            case GameEvent.GAME_OVER:
                GameOver();
                break;
            case GameEvent.LOAD_SCENE:
                LoadScene();
                break;
            default:
                Debug.Log("Nothing happened :/");
                break;
        }
    }

    private void LoadScene()
    {
        Debug.Log("TODO: Load Scene");
    }

    private void GameOver()
    {
        Debug.Log("TODO: Game Over");
    }
}