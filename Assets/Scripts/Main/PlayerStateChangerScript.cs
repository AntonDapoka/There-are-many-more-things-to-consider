using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStateChangerScript : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject player;
    private Transform playerTransform;

    [Header("Environments")]
    [SerializeField] private GameObject environmentOffice;
    [SerializeField] private GameObject environmentBattlefield;

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
   }

   public void SetOrangeManState()
    {
        environmentBattlefield.SetActive(false);
        environmentOffice.SetActive(true);

        soldierBlinky.SetActive(false);
        soldierPinky.SetActive(false);
        soldierInky.SetActive(false);
        soldierClaude.SetActive(false);

        playerTransform.position = Vector3.zero;

    }

    public void SetSoldierState()
    {
        environmentOffice.SetActive(false);
        environmentBattlefield.SetActive(true);

        soldierBlinky.SetActive(true);
        //soldierBlinky.transform.position = 
        soldierPinky.SetActive(true);
        //soldierPinky.transform.position = 
        soldierInky.SetActive(true);
        //soldierInky.transform.position = 
        soldierClaude.SetActive(true);
        //soldierClaude.transform.position = 

        playerTransform.position = Vector3.zero;
    }
}
