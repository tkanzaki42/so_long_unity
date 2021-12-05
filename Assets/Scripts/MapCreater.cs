using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapCreater : MonoBehaviour
{
    [SerializeField] private string mapFilePath; // マップファイルのパス

    [SerializeField] private GameObject bG;     // 背景のゲームオブジェクト
    [SerializeField] private int bGImageWidth;  // 背景画像の幅
    [SerializeField] private int bGImageHeight; // 背景画像の高さ
    private float bgScaleX; // 背景画像のXの拡大率
    private float bgScaleY; // 背景画像のYの拡大率

    [SerializeField] private GameObject player; // プレイヤーのゲームオブジェクト
    private int playerStartPosX; // プレイヤーのXの開始位置
    private int playerStartPosY; // プレイヤーのYの開始位置

    private List<List<BlockData>> mapBlocks = new List<List<BlockData>>(); // BlockDataクラスの二次元リスト
    private int mapBlocksWidth;  // マップ内の横のブロックの数
    private int mapBlocksHeight; // マップ内の縦のブロックの数
    [SerializeField] private float enableRange;  // ブロックの有効範囲
    [SerializeField] private float disableRange; // ブロックを無効範囲

    [SerializeField] private GameObject wall; // 壁のゲームオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        // マップを読み込み
        ReadMap(mapFilePath);

        // 背景を作成
        CreateBg();

        // プレイヤーを作成
        CreatePlayer();
    }

    private void ReadMap(string filePath)
    {
        try
        {
            using (var stream = new StreamReader(filePath))
            {
                InitBlocks(stream);
                // マップをコンソールに出力
                //PutDebugLog();
            }
        }
        catch (System.Exception e)
        {

            Debug.Log($"Error!{e}");
        }
    }

    // マップデータを読み込んでブロックリストに格納
    private void InitBlocks(StreamReader stream)
    {
        var count = 0;
        var line = stream.ReadLine();

        while (line != null)
        {
            mapBlocks.Add(new List<BlockData>());
            InitMapLine(line, count);
            line = stream.ReadLine();
            count++;
        }

        // マップの幅をセット
        this.mapBlocksHeight = count;

        // 背景の画像に対する大きさをセット
        this.bgScaleY = (float)this.mapBlocksHeight / (float)this.bGImageHeight;
    }

    // 一行分の読み込んだマップデータをブロックリストに格納
    private void InitMapLine(string line, int index)
    {
        var count = 0;

        foreach (var item in line)
        {
            var character = (BlockCharacter)item;
            var block = new BlockData(character);
            mapBlocks[index].Add(block);
            count++;

            // プレイヤーの開始ポジションをセット
            if (character == BlockCharacter.Player)
            {
                playerStartPosX = count;
                playerStartPosY = (-1) * index;
            }
        }

        // マップの幅をセット
        this.mapBlocksWidth = count;

        // 背景の幅をセット
        this.bgScaleX = (float)this.mapBlocksWidth / (float)this.bGImageWidth;
    }

    private void CreateBg()
    {
        var bGObject = Instantiate(this.bG);
        bGObject.transform.position = new Vector3(0.5f, 1f, 0f);
        bGObject.transform.localScale = new Vector3(this.bgScaleX, this.bgScaleY, 1);
    }

    private void CreatePlayer()
    {
        var player = Instantiate(this.player);
        player.transform.position = new Vector3(this.playerStartPosX, this.playerStartPosY, 1);
    }

    /* デバッグ用にマップデータを出力 */
    private void PutDebugLog()
    {
        foreach (var line in mapBlocks)
        {
            foreach (var block in line)
            {
                Debug.Log($"block char is {block.GetCharacter()}");
            }
        }
    }

    public void CreateAround(float posX, float posY)
    {
        posY = Mathf.Abs(posY);
        var startX = (posX - this.enableRange) > 0f ? (posX - this.enableRange) : 0f;
        var endX = (posX + this.enableRange) < (float)this.mapBlocksWidth ? (posX + this.enableRange) : (float)this.mapBlocksWidth;
        var startY = (posY - this.enableRange) > 0f ? (posY - this.enableRange) : 0f;
        var endY = (posY + this.enableRange) < (float)this.mapBlocksHeight ? (posY + this.enableRange) : (float)this.mapBlocksHeight;

        for (int i = (int)startY; i < (int)endY; i++)
        {
            for (int j = (int)startX; j < (int)endX; j++)
            {
                if (mapBlocks[i][j].IsAlreadyRead == true)
                {
                    continue;
                }

                if (mapBlocks[i][j].GetCharacter() == BlockCharacter.Wall)
                {
                    Debug.Log($"{j}, {i}, {mapBlocks[i][j].GetCharacter()} is created.");
                    mapBlocks[i][j].GameObject = Instantiate(this.wall);
                    mapBlocks[i][j].GameObject.transform.position = new Vector3(j + 1, -i, 0);
                }
                mapBlocks[i][j].IsAlreadyRead = true;
            }
        }
    }

    public void DeleteAround(float posX, float posY)
    {
        posY = Mathf.Abs(posY);
        var startX = (posX - this.disableRange) > 0f ? (posX - this.disableRange) : 0f;
        var endX = (posX + this.disableRange) < (float)this.mapBlocksWidth ? (posX + this.disableRange) : (float)this.mapBlocksWidth;
        var startY = (posY - this.disableRange) > 0f ? (posY - this.disableRange) : 0f;
        var endY = (posY + this.disableRange) < (float)this.mapBlocksHeight ? (posY + this.disableRange) : (float)this.mapBlocksHeight;

        for (int i = 0; i < this.mapBlocksHeight; i++)
        {
            for (int j = 0; j < mapBlocksWidth; j++)
            {
                // 一定範囲内なら削除しない
                if ((i >= startY && j >= startX && i <= endY && j <= endX) || mapBlocks[i][j].IsAlreadyRead == false)
                {
                    continue;
                }
                if (mapBlocks[i][j].GetCharacter() == BlockCharacter.Wall)
                {
                    Debug.Log($"{j}, {i}, {mapBlocks[i][j].GetCharacter()} is deleted.");
                    Destroy(mapBlocks[i][j].GameObject);
                }
                mapBlocks[i][j].IsAlreadyRead = false;
            }
        }
    }
}
