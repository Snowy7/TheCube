using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Snowy.Utilities
{
    public class BoneRenammer : UnityEditor.EditorWindow
    {
        public Animator sourceAnimatorController;
        public Animator humanoidSourceCopy;
        public Animator targetAnimator;
        
        public GenericHumanoidRig sourceRig;
        
        [MenuItem("Snowy/Utilities/Bone Renamer")]
        private static void Init()
        {
            BoneRenammer window = (BoneRenammer)GetWindow(typeof(BoneRenammer));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This tool allows you to retarget animations from a generic rig to a humanoid rig.",
                MessageType.Info);

            if (GUILayout.Button("Rename Bones"))
            {
                if (sourceRig.hips == null)
                {
                    Debug.LogError("The source rig must have a hips bone.");
                    return;
                }

                if (sourceAnimatorController == null)
                {
                    Debug.LogError("The source animator controller must be set.");
                    return;
                }

                if (targetAnimator == null)
                {
                    Debug.LogError("The target animator must be set.");
                    return;
                }

                RenameBones();
            }

            if (GUILayout.Button("Auto Fill Source Rig"))
            {
                AutoFillSourceRig();
            }
            if (sourceRig.hips == null)
            {
                EditorGUILayout.HelpBox("The source rig must have a hips bone.", MessageType.Warning);
            }


            sourceAnimatorController = (Animator)EditorGUILayout.ObjectField("Source Animator Controller",
                sourceAnimatorController, typeof(Animator), true);

            humanoidSourceCopy = (Animator)EditorGUILayout.ObjectField("Humanoid Source Copy",
                humanoidSourceCopy, typeof(Animator), true);
            
            targetAnimator =
                (Animator)EditorGUILayout.ObjectField("Target Animator", targetAnimator, typeof(Animator), true);
        }

        private void RenameBones()
        {
            CreateBoneMapping();
        }

        private void AutoFillSourceRig()
        {
            sourceRig.hips = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.Hips));
            sourceRig.spine = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.Spine));
            sourceRig.lowerChest = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.Chest));
            sourceRig.upperChest = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.UpperChest));
            sourceRig.neck = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.Neck));
            sourceRig.head = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.Head));

            sourceRig.leftArm.shoulder =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftShoulder));
            sourceRig.leftArm.upper = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftUpperArm));
            sourceRig.leftArm.lower = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLowerArm));
            sourceRig.leftArm.hand = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftHand));

            sourceRig.rightArm.shoulder =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightShoulder));
            sourceRig.rightArm.upper =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightUpperArm));
            sourceRig.rightArm.lower =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightLowerArm));
            sourceRig.rightArm.hand = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightHand));

            sourceRig.leftHand.thumb.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
            sourceRig.leftHand.thumb.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
            sourceRig.leftHand.thumb.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbDistal));
            sourceRig.leftHand.index.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexProximal));
            sourceRig.leftHand.index.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate));
            sourceRig.leftHand.index.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexDistal));
            sourceRig.leftHand.middle.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));
            sourceRig.leftHand.middle.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate));
            sourceRig.leftHand.middle.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleDistal));
            sourceRig.leftHand.ring.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingProximal));
            sourceRig.leftHand.ring.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingIntermediate));
            sourceRig.leftHand.ring.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingDistal));
            sourceRig.leftHand.pinky.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleProximal));
            sourceRig.leftHand.pinky.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate));
            sourceRig.leftHand.pinky.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleDistal));

            sourceRig.rightHand.thumb.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbProximal));
            sourceRig.rightHand.thumb.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate));
            sourceRig.rightHand.thumb.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftThumbDistal));
            sourceRig.rightHand.index.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexProximal));
            sourceRig.rightHand.index.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate));
            sourceRig.rightHand.index.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftIndexDistal));
            sourceRig.rightHand.middle.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleProximal));
            sourceRig.rightHand.middle.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate));
            sourceRig.rightHand.middle.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftMiddleDistal));
            sourceRig.rightHand.ring.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingProximal));
            sourceRig.rightHand.ring.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingIntermediate));
            sourceRig.rightHand.ring.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftRingDistal));
            sourceRig.rightHand.pinky.proximal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleProximal));
            sourceRig.rightHand.pinky.intermediate =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate));
            sourceRig.rightHand.pinky.distal =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLittleDistal));

            sourceRig.leftLeg.thigh = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftUpperLeg));
            sourceRig.leftLeg.calf = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftLowerLeg));
            sourceRig.leftLeg.foot = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftFoot));
            sourceRig.leftLeg.toes = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.LeftToes));

            sourceRig.rightLeg.thigh =
                FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightUpperLeg));
            sourceRig.rightLeg.calf = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightLowerLeg));
            sourceRig.rightLeg.foot = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightFoot));
            sourceRig.rightLeg.toes = FindBoneByName(humanoidSourceCopy.GetBoneTransform(HumanBodyBones.RightToes));
        }

        private Transform FindBoneByName(Transform ogTransform)
        {
            string ogName = ogTransform.name;
            Transform[] bones = sourceAnimatorController.GetComponentsInChildren<Transform>();
            foreach (Transform bone in bones)
            {
                if (bone.name == ogName)
                {
                    return bone;
                }
            }

            return null;
        }


        private void CreateBoneMapping()
        {
            targetAnimator.GetBoneTransform(HumanBodyBones.Hips).name = sourceRig.hips.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.Spine).name = sourceRig.spine.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.Chest).name = sourceRig.lowerChest.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.UpperChest).name = sourceRig.upperChest.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.Neck).name = sourceRig.neck.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.Head).name = sourceRig.head.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder).name = sourceRig.leftArm.shoulder.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm).name = sourceRig.leftArm.upper.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm).name = sourceRig.leftArm.lower.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand).name = sourceRig.leftArm.hand.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.RightShoulder).name = sourceRig.rightArm.shoulder.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).name = sourceRig.rightArm.upper.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm).name = sourceRig.rightArm.lower.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightHand).name = sourceRig.rightArm.hand.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).name = sourceRig.leftLeg.thigh.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).name = sourceRig.leftLeg.calf.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).name = sourceRig.leftLeg.foot.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftToes).name = sourceRig.leftLeg.toes.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg).name = sourceRig.rightLeg.thigh.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg).name = sourceRig.rightLeg.calf.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightFoot).name = sourceRig.rightLeg.foot.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightToes).name = sourceRig.rightLeg.toes.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbProximal).name =
                sourceRig.leftHand.thumb.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate).name =
                sourceRig.leftHand.thumb.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).name = sourceRig.leftHand.thumb.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).name =
                sourceRig.leftHand.index.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).name =
                sourceRig.leftHand.index.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).name = sourceRig.leftHand.index.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).name =
                sourceRig.leftHand.middle.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate).name =
                sourceRig.leftHand.middle.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).name =
                sourceRig.leftHand.middle.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingProximal).name =
                sourceRig.leftHand.ring.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate).name =
                sourceRig.leftHand.ring.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingDistal).name = sourceRig.leftHand.ring.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLittleProximal).name =
                sourceRig.leftHand.pinky.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate).name =
                sourceRig.leftHand.pinky.intermediate.name;

            targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbProximal).name =
                sourceRig.rightHand.thumb.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).name =
                sourceRig.rightHand.thumb.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbDistal).name =
                sourceRig.rightHand.thumb.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexProximal).name =
                sourceRig.rightHand.index.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).name =
                sourceRig.rightHand.index.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexDistal).name =
                sourceRig.rightHand.index.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).name =
                sourceRig.rightHand.middle.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).name =
                sourceRig.rightHand.middle.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).name =
                sourceRig.rightHand.middle.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightRingProximal).name =
                sourceRig.rightHand.ring.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).name =
                sourceRig.rightHand.ring.intermediate.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightRingDistal).name = sourceRig.rightHand.ring.distal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLittleProximal).name =
                sourceRig.rightHand.pinky.proximal.name;
            targetAnimator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).name =
                sourceRig.rightHand.pinky.intermediate.name;
        }
    }
}