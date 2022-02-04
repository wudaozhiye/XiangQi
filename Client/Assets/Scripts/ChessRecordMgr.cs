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
    public GameObject obj1;//第一个框
    public GameObject obj2;//第二个框
    public GameObject objfrist;//第一个名字
    public GameObject objsconde;//第二个棋子名字
    public bool BlackOrRed;//当前是红旗吃黑棋  还是黑棋吃红旗//true 红吃黑   false 黑吃红
    public HUIQIposition ChessID;//吃子时候的两个ID
}

public class ChessRecordMgr : MonoBehaviour
{
    public static QiZi[] pos = new QiZi[400];//最大话，避免象棋走棋出现重叠
    public static int Count;//统计的步数初始化为0

    //下面用来悔棋
    //开始位置的坐标和终点位置的坐标   
    public void AddChess(int count, int fromx, int fromy, int tox, int toy, bool IsTrueOrfalse, int ID1, int ID2)
    {//每次走棋都把棋子位置添加进去
        GameObject item1 = ItemOne(fromx, fromy, tox, toy);//得到第一个框名字
        GameObject item2 = ItemTow(fromx, fromy, tox, toy);//得到第二个框名字
                                                           //如果是吃子
        GameObject firstChess = chessOne(item1);//得到第一个旗子名字
        GameObject scondeChess = ChessTwo(item2);//得到第二个棋子名字
        pos[count].From.x = fromx;
        pos[count].From.y = fromy;
        pos[count].To.x = tox;
        pos[count].To.y = toy;
        pos[count].obj1 = item1;
        pos[count].obj2 = item2;
        pos[count].objfrist = firstChess;
        pos[count].objsconde = scondeChess;
        pos[count].BlackOrRed = IsTrueOrfalse;//判断当前是红吃黑还是黑吃红
        pos[count].ChessID.x = ID1;
        pos[count].ChessID.y = ID2;
        Count++;
    }
    //得到第一个对象名字
    public GameObject ItemOne(int fromx, int fromy, int tox, int toy)
    {//得到开始位置gameobject的对象名字
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
    //得到第二个旗子名字
    //得到第二个对象名字
    public GameObject ItemTow(int fromx, int fromy, int tox, int toy)
    {//得到点击目标位置gameobject的对象名字
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
    //得到第一个旗子名字
    public GameObject chessOne(GameObject obj)
    {
        string s = "";
        GameObject game = null;
        foreach (Transform child in obj.transform)
            s = child.name;//第一个象棋名字
        game = GameObject.Find(s);
        return game;
    }
    //得到第二个旗子名字
    public GameObject ChessTwo(GameObject obj)
    {
        string s = "";
        GameObject game = null;
        foreach (Transform child in obj.transform)
            s = child.name;//第二个象棋名字
        game = GameObject.Find(s);
        return game;
    }
}
