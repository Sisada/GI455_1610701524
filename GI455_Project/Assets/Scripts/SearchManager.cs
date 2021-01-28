using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : MonoBehaviour
{
    [SerializeField] private string[] wordList = null;
    [SerializeField] private Text wordListShow = null;
    [SerializeField] private Text inputTextType = null;
    [SerializeField] private Text outputText = null;

    private void Start()
    {
        foreach (string wordShow in wordList)
        {
            wordListShow.text += wordShow + "\n";
        }    
    }

    public void CheckText()
    {
        for (int i = 0; i < wordList.Length; i++)
        {
            if (inputTextType.text == wordList[i])
            {
                outputText.text = "[<color=green>" + inputTextType.text + "</color>] is Found.";
                break;
            }
            else 
            {
                outputText.text = "[<color=red>" + inputTextType.text + "</color>] Not Found.";
            }
        }
    }
}
