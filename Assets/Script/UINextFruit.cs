using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINextFruit : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetNextFruit(Sprite sprite)
    {
        this.image.transform.DOScale(Vector3.zero, 0.2f).OnComplete(delegate
        {
            this.image.sprite = sprite;
            this.image.transform.DOScale(Vector3.one, 0.2f);
        });
    }
}
