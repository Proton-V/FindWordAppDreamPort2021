using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Canvas _canvasGame, _canvasMainMenu;
    [SerializeField]
    private WindowsManager _windowsManager;

    public TextController TextController;
    [SerializeField]
    private Text _textFieldStr;

    private int _defAttmpts = 3;

    [HideInInspector]
    public int Score = 0, Attmpts = 0;
    [SerializeField]
    private Text _textScore, _textAttmpts;

    public Text TextFieldStr
    {
        get
        {
            return _textFieldStr;
        }
    }

    private void Awake()
    {
        Instance = this;

        _textScore.text = Score.ToString();
        Attmpts = _defAttmpts;
        _textAttmpts.text = Attmpts.ToString();
    }

    public void PlayGame()
    {
        _canvasGame.enabled = true;
        _canvasMainMenu.enabled = false;
        TextController.LoadText();
    }

    public void CheckAttempt(bool goodAttempt)
    {
        switch(goodAttempt){
            case true:
                break;
            case false:
                Attmpts--;
                if (Attmpts <= 0)
                {
                    GameOver(GO.EndedAttempts);
                    break;
                }
                _textAttmpts.text = Attmpts.ToString();
                break;
        }
    }
    public void NextStage()
    {
        Score += Attmpts;
        _textScore.text = Score.ToString();
        Attmpts = _defAttmpts;
        _textAttmpts.text = Attmpts.ToString();

        TextController.StartGame();
    }
    public async void GameOver(GO act)
    {
        _canvasGame.enabled = false;

        switch (act)
        {
            case GO.FailFindWord:
                _windowsManager.FailFindWordWindow.SetActive(true);
                break;
            case GO.EndedAttempts:
                _windowsManager.EndedAttemptsWindow.SetActive(true);
                break;
        }
        await Task.Delay(3000);
        try
        {
            SceneManager.LoadScene(0);
        }
        catch
        {
            Debug.LogError("Play mode now OFF");
        }
    }

    public enum GO
    {
        FailFindWord,
        EndedAttempts
    }
}
