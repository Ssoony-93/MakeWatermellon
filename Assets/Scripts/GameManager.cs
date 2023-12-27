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
    //유니티 또는 C# 에서 프레임을 고정시킬 수 있다.
    //하지만 번거롭기 때문에 코드로 고정시킨다. 이번엔 대깨코 승!

    // Start is called before the first frame update
    void Start()
    {
        bgmPlayer.Play();
        NextBall();
    }

    Ball MakeBall()
    {
        //Effect 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect" + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);
        //오브젝트 풀링(ObjectPooling) : 미리 생성해둔 오브젝트 재활용

        //Ball 생성
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
        lastBall.level = Random.Range(0, maxLevel); //마지막 값은 포함이 안된다 그러므로 8.
        lastBall.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
        StartCoroutine("WaitNext");  //코르틴 호출.
        //특강에선 추적이 불가능하기 때문에 스트링을 지양하라고 했는데?
    }

    IEnumerator WaitNext()  //오늘 배움! 이성언 만세!
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
        //1. 장면 안에 활성화 되어있는 모든 Ball 가져오기
        Ball[] balls = FindObjectsOfType<Ball>();

        //2. 지우기 전에 모든 Ball의 물리효과 비활성화
        for (int index = 0; index < balls.Length; index++)
        {
            balls[index].rigid.simulated = false;
        }

        //3. 1번의 목록을 하나씩 접근해서 지우기
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
