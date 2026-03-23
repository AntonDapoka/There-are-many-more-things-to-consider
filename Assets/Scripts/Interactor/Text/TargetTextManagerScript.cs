using System.Collections;
using UnityEngine;

public class TargetTextManagerScript : MonoBehaviour
{
    [SerializeField] private PlayerTextInteractorScript playerTextInteractor;
    [SerializeField] private PlayerStateChangerScript playerStateChanger;
    [SerializeField] private TextStorage[] textStorages;
    [SerializeField] private FadeInAndOutScript fadeInAndOut;
    private int currentTextStorageIndex;
    private int currentPhraseIndex;


    public void SetStart()
    {
        currentTextStorageIndex = UnityEngine.Random.Range(0, textStorages.Length); //Replace
        currentPhraseIndex = 0;
        SetNewTargetStorage();//Replace
    }

    private void SetNewTargetStorage()
    {
        if (currentPhraseIndex < textStorages[currentTextStorageIndex].phrases.Length)
            playerTextInteractor.SetNewTargetPhrase(textStorages[currentTextStorageIndex].phrases[currentPhraseIndex]);
    }

    public void OnThePhraseEnd()
    {
        currentPhraseIndex++;

        if (currentPhraseIndex < textStorages[currentTextStorageIndex].phrases.Length)
        {
            SetNewTargetStorage();
        }
        else
        {
            playerTextInteractor.SetNewTargetPhrase("");
            StartCoroutine(FadeSequence());
        }
    }

    private IEnumerator FadeSequence()
    {
        yield return StartCoroutine(fadeInAndOut.PlayFadeOut());

        playerStateChanger.SetSoldierState();
        yield return StartCoroutine(fadeInAndOut.PlayFadeIn());
    }
}
