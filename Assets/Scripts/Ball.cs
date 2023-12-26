using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int level;
    public bool isDrag;
    Rigidbody2D rigid;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetInteger("Level", level);
    }


    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //x�� ��� ����
            float leftBorder = -4.2f + transform.localScale.x / 2f;  //���� ������
            float rightBorder = 4.2f - transform.localScale.x / 2f;  //���� ������

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }

    }

    public void Drag() 
    {
        isDrag = true;
    }


    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }


}
