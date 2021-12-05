using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapCreater : MonoBehaviour
{
    [SerializeField] private string mapFilePath; // �}�b�v�t�@�C���̃p�X

    [SerializeField] private GameObject bG;     // �w�i�̃Q�[���I�u�W�F�N�g
    [SerializeField] private int bGImageWidth;  // �w�i�摜�̕�
    [SerializeField] private int bGImageHeight; // �w�i�摜�̍���
    private float bgScaleX; // �w�i�摜��X�̊g�嗦
    private float bgScaleY; // �w�i�摜��Y�̊g�嗦

    [SerializeField] private GameObject player; // �v���C���[�̃Q�[���I�u�W�F�N�g
    private int playerStartPosX; // �v���C���[��X�̊J�n�ʒu
    private int playerStartPosY; // �v���C���[��Y�̊J�n�ʒu

    private List<List<BlockData>> mapBlocks = new List<List<BlockData>>(); // BlockData�N���X�̓񎟌����X�g
    private int mapBlocksWidth;  // �}�b�v���̉��̃u���b�N�̐�
    private int mapBlocksHeight; // �}�b�v���̏c�̃u���b�N�̐�
    [SerializeField] private float enableRange;  // �u���b�N�̗L���͈�
    [SerializeField] private float disableRange; // �u���b�N�𖳌��͈�

    [SerializeField] private GameObject wall; // �ǂ̃Q�[���I�u�W�F�N�g

    // Start is called before the first frame update
    void Start()
    {
        // �}�b�v��ǂݍ���
        ReadMap(mapFilePath);

        // �w�i���쐬
        CreateBg();

        // �v���C���[���쐬
        CreatePlayer();
    }

    private void ReadMap(string filePath)
    {
        try
        {
            using (var stream = new StreamReader(filePath))
            {
                InitBlocks(stream);
                // �}�b�v���R���\�[���ɏo��
                //PutDebugLog();
            }
        }
        catch (System.Exception e)
        {

            Debug.Log($"Error!{e}");
        }
    }

    // �}�b�v�f�[�^��ǂݍ���Ńu���b�N���X�g�Ɋi�[
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

        // �}�b�v�̕����Z�b�g
        this.mapBlocksHeight = count;

        // �w�i�̉摜�ɑ΂���傫�����Z�b�g
        this.bgScaleY = (float)this.mapBlocksHeight / (float)this.bGImageHeight;
    }

    // ��s���̓ǂݍ��񂾃}�b�v�f�[�^���u���b�N���X�g�Ɋi�[
    private void InitMapLine(string line, int index)
    {
        var count = 0;

        foreach (var item in line)
        {
            var character = (BlockCharacter)item;
            var block = new BlockData(character);
            mapBlocks[index].Add(block);
            count++;

            // �v���C���[�̊J�n�|�W�V�������Z�b�g
            if (character == BlockCharacter.Player)
            {
                playerStartPosX = count;
                playerStartPosY = (-1) * index;
            }
        }

        // �}�b�v�̕����Z�b�g
        this.mapBlocksWidth = count;

        // �w�i�̕����Z�b�g
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

    /* �f�o�b�O�p�Ƀ}�b�v�f�[�^���o�� */
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
                // ���͈͓��Ȃ�폜���Ȃ�
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
