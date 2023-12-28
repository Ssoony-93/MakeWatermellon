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
    //����Ƽ �Ǵ� C# ���� �������� ������ų �� �ִ�.
    //������ ���ŷӱ� ������ �ڵ�� ������Ų��. �̹��� ����� ��!

    // Start is called before the first frame update
    public void GameStart()
    {
        //������Ʈ Ȱ��ȭ
        line.SetActive(true);
        bottom.SetActive(true);
        currentScore.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        highScore.gameObject.SetActive(true);
        maxScoreText.gameObject.SetActive(true);
        startGroup.SetActive(false);

        //���� �÷���
        bgmPlayer.Play();
        SfxPlay(Sfx.Button);

        //���� ����(Ball����)
        Invoke("NextBall", 1.5f);

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

        //�ְ� ���� ����
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore"));
        PlayerPrefs.SetInt("MaxScore", maxScore);
        //���ӿ��� UI ǥ��
        subScoreText.text = "���� : " + scoreText.text;
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
