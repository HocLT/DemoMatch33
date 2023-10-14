using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public GameObject bgTilePrefab;

    public Gem[] gems;

    public Gem[,] allGems;  // lưu lại thông tin các gem đang có trên board

    public float gemSpeed = 7f;

    public MatchFinder matchFinder;

    public enum BoardState { wait, move };
    public BoardState currentState = BoardState.move;

    private void Awake()
    {
        matchFinder = FindObjectOfType<MatchFinder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];

        Setup();
    }

    // Update is called once per frame
    private void Update()
    {
        matchFinder.FindAllMatches();
    }

    void Setup()
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject bgTile = Instantiate(bgTilePrefab, new Vector3(x, y, 0f), Quaternion.identity);  // Quaternion trong unity biểu diễn góc xoay
                bgTile.transform.parent = transform;
                bgTile.name = $"Background - {x}, {y}";

                // sinh ngẫu nhiên 1 số từ 0 - 4
                int gemToUse = UnityEngine.Random.Range(0, gems.Length);
                // gem[gemToUse] => thể hiện Gem Object đang sử dụng

                int iteration = 0;
                while (MatchesAt(new Vector2Int(x, y), gems[gemToUse]) && iteration < 100)
                {
                    gemToUse = UnityEngine.Random.Range(0, gems.Length);
                    iteration++;
                }

                SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
            }
        }
    }

    void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = $"Gem - {pos.x}, {pos.y}";

        allGems[pos.x, pos.y] = gem;

        // lưu thông tin cho gem
        gem.Setup(pos, this);
    }

    bool MatchesAt(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 1)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type && allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type && allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type)
            {
                return true;
            }
        }
        return false;
    }

    void DestroyMatchedGemAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatch)
            {
                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < matchFinder.currentMatches.Count; i++)
        {
            if (matchFinder.currentMatches[i] != null)
            {
                DestroyMatchedGemAt(matchFinder.currentMatches[i].pos);
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.5f);

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0) 
                {
                    // dồn hàng xuống
                    allGems[x, y].pos.y -= nullCounter;
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
            nullCounter = 0;
        }
        StartCoroutine(FillBoardCo());
    }

    IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();

        yield return new WaitForSeconds(.5f);
        matchFinder.FindAllMatches();

        if (matchFinder.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.2f);
            currentState = BoardState.move;
        }
    }

    void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null)
                {
                    int gemToUse = UnityEngine.Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
                }
            }
        }
        // kiểm tra lại số Gem đang có trên màn hình
        CheckMisplacedGems();
    }

    void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();
        foundGems.AddRange(FindObjectsOfType<Gem>());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        // xóa các gem bị dư => gem lỗi
        foreach (var gem in foundGems)
        {
            Destroy(gem.gameObject);
        }
    }
}
