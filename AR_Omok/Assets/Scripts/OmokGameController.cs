using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Omok {

    public int[,] board;
    private int currentTurn;
    public RenjuRule renjuRule;

    public Omok()
    {
        this.currentTurn = 1; // 흑돌
        this.board = new int[9,9] { {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0}};

        this.renjuRule = new RenjuRule(this.board);
    }
    
    public int getCurrentTurn()
    {
        return currentTurn;
    }
    public void setCurrentTurn(int turn)
    {
        if(turn == 1)
        {
            this.currentTurn = 0;
        }
        else
        {
            this.currentTurn = 1;
        }
    }

    public void setStone(int x, int y, int turn)
    {
        this.board[x, y] = turn;
        this.renjuRule.setStone(x, y, turn);
        
    }
    // 오목인가
    public bool isGameOver(int x, int y, int turn)
    {
        if (this.renjuRule.isGameOver(x, y, turn)) return true;

        return false;
    }

    // 금수체크
    public bool isForbidden(int x, int y, int turn)
    {
        if (this.renjuRule.forbiddenPoint(x, y, turn)) return true;

        return false;
    }

    public void showBoard()
    {
        for(int i=0; i<9; i++)
        {
            for(int j=0; j<9; j++)
            {
                if (this.board[i, j] != 0) Debug.Log(i + "-" + j);
            }
        }
    }

}

public class OmokGameController : MonoBehaviour
{
    //public Omok OmokGame = new Omok();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
