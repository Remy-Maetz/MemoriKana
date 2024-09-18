using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "charactersGroup", menuName = "MemoryKana/Characters Group", order = 0)]
public class CharactersGroup : ScriptableObject
{
    public List<Character> characters = new List<Character>();
}
