using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Face
{
    public int vertex;
    public int uv;
    public int normal;
}

public class CreateObj : MonoBehaviour {

    const string objFilePath = "/cube.obj";
    const string KEY_VERTEX = "v";
    const string KEY_UV = "vt";
    const string KEY_VERTEXNormal = "vn";
    const string KEY_FACE = "f";
    List<Vector3> vertexPosList;
    List<Vector2> uvPosList;
    List<Vector3> vertexNormalPosList;
    List<Face> FaceList;
    // Use this for initialization
    void Start () {
        vertexPosList = new List<Vector3>();
        uvPosList = new List<Vector2>();
        vertexNormalPosList = new List<Vector3>();
        FaceList = new List<Face>();

        StreamReader sR = File.OpenText(Application.streamingAssetsPath + objFilePath);
        string nextLine;
        while ((nextLine = sR.ReadLine()) != null)
        {
            //Debug.Log(nextLine);
            ParseString(nextLine);
        }
        sR.Close();
        GameObject obj = new GameObject("cubeFromObj");
        GameObject newGO = Instantiate(obj, Vector3.zero, Quaternion.identity);
        newGO.AddComponent<MeshFilter>();
        MeshRenderer mr = newGO.AddComponent<MeshRenderer>();
        Material mat = Resources.Load<Material>("Material/cubeTexture");
        mr.material = mat;
        Mesh cubeMesh = newGO.GetComponent<MeshFilter>().mesh;
        cubeMesh.Clear();
        List<Vector3> vertexes = new List<Vector3>();
        List<int> UVList = new List<int>();
        List<int> TriList = new List<int>();
        for(int i = 0; i < uvPosList.Count; i++)
        {
            Face currFace = FaceList.Find((face) =>
            {
                return face.uv == i + 1;
                });

            vertexes.Add(vertexPosList[currFace.vertex - 1]);
        }
        
        for(int i= 0; i < FaceList.Count; i+=4)
        {
            TriList.Add(FaceList[i].uv-1);
            TriList.Add(FaceList[i+1].uv - 1);
            TriList.Add(FaceList[i+2].uv - 1);
            TriList.Add(FaceList[i+2].uv - 1);
            TriList.Add(FaceList[i+3].uv - 1);
            TriList.Add(FaceList[i].uv - 1);
        }

        cubeMesh.vertices = vertexes.ToArray();
        cubeMesh.triangles = TriList.ToArray();
        cubeMesh.uv = uvPosList.ToArray();
        //Vector2[] uvArray = new Vector2[UVList.Count];
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ParseString(string str)
    {
        string[] strArray = str.Split(' ');
        switch (strArray[0])
        {
            case KEY_VERTEX:
                vertexPosList.Add(new Vector3(Str2Float(strArray[1]), Str2Float(strArray[2]), Str2Float(strArray[3])));
                break;
            case KEY_UV:
                uvPosList.Add(new Vector2(Str2Float(strArray[1]), Str2Float(strArray[2])));
                break;
            case KEY_VERTEXNormal:
                vertexNormalPosList.Add(new Vector3(Str2Float(strArray[1]), Str2Float(strArray[2]), Str2Float(strArray[3])));
                break;
            case KEY_FACE:
                for(int i = 1; i < strArray.Length; i++)
                {
                    string[] intStrArray = strArray[i].Split('/');
                    Face face = new Face();
                    face.vertex = int.Parse(intStrArray[0]);
                    face.uv = int.Parse(intStrArray[1]);
                    face.normal = int.Parse(intStrArray[2]);
                    FaceList.Add(face);
                }
                break;
        }
    }


    float Str2Float(string str)
    {
        return float.Parse(str);
    }
}
