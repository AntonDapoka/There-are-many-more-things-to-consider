using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTextManagerScript : MonoBehaviour
{
    [SerializeField] private PlayerTextInteractorScript playerTextInteractor;
    [SerializeField] private TextStorage[] textStorages;

    private void Start()
    {
        int x = UnityEngine.Random.Range(0, textStorages.Length); //Replace
    }
}
