using UnityEngine;

namespace Utilities
{
/// <summary>
    /// Array Utilities.
    /// </summary>
    public static class UtilitiesArrays
    {
        /// <summary>
        /// Returns true if the array contains this index.
        /// </summary>
        public static bool IsValidIndex<T>(this T[] array, int index) => array.Length > index && index >= 0;
        /// <summary>
        /// Returns true if the array is valid.
        /// </summary>
        public static bool IsValid<T>(this T[] array) => !array.Equals(null) && array.Length > 0;
        /// <summary>
        /// Returns a random audio clip from an array of clips.
        /// </summary>
        public static T GetRandom<T>(this T[] array) => array[Random.Range(0, array.Length)];
        
        /// <summary>
        /// Enables one object, disables all others.
        /// </summary>
        public static T SelectAndSetActive<T>(this T[] array, int index) where T : MonoBehaviour
        {
            //Make sure we have objects in the array! If we don't, we could get an error or a crash here.
            if (!array.IsValid()) 
                return null;
            
            //Deactivate All. This way we don't have to do it manually.
            array.ForEach(obj => obj.gameObject.SetActive(false));

            //Error Check.
            if (!array.IsValidIndex(index)) 
                return null;
                
            //Activate.
            T behaviour = array[index];
            if(behaviour != null)
                behaviour.gameObject.SetActive(true);

            //Return.
            return behaviour;
        }
        
        /// <summary>
        /// Instantiate one object, using position and rotation.
        /// </summary>
        public static T SelectAndInstantiate<T>(this T[] array, int index, Vector3 position, Quaternion rotation) where T : MonoBehaviour
        {
            //Make sure we have objects in the array! If we don't, we could get an error or a crash here.
            if (!array.IsValid()) 
                return null;
            
            //Error Check.
            if (!array.IsValidIndex(index)) 
                return null;
                
            //Instantiate.
            T behaviour = Object.Instantiate(array[index], position, rotation);
            if(behaviour != null)
                behaviour.gameObject.SetActive(true);

            //Return.
            return behaviour;
        }
        
        /// <summary>
        /// Instantiate one object, using Transform
        /// </summary>
        public static T SelectAndInstantiate<T>(this T[] array, int index, Transform parent) where T : MonoBehaviour
        {
            //Make sure we have objects in the array! If we don't, we could get an error or a crash here.
            if (!array.IsValid()) 
                return null;
            
            //Error Check.
            if (!array.IsValidIndex(index)) 
                return null;
                
            //Instantiate.
            T behaviour = Object.Instantiate(array[index], parent);
            if(behaviour != null)
                behaviour.gameObject.SetActive(true);

            //Return.
            return behaviour;
        }
    }
}