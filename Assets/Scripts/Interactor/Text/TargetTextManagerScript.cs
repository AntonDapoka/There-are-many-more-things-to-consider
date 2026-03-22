using UnityEngine;

public class TargetTextManagerScript : MonoBehaviour
{
    [SerializeField] private PlayerTextInteractorScript playerTextInteractor;
    [SerializeField] private TextStorage[] textStorages;
    private int currentTextStorageIndex;
    private int currentPhraseIndex;

    private void OnEnable()
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
            //
        }
    }
}
