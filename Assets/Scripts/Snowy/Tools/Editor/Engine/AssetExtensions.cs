﻿using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace SnowyEditor.Engine
{
    public static class AssetExtensions
    {
        public static bool IsFolder(this UnityObject self)
        {
            return AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(self));
        }

        public static bool IsNativeAsset(this UnityObject self)
        {
            return AssetDatabase.IsNativeAsset(self);
        }

        public static string GetAssetPath(this UnityObject self)
        {
            return AssetDatabase.GetAssetPath(self);
        }

        public static string GetAssetGuid(this UnityObject self)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(self, out string guid, out long _);
            return guid;
        }

        public static Texture2D GetAssetIcon(this UnityObject self)
        {
            return AssetPreview.GetMiniThumbnail(self);
        }
    }
}
