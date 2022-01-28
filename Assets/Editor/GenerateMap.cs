using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class GenerateMap : EditorWindow
{


    public GameObject PlaneMesh;
    public int NumberOfColumns = 10;
    public int NumberOfRows = 10;
    public float WidthOfPlane = 1;

    private ListOfHashes AllTilesAsset;

    private ListOfHashes UnqiueTileAsset;
    private FileInfo[] info;
    private FileInfo[] info2;
    private FileInfo[] info3;

    //private string pathToMapTiles = "Assets/Tiles/Map Tiles";
    private string pathToUniqueTiles = "Assets/Tiles/Unique Map Tiles";
    private string pathToTileMaterials = "Assets/Meshes/Tile Materials";
    private string pathTo3DTiles = "Assets/Meshes/3D Tiles";

    int currentRow;
    int ColumnCount;
    int RowCount;

    Material GeneratedMaterial;

    int UniqueTileIndex;
    GameObject MapParent;
    GameObject Parent2d;
        GameObject Parent3d;

    bool Spawn2d = true;
    bool spawn3d = true;
    GameObject NewMesh;
    Vector3 TilePosition;

    [MenuItem("Tools/Generate Map From Tiles")]


    public static void GenerateMapWindow()
    {
        GetWindow<GenerateMap>("Generate Map");


    }

   /* private void DisplayShaderContext(Rect r)
    {
        if (mc == null)
            mc = new MenuCommand(this, 0);
        // Create dummy material to make it not highlight any shaders inside:
        string tmpStr = "Shader \"Hidden/tmp_shdr\"{SubShader{Pass{}}}";
        Material temp = new Material(tmpStr);

        // Rebuild shader menu:
        UnityEditorInternal.InternalEditorUtility.SetupShaderMenu(temp);
        // Destroy temporary shader and material:
        DestroyImmediate(temp.shader, true);
        DestroyImmediate(temp, true);
        // Display shader popup:
        EditorUtility.DisplayPopupMenu(r, "CONTEXT/ShaderPopup", mc);
    }*/

    void OnGUI()
    {

        PlaneMesh = EditorGUILayout.ObjectField("2D Plane Mesh", PlaneMesh, typeof(GameObject), true) as GameObject;

       

        WidthOfPlane = EditorGUILayout.FloatField("Width Of 2D Plane Mesh:", WidthOfPlane);

        NumberOfColumns = EditorGUILayout.IntField("Number Of Columns:", NumberOfColumns);
       NumberOfRows = EditorGUILayout.IntField("Number Of Rows:", NumberOfRows);

        // pathToMapTiles = EditorGUILayout.TextField("Path To Tiles", pathToMapTiles);
        pathToUniqueTiles = EditorGUILayout.TextField("Path To Unique Tiles", pathToUniqueTiles);
        pathToTileMaterials = EditorGUILayout.TextField("Path To Place Generated Materials", pathToTileMaterials);

        AllTilesAsset = EditorGUILayout.ObjectField("All Tiles", AllTilesAsset, typeof(ListOfHashes), true) as ListOfHashes;
        UnqiueTileAsset = EditorGUILayout.ObjectField("Unique Tiles", UnqiueTileAsset, typeof(ListOfHashes), true) as ListOfHashes;


        pathTo3DTiles = EditorGUILayout.TextField("Path To 3D Assets", pathTo3DTiles);

        // ShaderToImport = EditorGUILayout.ObjectField(ShaderToImport, typeof(Shader), true) as Shader;
        //DisplayShaderContext(new Rect(0,0,100,100));

        Spawn2d = EditorGUILayout.Toggle("Spawn 2D Tiles", Spawn2d);
        spawn3d = EditorGUILayout.Toggle("Spawn 3D Tiles", spawn3d);

        if (GUILayout.Button("Generate Materials"))
        {
            GenerateMaterials();
        }

        if (GUILayout.Button("Proceed"))
        {
            GenreatePlanes();
        }


       
    }

    /*void Generate3D()
    {
  


    }*/


    void GenreatePlanes()
    {
        MapParent = new GameObject("Map Parent");
        Parent2d = new GameObject("2D Parent");
        Parent2d.transform.parent = MapParent.transform;
        Parent3d = new GameObject("3D Parent");
        Parent3d.transform.parent = MapParent.transform;
        ColumnCount = 0;
        currentRow = 0;
        RowCount = 0;


        DirectoryInfo dir = new DirectoryInfo(pathToTileMaterials);
        info = dir.GetFiles("*.mat");

        DirectoryInfo dir3 = new DirectoryInfo(pathToUniqueTiles);
        info3 = dir3.GetFiles("*.png");

        DirectoryInfo dir2 = new DirectoryInfo(pathTo3DTiles);
       info2 = dir2.GetFiles("*.fbx");
        //Debug.Log(info2.Length);

        // NumberOfRows = AllTilesAsset.HashList.Count / NumberOfColumns;


        for (int i = 0; i < AllTilesAsset.HashList.Count; i++)
        {
           
            if (Spawn2d)
            {
               
                NewMesh = PrefabUtility.InstantiatePrefab(PlaneMesh) as GameObject;
                NewMesh.transform.rotation = Quaternion.identity;
                NewMesh.name = PlaneMesh.name + i;
            }

            TilePosition = new Vector3(0 - (ColumnCount * WidthOfPlane), 0, 0 + (currentRow * WidthOfPlane));
            if (Spawn2d)
            {
                NewMesh.transform.position = TilePosition; //new Vector3(NewMesh.transform.position.x - (ColumnCount * WidthOfPlane), NewMesh.transform.position.y, NewMesh.transform.position.z + (currentRow * WidthOfPlane));
                NewMesh.transform.parent = Parent2d.transform;
            }
            ColumnCount++;
            RowCount++;

            if(RowCount == NumberOfRows)
            {
                RowCount = 0;
            }

            if (ColumnCount == NumberOfColumns)
            {
                currentRow++;
                ColumnCount = 0;
            }

           

            for (int a = 0; a < UnqiueTileAsset.HashList.Count; a++)
            {
                if(AllTilesAsset.HashList[i] == UnqiueTileAsset.HashList[a])
                {
                    UniqueTileIndex = a;
                }
            }

            if (Spawn2d)
            {
                string pathToFile = info[UniqueTileIndex].Directory + "\\" + info[UniqueTileIndex].Name;
                string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);

                Material m = (Material)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Material));

                NewMesh.GetComponent<Renderer>().material = m;
            }


            if (spawn3d)
            {
                for (int b = 0; b < info2.Length; b++)
                {
                    string modelname = info2[b].Name.Replace(".fbx", "");
                    string tilename = info3[UniqueTileIndex].Name.Replace(".png", "");
                    // tilename = info[UniqueTileIndex].Name.Replace(".mat", "");

                    //  

                    if (modelname == tilename)
                    {
                        //Debug.Log(modelname + "-" + tilename);

                        string pathToFile3 = info2[b].Directory + "\\" + info2[b].Name;
                        string relativepath3 = "Assets" + pathToFile3.Substring(Application.dataPath.Length);

                      //  Debug.Log(relativepath3);

                        GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath(relativepath3, typeof(GameObject));

                        GameObject NewMesh2;
                        NewMesh2 = PrefabUtility.InstantiatePrefab(g) as GameObject;
                        NewMesh2.transform.rotation = Quaternion.identity;

                        NewMesh2.transform.position = TilePosition;
                        NewMesh2.transform.parent = Parent3d.transform;
                    }
                }
            }


        }
    }

    void GenerateMaterials()
    {
        DirectoryInfo dir3 = new DirectoryInfo(pathToUniqueTiles);
        info3 = dir3.GetFiles("*.png");

        for (int i = 0; i < UnqiueTileAsset.HashList.Count; i++)
        {

            GeneratedMaterial = new Material(Shader.Find("Standard"));

            //Texture2D tex = new Texture2D(2, 2);



                 string pathToFile = info[i].Directory + "\\" + info[i].Name;
            string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);

            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Texture2D));

            // tex.LoadImage(info[i] as Texture2D);
             GeneratedMaterial.SetTexture("_MainTex", t);
            string myName = info[i].Name;
            myName.Replace(".png", "");
            AssetDatabase.CreateAsset(GeneratedMaterial, pathToTileMaterials + "/" + myName + ".mat");
        }

    }

}
