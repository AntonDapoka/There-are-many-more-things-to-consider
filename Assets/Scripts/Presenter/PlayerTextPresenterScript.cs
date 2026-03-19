using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextPresenterScript : MonoBehaviour
{
    [SerializeField] private PlayerTextViewScript playerTextView;

    public void UpdateMatched(char currentInput)
    {
        playerTextView.UpdateMatched(currentInput);
    }

    public void OnWrongInput(char wrongInput)
    {
        playerTextView.OnWrongInput(wrongInput);
    }

    public void SetNewTargetPhrase(string phrase)
    {
        playerTextView.DisplayTargetText(phrase);
    }

    public void OnPhraseCompleted()
    {
        playerTextView.OnPhraseCompleted();
    }

    public void ClearText()
    {
        playerTextView.ClearText();
    }
}
