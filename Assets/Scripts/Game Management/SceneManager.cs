using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scene
{
    MainMenu = 0,
    Combat = 1
}

public class SceneManager : MonoBehaviour
{
    public void LoadScene(Scene scene)
    {
        if (scene == Scene.Combat)
            PlayerData.Instance.PreserveThis();
        else if (scene == Scene.MainMenu)
            Destroy(PlayerData.Instance);

        UnityEngine.SceneManagement.SceneManager.LoadScene((int)scene);
    }
}
