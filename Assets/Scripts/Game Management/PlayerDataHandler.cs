using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataHandler : MonoBehaviour
{
    [SerializeField] private Slider _allySlider;
    [SerializeField] private Slider _enemySlider;
    [SerializeField] private Button _startButton;
    [SerializeField] private SceneManager _sceneManager;
    private PlayerData _playerData;


    private void Awake()
    {
        _playerData = GetComponent<PlayerData>();

        _allySlider.onValueChanged.AddListener((numberOfRats) => SetAllyRats((int)numberOfRats));
        _enemySlider.onValueChanged.AddListener((numberOfRats) => SetEnemyRats((int)numberOfRats));
        _startButton.onClick.AddListener(() => _sceneManager.LoadScene(Scene.Combat));
    }
    public void SetAllyRats(int numberOfRats)
    {
        _playerData.AllyRats = numberOfRats;
        Debug.Log(_playerData.AllyRats);
    }
    public void SetEnemyRats(int numberOfRats)
    {
        _playerData.EnemyRats = numberOfRats;
        Debug.Log(_playerData.EnemyRats);
    }
}
