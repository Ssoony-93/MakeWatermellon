using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject ballPrefab;
    public Transform ballGroup;
    public List<Ball> ballPool;

    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    [Range(1, 30)]
    public int poolSize;
    public int poolCursor;
    public Ball lastBall;

    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, Over };
    int sfxCursor;

    public int score;
    public int maxLevel;
    public bool isOver;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        ballPool = new List<Ball>();
        effectPool = new List<ParticleSystem>();
        for(int index = 0; index < poolSize; index++)
        {
            MakeBall();
        }
    }
    //����Ƽ �Ǵ� C# ���� �������� ������ų �� �ִ�.
    //������ ���ŷӱ� ������ �ڵ�� ������Ų��. �̹��� ����� ��!

    // Start is called before the first frame update
    void Start()
    {
        bgmPlayer.Play();
        NextBall();
    }

    Ball MakeBall()
    {
        //Effect ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect" + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);
        //������Ʈ Ǯ��(ObjectPooling) : �̸� �����ص� ������Ʈ ��Ȱ��

        //Ball ����
        GameObject instantBallObj = Instantiate(ballPrefab, ballGroup);
        instantBallObj.name = "Ball" + ballPool.Count;
        Ball instantBall = instantBallObj.GetComponent<Ball>();
        instantBall.manager = this;
        instantBall.effect = instantEffect;
        ballPool.Add(instantBall);

        return instantBall;
    }
    Ball GetBall()
    {
        for(int index = 0; index < ballPool.Count; index++)
        {
            poolCursor =  (poolCursor+1) % ballPool.Count;
            if (!ballPool[poolCursor].gameObject.activeSelf)
            {
                return ballPool[poolCursor];
            }
        }
        return MakeBall();
    }

    void NextBall()
    {
        if (isOver)
        {
            return;
        }


        lastBall = GetBall();
        lastBall.level = Random.Range(0, maxLevel); //������ ���� ������ �ȵȴ� �׷��Ƿ� 8.
        lastBall.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
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

        yield return new WaitForSeconds(1f);

        SfxPlay(Sfx.Over);
    }

    public void SfxPlay(Sfx type)
    {
        switch(type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.Attach:
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;

        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor+1) % sfxPlayer.Length;

    }

}
