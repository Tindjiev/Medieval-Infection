using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private GameObject _tipsText, _loreText;

    private void Start()
    {
        ShowLore();
    }

    public void BeginGame()
    {
        MySceneManager.LoadVillageScene();
    }

    public void ShowTips()
    {
        _loreText.SetActive(false);
        _tipsText.SetActive(true);
    }

    public void ShowLore()
    {
        _tipsText.SetActive(false);
        _loreText.SetActive(true);
    }

}
