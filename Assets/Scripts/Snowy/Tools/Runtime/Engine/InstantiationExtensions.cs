﻿using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Snowy.Engine
{
    public static class InstantiationExtensions
    {
        /// <summary>
        /// Instantiates unity object of defined type.
        /// </summary>
        public static T Install<T>(this T self) where T : UnityObject
        {
            return UnityObject.Instantiate(self);
        }

        /// <summary>
        /// Instantiates game object as a child of the specified parent.
        /// </summary>
        public static GameObject Install(this GameObject self, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates game object as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, Transform parent)
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates game object as a child of the specified parent.
        /// </summary>
        public static T Install<T>(this T self, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(self, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates defined component as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this T self, Transform parent) where T : Component
        {
            return UnityObject.Instantiate(self, parent, false);
        }

        /// <summary>
        /// Instantiates game object to the specified position.
        /// </summary>
        public static GameObject Install(this GameObject self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, self.transform.localRotation);
        }

        /// <summary>
        /// Instantiates game object to the specified position with the specified rotation.
        /// </summary>
        public static GameObject Install(this GameObject self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static Transform Install(this Transform self, in Vector3 position)
        {
            return UnityObject.Instantiate(self, position, self.localRotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position) where T : Component
        {
            return UnityObject.Instantiate(self, position, self.transform.localRotation);
        }

        /// <summary>
        /// Instantiates defined component to specified position with specified rotation.
        /// </summary>
        public static T Install<T>(this T self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self, position, rotation);
        }

        /// <summary>
        /// Instantiates game object as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this GameObject self, Transform parent, in Vector3 targetPos, in Quaternion targetRot, bool local)
        {
            if (local)
                return UnityObject.Instantiate(self, parent.TransformPoint(targetPos), parent.rotation * targetRot, parent);

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates object as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this T self, Transform parent, in Vector3 targetPos, in Quaternion targetRot, bool local) where T : Component
        {
            if (local)
                return UnityObject.Instantiate(self, parent.TransformPoint(targetPos), parent.rotation * targetRot, parent);

            return UnityObject.Instantiate(self, targetPos, targetRot, parent);
        }

        /// <summary>
        /// Instantiates asset.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self) where T : UnityObject
        {
            return UnityObject.Instantiate(self.asset);
        }

        /// <summary>
        /// Instantiates game object asset as a child of the specified parent.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent, bool worldPositionStays)
        {
            return UnityObject.Instantiate(self.asset, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates game object asset as a child with default local position and rotation.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent)
        {
            return UnityObject.Instantiate(self.asset, parent, false);
        }

        /// <summary>
        /// Instantiates game object asset as a child of the specified parent.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent, bool worldPositionStays) where T : Component
        {
            return UnityObject.Instantiate(self.asset, parent, worldPositionStays);
        }

        /// <summary>
        /// Instantiates asset as a child with default local position and rotation.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent) where T : Component
        {
            return UnityObject.Instantiate(self.asset, parent, false);
        }

        /// <summary>
        /// Instantiates game object asset to the specified position.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, in Vector3 position)
        {
            return Install(self.asset, position);
        }

        /// <summary>
        /// Instantiates game object asset to the specified position with the specified rotation.
        /// </summary>
        public static GameObject Install(this LazyLoadReference<GameObject> self, in Vector3 position, in Quaternion rotation)
        {
            return UnityObject.Instantiate(self.asset, position, rotation);
        }

        /// <summary>
        /// Instantiates asset to specified position.
        /// </summary>
        public static Transform Install(this LazyLoadReference<Transform> self, in Vector3 position)
        {
            return Install(self.asset, position);
        }

        /// <summary>
        /// Instantiates asset to specified position.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, in Vector3 position) where T : Component
        {
            return Install(self.asset, position);
        }

        /// <summary>
        /// Instantiates asset to specified position with specified rotation.
        /// </summary>
        public static T Install<T>(this LazyLoadReference<T> self, in Vector3 position, in Quaternion rotation) where T : Component
        {
            return UnityObject.Instantiate(self.asset, position, rotation);
        }

        /// <summary>
        /// Instantiates game object asset as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static GameObject Install(this LazyLoadReference<GameObject> self, Transform parent, in Vector3 targetPos, in Quaternion targetRot, bool local)
        {
            return Install(self.asset, parent, targetPos, targetRot, local);
        }

        /// <summary>
        /// Instantiates game object asset as a child with the specified position and rotation.
        /// </summary>
        /// <param name="local">If true targetPos and targetRot are considered as local, otherwise as world.</param>
        public static T Install<T>(this LazyLoadReference<T> self, Transform parent, in Vector3 targetPos, in Quaternion targetRot, bool local) where T : Component
        {
            return Install(self.asset, parent, targetPos, targetRot, local);
        }
    }
}
