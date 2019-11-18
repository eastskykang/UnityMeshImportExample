using System.Collections.Generic;
using System.IO;
using Assimp;
using UnityEngine;
using Material = UnityEngine.Material;

namespace UnityMeshImporter
{
    public class MeshImporter
    {
        public static GameObject Load(string meshPath)
        {
            if(!File.Exists(meshPath))
                return null;
            
            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(meshPath);
            if (scene == null)
                return null;

            GameObject uOb = null;
            
            // material
            List<UnityEngine.Material> materialList = new List<Material>();

            if (scene.HasMaterials)
            {
                foreach (var m in scene.Materials)
                {
                    UnityEngine.Material uMaterial = new UnityEngine.Material(Shader.Find("Standard"));

                    // Albedo
                    if (m.HasColorDiffuse)
                    {
                        Color color = new Color(
                            m.ColorDiffuse.R,
                            m.ColorDiffuse.G,
                            m.ColorDiffuse.B,
                            m.ColorDiffuse.A
                        );
                        uMaterial.color = color;
                    }

                    // Emission
                    if (m.HasColorEmissive)
                    {
                        Color color = new Color(
                            m.ColorEmissive.R,
                            m.ColorEmissive.G,
                            m.ColorEmissive.B,
                            m.ColorEmissive.A
                        );
                        uMaterial.SetColor("_EmissionColor", color);
                        uMaterial.EnableKeyword("_EMISSION");
                    }

                    materialList.Add(m.uMaterial);
                }
            }

            // mesh
            if (scene.HasMeshes)
            {
                uOb = new GameObject();

                foreach (var m in scene.Meshes)
                {
                    List<Vector3> uVertices = new List<Vector3>();
                    List<Vector3> uNormals = new List<Vector3>();
                    List<int> uIndices = new List<int>();
                
                    // vertices
                    foreach (var v in m.Vertices)
                    {
                        uVertices.Add(new Vector3(v.X, v.Y, v.Z));
                    }
                
                    // normals
                    foreach (var n in m.Normals)
                    {
                        uNormals.Add(new Vector3(n.X, n.Y, n.Z));
                    }
                
                    // triangles
                    foreach (var f in m.Faces)
                    {
                        // ignore non-triangle faces
                        if (f.IndexCount != 3)
                            continue;
                    
                        uIndices.Add(f.Indices[0]);
                        uIndices.Add(f.Indices[1]);
                        uIndices.Add(f.Indices[2]);
                    }
                
                    UnityEngine.Mesh uMesh = new UnityEngine.Mesh();
                    uMesh.vertices = uVertices.ToArray();
                    uMesh.normals = uNormals.ToArray();
                    uMesh.triangles = uIndices.ToArray();
                
                    var uSubOb = new GameObject(m.Name);
                    uSubOb.AddComponent<MeshFilter>();
                    uSubOb.AddComponent<MeshRenderer>();
                    uSubOb.AddComponent<MeshCollider>();
                    
                    uSubOb.GetComponent<MeshFilter>().mesh = uMesh;
                    uSubOb.GetComponent<MeshRenderer>().material = materialList[m.MaterialIndex];
                    uSubOb.transform.SetParent(uOb.transform, true);
                }
            }

            return uOb;
        }
        
    }
}