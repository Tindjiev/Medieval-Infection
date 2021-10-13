using UnityEngine;
using InputNM;
using Zenject;

public class Pause : MonoBehaviour
{
    private GameObject _menuPanel;
    private GameObject _tipsPanel;
    private Player _player;


    [Inject]
    public void Construct(MapSetup menuPanel, Player player)
    {
        _player = player;
        _menuPanel = menuPanel.transform.parent.Find("Escape Menu Panel").gameObject;
        _tipsPanel = menuPanel.transform.parent.Find("Tips Panel").gameObject;
    }

    private Inputstruct _pauseKey = new Inputstruct(Input.GetKeyDown, KeyCode.Escape);

    private bool _paused;



    private void OnEnable()
    {
        UnPauseGame();
    }

    private void OnDisable()
    {
        freeCursor();
    }


    void Update()
    {
        if (DayNightCycle.ChangingDay)
        {
            return;
        }
        if (_pauseKey.CheckInput())
        {
            if (_tipsPanel.activeSelf)
            {
                HideTips();
            }
            else if (!_paused)
            {
                PauseGame();
            }
            else
            {
                UnPauseGame();
            }
        }
    }

    public void PauseGame()
    {
        _paused = true;
        freeCursor();
        _player.Pause();
        _menuPanel.SetActive(true);
    }

    public void UnPauseGame()
    {
        _paused = false;
        lockCursor();
        _player.UnPause();
        _menuPanel.SetActive(false);
    }

    private void lockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void freeCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowTips()
    {
        _tipsPanel.SetActive(true);
    }

    public void HideTips()
    {
        _tipsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        MySceneManager.ExitGame();
    }

}
