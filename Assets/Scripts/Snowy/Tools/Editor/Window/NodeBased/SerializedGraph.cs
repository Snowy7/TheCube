﻿using System;
using System.Runtime.CompilerServices;
using Snowy;
using Snowy.CSharp;
using Snowy.IdGenerating;
using Snowy.Mathematics;
using Snowy.NodeBased.Service;
using SnowyEditor.Engine;
using SnowyEditor.Window.NodeBased.NodeDrawing;
using UnityEditor;
using UnityEngine;

namespace SnowyEditor.Window.NodeBased
{
    internal class SerializedGraph
    {
        private const float MIN_NODE_WIDTH = 200f;
        private const float MAX_NODE_WIDTH = 400f;
        private const float NODE_WIDTH_STEP = 1f;

        private RawGraph _graphAsset;
        private SerializedObject _serializedObject;
        private SerializedProperty _nodesProperty;
        private SerializedProperty _rootNodeProperty;
        private SerializedProperty _commonNodeProperty;

        private float _nodeWidth;
        private IntIdGenerator _idGenerator;

        public RawGraph GraphAsset => _graphAsset;
        public SerializedObject SerializedObject => _serializedObject;
        public SerializedProperty NodesProperty => _nodesProperty;
        public SerializedProperty RootNodeProperty => _rootNodeProperty;
        public SerializedProperty CommonNodeProperty => _commonNodeProperty;
        public float NodeWidth => _nodeWidth;

        public SerializedGraph(RawGraph graphAsset)
        {
            InitAssetReference(graphAsset);
        }

        public void InitAssetReference(RawGraph graphAsset)
        {
            _graphAsset = graphAsset;
            _serializedObject = new SerializedObject(graphAsset);
            _idGenerator = new IntIdGenerator(_serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue);
            _nodesProperty = _serializedObject.FindProperty(RawGraph.NodesFieldName);
            _rootNodeProperty = _serializedObject.FindProperty(RawGraph.RootNodeFieldName);
            _commonNodeProperty = _serializedObject.FindProperty(RawGraph.CommonNodeFieldName);
            _nodeWidth = _serializedObject.FindProperty(RawGraph.WidthFieldName).floatValue.Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
        }

        public void ChangeNodeWidth(int dir)
        {
            _nodeWidth = (_nodeWidth + (NODE_WIDTH_STEP * dir)).Clamp(MIN_NODE_WIDTH, MAX_NODE_WIDTH);
        }

        public void SetAsRoot(NodeViewer node)
        {
            _serializedObject.FindProperty(RawGraph.RootNodeFieldName).intValue = node.Id;
        }

        public SerializedProperty CreateNode(Vector2 position, Type type)
        {
            NodeType nodeType = GraphUtility.GetNodeType(type);

            RawNode newNodeAsset = Activator.CreateInstance(type) as RawNode;

            newNodeAsset.Id = _idGenerator.GetNewId();
            newNodeAsset.Owner = _graphAsset;
            newNodeAsset.Position = position;
            newNodeAsset.NodeName = GetDefaultNodeName(type, newNodeAsset.Id);

            _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = newNodeAsset.Id;

            SerializedProperty nodeProp = nodeType == NodeType.Common ? _commonNodeProperty : _nodesProperty.AddArrayElement();
            nodeProp.managedReferenceValue = newNodeAsset;

            SerializedProperty rootNodeIdProp = _serializedObject.FindProperty(RawGraph.RootNodeFieldName);
            if (rootNodeIdProp.intValue == 0 && nodeType.IsRegular())
                rootNodeIdProp.intValue = newNodeAsset.Id;

            return nodeProp;
        }

        public SerializedProperty CloneNode(Vector2 position, int sourceNodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(NodeViewer node)
        {
            if (node.Type == NodeType.Common)
            {
                _commonNodeProperty.managedReferenceValue = null;
                return;
            }

            int index = _nodesProperty.GetArrayElement(out var deletedNode, item => item.FindPropertyRelative(RawNode.IdFieldName).intValue == node.Id);
            deletedNode.managedReferenceValue = null;
            _nodesProperty.DeleteArrayElementAtIndex(index);

            if (_nodesProperty.arraySize == 0)
            {
                _serializedObject.FindProperty(RawGraph.RootNodeFieldName).intValue = 0;
                _idGenerator = new IntIdGenerator();
                _serializedObject.FindProperty(RawGraph.IdGeneratorFieldName).intValue = 0;
            }
            else
            {
                SerializedProperty rootNodeIdProp = _serializedObject.FindProperty(RawGraph.RootNodeFieldName);
                if (node.Id == rootNodeIdProp.intValue)
                {
                    bool replaced = false;

                    foreach (SerializedProperty item in _nodesProperty.EnumerateArrayElements())
                    {
                        Type type = EditorUtilityExt.GetTypeFromSerializedPropertyTypename(item.managedReferenceFullTypename);
                        if (GraphUtility.GetNodeType(type).IsRegular())
                        {
                            rootNodeIdProp.intValue = item.FindPropertyRelative(RawNode.IdFieldName).intValue;
                            replaced = true;
                            break;
                        }
                    }

                    if (!replaced)
                        rootNodeIdProp.intValue = 0;
                }
            }
        }

        public void Save()
        {
            _serializedObject.FindProperty(RawGraph.WidthFieldName).floatValue = _nodeWidth;
        }

        public static string GetDefaultNodeName(Type type, int id)
        {
            NodeType nodeType = GraphUtility.GetNodeType(type);

            switch (nodeType)
            {
                case NodeType.Regular: return $"{type.Name} {id}";
                case NodeType.Hub: return $"{nodeType.GetName()} {id}";
                case NodeType.Common: return "Any";
                case NodeType.Exit: return nodeType.GetName();
                default: throw new SwitchExpressionException(nodeType);
            }
        }
    }
}
