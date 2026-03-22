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
        playerTextView.OnWrongInput();
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

    public void SetCandidatePhrases(List<string> phrases)
    {
        playerTextView.SetCandidatePhrases(phrases);
    }

    public void RebuildMatched(string input)
    {
        playerTextView.RebuildMatched(input);
    }
}
