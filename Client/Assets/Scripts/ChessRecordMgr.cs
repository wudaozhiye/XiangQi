using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HUIQIposition
{
    public int x;
    public int y;
}
public struct QiZi
{
    public Vector3 From;
    public Vector3 To;
    public GameObject obj1;//��һ����
    public GameObject obj2;//�ڶ�����
    public GameObject objfrist;//��һ������
    public GameObject objsconde;//�ڶ�����������
    public bool BlackOrRed;//��ǰ�Ǻ���Ժ���  ���Ǻ���Ժ���//true ��Ժ�   false �ڳԺ�
    public HUIQIposition ChessID;//����ʱ�������ID
}

public class ChessRecordMgr : MonoBehaviour
{
    public static QiZi[] pos = new QiZi[400];//��󻰣�����������������ص�
    public static int Count;//ͳ�ƵĲ�����ʼ��Ϊ0

    //������������
    //��ʼλ�õ�������յ�λ�õ�����   
    public void AddChess(int count, int fromx, int fromy, int tox, int toy, bool IsTrueOrfalse, int ID1, int ID2)
    {//ÿ�����嶼������λ����ӽ�ȥ
        GameObject item1 = ItemOne(fromx, fromy, tox, toy);//�õ���һ��������
        GameObject item2 = ItemTow(fromx, fromy, tox, toy);//�õ��ڶ���������
                                                           //����ǳ���
        GameObject firstChess = chessOne(item1);//�õ���һ����������
        GameObject scondeChess = ChessTwo(item2);//�õ��ڶ�����������
        pos[count].From.x = fromx;
        pos[count].From.y = fromy;
        pos[count].To.x = tox;
        pos[count].To.y = toy;
        pos[count].obj1 = item1;
        pos[count].obj2 = item2;
        pos[count].objfrist = firstChess;
        pos[count].objsconde = scondeChess;
        pos[count].BlackOrRed = IsTrueOrfalse;//�жϵ�ǰ�Ǻ�Ժڻ��ǺڳԺ�
        pos[count].ChessID.x = ID1;
        pos[count].ChessID.y = ID2;
        Count++;
    }
    //�õ���һ����������
    public GameObject ItemOne(int fromx, int fromy, int tox, int toy)
    {//�õ���ʼλ��gameobject�Ķ�������
        GameObject obj;
        string s3 = "";
        for (int i = 1; i <= 90; i++)
        {
            obj = GameObject.Find("item" + i.ToString());
            int x = System.Convert.ToInt32((obj.transform.localPosition.x) / 130);
            int y = System.Convert.ToInt32(Mathf.Abs((obj.transform.localPosition.y) / 128));
            if (x == fromx && y == fromy)
                s3 = obj.name;
        }
        obj = GameObject.Find(s3);
        return obj;
    }
    //�õ��ڶ�����������
    //�õ��ڶ�����������
    public GameObject ItemTow(int fromx, int fromy, int tox, int toy)
    {//�õ����Ŀ��λ��gameobject�Ķ�������
        GameObject obj;
        string s3 = "";
        for (int i = 1; i <= 90; i++)
        {
            obj = GameObject.Find("item" + i.ToString());
            int x = System.Convert.ToInt32((obj.transform.localPosition.x) / 130);
            int y = System.Convert.ToInt32(Mathf.Abs((obj.transform.localPosition.y) / 128));
            if (x == tox && y == toy)
                s3 = obj.name;
        }
        obj = GameObject.Find(s3);
        return obj;
    }
    //�õ���һ����������
    public GameObject chessOne(GameObject obj)
    {
        string s = "";
        GameObject game = null;
        foreach (Transform child in obj.transform)
            s = child.name;//��һ����������
        game = GameObject.Find(s);
        return game;
    }
    //�õ��ڶ�����������
    public GameObject ChessTwo(GameObject obj)
    {
        string s = "";
        GameObject game = null;
        foreach (Transform child in obj.transform)
            s = child.name;//�ڶ�����������
        game = GameObject.Find(s);
        return game;
    }
}
