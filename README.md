# UnityMeshImportExample

Runtime mesh import example for Unity.

**Update**: Tested on Linux, macOS and Windows.

## Dependencies

The following dependency is already included in the project. 

- [SimpleFileBrowser](https://assetstore.unity.com/packages/tools/gui/runtime-file-browser-113006)

The following dependency is imported as Unity package via [Packages/manifest.json](Packages/manifest.json).

- [UnityMeshImporter](https://github.com/eastskykang/UnityMeshImporter)

Note that UnityMeshImporter uses C# .NET wrapper for the Assimp, [AssimpNet](https://bitbucket.org/Starnick/assimpnet/src/master/) 

## Quickstart with Example

![quick-start-gif](Images/quickstart.gif)

This project has .obj and .dae (collada) mesh examples in ```Examples``` directory.

## Notes

UnityMeshImporter uses Unity Standard shader. Standard shader is added to ```Project Settings > Graphics > Built-in Shader Settings > Always Included Shaders```. 
