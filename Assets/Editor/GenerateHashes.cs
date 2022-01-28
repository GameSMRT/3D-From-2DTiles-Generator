using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class GenerateHashes : EditorWindow
{

    private string _name = "Assets/Tiles/Map Tiles";
    private string _name2 = "Assets/Tiles/Unique Map Tiles";

   // private string _Tiles = "Assets/AllTiles.asset";

   // private string _UniqueTiles = "Assets/UniqueTiles.asset";
    private  Texture2D myGUITexture;

    private FileInfo[] info;

   // private ListOfHashes source;
   // private ListOfHashes UniqueHashFile;

    private ListOfHashes AllTilesAsset;

    private ListOfHashes UnqiueTileAsset;

  //  private ListOfHashes Names;

    public List<string> HashList = new List<string>();

    [MenuItem("Tools/Generate Tile Hashes")]
    public static void GenerateHashesWindow()
    {
        GetWindow<GenerateHashes>("Generate Tile Hashes");
        

    }

    void OnGUI()
    {
        _name = EditorGUILayout.TextField("Path To Tiles", _name);
        _name2 = EditorGUILayout.TextField("Path To Place Unique Tiles", _name2);
        // _Tiles = EditorGUILayout.TextField("All Tiles Asset", _Tiles);

        // _UniqueTiles = EditorGUILayout.TextField("Unique Tiles", _UniqueTiles);


        AllTilesAsset = EditorGUILayout.ObjectField("All Tiles",AllTilesAsset, typeof(ListOfHashes), true) as ListOfHashes;
        UnqiueTileAsset = EditorGUILayout.ObjectField("Unique Tiles",UnqiueTileAsset, typeof(ListOfHashes), true) as ListOfHashes;


      //  Names = EditorGUILayout.ObjectField("Names", Names, typeof(ListOfHashes), true) as ListOfHashes;

        if (GUILayout.Button("Proceed"))
        {
            StoreHashes();
        }
    }

     private static int SortByName(string o1, string o2) {
     return o1.CompareTo(o2);
 }



    void StoreHashes()
    {
        // string path1 = 
        // source = (ListOfHashes)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(AllTilesAsset), typeof(ListOfHashes));
        //UniqueHashFile = (ListOfHashes)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(UnqiueTileAsset), typeof(ListOfHashes));

        AllTilesAsset.HashList.Clear();
        UnqiueTileAsset.HashList.Clear();
       // Names.HashList.Clear();

        HashList.Clear();

        DirectoryInfo dir = new DirectoryInfo(_name);
         info = dir.GetFiles("*.png");
        foreach (FileInfo f in info)
        {
            string pathToFile = f.Directory + "\\" + f.Name;
            string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);
              myGUITexture = (Texture2D)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Texture2D));

            //  HashList.Add(f.Name);
         //   Names.HashList.Add(f.Name);
             

            

            //   source.HashList.add
            // Debug.Log(relativepath);

            string myString = myGUITexture.imageContentsHash.ToString();
            AllTilesAsset.HashList.Add(myString);
            //  Debug.Log(myGUITexture.imageContentsHash);
            // Debug.Log(f.Directory + "\\" + f.Name);

          
        }
        //  Names.HashList = HashList.OrderBy(go => go).ToList();
       // Names.HashList.Sort();
        UniqueTileStore();

    }

    void UniqueTileStore()
    {
        

       

        
        for (int i = 0; i < AllTilesAsset.HashList.Count; i++)
        {
            if (!UnqiueTileAsset.HashList.Contains(AllTilesAsset.HashList[i]))
            {
                UnqiueTileAsset.HashList.Add(AllTilesAsset.HashList[i]);
                // UniqueHashFile.HashList.Add();

                string pathToFile = info[i].Directory.ToString();
                string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);

                string oldfile = relativepath + "/" + info[i].Name;
                string newfile = _name2 + "/" + info[i].Name;

                // FileUtil.CopyFileOrDirectory(oldfile, _name2);
                File.Copy(oldfile, newfile, true);
            }
            else
            {
                //    Debug.Log("Delete" + info[i].Name);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(AllTilesAsset);
        EditorUtility.SetDirty(UnqiueTileAsset);
        // info = null;

    }
}