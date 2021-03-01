using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

public static class StringExt
{
    public static string ReplaceSymbols(this string str)
    {
        Regex rgx = new Regex("[^a-zA-Z0-9 -]");
        string newStr = rgx.Replace(str, "");
        return newStr;
    }

    public static List<int> AllIndexesOf(this string str, string value)
    {
        if (System.String.IsNullOrEmpty(value))
            throw new System.ArgumentException("the string to find may not be empty", "value");
        List<int> indexes = new List<int>();
        for (int index = 0; ; index += value.Length)
        {
            index = str.IndexOf(value, index);
            if (index == -1)
                return indexes;
            indexes.Add(index);
        }
    }
}

public class TextController : MonoBehaviour
{
    public const char blackSquare = '\u25A0';

    [SerializeField]
    [Range(0,10)]
    private int _minNumOfLetter = 3;
    [SerializeField]
    [Range(20, 35)]
    private int _minCountStr = 30;

    private List<string> _lines = new List<string>();

    private string nowWord = null;
    private List<string> _useWords = new List<string>();

    [HideInInspector]
    public string BlackSquareString = null;
    public bool IsLetterInNowWord(string letter)
    {
        if (nowWord == null) return false;
        if (nowWord.Contains(letter)) {
            var aStringBuilder = new StringBuilder(BlackSquareString);
            foreach(int index in nowWord.AllIndexesOf(letter))
            {
                aStringBuilder.Remove(index, 1);
                aStringBuilder.Insert(index, letter.ToCharArray()[0]);
            }
            string newString = aStringBuilder.ToString();
            GameManager.Instance.TextFieldStr.text = GameManager.Instance.TextFieldStr.text.Replace(BlackSquareString, newString);
            BlackSquareString = newString;
            if (!BlackSquareString.Contains(blackSquare))
            {
                //NextStage
                GameManager.Instance.NextStage();
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LoadText()
    {
        var textAsset = Resources.Load("TxtFiles/alice30", typeof(TextAsset)) as TextAsset;
        _lines = textAsset.text.Split('\n').ToList<string>();
        StartGame();
    }

    public void StartGame()
    {
        try
        {
            var find = FindWord();
            SetText(find.str, find.word);
            KeyBoardController.Instance.SpawnKeyBoard();
        }
        catch
        {
            //GameOver
            Debug.LogError("FAIL FIND WORD");
            GameManager.Instance.GameOver(GameManager.GO.FailFindWord);
        }

    }
    


    private (string str, string word) FindWord()
    {
        List<string> foundStrings = _lines
            .Where((str) => str.Count()>= _minCountStr && str.Split(' ').Where((word) =>
            word.Count() >= _minNumOfLetter && !_useWords.Contains(word.ToLower().ReplaceSymbols())) != null).ToList();
        string chooseString = foundStrings[Random.Range(0, foundStrings.Count())];
        //remove this line in List _lines
        _lines.Remove(chooseString);

        List<string> foundWords = chooseString.Split(' ').Where((word) =>
            word.Count() >= _minNumOfLetter).ToList();
        
        //replace all symbols in word
        string chooseWord = foundWords[Random.Range(0, foundWords.Count())].ReplaceSymbols();

        chooseString = chooseString.Replace(chooseWord, chooseWord.ToLower());
        chooseWord = chooseWord.ToLower();
        //write this word in List _useWords
        _useWords.Add(chooseWord);
        //set this word in nowWord;
        nowWord = chooseWord;

        Debug.LogError(chooseWord);

        return (chooseString, chooseWord);

    }

    private void SetText(string chooseStr, string chooseWord)
    {
        BlackSquareString = string.Concat(Enumerable.Repeat(blackSquare, chooseWord.Count()));
        GameManager.Instance.TextFieldStr.text = $"...{chooseStr.Replace(chooseWord, BlackSquareString)}...";
    }
}
