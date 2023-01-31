using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTest : MonoBehaviour
{
    public Button startBtn;
    public Button huiQiBtn;
    public Button restartBtn;

    // Start is called before the first frame update
    void Awake()
    {
        startBtn.onClick.AddListener(StartGame);
        huiQiBtn.onClick.AddListener(HuiQi);
        restartBtn.onClick.AddListener(ReStart);
    }

    private void StartGame()
    {
        //ChessManager.Instance.StartGame();
    }
    private void HuiQi()
    {

    }
    private void ReStart()
    {

    }
}
