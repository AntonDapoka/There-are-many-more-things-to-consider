using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerTextInteractorScript : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private PlayerTextPresenterScript playerTextPresenter;
    [SerializeField] private TargetTextManagerScript targetTextManager;

    [Header("Current Text Data:")]
    [SerializeField] private string currentTargetPhrase;
    [SerializeField] private string currentInput = "";
    [SerializeField] private int matchIndex = 0; 
    [SerializeField] private char lastChar;

    private void Start()
    {
        SetNewTargetPhrase("I WANT WAR"); //remove
    }

    public void SetNewTargetPhrase(string newPhrase)
    {
        currentTargetPhrase = NormalizePhrase(newPhrase);

        matchIndex = 0;
        currentInput = "";
        lastChar = '\0';

        playerTextPresenter.SetNewTargetPhrase(newPhrase);
    }

    private string NormalizePhrase(string input)
    {
        StringBuilder sb = new();

        foreach (char c in input)
        {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
            {
                sb.Append(char.ToUpper(c));
            }
        }

        return sb.ToString();
    }

    public void ProcessSymbol(char newLetter)
    {
        lastChar = newLetter;
        Debug.Log(lastChar.ToString());

        if (matchIndex >= currentTargetPhrase.Length)
            return;

        if (newLetter == currentTargetPhrase[matchIndex])
        {
            matchIndex++;
            currentInput += newLetter;

            playerTextPresenter.UpdateMatched(newLetter);

            if (matchIndex >= currentTargetPhrase.Length)
            {
                OnPhraseCompleted();
            }
        }
        else
        {
            if (matchIndex > 0) matchIndex--;

            OnWrongInput(newLetter);
        }
    }

    private void OnWrongInput(char wrongChar)
    {
        Debug.Log("Wrong input: " + wrongChar);

        playerTextPresenter.OnWrongInput(wrongChar);
    }

    private void OnPhraseCompleted()
    {
        Debug.Log("Phrase completed!");

        playerTextPresenter.OnPhraseCompleted();
    }
}
