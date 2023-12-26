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
    //����Ƽ �Ǵ� C# ���� �������� ������ų �� �ִ�.
    //������ ���ŷӱ� ������ �ڵ�� ������Ų��. �̹��� ����� ��!

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
        lastBall.level = Random.Range(0, 8); //������ ���� ������ �ȵȴ� �׷��Ƿ� 8.
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
