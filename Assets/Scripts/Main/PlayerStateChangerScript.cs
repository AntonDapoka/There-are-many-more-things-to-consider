using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStateChangerScript : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerMovementInteractorScript playerMovementInteractor;
    private Transform playerTransform;

    [SerializeField] private OrangeManTextInteractorScript orangeManTextInteractor;
    [SerializeField] private TargetTextManagerScript orangeManTextManager;
    [SerializeField] private SoldierTextInteractorScript soldieranTextInteractor;
    [SerializeField] private PlayerTextControllerScript playerTextController;

    [SerializeField] private PlayerTextPresenterScript soldierTextPresenter;
    [SerializeField] private PlayerTextPresenterScript orangeManTextPresenter;
    [SerializeField] private OrangeManTextViewScript orangeManTextView;
    [SerializeField] private PlayerTextViewScript soldierTextView;


    [Header("Environments")]
    [SerializeField] private GameObject environmentOffice;
    [SerializeField] private GameObject environmentBattlefield;

    [Header("UIs")]
    [SerializeField] private Canvas uiOffice;
    [SerializeField] private Canvas uiBattlefield;

    [Header("Prefabs Soldiers")]
    [SerializeField] private GameObject soldierBlinky;
    [SerializeField] private GameObject soldierPinky;
    [SerializeField] private GameObject soldierInky;
    [SerializeField] private GameObject soldierClaude;

    [Header("Prefabs Enemies")]
    [SerializeField] private GameObject enemySuicider;
    [SerializeField] private GameObject enemyShooter;

    [Header("UI")]
    [SerializeField] private Image image;

   private void Awake()
   {
        playerTransform = player.GetComponent<Transform>();
        SetOrangeManState();
   }

   public void SetOrangeManState()
    {
        player.SetActive(false);
        environmentBattlefield.SetActive(false);
        environmentOffice.SetActive(true);

        playerTextController.isTrump = true;
/*
        soldierBlinky.SetActive(false);
        soldierPinky.SetActive(false);
        soldierInky.SetActive(false);
        soldierClaude.SetActive(false);*/

        uiOffice.gameObject.SetActive(true);
        uiBattlefield.gameObject.SetActive(false);

        orangeManTextInteractor.gameObject.SetActive(true);
        orangeManTextManager.gameObject.SetActive(true);
        soldieranTextInteractor.gameObject.SetActive(false);
        soldierTextPresenter.gameObject.SetActive(false);
        orangeManTextPresenter.gameObject.SetActive(true);
        orangeManTextView.gameObject.SetActive(true);
        playerMovementInteractor.gameObject.SetActive(false);
        soldierTextView.gameObject.SetActive(false);

        //playerTransform.position = Vector3.zero;

    }

    public void SetSoldierState()
    {
        player.SetActive(true);

        environmentOffice.SetActive(false);
        environmentBattlefield.SetActive(true);

        playerTextController.isTrump = false;

        uiOffice.gameObject.SetActive(false);
        uiBattlefield.gameObject.SetActive(true);

        orangeManTextInteractor.gameObject.SetActive(false);
        //orangeManTextManager.gameObject.SetActive(false);
        soldieranTextInteractor.gameObject.SetActive(true);
        soldierTextPresenter.gameObject.SetActive(true);
        orangeManTextPresenter.gameObject.SetActive(false);
        orangeManTextView.gameObject.SetActive(false);
        playerMovementInteractor.gameObject.SetActive(true);
        soldierTextView.gameObject.SetActive(true);
        /*
        soldierBlinky.SetActive(true);
        //soldierBlinky.transform.position = 
        soldierPinky.SetActive(true);
        //soldierPinky.transform.position = 
        soldierInky.SetActive(true);
        //soldierInky.transform.position = 
        soldierClaude.SetActive(true);
        //soldierClaude.transform.position = */

        playerTransform.position = Vector3.zero;
    }
}
