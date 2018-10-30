using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class VerticalTan
{
    public int index = 0;
    public int VerticalInTriNum = 0;
    public Vector4 Tangent = Vector4.zero;
}

public class CreateSphere : MonoBehaviour {

    //public GameObject go;
    public int angleTt = 10;
    public int angleFy = 10;
    public float r = 10;

    int _verticlesCount;
    int _triCount;

    int horizontalNum;
    int verticalNum;

   List<Vector3> _verticlesList;
    List<int> _trianglesList;
    List<Vector2> _uvs;
    List<Vector4> _tangents;
    //Dictionary<int, VerticalTan> VerticalInTriNum = new Dictionary<int, VerticalTan>();
    List<VerticalTan> vertTans = new List<VerticalTan>();

    const string matPath = "Materials/mat_earth";
    const string texturePath = "Textures/worldmap";

    void Start()
    {
        CreateEarth();
    }

    void CreateEarth()
    {

        horizontalNum = 360 / angleTt;
        verticalNum = 180 / angleFy;
        _verticlesList = new List<Vector3>();
        _trianglesList = new List<int>();
        _uvs = new List<Vector2>();
        _tangents = new List<Vector4>();

        GameObject newGO = new GameObject("Earth");
        newGO.transform.position = Vector3.zero;
        newGO.AddComponent<MeshFilter>();
        newGO.AddComponent<MeshRenderer>();

        Material mat = Resources.Load<Material>(matPath);
        Texture texEarth = Resources.Load<Texture>(texturePath);

        newGO.GetComponent<MeshRenderer>().material = mat;

        Mesh mesh = newGO.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        //创建顶点面片等
        CreateObj(ref _verticlesList, ref _trianglesList);
        CreateUv(ref _uvs);
        CreateTangent(ref _tangents);
        mesh.vertices = _verticlesList.ToArray();
        mesh.triangles = _trianglesList.ToArray();
        mesh.uv = _uvs.ToArray();
        mesh.tangents = _tangents.ToArray();
    }


    void CreateObj(ref List<Vector3> vList, ref List<int> tList)
    {
        for (int i =0; i < 360; i += angleFy)
        {
            for (int j = 0; j < 360; j += angleTt)
            {

                float x = r * Mathf.Sin(Deg2Rad(i)) * Mathf.Cos(Deg2Rad(j));
                float y = r * Mathf.Cos(Deg2Rad(i));
                float z = r * Mathf.Sin(Deg2Rad(i))* Mathf.Sin(Deg2Rad(j));
                Vector3 pos = new Vector3(x, y, z);
                vList.Add(pos);
                VerticalTan vert = new VerticalTan();
                vert.index = vList.Count - 1;
                vertTans.Add(vert);
                //Instantiate(go, pos, Quaternion.identity);
                //Debug.Log("pos:" + pos.ToString());
            }
        }

        for(int i = 0; i < verticalNum; i++)
        {
            for(int j = 0; j < horizontalNum; j++)
            {
                tList.Add(i * horizontalNum + j);
                tList.Add((i + 1) * horizontalNum + (j+1));
                tList.Add((i+1) * horizontalNum + j);
                tList.Add((i + 1) * horizontalNum +(j + 1));
                tList.Add(i * horizontalNum + j);
                tList.Add(i * horizontalNum + (j + 1));

               var vertTan =  vertTans.Find((item) =>
                {
                    return item.index == i * horizontalNum + j;
                });
                var vertTan1 = vertTans.Find((item) =>
                {
                    return item.index == (i + 1) * horizontalNum + (j + 1);
                });
                var vertTan2 = vertTans.Find((item) =>
                {
                    return item.index == (i + 1) * horizontalNum + j;
                });
                var vertTan3 = vertTans.Find((item) =>
                {
                    return item.index == (i + 1) * horizontalNum + (j + 1);
                });
                var vertTan4 = vertTans.Find((item) =>
                {
                    return item.index == i * horizontalNum + j;
                });
                var vertTan5 = vertTans.Find((item) =>
                {
                    return item.index == i * horizontalNum + (j + 1);
                });

                vertTan.VerticalInTriNum += 1;
                vertTan1.VerticalInTriNum += 1;
                vertTan2.VerticalInTriNum += 1;
                vertTan3.VerticalInTriNum += 1;
                vertTan4.VerticalInTriNum += 1;
                vertTan5.VerticalInTriNum += 1;
            }
        }
    }

    void CreateUv(ref List<Vector2> uvs)
    {
        //for(int i = 0;i<)
        int vertGroupNum = (180 / angleFy)+1;

        float verticalOffset = 1.0f / verticalNum;
        float horizontalOffset = 1.0f / horizontalNum;

        //Debug.Log("____vertarraycount:" + _verticlesList.Count);

        for(int i = 0; i < verticalNum*2; i++)
        {
            for(int j = 0; j < horizontalNum; j++)
            {
                uvs.Add(new Vector2(j * horizontalOffset, i*verticalOffset));
            }
        }
    }

    void CreateTangent(ref List<Vector4> tangents)
    {
        int triCount = _trianglesList.Count;
        for(int i = 0; i < triCount; i += 3)
        {
            Vector3 po1 = _verticlesList[_trianglesList[i]];
            Vector3 po2 = _verticlesList[_trianglesList[i + 1]];
            Vector3 po3 = _verticlesList[_trianglesList[i + 2]];

            Vector2 uv1 = _uvs[_trianglesList[i]];
            Vector2 uv2 = _uvs[_trianglesList[i+1]];
            Vector2 uv3 = _uvs[_trianglesList[i+2]];

            Vector3 edg1 = po2 - po1;
            Vector3 edg2 = po3 - po1;

            Vector2 deltaUv1 = uv2 - uv1;
            Vector2 deltaUv2 = uv3 - uv1;

            float ratio = 1 / (deltaUv1.x * deltaUv2.y - deltaUv1.y * deltaUv2.x);

            float x = ratio * (deltaUv2.y * edg1.x - deltaUv1.y * edg2.x);
            float y = ratio * (deltaUv2.y * edg1.y - deltaUv1.y * edg2.y);
            float z = ratio * (deltaUv2.y * edg1.z - deltaUv1.y * edg2.z);

            Vector4 tangent = Vector4.Normalize(new Vector4(x, y, z, 0));
            var vertTran = vertTans.Find((item) =>
            {
                return item.index == _trianglesList[i];
            });

            var vertTran1 = vertTans.Find((item) =>
            {
                return item.index == _trianglesList[i+1];
            });

            var vertTran2 = vertTans.Find((item) =>
            {
                return item.index == _trianglesList[i+2];
            });
            vertTran.Tangent += tangent;
            vertTran1.Tangent += tangent;
            vertTran2.Tangent += tangent;
        }
     
        foreach(var tan in vertTans)
        {
            tangents.Add(tan.Tangent / tan.VerticalInTriNum);
        }
    }

    float Deg2Rad(float angle)
    {
        return angle * Mathf.Deg2Rad;
    }
}
