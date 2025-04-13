using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoFruit : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    private Coroutine checkGameOver;

    private int level;
    private Rigidbody2D rb;

    private bool isColider;
    public bool IsColider
    {
        get => this.isColider;
    }

    public int Level
    {
        get => this.level;
    }


    private Action<InfoFruit, InfoFruit, int> onMarge;
    private Action endGame;
    private const float BASERADIUS = 0.35f;
    private SpriteRenderer sr;
    private CircleCollider2D circleCollider;
    private void Awake() // Start
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.sr = GetComponentInChildren<SpriteRenderer>();
        this.circleCollider = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        ObserverManager<EvenID>.AddDesgisterEvent(EvenID.UpdataScore, parant => UpdateViewScore((int)parant));
    }
    private void OnDisable()
    {
        ObserverManager<EvenID>.RemoveAddListener(EvenID.UpdataScore, parant => UpdateViewScore((int)parant));
    }

    public void UpdateViewScore(int value)
    {
        
    }
    public void Init(int level, Action<InfoFruit, InfoFruit, int> actineMerge, Action endGame, bool isFall = false)
    {
        this.level = level;
        onMarge = actineMerge;
        this.endGame = endGame;
        isColider = false;
        rb.bodyType = !isFall ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }

    public void OnFall()
    {
        this.rb.bodyType = RigidbodyType2D.Dynamic;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out InfoFruit infoFruit))
        {
            if (level + 1 >= GameController.Instance.Model.DataFruits.Count) return;

            if(infoFruit.level == this.level)
            {
                onMarge?.Invoke(this, infoFruit, level + 1);
                this.isColider = true;
                infoFruit.isColider = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Line"))
        {
            this.checkGameOver = StartCoroutine(CheckGameOver(1f, collision));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Line"))
        {
            StopCoroutine(this.checkGameOver);
        }
    }
    IEnumerator CheckGameOver(float time , Collider2D collider2D)
    {
        yield return new WaitForSeconds(time);
        if (collider2D != null && collider2D.bounds.Intersects(GetComponent<Collider2D>().bounds))
        {
            endGame?.Invoke();
        }
    }

   
}

