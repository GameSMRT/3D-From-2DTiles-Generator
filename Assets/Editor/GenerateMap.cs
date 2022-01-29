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
    private FileInfo[] AllMaterialFiles;
    private FileInfo[] AllFBXModels;
    private FileInfo[] AllUniquePNGfiles;


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


    //UI
    void OnGUI()
    {

        PlaneMesh = EditorGUILayout.ObjectField("2D Plane Mesh", PlaneMesh, typeof(GameObject), true) as GameObject;



        WidthOfPlane = EditorGUILayout.FloatField("Width Of 2D Plane Mesh:", WidthOfPlane);

        NumberOfColumns = EditorGUILayout.IntField("Number Of Columns:", NumberOfColumns);
        NumberOfRows = EditorGUILayout.IntField("Number Of Rows:", NumberOfRows);

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


    void GenreatePlanes()
    {
        //Gerneate empty parent gameobjects
        MapParent = new GameObject("Map Parent");
        Parent2d = new GameObject("2D Parent");
        Parent2d.transform.parent = MapParent.transform;
        Parent3d = new GameObject("3D Parent");
        Parent3d.transform.parent = MapParent.transform;

        //reset counters
        ColumnCount = 0;
        currentRow = 0;
        RowCount = 0;


        //Get all materials from the provided folder
        DirectoryInfo dir = new DirectoryInfo(pathToTileMaterials);
        AllMaterialFiles = dir.GetFiles("*.mat");

        //get all png textures from the Unique tile folder
        DirectoryInfo dir3 = new DirectoryInfo(pathToUniqueTiles);
        AllUniquePNGfiles = dir3.GetFiles("*.png");

        //get all FBX files from the 3D tile folder
        DirectoryInfo dir2 = new DirectoryInfo(pathTo3DTiles);
        AllFBXModels = dir2.GetFiles("*.fbx");


        //Loop through all tiles
        for (int i = 0; i < AllTilesAsset.HashList.Count; i++)
        {

            //spawn a new isntance of the plane mesh
            if (Spawn2d)
            {

                NewMesh = PrefabUtility.InstantiatePrefab(PlaneMesh) as GameObject;
                NewMesh.transform.rotation = Quaternion.identity;
                NewMesh.name = PlaneMesh.name + i;
            }



            //calulate where the plane should be in the loop,a nd move it into place.
            TilePosition = new Vector3(0 - (ColumnCount * WidthOfPlane), 0, 0 + (currentRow * WidthOfPlane));
            if (Spawn2d)
            {
                NewMesh.transform.position = TilePosition; //new Vector3(NewMesh.transform.position.x - (ColumnCount * WidthOfPlane), NewMesh.transform.position.y, NewMesh.transform.position.z + (currentRow * WidthOfPlane));
                NewMesh.transform.parent = Parent2d.transform;
            }

            //Iterate the counters
            ColumnCount++;
            RowCount++;

            //when the row count reaches the max, reset back to 0
            if (RowCount == NumberOfRows)
            {
                RowCount = 0;
            }

            //when the collume count reaches the max, reset to 0 and iterate the current row
            if (ColumnCount == NumberOfColumns)
            {
                currentRow++;
                ColumnCount = 0;
            }

            //loop though all the Unique tiles and compare it to the current index to determine what Unique tile we should be using.
            for (int a = 0; a < UnqiueTileAsset.HashList.Count; a++)
            {
                if (AllTilesAsset.HashList[i] == UnqiueTileAsset.HashList[a])
                {
                    UniqueTileIndex = a;
                }
            }

            //Load the unique material we found and apply it to the spawned mesh.
            if (Spawn2d)
            {
                string pathToFile = AllMaterialFiles[UniqueTileIndex].Directory + "\\" + AllMaterialFiles[UniqueTileIndex].Name;
                string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);

                Material m = (Material)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Material));

                NewMesh.GetComponent<Renderer>().material = m;
            }


            //Spawn 3d Meshes
            if (spawn3d)
            {
                for (int b = 0; b < AllFBXModels.Length; b++)
                {
                    //Get the current names of 3D model and current Unique PNG name and strip out the file extentions
                    string modelname = AllFBXModels[b].Name.Replace(".fbx", "");
                    string tilename = AllUniquePNGfiles[UniqueTileIndex].Name.Replace(".png", "");


                    //if the files have the same name
                    if (modelname == tilename)
                    {
                        //load the 3D file and spawn it in the same position.
                        string pathToFile3 = AllFBXModels[b].Directory + "\\" + AllFBXModels[b].Name;
                        string relativepath3 = "Assets" + pathToFile3.Substring(Application.dataPath.Length);

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
        //get all the .png files inside the provided Unique tiles folder
        DirectoryInfo dir3 = new DirectoryInfo(pathToUniqueTiles);
        AllUniquePNGfiles = dir3.GetFiles("*.png");

        //loop through the UnqiueTileAsset list
        for (int i = 0; i < UnqiueTileAsset.HashList.Count; i++)
        {
            //load the texture at this index
            string pathToFile = AllMaterialFiles[i].Directory + "\\" + AllMaterialFiles[i].Name;
            string relativepath = "Assets" + pathToFile.Substring(Application.dataPath.Length);
            Texture2D t = (Texture2D)AssetDatabase.LoadAssetAtPath(relativepath, typeof(Texture2D));

            //create a new mateiral, and assign the main tex to the loaded texture
            GeneratedMaterial = new Material(Shader.Find("Standard"));
            GeneratedMaterial.SetTexture("_MainTex", t);

            //Grab the name of the texture and remove .png from it
            string myName = AllMaterialFiles[i].Name;
            myName.Replace(".png", "");

            //save the material.
            AssetDatabase.CreateAsset(GeneratedMaterial, pathToTileMaterials + "/" + myName + ".mat");
        }

    }

}
