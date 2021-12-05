using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップのブロックのデータ
/// </summary>
public class BlockData : MonoBehaviour
{
    private BlockCharacter character;

    public bool IsAlreadyRead { get; set; }
    public GameObject GameObject { get; set; }

    public BlockData(BlockCharacter character)
    {
        this.character = character;
        IsAlreadyRead = false;
    }

    public BlockCharacter GetCharacter()
    {
        return (character);
    }
}
