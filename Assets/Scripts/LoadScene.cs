using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public GameObject panelToAct;
    public GameObject panelToDeact;

    public string sceneName;

    public void LoadPanel()
    {
        panelToAct.SetActive(true);
        panelToDeact.SetActive(false);
    }

    public void LoadSc()
    {
        SceneManager.LoadScene(sceneName);
    }
}
