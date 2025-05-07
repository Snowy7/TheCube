using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Snowy.Utilities
{
    public class MultipleImportMaterialEditor : EditorWindow
    {
        // Multiple imported models editor
        [SerializeField] private List<ModelImporter> importers;
        [SerializeField] private ModelImporterMaterialImportMode materialImportMode;
        
        
        [MenuItem("Snowy/Utilities/Multiple Material Editor")]
        private static void ShowWindow()
        {
            GetWindow<MultipleImportMaterialEditor>("Multiple Material Editor");
        }

        private void OnGUI()
        {
            // Select multiple importers
            // sHOW AN ARRAY OF IMPORTERS
            if (GUILayout.Button("Select multiple importers"))
            {
                var objects = Selection.GetFiltered<GameObject>(SelectionMode.Assets);
                importers = new List<ModelImporter>();
                foreach (var obj in objects)
                {
                    var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(obj)) as ModelImporter;
                    if (importer != null)
                    {
                        importers.Add(importer);
                    }
                }
            }
            
            materialImportMode = (ModelImporterMaterialImportMode) EditorGUILayout.EnumPopup("Material Import Mode", materialImportMode);
            
            // Show selected importers count
            EditorGUILayout.LabelField("Selected importers count: " + (importers?.Count ?? 0));
            
            if (GUILayout.Button("Apply"))
            {
                if (importers != null)
                    foreach (var importer in importers)
                    {
                        var modelImporter = importer;
                        if (modelImporter != null)
                        {
                            modelImporter.materialImportMode = materialImportMode;
                            modelImporter.SaveAndReimport();
                        }
                    }
            }
        }
    }
}