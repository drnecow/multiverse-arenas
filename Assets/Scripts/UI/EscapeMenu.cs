using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private SceneManager _sceneManager;
    [SerializeField] private GameObject _escapeMenuWindow;
    [SerializeField] private GameObject _tutorialWindow;

    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _tutorialButton;


    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(() => _sceneManager.LoadScene(Scene.MainMenu));
        _tutorialButton.onClick.AddListener(() => _tutorialWindow.SetActive(true));
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!_escapeMenuWindow.activeSelf)
                _escapeMenuWindow.SetActive(true);
            else
                _escapeMenuWindow.SetActive(false);
        }
    }
}
