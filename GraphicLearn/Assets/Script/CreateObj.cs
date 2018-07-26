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
    List<Face[]> FaceList;
    // Use this for initialization
    void Start () {
        vertexPosList = new List<Vector3>();
        uvPosList = new List<Vector2>();
        vertexNormalPosList = new List<Vector3>();
        FaceList = new List<Face[]>();

        StreamReader sR = File.OpenText(Application.streamingAssetsPath + objFilePath);
        string nextLine;
        while ((nextLine = sR.ReadLine()) != null)
        {
            //Debug.Log(nextLine);
            ParseString(nextLine);
        }
        sR.Close();
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
                Face[] faces = new Face[4];
                for(int i = 1; i < strArray.Length - 1; i++)
                {
                    string[] intStrArray = strArray[i].Split('/');
                    Face face = new Face();
                    face.vertex = int.Parse(intStrArray[0]);
                    face.uv = int.Parse(intStrArray[1]);
                    face.normal = int.Parse(intStrArray[2]);
                    faces[i] = face;
                }
                FaceList.Add(faces);
                break;
        }
    }


    float Str2Float(string str)
    {
        return float.Parse(str);
    }
}
