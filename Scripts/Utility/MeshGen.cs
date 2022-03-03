using Godot;

namespace CombatGame
{
    public static class MeshGen
    {
        public const int ARRAY_MAX = (int)Mesh.ArrayType.Max;

        public static ArrayMesh GenerateTiledPlane(int size, System.Func<int, int, Vector2> uvFunc1 = null, System.Func<int, int, Vector2> uvFunc2 = null)
        {
            Vector3[] vertices = new Vector3[size * size];
            Vector3[] normals = new Vector3[size * size];
            Vector2[] uvs = new Vector2[size * size];
            Vector2[] uvs2 = new Vector2[size * size];

            int triangleCount = (size-1) * (size-1) * 2;
            int[] triangles = new int[triangleCount * 3];

            int XYToIndex(int x, int y) => (y * size) + x;
            bool Valid(int coord) => coord >= 0 && coord < size;
            int triInd = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int vertexIndex = XYToIndex(x,y);
                    vertices[vertexIndex] = new Vector3(x, y, 0);
                    normals[vertexIndex] = -Vector3.Forward;
                    uvs[vertexIndex] = uvFunc1?.Invoke(x,y) ?? new Vector2(x,y) / new Vector2(size-1, size-1);
                    uvs2[vertexIndex] = uvFunc2?.Invoke(x,y) ?? default;
                    
                    //x,y; x+1,y; x,y+1
                    if (Valid(x+1) && Valid(y+1))
                    {
                        // triangles[triInd] = XYToIndex(x, y);
                        // triInd++;
                        // triangles[triInd] = XYToIndex(x+1, y);
                        // triInd++;
                        // triangles[triInd] = XYToIndex(x, y+1);
                        // triInd++;
                        triangles[triInd] = XYToIndex(x, y);
                        triInd++;
                        triangles[triInd] = XYToIndex(x, y+1);
                        triInd++;
                        triangles[triInd] = XYToIndex(x+1, y);
                        triInd++;

                    }
                    //x,y; x+1,y-1; x+1,y
                    if (Valid(x+1) && Valid(y-1))
                    {
                        // triangles[triInd] = XYToIndex(x, y);
                        // triInd++;
                        // triangles[triInd] = XYToIndex(x+1, y-1);
                        // triInd++;
                        // triangles[triInd] = XYToIndex(x+1, y);
                        // triInd++;
                        triangles[triInd] = XYToIndex(x, y);
                        triInd++;
                        triangles[triInd] = XYToIndex(x+1, y);
                        triInd++;
                        triangles[triInd] = XYToIndex(x+1, y-1);
                        triInd++;
                    }
                }
            }

            Godot.Collections.Array meshArray = new Godot.Collections.Array();
            meshArray.Resize(ARRAY_MAX);
            meshArray[(int)Mesh.ArrayType.Vertex] = vertices;
            meshArray[(int)Mesh.ArrayType.Index] = triangles;
            meshArray[(int)Mesh.ArrayType.Normal] = normals;
            meshArray[(int)Mesh.ArrayType.TexUv] = uvs;

            if (uvFunc2 != null){
                meshArray[(int)Mesh.ArrayType.TexUv2] = uvs2;
            }
            
            ArrayMesh mesh = new ArrayMesh();
            mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArray);
            return mesh;
        }
    }
}