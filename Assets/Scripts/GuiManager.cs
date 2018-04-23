using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField]
    private Game _game;
    [SerializeField]
    private Text _timerLabel;
    [SerializeField]
    private GameObject _gameOverPanel;

    private void Awake()
    {
        _gameOverPanel.SetActive(false);
        _game.GameOvered += _game_GameOvered;
    }

    private void _game_GameOvered()
    {
        _gameOverPanel.SetActive(true);
    }

    private void Update()
    {
        _timerLabel.text = Mathf.Floor(_game.GameTime).ToString();
    }
}

