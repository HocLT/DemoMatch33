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

    public enum GemType { blue, green, purple, yellow, red };
    public GemType type;

    public bool isMatch;    // default value is false

    Vector2Int previousPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // swap gem
        if (Vector2.Distance(transform.position, pos) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * board.gemSpeed);
        }
        else
        {
            transform.position = new Vector3(pos.x, pos.y, 0f);
        }
        // xử lý sự kiện mouse release
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;

            if (board.currentState == Board.BoardState.move)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
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
        previousPos = pos;  // lưu lại vị trí cũ của gem

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
        if (otherGem != null)
        {
            board.allGems[pos.x, pos.y] = this;
            board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;

            // run coroutine để kiểm tra xem có match3 hay không?
            StartCoroutine(CheckMoveCo());
        }
    }

    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(.5f);

        // thực hiện kiểm tra match3
        board.matchFinder.FindAllMatches();
        if (otherGem != null)
        {
            // chuyển ngược nếu không match3
            if (!isMatch && !otherGem.isMatch)
            {
                otherGem.pos = pos;
                pos = previousPos;

                board.allGems[pos.x, pos.y] = this;
                board.allGems[otherGem.pos.x, otherGem.pos.y] = otherGem;

                yield return new WaitForSeconds(.2f);
                board.currentState = Board.BoardState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }
}
