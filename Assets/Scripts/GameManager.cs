using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ball lastBall;
    public GameObject ballPrefab;
    public Transform ballGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int score;
    public int maxLevel;
    public bool isOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    //����Ƽ �Ǵ� C# ���� �������� ������ų �� �ִ�.
    //������ ���ŷӱ� ������ �ڵ�� ������Ų��. �̹��� ����� ��!

    // Start is called before the first frame update
    void Start()
    {
        NextBall();
    }
    Ball GetBall()
    {
        //Effect ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();

        //Ball ����
        GameObject instantBallObj = Instantiate(ballPrefab,ballGroup);
        Ball instantBall = instantBallObj.GetComponent<Ball>();
        instantBall.effect = instantEffect;
        return instantBall;

    }

    void NextBall()
    {
        if (isOver)
        {
            return;
        }

        Ball newball = GetBall();
        lastBall = newball;
        lastBall.manager = this;
        lastBall.level = Random.Range(0, maxLevel); //������ ���� ������ �ȵȴ� �׷��Ƿ� 8.
        lastBall.gameObject.SetActive(true);

        StartCoroutine("WaitNext");  //�ڸ�ƾ ȣ��.
        //Ư������ ������ �Ұ����ϱ� ������ ��Ʈ���� �����϶�� �ߴµ�?
    }

    IEnumerator WaitNext()  //���� ���! �̼��� ����!
    {
        while(lastBall != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        NextBall();
    }

    public void TouchDown()
    {
        if (lastBall! == null)
            return;
        lastBall.Drag();
    }   
    
    public void TouchUp()
    {
        if (lastBall! == null)
            return;
        lastBall.Drop();
        lastBall = null;
    }

    public void GameOver()
    {
        if(isOver)
        {
            return;
        }
        isOver = true;

        StartCoroutine("GameOverRoutine");

    }

    IEnumerator GameOverRoutine()
    {
        //1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� Ball ��������
        Ball[] balls = FindObjectsOfType<Ball>();

        //2. ����� ���� ��� Ball�� ����ȿ�� ��Ȱ��ȭ
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].rigid.simulated = false;
        }

        //3. 1���� ����� �ϳ��� �����ؼ� �����
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
