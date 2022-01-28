using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;


[CreateAssetMenu(fileName = "New List Of Hashes", menuName = "Hash List", order = 51)]
public class ListOfHashes : ScriptableObject
{
   

   [SerializeField]
    public List<string> HashList = new List<string>();
}
