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

    public Color32 white = new(255, 255, 255, 255);
    public Color32 red = new(255, 0, 0, 255);
    public Color32 orange = new(255, 165, 0, 255);

    protected TMP_TextInfo textInfo;

    string inputCache = "";
    private List<string> candidates = new();
    [SerializeField] private float candidateScrollDelay = 0.5f;

    private Coroutine candidateRoutine;

    public int matchIndex = 0;

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

        for (int i = 0; i < textInfo.characterCount; i++)
            SetCharColor(i, white);

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            char c = text.text[i];
            if (i <= matchIndex)
            {
                SetCharColor(matchIndex, red);
            }
            else
            {
                SetCharColor(matchIndex, red);
            }
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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

    private void SetCharColor(int index, Color32 color)
    {
        var charInfo = textInfo.characterInfo[index];
        if (!charInfo.isVisible)
            return;

        int vertexIndex = charInfo.vertexIndex;
        int materialIndex = charInfo.materialReferenceIndex;

        var colors = textInfo.meshInfo[materialIndex].colors32;

        colors[vertexIndex + 0] = color;
        colors[vertexIndex + 1] = color;
        colors[vertexIndex + 2] = color;
        colors[vertexIndex + 3] = color;
    }
}