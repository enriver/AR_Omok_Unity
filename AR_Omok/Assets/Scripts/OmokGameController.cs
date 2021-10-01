using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Omok {
    public int[,] board;
    private int currentTurn;

        public Omok()
    {
        this.currentTurn = 1;
        this.board = new int[9,9] { {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0}};
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

}

public class OmokGameController : MonoBehaviour
{
    public Omok OmokGame = new Omok();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
