using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public abstract class PlayerTextInteractorScript : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] protected PlayerTextPresenterScript playerTextPresenter;

    [Header("Current Text Data:")]
    [SerializeField] protected string currentTargetPhrase = "";
    [SerializeField] protected string currentInput = "";
    public int matchIndex = 0; 
    [SerializeField] protected char lastChar;

    public virtual void SetNewTargetPhrase(string newPhrase)
    {
        currentTargetPhrase = NormalizePhrase(newPhrase);

        matchIndex = 0;
        currentInput = "";
        lastChar = '\0';

        playerTextPresenter.SetNewTargetPhrase(newPhrase);
    }

    protected string NormalizePhrase(string input)
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

    public abstract void ProcessSymbol(char newLetter);

    protected virtual void OnWrongInput(char wrongChar)
    {
        Debug.Log("Wrong input: " + wrongChar);

        playerTextPresenter.OnWrongInput(wrongChar);
    }

    protected virtual void OnPhraseCompleted()
    {
        Debug.Log("Phrase completed!");

        playerTextPresenter.OnPhraseCompleted();
    }
}
