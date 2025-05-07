using Snowy.Inspector;
using UnityEngine;

namespace Inventory
{
    public class WeaponBaker : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private Transform reference;
        [SerializeField] private string stateName;
        [SerializeField] Animator animator;
        
        # if UNITY_EDITOR
        [InspectorButton("Bake Transform")]
        public void BakeTransform()
        {
            // SET THE ANIMATOR TO RUN THE WEAPON ANIMATION FIRST FRAME
            if (animator)
            {
                animator.Play(stateName, 0, 0);
                animator.Update(0);
            }
            
            if (!weapon || !reference) return;
            
            // spawn the weapon
            var spawned = Instantiate(weapon, transform.position, transform.rotation, reference);
            weapon.SetBakesData(spawned.transform.localPosition, spawned.transform.localEulerAngles);
            
            # if UNITY_EDITOR
            
            // Save the editor
            UnityEditor.EditorUtility.SetDirty(weapon);
            // refresh the editor
            UnityEditor.AssetDatabase.Refresh();
            
            # endif
            
            
            // destroy the spawned weapon
            DestroyImmediate(spawned.gameObject);
        }
        
        # endif
    }
}