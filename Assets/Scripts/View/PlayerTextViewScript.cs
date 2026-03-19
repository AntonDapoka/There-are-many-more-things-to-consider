using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerTextViewScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI textOutline;
    [SerializeField] private int matchIndex = 0;

    private TMP_TextInfo textInfo;

    [SerializeField] private float outlineDepth = 1.5f;

    private PlayerTextSineWaveScript waveEffect;

    [SerializeField] private float waveAmplitude = 10f;
    [SerializeField] private float waveFrequency = 2f;
    [SerializeField] private float waveSpeed = 2f;
    
    private Color32 white = new(255, 255, 255, 255);
    private Color32 red = new(255, 0, 0, 255);
    private Color32 orange = new(255, 165, 0, 255);

    public void DisplayTargetText(string targetText)
    {
        text.text = targetText;
        textOutline.text = targetText;

        matchIndex = 0;

        textOutline.fontMaterial.EnableKeyword("OUTLINE_ON");
        textOutline.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineDepth);
        textOutline.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);

        TMP_Text[] texts = new TMP_Text[] { text, textOutline };
        waveEffect = new PlayerTextSineWaveScript(texts, waveAmplitude, waveFrequency, waveSpeed);
        waveEffect.CacheMesh();

        textInfo = text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
            SetCharColor(i, white);

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void Update()
    {
        waveEffect?.ApplyWave();
    }

    public void UpdateMatched(char currentInput)
    {
        currentInput = char.ToUpper(currentInput);

        while (matchIndex < textInfo.characterCount)
        {
            char c = text.text[matchIndex];

            if (!char.IsLetter(c))
            {
                SetCharColor(matchIndex, red);
                matchIndex++;
                continue;
            }

            if (char.ToUpper(c) == currentInput)
            {
                SetCharColor(matchIndex, red);
                matchIndex++;
            }

            break;
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void ClearText()
    {
        DisplayTargetText("");
    }

    public void OnWrongInput(char wrongInput)
    {
        if (matchIndex <= 0)
            return;

        matchIndex--;

        while (matchIndex >= 0)
        {
            char c = text.text[matchIndex];

            SetCharColor(matchIndex, white);

            if (char.IsLetter(c))
                break;

            matchIndex--;
        }

        if (matchIndex < 0)
            matchIndex = 0;

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void OnPhraseCompleted()
    {
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            SetCharColor(i, orange);
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
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