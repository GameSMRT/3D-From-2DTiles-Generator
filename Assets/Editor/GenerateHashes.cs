using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class GenerateHashes : EditorWindow
{

    private string PathToMapTiles = "Assets/Tiles/Map Tiles";
    private string PathToUniqueTiles = "Assets/Tiles/Unique Map Tiles";
    private  Texture2D myGUITexture;
    private FileInfo[] info;
    private ListOfHashes AllTilesAsset;
    private ListOfHashes UnqiueTileAsset;
   // public List<string> HashList = new List<string>();

    [MenuItem("Tools/Generate Tile Hashes")]
    public static void GenerateHashesWindow()
    {
        GetWindow<GenerateHashes>("Generate Tile Hashes");
        

    }


    //UI
    void OnGUI()
    {
        PathToMapTiles = EditorGUILayout.TextField("Path To Tiles", PathToMapTiles);
        PathToUniqueTiles = EditorGUILayout.TextField("Path To Place Unique Tiles", PathToUniqueTiles);


        AllTilesAsset = EditorGUILayout.ObjectField("All Tiles",AllTilesAsset, typeof(ListOfHashes), true) as ListOfHashes;
        UnqiueTileAsset = EditorGUILayout.ObjectField("Unique Tiles",UnqiueTileAsset, typeof(ListOfHashes), true) as ListOfHashes;

        if (GUILayout.Button("Proceed"))
        {
            StoreHashes();
        }
    }


    void StoreHashes()
    {
        //clear the lists
        AllTilesAsset.HashList.Clear();
        UnqiueTileAsset.HashList.Clear();
        //HashList.Clear();

        //get all the .png files in the provided directory
        DirectoryInfo dir = new DirectoryInfo(PathToMapTiles);
         info = dir.GetFiles("*.png");

        foreach (FileInfo f in info)
        {
            //load the texture
            string pathToFile = f.Directory + "\\" + f.Name;
            string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);
             myGUITexture = (Texture2D)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Texture2D));

            //store the hash
            string myString = myGUITexture.imageContentsHash.ToString();
            AllTilesAsset.HashList.Add(myString);          
        }
        UniqueTileStore();

    }

    void UniqueTileStore()
    {   
        //loop though all the hashes we just saved
        for (int i = 0; i < AllTilesAsset.HashList.Count; i++)
        {

            //if the hash is not contained in the unique tile list
            if (!UnqiueTileAsset.HashList.Contains(AllTilesAsset.HashList[i]))
            {

                //add it to the list
                UnqiueTileAsset.HashList.Add(AllTilesAsset.HashList[i]);


                //get the image at this index and copy it to the unique tiles folder
                string pathToFile = info[i].Directory.ToString();
                string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);

                string oldfile = relativepath + "/" + info[i].Name;
                string newfile = PathToUniqueTiles + "/" + info[i].Name;

            
                File.Copy(oldfile, newfile, true);
            }
            else
            {
                //    Debug.Log("Delete" + info[i].Name);
            }
        }

        //refresh the asset database and mark the files dirty so Unity saves them.
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(AllTilesAsset);
        EditorUtility.SetDirty(UnqiueTileAsset);
    }
}