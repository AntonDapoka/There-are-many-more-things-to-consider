using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OrangeManTextViewScript : PlayerTextViewScript
{
    
    [SerializeField] private int matchIndexx = 0;

    [SerializeField] private float outlineDepth = 1.5f;


    private Color32 white = new(255, 255, 255, 255);
    private Color32 red = new(255, 0, 0, 255);
    private Color32 orange = new(255, 165, 0, 255);

    public override void DisplayTargetText(string targetText)
    {
        text.text = targetText;
        outlineText.text = targetText;

        matchIndexx = 0;

        outlineText.fontMaterial.EnableKeyword("OUTLINE_ON");
        outlineText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineDepth);
        outlineText.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black);

        TMP_Text[] texts = new TMP_Text[] { text, outlineText };

        textInfo = text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)

            SetCharColor(i, white);


        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public override void UpdateMatched(char currentInput)
    {
        currentInput = char.ToUpper(currentInput);

        while (matchIndexx < textInfo.characterCount)
        {
            char c = text.text[matchIndexx];

            if (!char.IsLetter(c))
            {
                SetCharColor(matchIndexx, red);
                matchIndexx++;
                continue;
            }

            if (char.ToUpper(c) == currentInput)
            {
                SetCharColor(matchIndexx, red);
                matchIndexx++;
            }

            break;
        }

        text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    public void OnWrongWrongInput()
    {
        Debug.Log("Hello");

        if (matchIndexx <= 0)
            return;

        matchIndexx--;

        while (matchIndexx >= 0)
        {
            char c = text.text[matchIndexx];

            SetCharColor(matchIndexx, white);

            if (char.IsLetter(c))
            {
                break;
            }

            matchIndexx--;
        }

        if (matchIndexx < 0)
            matchIndexx = 0;

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