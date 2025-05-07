using System;
using UnityEngine;

namespace Plugins.Snowy.SnLevelDesigner
{
    [ExecuteAlways]
    public class SnLevelDesigner : MonoBehaviour
    {
        private bool toolActive;
        [SerializeField] public SnTileSet tileSet;
        [Space(15)] 
        [SerializeField] public float tileSize = 2f;
        [SerializeField] public bool spawnWalls;
        [SerializeField] public bool spawnFloors;
        [SerializeField] public bool spawnPillars;

        private Vector3 lastHandlePos;
        private Vector3 snapLastHandlePos;
        private Vector3 handlePos;
        private Vector3 lastTileDelta;
        private Vector3 tileDelta;
        private GameObject geometryRoot;
        Color gizmoColor = Color.red;

        private void OnEnable()
        {
            snapLastHandlePos = transform.position;
        }

        private void Start()
        {
            snapLastHandlePos = transform.position;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                Vector3 snappedPos = new Vector3(Mathf.Ceil(transform.position.x), Mathf.Ceil(transform.position.y), Mathf.Ceil(transform.position.z));
                transform.position = snappedPos;

                if (snappedPos.x % 2 == 0 && snappedPos.x % 2 == 0)
                {
                    transform.position = snappedPos;
                    
                    lastHandlePos = handlePos;
                    handlePos = transform.position;
                    HandleCallback();
                    transform.hasChanged = false;
                    snapLastHandlePos = handlePos;
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        void HandleCallback()
        {
            if (toolActive)
            {
                lastTileDelta = tileDelta;
                tileDelta = lastHandlePos - handlePos;
                tileDelta /= tileSize;

                if (tileDelta.magnitude > 0f)
                {
                    if (Mathf.Approximately(handlePos.y, lastHandlePos.y))
                    {
                        if (spawnFloors)
                        {
                            CreateInstance(tileSet.floorTile, transform.position,
                                Quaternion.LookRotation(Vector3.right, Vector3.up));
                        }
                        
                        if (spawnWalls)
                        {
                            CreateInstance(tileSet.wallTile, transform.position,
                                Quaternion.LookRotation(tileDelta, Vector3.up));
                        }
                    }

                    if (spawnPillars)
                    {
                        CreateInstance(tileSet.pillarTitle, transform.position,
                            Quaternion.identity);
                    }
                    else
                    {
                        if (tileDelta != lastTileDelta && spawnWalls)
                        {
                            CreateInstance(tileSet.wallTile, lastHandlePos,
                                Quaternion.identity);
                        }
                    }
                }
            }
        }

        public void CreateInstance(SnTileComponent instanceType, Vector3 spawnPos, Quaternion spawnRot)
        {
            # if UNITY_EDITOR
            SnTileComponent instance = UnityEditor.PrefabUtility.InstantiatePrefab(instanceType) as SnTileComponent;
            if (instance != null)
            {
                instance.transform.position = spawnPos;
                instance.transform.rotation = spawnRot;
                instance.transform.localScale = Vector3.one;
                instance.tileSet = tileSet;
                instance.transform.SetParent(geometryRoot.transform);
                instance.gameObject.name = instanceType.name;
            }
            # endif
        }
        
        public void ToggleTool()
        {
            toolActive = !toolActive;
            gizmoColor = toolActive ? Color.green : Color.red;
        }
        
        
        public void SetGeometryRoot(GameObject root)
        {
            geometryRoot = root;
        }
    }
}