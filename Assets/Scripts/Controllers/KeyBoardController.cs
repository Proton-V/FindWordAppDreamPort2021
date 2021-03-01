using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardController : MonoBehaviour
{
    public static KeyBoardController Instance { get; private set; }

    private List<GameObject> keyLinesList = new List<GameObject>();

    private List<string> lettersEN = new List<string>{"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q"
        ,"r","s","t","u","v","w","x","y","z","-" };
    [SerializeField]
    private GameObject _keyPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void ClearKeyBoardField()
    {
        foreach(GameObject i in keyLinesList)
        {
            Destroy(i);
        }
        keyLinesList = new List<GameObject>();
    }

    public void SpawnKeyBoard()
    {
        ClearKeyBoardField();

        int lettersinLine = 8;
        int lines = Mathf.CeilToInt((float)lettersEN.Count/lettersinLine);
        var rect = GetComponent<RectTransform>();
        var widthKey = rect.rect.width/ lettersinLine;
        if(rect.rect.height < widthKey * lines)
            widthKey = rect.rect.height / lines;

        int letterNow = 0;
        for(int ln = -lines/2; ln < lines/2; ln++)
        {
            GameObject line = new GameObject($"line{ln}");
            line.transform.SetParent(transform);
            line.transform.localScale = new Vector3(1, 1, 1);
            line.transform.localPosition = Vector3.zero;
            HorizontalLayoutGroup horLine = line.AddComponent<HorizontalLayoutGroup>();
            horLine.childControlHeight = false;
            horLine.childControlWidth = false;
            keyLinesList.Add(line);

            for (int i = -lettersinLine / 2 + 1; i < lettersinLine / 2; i++)
            {
                letterNow++;
                if (letterNow > lettersEN.Count) break;

                Button newObj = Instantiate(_keyPrefab, line.transform).GetComponent<Button>();
                newObj.GetComponent<RectTransform>().sizeDelta = new Vector2(widthKey, widthKey);
                newObj.GetComponentInChildren<Text>().text = lettersEN[letterNow-1];

                newObj.onClick.AddListener(()=> {
                    GameManager gm = GameManager.Instance;

                    bool okTap = gm.TextController.IsLetterInNowWord(newObj.GetComponentInChildren<Text>().text);
                    gm.CheckAttempt(okTap);
                    Destroy(newObj.gameObject);
                });
            }
        }
        Debug.LogError(letterNow);

    }
}
