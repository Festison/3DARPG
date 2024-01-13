using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Festison;

public class PlaygroundScene : MonoBehaviour
{
    void Start()
    {
        LoadingSceneManager.Instance.ChangeScene("PracticeScene");
    }
}
