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
    // Start is called before the first frame update
    void Start()
    {
        allGems = new Gem[width, height];

        Setup();
    }

    // Update is called once per frame
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
                SpawnGem(new Vector2Int(x, y), gems[gemToUse]);
            }
        }
    }

    void SpawnGem(Vector2Int pos, Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = $"Gem - {pos.x}, {pos.y}";

        allGems[pos.x, pos.y] = gem;

        // lưu thông tin cho gem
        gem.Setup(pos, this);
    }
}
