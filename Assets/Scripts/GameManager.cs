using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event Action<string, bool> OnGameOver;
    [SerializeField]
    LayerMask _LayerMask;
    private Logger _LoggerInstance;

    private const string LOST_MESSAGE = "You have lost, LOOSER!";
    private const string WON_MESSAGE = "You did it, Rambo man!";    
    private const string LOST_MESSAGE_LOG = "You lost!";
    private const string WON_MESSAGE_LOG = "You won!";
    private void Start()
    {
        _LoggerInstance = Logger.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((_LayerMask.value & (1 << other.gameObject.layer)) > 0))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (!player.IsOnGround())
            {
                GameOver(false);
            }
            else
            {
                GameOver(true);
            }
            player.DeactivateCursor();
        }
    }
    private void GameOver(bool isWon)
    {
        if (isWon)
        {
            OnGameOver?.Invoke(WON_MESSAGE, true);
            _LoggerInstance.WriteLogMessage(WON_MESSAGE_LOG);
        }
        else
        {
            OnGameOver?.Invoke(LOST_MESSAGE, false);
            _LoggerInstance.WriteLogMessage(LOST_MESSAGE_LOG);
        }
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(3f);
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
