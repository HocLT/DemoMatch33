using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    //[HideInInspector]
    public Vector2Int pos;

    //[HideInInspector]
    public Board board;

    Vector3 firstTouchPosition, finalTouchPosition;
    bool mousePressed = false;
    float swipeAngle = 0f;

    Gem otherGem;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // swap gem

        // xử lý sự kiện mouse release
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    public void Setup(Vector2Int pos, Board board)
    {
        this.pos = pos;
        this.board = board;
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePressed = true;    // đang nhấn đè vào nút chuột trái
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(
            finalTouchPosition.y - firstTouchPosition.y,
            finalTouchPosition.x - firstTouchPosition.x
            );
        swipeAngle = swipeAngle * 180 / Mathf.PI;
        //Debug.Log(swipeAngle);
        MovePieces();
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && pos.x < board.width - 1)
        {
            otherGem = board.allGems[pos.x + 1, pos.y];
            otherGem.pos.x--;
            pos.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && pos.y < board.height - 1)
        {
            otherGem = board.allGems[pos.x, pos.y + 1];
            otherGem.pos.y--;
            pos.y++;
        }
        else if (swipeAngle > -135 && swipeAngle <= -45 && pos.y > 0)
        {
            otherGem = board.allGems[pos.x, pos.y - 1];
            otherGem.pos.y++;
            pos.y--;
        }
        else if ((swipeAngle <= -135 || swipeAngle > 135) && pos.x > 0)
        {
            otherGem = board.allGems[pos.x - 1, pos.y];
            otherGem.pos.x++;
            pos.x--;
        }

        // update lại thông tin của các gem
        board.allGems[pos.x, pos.y] = this;
        board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;
    }
}
