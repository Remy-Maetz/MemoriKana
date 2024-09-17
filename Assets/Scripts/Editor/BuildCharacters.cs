using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BuildCharacters : EditorWindow
{
    static string pattern = @"
a i u e o
ka ki ku ke ko
ga gi gu ge go
sa shi su se so
za ji zu ze zo
ta chi tsu te to
da dji du de do
ha hi fu he ho
ba bi bu be bo
pa pi pu pe po
ma mi mu me mo
ya yu yo
ra ri ru re ro
wa wo n
";

    static string parentFolder = "Assets/Data/Characters";

    [MenuItem("MemoryKana/Build Characters")]
    public static void BuildCharacter()
    {
        List<Character> characters = new List<Character>();
        var groups = pattern.Split(Environment.NewLine);
        int groupID = 0;
        string groupIDString = "";
        int charIDInGroup = 0;

        foreach( var group in groups )
        {
            if (string.IsNullOrEmpty( group ) )
                continue;

            var romajis = group.Split(" "[0]);

            groupIDString = groupID.ToString("D2");
            var groupName = $"{groupIDString}_{romajis[0]}";
            groupID++;
            var groupPath = Path.Combine(parentFolder, groupName);

            if (!AssetDatabase.IsValidFolder(groupPath))
                AssetDatabase.CreateFolder(parentFolder, groupName);

            charIDInGroup = 0;
            var newChars = romajis
                .Select(s => {
                    var path = Path.Combine(groupPath, $"{groupIDString}_{charIDInGroup}_{s}.asset");
                    charIDInGroup++;
                    Character so = null;

                    bool needCreateAsset = false;

                    if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path)))
                    {
                        needCreateAsset = true;
                    }
                    else
                    {
                        so = AssetDatabase.LoadAssetAtPath<Character>(path);
                        needCreateAsset = so == null;
                    }

                    if (needCreateAsset)
                    {
                        so = ScriptableObject.CreateInstance<Character>();
                        AssetDatabase.CreateAsset(so, path);
                    }

                    so.romaji = s;
                    return so;
                })
                .ToList();
            characters.AddRange(newChars);
        }

        string charList = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/TextMesh Pro/Fonts/JPN_chars_list.txt").text;
        int c = 0;

        for (int j = 0; j<2; j++)
            for (int i = 0 ; i < characters.Count; i++)
            {
                while (char.IsWhiteSpace(charList[c]) && c < charList.Length )
                    c++;

                if (j == 0)
                    characters[i].hiragana = charList[c];
                else
                {
                    characters[i].katakana = charList[c];
                    EditorUtility.SetDirty(characters[i] );
                }

                c++;
            }

        AssetDatabase.SaveAssets();
    }
}
