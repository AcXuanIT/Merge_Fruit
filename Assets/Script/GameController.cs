
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController>
{
    [Space]
    [Header("Setup")]
    [SerializeField] private ModeFruit model;
    [SerializeField] private Transform objectSpawn;
    [SerializeField] private Transform objPool;
    [SerializeField] private InfoFruit fruit;
    [SerializeField] private ParticleSystem effectMerge;
    [SerializeField] private UINextFruit uINextFruit;
    [SerializeField] private GameObject line;
    [SerializeField] private Transform inGame;
    private int indexNextFruite;

    [Space]
    [Header("Time Spawn")]
    [SerializeField] private float timeSpawn;

    [Space]
    [Header("Tigs")]
    [SerializeField] private List<InfoFruit> allFruit;
    [SerializeField] private Animator animatorBox;
    private bool isUseTig = false;

    [Space]
    [Header("Game State")]
    [SerializeField] private bool isLose;
    public bool IsLose
    {
        get => this.isLose;
        set => this.isLose = value;
    }

    private const float POSITIONNOMOVE = 4.8f;
    private bool canSwipe;
    private bool isDelay;

    public ModeFruit Model
    {
        get => this.model;
    }
    private void Start()
    {
        this.allFruit = new List<InfoFruit>();
        this.targetsPrefabs = new List<Sprite>();
        this.NextFruit();
        this.SpawnFruit();
    }
  

    private void Update()
    {
        if (isLose) return;

        this.GetButtonFeature();
        if (this.isUseTig) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    OnDown(touch);
                    break;
                case TouchPhase.Moved:
                    OnMove(touch);
                    break;
                case TouchPhase.Ended:
                    OnUp();
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            this.line.SetActive(false);
            OnDown();
        }
        if(Input.GetMouseButton(0))
        {
            OnMove();
        }
        if(Input.GetMouseButtonUp(0))
        {
            OnUp();
        }
    }
    private Vector3 GetMousePos(Touch touch)
    {
        return Camera.main.ScreenToWorldPoint(touch.position);
    }
    private Vector3 GetMousePos(Vector3 touch)
    {
        return Camera.main.ScreenToWorldPoint(touch);
    }
    private void OnDown()
    {
        if (!this.isDelay)
        {
            this.canSwipe = true;
            MoveObject(Input.mousePosition);
        }
    }
    private void OnDown(Touch touch)
    {
 
        if(!this.isDelay)
        {
            this.canSwipe = true;
            MoveObject(touch.position);
        }
    }
    private void OnMove()
    {
        if (this.canSwipe)
        {
            MoveObject(Input.mousePosition);
        }
    }
    private void OnMove(Touch touch)
    {
        if(this.canSwipe)
        {
            MoveObject(touch.position);
        }
    }
    private void OnUp()
    {
        if (!this.canSwipe) return;

        this.fruit.OnFall();

        this.isDelay = true;
        this.canSwipe = false;
        if (this.fruit)
        {
            this.allFruit.Add(fruit);
            this.fruit.transform.SetParent(objPool);
            this.fruit = null;
        }

        DOVirtual.DelayedCall(timeSpawn, delegate
        {
            SpawnFruit();
            this.line.SetActive(true);
            this.isDelay = false;
        });
    }
    private void MoveObject(Vector3 position)
    {
        Vector3 pos = GetMousePos(position);

        if (pos.x > POSITIONNOMOVE || pos.x < -POSITIONNOMOVE) return;

        objectSpawn.transform.position = new Vector3(pos.x, objectSpawn.transform.position.y, 0f);
    }
    private void NextFruit()
    {

        this.indexNextFruite = Random.Range(0, model.LimitFruit);

        this.uINextFruit.SetNextFruit(model.DataFruits[indexNextFruite].GetComponentInChildren<SpriteRenderer>().sprite);
    }
    private void SpawnFruit()
    {
        if (isLose) return;

        this.fruit = PoolingManager.Spawn(model.DataFruits[indexNextFruite], objectSpawn.position, Quaternion.identity, objectSpawn);
        this.fruit.Init(indexNextFruite, MergeFruit, GameOver);

        this.fruit.transform.DOScale(Vector3.zero, 0.02f);
        this.fruit.transform.DOScale(Vector3.one, 0.2f);
        NextFruit();
    }

    private void MergeFruit(InfoFruit fruit1, InfoFruit fruit2, int level)
    {
        if (fruit1.IsColider && fruit2.IsColider) return;

        Vector3 newPosSpawn = (fruit1.transform.position + fruit2.transform.position)/ 2;

        ParticleSystem effect = PoolingManager.Spawn(this.effectMerge, newPosSpawn, Quaternion.identity, objPool);
        effect.Play();

        InfoFruit newFruit = PoolingManager.Spawn(model.DataFruits[level], newPosSpawn, Quaternion.identity, objPool);
        newFruit.Init(level, MergeFruit, GameOver, true);
        this.allFruit.Add(newFruit);

        PoolingManager.Despawn(fruit1.gameObject);
        PoolingManager.Despawn(fruit2.gameObject);

        if (this.allFruit.Contains(fruit1)) this.allFruit.Remove(fruit1);
        if (this.allFruit.Contains(fruit2)) this.allFruit.Remove(fruit2);

        ObserverManager<EvenID>.PostEven(EvenID.UpdataScore, 10);
        DOVirtual.DelayedCall(1.2f, delegate
        {
            PoolingManager.Despawn(effect.gameObject);
        });
    }
    private void GameOver()
    {
        this.isLose = false;
    }



    #region Tigs
    [Space]
    [Header("Tips")]
    private bool isUseTip1 = false;
    private bool isUseTip2 = false;
    private bool isUseTip3 = false;
    private bool isUseTip4 = false;
    [SerializeField] private Button btnTip1;
    [SerializeField] private Button btnTip2;
    [SerializeField] private Button btnTip3;
    [SerializeField] private Button btnTip4;

    [SerializeField] private List<Sprite> targetsPrefabs;

    [SerializeField] private GameObject UItop;
    [SerializeField] private GameObject Feature;
    
    public bool checkTips()
    {
        return this.isUseTip1 || this.isUseTip2 || this.isUseTip3 || this.isUseTip4;
    }
    public void UseTips()
    {
        if (!checkTips()) return;

        if(this.isUseTig)
        {
            this.UItop.SetActive(false);
            this.Feature.SetActive(false);
        }

        if(this.isUseTip1)
        {
            this.RemoveAllFruitLevel0And1();
        }
        else if(this.isUseTip2)
        {
            this.RemoveOneFruit();
        }
        else if(this.isUseTip3)
        {
            this.UpdateLevelOneFruit();
        }
        else if(this.isUseTip4)
        {
            this.ShakeBox();
        }

        if(!this.isUseTig)
        {
            this.UItop.SetActive(true);
            this.Feature.SetActive(true);
        }
    }

    public void GetButtonFeature()
    {
        btnTip1.onClick.AddListener(delegate
        {
            this.RemoveFruit();
            this.isUseTip1 = true;
            this.isUseTig = true;
        }
        );
        btnTip2.onClick.AddListener(delegate
        {
            this.RemoveFruit();
            this.isUseTip2 = true;
            this.isUseTig = true;
        }
        );
        btnTip3.onClick.AddListener(delegate
        {
            this.RemoveFruit();
            this.isUseTip3 = true;
            this.isUseTig = true;
        }
        );
        btnTip4.onClick.AddListener(delegate
        {
            this.RemoveFruit();
            this.isUseTip4 = true;
            this.isUseTig = true;
        }
        );
        this.UseTips();
    }

    public void RemoveFruit()
    {
        Transform objSpawn = this.inGame.GetChild(0);
        Transform fruit = objSpawn.GetChild(1);

        if ( fruit != null )
        {
            PoolingManager.Despawn(fruit.gameObject);
        }

    }
    public void RemoveAllFruitLevel0And1()
    {
        List<InfoFruit> fruitsRemove = new List<InfoFruit>();
        foreach(var fruit in allFruit)
        {
            if (fruit.Level == 1 || fruit.Level == 0)
            {
                PoolingManager.Despawn(fruit.gameObject);
                fruitsRemove.Add(fruit);
            }
        }
        foreach(var fruit in fruitsRemove)
        {
            this.allFruit.Remove(fruit);
        }

        this.isUseTip1 = false;
        this.isUseTig = false;
    }
    public InfoFruit GetFruit(Vector3 pos)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(pos);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.down);

        if(hit.collider.TryGetComponent(out InfoFruit fruit))
        {
            return fruit;
        }
        return null;
    }
    //private List<G>
    public void AddTargetOfFruit()
    {
        foreach (var fruit in this.allFruit)
        {
            Instantiate(this.targetsPrefabs[fruit.Level]);
        }
    }
    public void RemoveAllTarget()
    {
        
    }
    public void RemoveOneFruit()
    {
        //Debug.Log("Tip2");
        //this.AddTargetOfFruit();
        InfoFruit fruit;
        if(Input.GetMouseButtonDown(0))
        {
            fruit = GetFruit(Input.mousePosition);
            PoolingManager.Despawn(fruit.gameObject);
            this.allFruit.Remove(fruit);
            this.isUseTip2 = false;
        }
        if(!this.isUseTip2) this.isUseTig = false;
    }

    public void UpdateLevelOneFruit()
    {
        InfoFruit fruit;
        if (Input.GetMouseButtonDown(0))
        {
            fruit = GetFruit(Input.mousePosition);
            InfoFruit newFruit = PoolingManager.Spawn(model.DataFruits[fruit.Level + 1], fruit.gameObject.transform.position, Quaternion.identity);
            newFruit.Init(fruit.Level + 1, MergeFruit, GameOver);
            newFruit.transform.SetParent(objPool);

            PoolingManager.Despawn(fruit.gameObject);

            this.allFruit.Remove(fruit);
            this.allFruit.Add(newFruit);

            this.isUseTip3 = false;
        }
        if (!this.isUseTip3) this.isUseTig = false;
    }
    public void ShakeBox()
    {

        animatorBox.SetTrigger("PlayShakeBox");

        this.isUseTip4 = false;
        this.isUseTig = false;
    }

    /*public void ShakeBox()
    {
        this.animatorBox.SetTrigger("Shake");
        StartCoroutine("ResetTipShake", 2f);
    }*/

    /*IEnumerator ResetTipShake(float timeDelay)
    {
        yield return timeDelay;

    }*/
    #endregion

}
