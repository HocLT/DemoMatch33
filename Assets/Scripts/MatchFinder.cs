using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchFinder : MonoBehaviour
{
    private Board board;

    // danh sách chứa các Gem đang trùng (match3)
    public List<Gem> currentMatches = new List<Gem>();

    private void Awake()
    {
        board = FindAnyObjectByType<Board>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FindAllMatches()
    {
        currentMatches.Clear();

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];

                if (currentGem != null)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        Gem leftGem = board.allGems[x - 1, y];
                        Gem rightGem = board.allGems[x + 1, y];
                        if (leftGem != null && rightGem != null)
                        {
                            if (leftGem.type == currentGem.type && rightGem.type == currentGem.type)
                            {
                                currentGem.isMatch = true;
                                leftGem.isMatch = true;
                                rightGem.isMatch = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(leftGem);
                                currentMatches.Add(rightGem);
                            }
                        }
                    }

                    if (y > 0 && y < board.height - 1)
                    {
                        Gem belowGem = board.allGems[x, y - 1];
                        Gem aboveGem = board.allGems[x, y + 1];
                        if (belowGem != null && aboveGem != null)
                        {
                            if (belowGem.type == currentGem.type && aboveGem.type == currentGem.type)
                            {
                                currentGem.isMatch = true;
                                belowGem.isMatch = true;
                                aboveGem.isMatch = true;

                                currentMatches.Add(currentGem);
                                currentMatches.Add(belowGem);
                                currentMatches.Add(aboveGem);
                            }
                        }
                    }
                }
            }
        }

        // xóa phần tử trùng
        if (currentMatches.Count > 0)
        {
            // hàm Distinct() -> sẽ xóa phần tử trùng của danh sách
            currentMatches = currentMatches.Distinct().ToList();
        }
    }
}
