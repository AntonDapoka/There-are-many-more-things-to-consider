using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierTextInteractorScript : PlayerTextInteractorScript
{
    [Header("Phrases Pool:")]
    [SerializeField] protected TextStorage textStorage;

    protected List<string> normalizedPhrases = new();

    protected List<int> candidateIndices = new();
    protected Coroutine cyclingCoroutine;
    protected int currentCandidatePointer = 0;

    private void Awake()
    {
        foreach (var phrase in textStorage.phrases)
        {
            normalizedPhrases.Add(NormalizePhrase(phrase));
        }
    }

    protected void TrySelectPhrase(char firstLetter)
    {
        candidateIndices.Clear();

        for (int i = 0; i < normalizedPhrases.Count; i++)
        {
            if (normalizedPhrases[i].Length == 0) continue;

            if (normalizedPhrases[i][0] == firstLetter)
            {
                candidateIndices.Add(i);
            }
        }

        if (candidateIndices.Count == 0)
        {
            Debug.Log("No phrase found for letter: " + firstLetter);
            return;
        }

        currentCandidatePointer = 0;

        int firstIdx = candidateIndices[0];
        SetCurrentPhrase(textStorage.phrases[firstIdx], normalizedPhrases[firstIdx]);

        matchIndex = 1;
        currentInput = firstLetter.ToString();
        playerTextPresenter.UpdateMatched(firstLetter);

        if (candidateIndices.Count > 1)
        {
            if (cyclingCoroutine != null)
                StopCoroutine(cyclingCoroutine);

            cyclingCoroutine = StartCoroutine(CyclePhrases());
        }

        if (matchIndex >= currentTargetPhrase.Length)
        {
            OnPhraseCompleted();
        }
    }

    protected IEnumerator CyclePhrases()
    {
        while (candidateIndices.Count > 1)
        {
            currentCandidatePointer = (currentCandidatePointer + 1) % candidateIndices.Count;

            int idx = candidateIndices[currentCandidatePointer];
            playerTextPresenter.SetNewTargetPhrase(textStorage.phrases[idx]);

            yield return new WaitForSeconds(0.5f);
        }
    }

    protected void SetCurrentPhrase(string original, string normalized)
    {
        currentTargetPhrase = normalized;

        matchIndex = 0;
        currentInput = "";
        lastChar = '\0';

        playerTextPresenter.SetNewTargetPhrase(original);
    }

    protected void ResetPhrase()
    {
        currentTargetPhrase = "";
        currentInput = "";
        matchIndex = 0;

        candidateIndices.Clear();

        if (cyclingCoroutine != null)
        {
            StopCoroutine(cyclingCoroutine);
            cyclingCoroutine = null;
        }

        playerTextPresenter.ClearText();
    }

    public override void ProcessSymbol(char newLetter)
    {
        newLetter = char.ToUpper(newLetter);
        lastChar = newLetter;

        if (currentTargetPhrase == "")
        {
            TrySelectPhrase(newLetter);
            return;
        }

        if (matchIndex >= currentTargetPhrase.Length)
            return;

        if (newLetter == currentTargetPhrase[matchIndex])
        {
            matchIndex++;
            currentInput += newLetter;

            playerTextPresenter.UpdateMatched(newLetter);

            FilterCandidates();

            if (matchIndex >= currentTargetPhrase.Length)
            {
                OnPhraseCompleted();
            }
        }
        else
        {
            if (matchIndex == 0)
            {
                ResetPhrase();
                return;
            }

            matchIndex--;

            OnWrongInput(newLetter);
        }
    }

    protected void FilterCandidates()
    {
        candidateIndices.RemoveAll(i =>
        {
            string phrase = normalizedPhrases[i];

            if (phrase.Length <= matchIndex) return true;

            return phrase[matchIndex] != lastChar;
        });

        if (candidateIndices.Count == 0)
        {
            ResetPhrase();
            return;
        }

        if (candidateIndices.Count == 1)
        {
            if (cyclingCoroutine != null)
            {
                StopCoroutine(cyclingCoroutine);
                cyclingCoroutine = null;
            }

            int idx = candidateIndices[0];
            SetCurrentPhrase(textStorage.phrases[idx], normalizedPhrases[idx]);

            playerTextPresenter.UpdateMatched(currentInput.ToCharArray()[currentInput.Length]);
        }
    }

    protected override void OnPhraseCompleted()
    {
        Debug.Log("Phrase completed!");

        playerTextPresenter.OnPhraseCompleted();

        ResetPhrase();
    }
}