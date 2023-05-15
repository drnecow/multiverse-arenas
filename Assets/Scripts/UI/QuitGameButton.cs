using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    private Button _quitButton;


    private void Awake()
    {
        _quitButton = GetComponent<Button>();
        _quitButton.onClick.AddListener(() => QuitGame());
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
