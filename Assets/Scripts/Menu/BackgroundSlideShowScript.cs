using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundSlideShowScript : MonoBehaviour
{
    [SerializeField] private Sprite[] backgroundSprites;
    [SerializeField] private Image bakgroundImage; 
    [SerializeField] private float interval = 5f;

    private int lastIndex = -1;
    private int secondLastIndex = -1;

    private void Start()
    {
        if (bakgroundImage == null || backgroundSprites.Length == 0)
            return;
        
        StartCoroutine(ChangeImageRoutine());
    }

    private IEnumerator ChangeImageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            int newIndex;
            do
            {
                newIndex = Random.Range(0, backgroundSprites.Length);
            }
            while (backgroundSprites.Length > 2 &&  (newIndex == lastIndex || newIndex == secondLastIndex));

            secondLastIndex = lastIndex;
            lastIndex = newIndex;
            bakgroundImage.sprite = backgroundSprites[newIndex];
        }
    }
}