using UnityEngine;

public class OrangeManTextInteractorScript : PlayerTextInteractorScript
{
    [Header("Target Text Manager")]
    [SerializeField] protected TargetTextManagerScript targetTextManager;

    public override void ProcessSymbol(char newLetter)
    {
        newLetter = char.ToUpper(newLetter);

        if (string.IsNullOrEmpty(currentTargetPhrase))
            return;

        if (matchIndex >= currentTargetPhrase.Length)
            return;

        if (newLetter == currentTargetPhrase[matchIndex])
        {
            matchIndex++;
            currentInput += newLetter;
            lastChar = newLetter;

            playerTextPresenter.UpdateMatched(newLetter);

            if (matchIndex >= currentTargetPhrase.Length)
                OnPhraseCompleted();
        }
        else
        {
            OnWrongInput(newLetter);

            if (matchIndex > 0 && currentInput.Length > 0)
            {
                matchIndex--;
                currentInput = currentInput.Substring(0, currentInput.Length - 1);
                playerTextPresenter.RebuildMatched(currentInput);
            }
        }
    }

    protected override void OnPhraseCompleted()
    {
        playerTextPresenter.OnPhraseCompleted();
        targetTextManager.OnThePhraseEnd();
    }
}
