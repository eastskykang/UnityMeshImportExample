using System;
using System.Collections.Generic;
using System.IO;
using Assimp;
using UnityEngine;
using Material = UnityEngine.Material;
using Mesh = UnityEngine.Mesh;

namespace UnityMeshImporter
{
    class MeshMaterialBinding
    {
        private string meshName;
        private UnityEngine.Mesh mesh;
        private UnityEngine.Material material;
        
        private MeshMaterialBinding() {}    // Do not allow default constructor

        public MeshMaterialBinding(string meshName, Mesh mesh, Material material)
        {
            this.meshName = meshName;
            this.mesh = mesh;
            this.material = material;
        }

        public Mesh Mesh { get => mesh; }
        public Material Material { get => material; }
        public string MeshName { get => meshName; }
    }
    
    public class MeshImporter
    {
        public static GameObject Load(string meshPath, float scaleX=1, float scaleY=1, float scaleZ=1)
        {
            if(!File.Exists(meshPath))
                return null;

            AssimpContext importer = new AssimpContext();
            Scene scene = importer.ImportFile(meshPath);
            if (scene == null)
                return null;
            
            string parentDir = Directory.GetParent(meshPath).FullName;

            // Materials
            List<UnityEngine.Material> uMaterials = new List<Material>();
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
                    
                    // Reflectivity
                    if (m.HasReflectivity)
                    {
                        uMaterial.SetFloat("_Glossiness", m.Reflectivity);
                    }
                    
                    // Texture
                    if (m.HasTextureDiffuse)
                    {
                        Texture2D uTexture = new Texture2D(2,2);
                        string texturePath = Path.Combine(parentDir, m.TextureDiffuse.FilePath);
                        
                        byte[] byteArray = File.ReadAllBytes(texturePath);
                        bool isLoaded = uTexture.LoadImage(byteArray);
                        if (!isLoaded)
                        {
                            throw new Exception("Cannot find texture file: " + texturePath);
                        }
                        
                        uMaterial.SetTexture("_MainTex", uTexture);
                    }

                    uMaterials.Add(uMaterial);
                }
            }

            // Mesh
            List<MeshMaterialBinding> uMeshAndMats = new List<MeshMaterialBinding>();
            if (scene.HasMeshes)
            {
                foreach (var m in scene.Meshes)
                {
                    List<Vector3> uVertices = new List<Vector3>();
                    List<Vector3> uNormals = new List<Vector3>();
                    List<Vector2> uUv = new List<Vector2>();
                    List<int> uIndices = new List<int>();
                
                    // Vertices
                    if (m.HasVertices)
                    {
                        foreach (var v in m.Vertices)
                        {
                            uVertices.Add(new Vector3(-v.X, v.Y, v.Z));
                        }
                    }

                    // Normals
                    if (m.HasNormals)
                    {
                        foreach (var n in m.Normals)
                        {
                            uNormals.Add(new Vector3(-n.X, n.Y, n.Z));
                        }
                    }

                    // Triangles
                    if (m.HasFaces)
                    {
                        foreach (var f in m.Faces)
                        {
                            // Ignore non-triangle faces
                            if (f.IndexCount != 3)
                                continue;

                            uIndices.Add(f.Indices[2]);
                            uIndices.Add(f.Indices[1]);
                            uIndices.Add(f.Indices[0]);
                        }
                    }

                    // Uv (texture coordinate) 
                    if (m.HasTextureCoords(0))
                    {
                        foreach (var uv in m.TextureCoordinateChannels[0])
                        {
                            uUv.Add(new Vector2(uv.X, uv.Y));
                        }
                    }
                
                    UnityEngine.Mesh uMesh = new UnityEngine.Mesh();
                    uMesh.vertices = uVertices.ToArray();
                    uMesh.normals = uNormals.ToArray();
                    uMesh.triangles = uIndices.ToArray();
                    uMesh.uv = uUv.ToArray();

                    uMeshAndMats.Add(new MeshMaterialBinding(m.Name, uMesh, uMaterials[m.MaterialIndex]));
                }
            }
            
            // Create GameObjects from nodes
            GameObject NodeToGameObject(Node node)
            {
                GameObject uOb = new GameObject(node.Name);
            
                // Set Mesh
                if (node.HasMeshes)
                {
                    foreach (var mIdx in node.MeshIndices)
                    {
                        var uMeshAndMat = uMeshAndMats[mIdx];
                        
                        GameObject uSubOb = new GameObject(uMeshAndMat.MeshName);
                        uSubOb.AddComponent<MeshFilter>();
                        uSubOb.AddComponent<MeshRenderer>();
                        uSubOb.AddComponent<MeshCollider>();
                    
                        uSubOb.GetComponent<MeshFilter>().mesh = uMeshAndMat.Mesh;
                        uSubOb.GetComponent<MeshRenderer>().material = uMeshAndMat.Material;
                        uSubOb.transform.SetParent(uOb.transform, true);
                        uSubOb.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                    }
                }
            
                // Transform
                UnityEngine.Matrix4x4 uTransform = new UnityEngine.Matrix4x4();
                uTransform.SetColumn(0, new Vector4(
                    node.Transform.A1, 
                    node.Transform.B1,
                    node.Transform.C1,
                    node.Transform.D1
                ));
                uTransform.SetColumn(1, new Vector4(
                    node.Transform.A2, 
                    node.Transform.B2,
                    node.Transform.C2,
                    node.Transform.D2
                ));
                uTransform.SetColumn(2, new Vector4(
                    node.Transform.A3, 
                    node.Transform.B3,
                    node.Transform.C3,
                    node.Transform.D3
                ));
                uTransform.SetColumn(3, new Vector4(
                    node.Transform.A4,
                    node.Transform.B4,
                    node.Transform.C4,
                    node.Transform.D4
                ));
                
                var euler = uTransform.rotation.eulerAngles;
                
                uOb.transform.localPosition = uTransform.GetColumn(3);
                uOb.transform.localRotation = UnityEngine.Quaternion.Euler(euler.x, -euler.y, euler.z);
//                uOb.transform.localRotation = UnityEngine.Quaternion.LookRotation(uTransform.GetColumn(2), uTransform.GetColumn(1));
            
                if (node.HasChildren)
                {
                    foreach (var cn in node.Children)
                    {
                        var uObChild = NodeToGameObject(cn);
                        uObChild.transform.SetParent(uOb.transform, false);
                    }
                }
            
                return uOb;
            }
            
            return NodeToGameObject(scene.RootNode);;
        }
    }
}