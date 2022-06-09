using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextContoller : MonoBehaviour
{
    [SerializeField]
    private GameManager _GameManager;

    private TMP_Text text;
    private void Start()
    {
        text = GetComponent<TMP_Text>();
        _GameManager.OnGameOver += OnGameOver;
    }

    private void OnGameOver(string message, bool isWon)
    {
        text.text = message;
        if (isWon)
        {
            text.color = Color.green;
        }
        else
        {
            text.color = Color.red;
        }
    }
}
