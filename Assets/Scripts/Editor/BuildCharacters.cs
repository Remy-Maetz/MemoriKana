using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildCharacters : EditorWindow
{
    static string groupsFile = "Assets/Data/CharGroups.txt";

    static string parentFolder = "Assets/Data/Characters";

    [MenuItem("MemoryKana/Build Characters")]
    public static void BuildCharacter()
    {
        var groupsTXT = AssetDatabase.LoadAssetAtPath<TextAsset>(groupsFile);

        List<Character> characters = new List<Character>();
        List<CharactersGroup> charactersGroups = new List<CharactersGroup>();

        var groups = groupsTXT.text.Split(Environment.NewLine);
        int groupID = 0;
        string groupIDString = "";
        string groupName;
        string groupFolderPath;
        string groupSOPath;
        CharactersGroup groupSO = null;

        int charIDInGroup = 0;
        string[] splits;
        string charPath = "";
        string ro, hi, ka;
        Character charSO = null;
        StringBuilder jpnCharListSB = new StringBuilder();

        foreach (var group in groups)
        {
            if (string.IsNullOrEmpty(group))
                continue;

            groupIDString = groupID.ToString("D2");

            splits = group.Split(","[0]);

            if ((splits.Length % 3) != 0 )
            {
                Debug.LogError("Wrong number of characters for group");
            }

            groupName = $"{groupIDString}_{splits[0]}";
            groupID++;
            groupFolderPath = Path.Combine(parentFolder, groupName);


            if (!AssetDatabase.IsValidFolder(groupFolderPath))
                AssetDatabase.CreateFolder(parentFolder, groupName);

            groupSOPath = Path.Combine(groupFolderPath, $"{groupName}_grp.asset");
            groupSO = SafeLoadOrCreateSOAsset<CharactersGroup>(groupSOPath);
            groupSO.characters.Clear();

            charIDInGroup = 0;

            for (int i = 0; i < splits.Length; i += 3)
            {
                ro = splits[i];
                hi = splits[i + 1];
                ka = splits[i + 2];

                charPath = Path.Combine(groupFolderPath, $"{groupIDString}_{charIDInGroup}_{ro}.asset");
                charIDInGroup++;

                charSO = SafeLoadOrCreateSOAsset<Character>(charPath);

                charSO.romaji = ro;
                charSO.hiragana = hi[0];
                charSO.katakana = ka[0];

                characters.Add(charSO);
                groupSO.characters.Add(charSO);

                jpnCharListSB.Append(hi);
                jpnCharListSB.Append(ka);

                EditorUtility.SetDirty(charSO);
                AssetDatabase.SaveAssetIfDirty(charSO);
            }

            EditorUtility.SetDirty(groupSO);
            AssetDatabase.SaveAssetIfDirty(groupSO);
        }

        string charListPath = "TextMesh Pro/Fonts/JPN_chars_list.txt";

        System.IO.File.WriteAllText(
            Path.Combine(Application.dataPath, charListPath),
            jpnCharListSB.ToString()
            );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static T SafeLoadOrCreateSOAsset<T>(string path) where T : ScriptableObject
    {
        bool needCreateAsset = false;
        T obj = null;

        if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path)))
        {
            needCreateAsset = true;
        }
        else
        {
            obj = AssetDatabase.LoadAssetAtPath<T>(path);
            needCreateAsset = obj == null;
        }

        if (needCreateAsset)
        {
            obj = CreateInstance<T>();
            AssetDatabase.CreateAsset(obj, path);
        }

        return obj;
    }
}
