using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeManTextInteractorScript : PlayerTextInteractorScript
{

    [SerializeField] protected TargetTextManagerScript targetTextManager;

   public override void ProcessSymbol(char newLetter)
   {
        lastChar = newLetter;

        if (currentTargetPhrase == "")
        {
            return;
        }

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

    protected override void OnPhraseCompleted()
    {
        Debug.Log("Phrase completed!");

        playerTextPresenter.OnPhraseCompleted();
        targetTextManager.OnThePhraseEnd();

    }
}
