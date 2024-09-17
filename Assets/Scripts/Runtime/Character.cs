using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "character", menuName = "MemoryKana/Character", order = 0)]
public class Character : ScriptableObject
{
    public string romaji;
    public char hiragana, katakana;
}
