using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    [Header("---------[ Core ]")]
    public bool isOver;
    public int score;
    public int maxLevel;

    [Header("---------[ Object Pooling ]")]
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

    [Header("---------[ Audio ]")]
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, Over };
    int sfxCursor;

    [Header("---------[ UI ]")]
    public GameObject startGroup;
    public GameObject endGroup;
    public Text currentScore;
    public Text scoreText;
    public Text highScore;
    public Text maxScoreText;
    public Text subScoreText;

    [Header("---------[ ETC ]")]
    public GameObject line;
    public GameObject bottom;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        ballPool = new List<Ball>();
        effectPool = new List<ParticleSystem>();
        for(int index = 0; index < poolSize; index++)
        {
            MakeBall();
        }

        if (!PlayerPrefs.HasKey("MaxScore"))
        {
            PlayerPrefs.SetInt("MaxScore", 0);
        }

        maxScoreText.text = PlayerPrefs.GetInt("MaxScore").ToString();

    }
    //유니티 또는 C# 에서 프레임을 고정시킬 수 있다.
    //하지만 번거롭기 때문에 코드로 고정시킨다. 이번엔 대깨코 승!

    // Start is called before the first frame update
    public void GameStart()
    {
        //오브젝트 활성화
        line.SetActive(true);
        bottom.SetActive(true);
        currentScore.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        highScore.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);

        //사운드 플레이
        bgmPlayer.Play();
        SfxPlay(Sfx.Button);

        //게임 시작(Ball생성)
        Invoke("NextBall", 1.5f);

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

        //최고 점수 갱신
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);
        //게임오버 UI 표시
        subScoreText.text = "점수 : " + scoreText.text;
        endGroup.SetActive(true);

        bgmPlayer.Stop();
        SfxPlay(Sfx.Over);
    }

    public void Reset()
    {
        SfxPlay(Sfx.Button);
        StartCoroutine("ResetCoroutine");
    }

    IEnumerator ResetCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Main");
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

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }

    private void LateUpdate()
    {
        scoreText.text = score.ToString();
    }


}
