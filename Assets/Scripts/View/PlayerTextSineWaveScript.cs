using UnityEngine;
using TMPro;

public class PlayerTextSineWaveScript
{
    private TMP_Text[] texts;
    private float amplitude;
    private float frequency;
    private float speed;

    private TMP_TextInfo[] textInfos;
    private Vector3[][][] originalVertices; // [textIndex][materialIndex][vertex]

    public PlayerTextSineWaveScript(TMP_Text[] texts, float amplitude, float frequency, float speed)
    {
        this.texts = texts;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.speed = speed;

        textInfos = new TMP_TextInfo[texts.Length];
        originalVertices = new Vector3[texts.Length][][];
    }

    public void CacheMesh()
    {
        for (int t = 0; t < texts.Length; t++)
        {
            var text = texts[t];
            text.ForceMeshUpdate();
            textInfos[t] = text.textInfo;

            originalVertices[t] = new Vector3[textInfos[t].meshInfo.Length][];
            for (int m = 0; m < textInfos[t].meshInfo.Length; m++)
            {
                var verts = textInfos[t].meshInfo[m].vertices;
                originalVertices[t][m] = new Vector3[verts.Length];
                System.Array.Copy(verts, originalVertices[t][m], verts.Length);
            }
        }
    }

    public void ApplyWave()
    {
        float time = Time.time * speed;

        for (int t = 0; t < texts.Length; t++)
        {
            var textInfo = textInfos[t];
            var text = texts[t];

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                float wave = Mathf.Sin(time + i * frequency) * amplitude;
                Vector3 offset = new Vector3(0, wave, 0);

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = originalVertices[t][materialIndex][vertexIndex + j] + offset;
                }
            }

            for (int m = 0; m < textInfo.meshInfo.Length; m++)
            {
                textInfo.meshInfo[m].mesh.vertices = textInfo.meshInfo[m].vertices;
                text.UpdateGeometry(textInfo.meshInfo[m].mesh, m);
            }
        }
    }
}