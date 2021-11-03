using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Omok {

    public int[,] board;
    private int currentTurn;
    //public RenjuRule renjuRule;
    public int[] dx = new int[] { -1, 1, -1, 1, 0, 0, 1, -1 };
    public int[] dy = new int[] { 0, 0, -1, 1, -1, 1, -1, 1 };

    public Omok()
    {
        this.currentTurn = 1; // �浹
        this.board = new int[9,9] { {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0},
                                    {0,0,0,0,0,0,0,0,0}};

        //this.renjuRule = new RenjuRule(this.board);
    }
    
    public int getCurrentTurn()
    {
        return currentTurn;
    }
    public void setCurrentTurn(int turn)
    {
        if(turn == 1)
        {
            this.currentTurn = 2;
        }
        else
        {
            this.currentTurn = 1;
        }
    }

    public void setStone(int x, int y, int turn)
    {
        this.board[x, y] = turn;
        //this.renjuRule.setStone(x, y, turn);
        
    }

    // �ݼ�üũ
    public bool isForbidden(int x, int y, int turn)
    {
        if (forbiddenPoint(x, y, turn)) return true;

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

    // board ����� ��� ���
    bool isInValid(int x, int y)
    {
        if (x < 0) return true;
        if (x > 8) return true;
        if (y < 0) return true;
        if (y > 8) return true;

        return false;
    }

    // �Է� �������� �������� �����ִ� ���� ���� ����
    public int get_stone_count(int x, int y, int currentTurn, int direction)
    {
        int x1 = x;
        int y1 = y;
        int count = 1;

        for (int i = 0; i < 2; i++)
        {
            x = x1;
            y = y1;

            while (true)
            {
                x += dx[direction * 2 + i];
                y += dy[direction * 2 + i];

                if (isInValid(x, y) || this.board[x, y] == 0) break;
                else if (this.board[x, y] == currentTurn) count += 1;
            }
        }

        //Debug.Log("���� �� : "+currentTurn+", ���� ���� : "+count);

        return count;
    }

    // ���� �������� (���� or ����[��])
    public bool isGameOver(int x, int y, int currentTurn)
    {
        int count;
        for (int i = 0; i < 4; i++)
        {
            count = get_stone_count(x, y, currentTurn, i);

            if (count >= 5) return true;
        }

        return false;
    }

    // �����ΰ�
    public bool isSix(int x, int y, int currentTurn)
    {
        int count;
        for (int i = 0; i < 4; i++)
        {
            count = get_stone_count(x, y, currentTurn, i);

            if (count > 5) return true;
        }

        return false;
    }

    // �����ΰ�
    public bool isFive(int x, int y, int currentTurn)
    {
        int count;
        for (int i = 0; i < 4; i++)
        {
            count = get_stone_count(x, y, currentTurn, i);

            if (count == 5) return true;
        }

        return false;
    }

    // �� ���� ã�� -- �̸� �ݼ��� ������ ���س��� ����
    public (int, int) find_empty_point(int x, int y, int currentTurn, int direction)
    {
        int nx = x;
        int ny = y;
        while (true)
        {
            nx += dx[direction];
            ny += dy[direction];

            if (isInValid(nx, ny) || this.board[nx, ny] != currentTurn) break;
        }

        if (!isInValid(nx, ny) && this.board[nx, ny] == 0) return (x, y);
        else return (404, 404); // Null�� ��ü
    }

    // �ֻ�1
    public bool openThree(int x, int y, int currentTurn, int direction)
    {
        int nx;
        int ny;

        for (int i = 0; i < 2; i++)
        {
            (nx, ny) = find_empty_point(x, y, currentTurn, direction * 2 + i);

            if ((nx, ny) != (404, 404))
            {
                setStone(nx, ny, currentTurn);

                if (openFour(nx, ny, currentTurn, direction) == 1)
                {
                    if (!forbiddenPoint(nx, ny, currentTurn))
                    {
                        setStone(nx, ny, 0);
                        return true;
                    }
                }
                setStone(nx, ny, 0);
            }
        }

        return false;
    }

    // �ֻ�2
    public int openFour(int x, int y, int currentTurn, int direction)
    {
        if (isFive(x, y, currentTurn)) return 404; // false

        int count = 0;
        int nx, ny;
        for (int i = 0; i < 2; i++)
        {
            (nx, ny) = find_empty_point(x, y, currentTurn, direction * 2 + i);

            if ((nx, ny) != (404, 404))
            {
                if (Five(nx, ny, currentTurn, direction)) count += 1;
            }

        }

        if (count == 2)
        {
            if (get_stone_count(x, y, currentTurn, direction) == 4) count = 1;
        }
        else
        {
            count = 0;
        }

        return count;
    }

    // 4���� �����ֳ�
    public bool Four(int x, int y, int currentTurn, int direction)
    {
        int nx, ny;

        for (int i = 0; i < 2; i++)
        {
            (nx, ny) = find_empty_point(x, y, currentTurn, direction * 2 + i);

            if ((nx, ny) != (404, 404))
            {
                if (Five(nx, ny, currentTurn, direction)) return true;
            }
        }

        return false;
    }

    // 5���� �����ֳ�

    public bool Five(int x, int y, int currentTurn, int direction)
    {
        if (get_stone_count(x, y, currentTurn, direction) == 5) return true;

        return false;
    }

    public bool doubleThree(int x, int y, int currentTurn)
    {
        int count = 0;

        setStone(x, y, currentTurn);

        for (int i = 0; i < 4; i++)
        {
            if (openThree(x, y, currentTurn, i)) count += 1;
        }

        setStone(x, y, 0);

        if (count >= 2) return true;

        return false;
    }

    public bool doubleFour(int x, int y, int currentTurn)
    {
        int count = 0;

        setStone(x, y, currentTurn);

        for (int i = 0; i < 4; i++)
        {
            if (openFour(x, y, currentTurn, i) == 2) count += 2;
            else if (Four(x, y, currentTurn, i)) count += 1;
        }

        setStone(x, y, 0);

        if (count >= 2) return true;

        return false;
    }

    // �浹 ���� �ݼ�
    public bool forbiddenPoint(int x, int y, int currentTurn)
    {
        if (isFive(x, y, currentTurn)) return false;
        else if (isSix(x, y, currentTurn)) return true;
        else if (doubleThree(x, y, currentTurn) || doubleFour(x, y, currentTurn)) return true;

        return false;
    }

    public List<(int, int)> getForbiddenPoints(int currentTurn)
    {
        List<(int, int)> forbiddenPoints = new List<(int, int)>();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (this.board[i, j] != 0) continue;
                if (forbiddenPoint(i, j, currentTurn)) forbiddenPoints.Add((i, j));
            }
        }

        return forbiddenPoints;
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
