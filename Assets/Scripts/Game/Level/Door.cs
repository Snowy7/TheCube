using System;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Level
{
    [Serializable] struct AnimatedDoor
    {
        public Transform transform;
        public NavMeshObstacle navMeshObstacle;
        
        public Vector3 openPosition;
        public Vector3 closePosition;
        
        public Vector3 openRotation;
        public Vector3 closeRotation;
    }
    
    public class Door : MonoBehaviour
    {
        [Title("Door Settings")]
        [SerializeField] private bool isDoubleDoor;
        
        [Title("Door Animation")]
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private float animationDuration;
        [SerializeField, ShowIf(nameof(isDoubleDoor), false)] private AnimatedDoor doorTransform;
        [SerializeField, ShowIf(nameof(isDoubleDoor), true)] private AnimatedDoor leftDoorTransform;
        [SerializeField, ShowIf(nameof(isDoubleDoor), true)] private AnimatedDoor rightDoorTransform;
        
        [Title("Door State")]
        [SerializeField, Disable] public int doorId;
        [SerializeField, Disable] private bool isOpen;
        
        public bool IsOpen => isOpen;
        
        private void Awake()
        {
            if (isDoubleDoor)
            {
                leftDoorTransform.navMeshObstacle.carving = true;
                rightDoorTransform.navMeshObstacle.carving = true;
                
                leftDoorTransform.navMeshObstacle.enabled = false;
                rightDoorTransform.navMeshObstacle.enabled = false;
            }
            else
            {
                doorTransform.navMeshObstacle.carving = false;
                doorTransform.navMeshObstacle.enabled = false;
            }
        }
        
        public void Open()
        {
            if (isDoubleDoor) OpenDoubleDoor();
            else OpenSingleDoor();
            
            isOpen = true;
        }
        
        public void Close()
        {
            if (isDoubleDoor) CloseDoubleDoor();
            else CloseSingleDoor();
            isOpen = false;
        }
        
        private void OpenSingleDoor()
        {
            LeanTween.moveLocal(doorTransform.transform.gameObject, doorTransform.openPosition, animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(doorTransform.transform.gameObject, doorTransform.openRotation, animationDuration)
                .setEase(animationCurve);

            if (doorTransform.navMeshObstacle != null)
            {
                doorTransform.navMeshObstacle.carving = false;
                doorTransform.navMeshObstacle.enabled = false;
            }
        }
        
        private void CloseSingleDoor()
        {
            LeanTween.moveLocal(doorTransform.transform.gameObject, doorTransform.closePosition, animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(doorTransform.transform.gameObject, doorTransform.closeRotation, animationDuration)
                .setEase(animationCurve);

            if (doorTransform.navMeshObstacle != null)
            {
                doorTransform.navMeshObstacle.enabled = true;
                doorTransform.navMeshObstacle.carving = true;
            }
        }
        
        private void OpenDoubleDoor()
        {
            LeanTween.moveLocal(leftDoorTransform.transform.gameObject, leftDoorTransform.openPosition, animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(leftDoorTransform.transform.gameObject, leftDoorTransform.openRotation, animationDuration)
                .setEase(animationCurve);
            
            LeanTween.moveLocal(rightDoorTransform.transform.gameObject, rightDoorTransform.openPosition, animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(rightDoorTransform.transform.gameObject, rightDoorTransform.openRotation, animationDuration)
                .setEase(animationCurve);

            if (leftDoorTransform.navMeshObstacle != null)
            {
                leftDoorTransform.navMeshObstacle.carving = false;
                leftDoorTransform.navMeshObstacle.enabled = false;
            }
            
            if (rightDoorTransform.navMeshObstacle != null)
            {
                rightDoorTransform.navMeshObstacle.carving = false;
                rightDoorTransform.navMeshObstacle.enabled = false;
            }
        }

        private void CloseDoubleDoor()
        {
            LeanTween.moveLocal(leftDoorTransform.transform.gameObject, leftDoorTransform.closePosition,
                    animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(leftDoorTransform.transform.gameObject, leftDoorTransform.closeRotation, animationDuration)
                .setEase(animationCurve);

            LeanTween.moveLocal(rightDoorTransform.transform.gameObject, rightDoorTransform.closePosition,
                    animationDuration)
                .setEase(animationCurve);
            LeanTween.rotateLocal(rightDoorTransform.transform.gameObject, rightDoorTransform.closeRotation,
                    animationDuration)
                .setEase(animationCurve);

            if (leftDoorTransform.navMeshObstacle != null)
            {
                leftDoorTransform.navMeshObstacle.enabled = true;
                leftDoorTransform.navMeshObstacle.carving = true;
            }
            
            if (rightDoorTransform.navMeshObstacle != null)
            {
                rightDoorTransform.navMeshObstacle.enabled = true;
                rightDoorTransform.navMeshObstacle.carving = true;
            }
        }
    }
}