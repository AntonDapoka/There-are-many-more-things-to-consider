using System.Collections.Generic;
using UnityEngine;

public class SoldierTextInteractorScript : PlayerTextInteractorScript
{
    [Header("Phrases Pool:")]
    [SerializeField] protected TextStorage textStorage;

    [Header("Soldiers:")]
    [SerializeField] private SoldierScript[] soldiers;

    protected List<string> normalizedPhrases = new();
    protected List<int> candidateIndices = new();

    private void Awake()
    {
        foreach (var phrase in textStorage.phrases)
        {
            normalizedPhrases.Add(NormalizePhrase(phrase));
        }
    }

    public override void ProcessSymbol(char newLetter)
    {
        newLetter = char.ToUpper(newLetter);

        string testInput = currentInput + newLetter;

        List<int> newCandidates = GetMatchingCandidates(testInput);

        if (newCandidates.Count > 0)
        {
            currentInput = testInput;
            candidateIndices = newCandidates;

            playerTextPresenter.UpdateMatched(newLetter);
            SendCandidatesToPresenter();

            if (candidateIndices.Count == 1)
            {
                string phrase = normalizedPhrases[candidateIndices[0]];
                if (currentInput.Length >= phrase.Length)
                    OnPhraseCompleted();
            }

            return;
        }
        OnWrongInput(newLetter);

        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);

            candidateIndices = GetMatchingCandidates(currentInput);

            SendCandidatesToPresenter();
            playerTextPresenter.RebuildMatched(currentInput);
        }
        else
        {
            SendCandidatesToPresenter();
        }
    }

    protected List<int> GetMatchingCandidates(string input)
    {
        if (string.IsNullOrEmpty(input))
            return new List<int>();

        List<int> result = new();

        for (int i = 0; i < normalizedPhrases.Count; i++)
        {
            string phrase = normalizedPhrases[i];

            if (phrase.Length < input.Length)
                continue;

            bool match = true;

            for (int j = 0; j < input.Length; j++)
            {
                if (phrase[j] != input[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                result.Add(i);
        }

        return result;
    }

    protected void SendCandidatesToPresenter()
    {
        List<string> result = new();

        foreach (var i in candidateIndices)
        {
            result.Add(textStorage.phrases[i]);
        }

        playerTextPresenter.SetCandidatePhrases(result);
    }

    protected void ResetPhrase()
    {
        currentInput = "";
        candidateIndices.Clear();

        playerTextPresenter.ClearText();
    }

    protected override void OnPhraseCompleted()
    {
        Debug.Log("Phrase completed!");
        foreach (SoldierScript soldier in soldiers)
        {
            soldier.SetCommand(currentInput);
        }

        playerTextPresenter.OnPhraseCompleted();
        ResetPhrase();
    }

   protected override void OnWrongInput(char wrongChar)
   {
        base.OnWrongInput(wrongChar);

        if (currentInput == "")
        {
            ResetPhrase();
        }
   }
}