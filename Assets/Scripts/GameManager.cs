using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Ball lastBall;
    public GameObject ballPrefab;
    public Transform ballGroup;


    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    //유니티 또는 C# 에서 프레임을 고정시킬 수 있다.
    //하지만 번거롭기 때문에 코드로 고정시킨다. 이번엔 대깨코 승!

    // Start is called before the first frame update
    void Start()
    {
        NextBall();
    }
    Ball GetBall()
    {
        GameObject instant = Instantiate(ballPrefab,ballGroup);
        Ball instantball = instant.GetComponent<Ball>();
        return instantball;

    }

    void NextBall()
    {
        Ball newball = GetBall();
        lastBall = newball;
        lastBall.level = Random.Range(0, 8); //마지막 값은 포함이 안된다 그러므로 8.
        lastBall.gameObject.SetActive(true);

        StartCoroutine("WaitNext");  //코르틴 호출.
        //특강에선 추적이 불가능하기 때문에 스트링을 지양하라고 했는데?
    }

    IEnumerator WaitNext()  //오늘 배움! 이성언 만세!
    {
        while(lastBall != null)
        {
            yield return null;
        }

        yield return new WaitForSeconds(2.5f);

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

}
