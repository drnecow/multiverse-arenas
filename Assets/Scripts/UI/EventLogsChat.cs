using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventLogsChat : MonoBehaviour
{
    [SerializeField] private GameObject _roundSeparatorPrefab;
    [SerializeField] private GameObject _turnStartLogPrefab;
    [SerializeField] private GameObject _eventLogPrefab;

    [SerializeField] private Color _friendlyColor;
    [SerializeField] private Color _enemyColor;

    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private Scrollbar _scrollbar;


    public void LogRoundSeparator(int numberOfRound)
    {
        GameObject roundSeparator = Instantiate(_roundSeparatorPrefab);
        TextMeshProUGUI text = roundSeparator.GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"Round {numberOfRound}";

        roundSeparator.transform.SetParent(_contentParent);
        ScrollToBottom();
    }
    public void LogMonsterNewTurn(Monster monster)
    {
        GameObject newTurnLog = Instantiate(_turnStartLogPrefab);
        TextMeshProUGUI text = newTurnLog.GetComponentInChildren<TextMeshProUGUI>();
        text.text = $"{monster.Name}'s turn.";

        Image backgroundImage = newTurnLog.GetComponent<Image>();
        backgroundImage.color = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;

        newTurnLog.transform.SetParent(_contentParent);
        ScrollToBottom();
    }
    public void LogEvent(Monster monster, string eventText)
    {
        GameObject eventLog = Instantiate(_eventLogPrefab);
        TextMeshProUGUI text = eventLog.GetComponentInChildren<TextMeshProUGUI>();
        text.text = eventText;

        Image backgroundImage = eventLog.GetComponent<Image>();
        backgroundImage.color = monster.IsPlayerControlled ? _friendlyColor : _enemyColor;

        eventLog.transform.SetParent(_contentParent);
        ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        _scrollbar.value = 0;
    }
}
