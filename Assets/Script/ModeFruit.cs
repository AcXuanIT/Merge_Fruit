using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ModeFruit : MonoBehaviour
{
    [SerializeField] private int limitFruit;
    [SerializeField] private List<InfoFruit> dataFruit;
    public int LimitFruit
    {
        get => this.limitFruit;
        set => this.limitFruit = value;
    }

    public List<InfoFruit> DataFruits
    {
        get => this.dataFruit;
        set => this.dataFruit = value;
    }

}
