using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textScore;
    private int score;
    private void OnEnable()
    {
        ObserverManager<EvenID>.AddDesgisterEvent(EvenID.UpdataScore, parant => UpdateViewScore((int)parant));
    }
    private void OnDisable()
    {
        ObserverManager<EvenID>.RemoveAddListener(EvenID.UpdataScore, parant => UpdateViewScore((int)parant));
    }
    private void UpdateViewScore(int value)
    {
        score += value;
        textScore.text = score.ToString();
    }
}
