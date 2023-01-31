using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFlag
{
    Red,    //己方
    Black,  //敌方
}
public enum ChessType
{
    Jiang = 1,    //敌方*10
    Che =2,
    Ma =3,
    Pao =4,
    Shi = 5,
    Xiang = 6,
    Zu = 7,

}
public class ChessManager : SingletonBehaviour<ChessManager>
{
    public static int row = 10;
    public static int colum = 9;
    public static int[,] chess = new int[10, 9]{

        {2,3,6,5,1,5,6,3,2},
        {0,0,0,0,0,0,0,0,0},
        {0,4,0,0,0,0,0,4,0},
        {7,0,7,0,7,0,7,0,7},
        {0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0},
        {14,0,14,0,14,0,14,0,14},
        {0,11,0,0,0,0,0,11,0},
        {0,0,0,0,0,0,0,0,0},
        {9,10,13,12,8,12,13,10,9}

    };

    public void StartGame()
    {
        ChessManager.chess = new int[10, 9]{  //此注释要取消
			{2,3,6,5,1,5,6,3,2},
            {0,0,0,0,0,0,0,0,0},
            {0,4,0,0,0,0,0,4,0},
            {7,0,7,0,7,0,7,0,7},
            {0,0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0,0},
            {14,0,14,0,14,0,14,0,14},
            {0,11,0,0,0,0,0,11,0},
            {0,0,0,0,0,0,0,0,0},
            {9,10,13,12,8,12,13,10,9}
        };
        SetPos();
        for(int i=0;i< chess.GetLength(0);i++)
        {
            for (int j = 0; j < chess.GetLength(1); j++)
            {
                if (chess[i,j] != 0)
                {
                    int chessType = chess[i, j];
                    if(chessType > 7)
                    {
                        LoadChess((ChessType)chess[i, j], PlayerFlag.Black);
                    }
                    else
                    {
                        LoadChess((ChessType)chess[i, j], PlayerFlag.Red);
                    }
                    
                }
            }
            
        }
    }
    private void SetPos()
    {
        int count = row * colum;
        GameObject chessP = GameObject.Find("Chess");
        int xx = -4, zz = 4;
        for (int i = 1; i <= count; i++)
        {
            GameObject item = new GameObject();//找到预设体
            item.name = "item"+i;
            item.transform.parent = chessP.transform;
            item.transform.localPosition = new Vector3(xx,0, zz);
            xx += 1;
            if(xx >4)
            {
                xx = -4;
                zz -= 1;
            }
        }
    }
    private void LoadChess(ChessType chessType,PlayerFlag playerFlag)
    {
        switch(chessType)
        {
            case ChessType.Jiang:
                {

                }
                break;
            case ChessType.Che:
                {

                }
                break;
            case ChessType.Ma:
                {

                }
                break;
            case ChessType.Pao:
                {

                }
                break;
            case ChessType.Shi:
                {

                }
                break;
            case ChessType.Xiang:
                {

                }
                break;
            case ChessType.Zu:
                {

                }
                break;
        }
    }
}
