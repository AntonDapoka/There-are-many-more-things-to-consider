using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerTextViewScript : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected TextMeshProUGUI outlineText;

    [Header("Outline")]
    [SerializeField] private float outlineWidth = 1.5f;

    protected TMP_TextInfo textInfo;

      string inputCache = "";
    private List<string> candidates = new();
    [SerializeField] private float candidateScrollDelay = 0.5f;

    private Coroutine candidateRoutine;

    protected int matchIndex = 0;

    public virtual void DisplayTargetText(string value)
    {
        inputCache = "";
        matchIndex = 0;

        text.text = value;
        outlineText.text = value;

        if (value != "")
        {
            SetupOutline();
        }

        UpdateTextInfo();
    }

    private void SetupOutline()
    {
        outlineText.fontMaterial.EnableKeyword("OUTLINE_ON");
        outlineText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
        outlineText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);
    }

    private void UpdateTextInfo()
    {
        text.ForceMeshUpdate();
        textInfo = text.textInfo;
    }

    public virtual void UpdateMatched(char c)
    {
        if (textInfo == null || textInfo.characterCount == 0)
            return;

        inputCache += char.ToUpper(c);
        ApplyInput();
        
    }

    public void OnWrongInput()
    {
        if (inputCache.Length == 0)
            return;

        inputCache = inputCache[..^1];
        ApplyInput();
    }

    private void ApplyInput()
    {
        matchIndex = 0;

        int inputIndex = 0;

        for (int i = 0; i < textInfo.characterCount && inputIndex < inputCache.Length; i++)
        {
            char c = text.text[i];

            if (!char.IsLetter(c))
                continue;

            if (char.ToUpper(c) != inputCache[inputIndex])
                break;

            matchIndex = i + 1;
            inputIndex++;
        }
    }

    public void SetCandidatePhrases(List<string> phrases)
    {
        if (phrases == null || phrases.Count == 0)
        {
            StopCandidateRoutine();
            ClearText();
            return;
        }

        candidates = phrases;

        StopCandidateRoutine();
        candidateRoutine = StartCoroutine(ScrollCandidates());
    }

    private void StopCandidateRoutine()
    {
        if (candidateRoutine != null)
        {
            StopCoroutine(candidateRoutine);
            candidateRoutine = null;
        }
    }

    public void ClearText()
    {
        StopCandidateRoutine();

        candidates.Clear();

        DisplayTargetText("");
    }

    public void RebuildMatched(string input)
    {
        inputCache = input.ToUpper();
        ApplyInput();
    }

    private IEnumerator ScrollCandidates()
    {
        while (candidates != null && candidates.Count > 0)
        {
            for (int i = 0; i < candidates.Count; i++)
            {
                string phrase = candidates[i];

                DisplayTargetText(phrase);
                ApplyInput();

                yield return new WaitForSeconds(candidateScrollDelay);
            }
        }

        candidateRoutine = null;
    }
}