using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Snowy.Utilities
{
    /// <summary>
    /// Allows the user to retarget animations from a generic rig to a humanoid rig.
    /// By mapping the bones of the generic rig to the bones of the humanoid rig, the user can retarget animations from the generic rig to the humanoid rig.
    /// </summary>
    public class AnimationRetargetingWindow : EditorWindow
    {
        // Generic rig
        public GenericHumanoidRig sourceRig;
        public Animator sourceAnimatorController;
        public Animator humanoidSourceCopy;
        public HumanTemplate template;
        public Animator targetAnimator;
        public string animationsSavePath;

        public Dictionary<string, string> boneMappings = new Dictionary<string, string>();

        private Vector2 scrollPosition;


        [MenuItem("Snowy/Utilities/Animation Retargeting (Beta)")]
        private static void OpenWindow()
        {
            GetWindow<AnimationRetargetingWindow>("Animation Retargeting");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("This tool allows you to retarget animations from a generic rig to a humanoid rig.",
                MessageType.Info);

            if (GUILayout.Button("Retarget Animations"))
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

                RetargetAnimations();
            }

            if (GUILayout.Button("Auto Fill Source Rig"))
            {
                AutoFillSourceRig();
            }

            if (GUILayout.Button("Debug the mappings"))
            {
                CreateBoneMapping();
                foreach (var mapping in boneMappings)
                {
                    Debug.Log(mapping.Key + " -> " + mapping.Value);
                }
            }

            if (sourceRig.hips == null)
            {
                EditorGUILayout.HelpBox("The source rig must have a hips bone.", MessageType.Warning);
            }

            // scroll view
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            sourceAnimatorController = (Animator)EditorGUILayout.ObjectField("Source Animator Controller",
                sourceAnimatorController, typeof(Animator), true);

            humanoidSourceCopy = (Animator)EditorGUILayout.ObjectField("Humanoid Source Copy",
                humanoidSourceCopy, typeof(Animator), true);

            template = (HumanTemplate)EditorGUILayout.ObjectField("Human Template",
                template, typeof(HumanTemplate), true);

            targetAnimator =
                (Animator)EditorGUILayout.ObjectField("Target Animator", targetAnimator, typeof(Animator), true);

            // SPACE
            EditorGUILayout.Space();

            DrawSourceRig();


            EditorGUILayout.EndScrollView();
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

        private void DrawSourceRig()
        {
            EditorGUILayout.LabelField("Source Rig", EditorStyles.boldLabel);

            sourceRig.hips = (Transform)EditorGUILayout.ObjectField("Hips", sourceRig.hips, typeof(Transform), true);
            sourceRig.spine = (Transform)EditorGUILayout.ObjectField("Spine", sourceRig.spine, typeof(Transform), true);
            sourceRig.lowerChest =
                (Transform)EditorGUILayout.ObjectField("Lower Chest", sourceRig.lowerChest, typeof(Transform), true);
            sourceRig.upperChest =
                (Transform)EditorGUILayout.ObjectField("Upper Chest", sourceRig.upperChest, typeof(Transform), true);
            sourceRig.neck = (Transform)EditorGUILayout.ObjectField("Neck", sourceRig.neck, typeof(Transform), true);
            sourceRig.head = (Transform)EditorGUILayout.ObjectField("Head", sourceRig.head, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Left Arm", EditorStyles.boldLabel);
            sourceRig.leftArm.shoulder =
                (Transform)EditorGUILayout.ObjectField("Shoulder", sourceRig.leftArm.shoulder, typeof(Transform), true);
            sourceRig.leftArm.upper =
                (Transform)EditorGUILayout.ObjectField("Upper Arm", sourceRig.leftArm.upper, typeof(Transform), true);
            sourceRig.leftArm.lower =
                (Transform)EditorGUILayout.ObjectField("Lower Arm", sourceRig.leftArm.lower, typeof(Transform), true);
            sourceRig.leftArm.hand =
                (Transform)EditorGUILayout.ObjectField("Hand", sourceRig.leftArm.hand, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Right Arm", EditorStyles.boldLabel);
            sourceRig.rightArm.shoulder =
                (Transform)EditorGUILayout.ObjectField("Shoulder", sourceRig.rightArm.shoulder, typeof(Transform),
                    true);
            sourceRig.rightArm.upper =
                (Transform)EditorGUILayout.ObjectField("Upper Arm", sourceRig.rightArm.upper, typeof(Transform), true);
            sourceRig.rightArm.lower =
                (Transform)EditorGUILayout.ObjectField("Lower Arm", sourceRig.rightArm.lower, typeof(Transform), true);
            sourceRig.rightArm.hand =
                (Transform)EditorGUILayout.ObjectField("Hand", sourceRig.rightArm.hand, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Left Hand", EditorStyles.boldLabel);
            sourceRig.leftHand.thumb.proximal = (Transform)EditorGUILayout.ObjectField("Thumb Proximal",
                sourceRig.leftHand.thumb.proximal, typeof(Transform), true);
            sourceRig.leftHand.thumb.intermediate = (Transform)EditorGUILayout.ObjectField("Thumb Intermediate",
                sourceRig.leftHand.thumb.intermediate, typeof(Transform), true);
            sourceRig.leftHand.thumb.distal = (Transform)EditorGUILayout.ObjectField("Thumb Distal",
                sourceRig.leftHand.thumb.distal, typeof(Transform), true);
            sourceRig.leftHand.index.proximal = (Transform)EditorGUILayout.ObjectField("Index Proximal",
                sourceRig.leftHand.index.proximal, typeof(Transform), true);
            sourceRig.leftHand.index.intermediate = (Transform)EditorGUILayout.ObjectField("Index Intermediate",
                sourceRig.leftHand.index.intermediate, typeof(Transform), true);
            sourceRig.leftHand.index.distal = (Transform)EditorGUILayout.ObjectField("Index Distal",
                sourceRig.leftHand.index.distal, typeof(Transform), true);
            sourceRig.leftHand.middle.proximal = (Transform)EditorGUILayout.ObjectField("Middle Proximal",
                sourceRig.leftHand.middle.proximal, typeof(Transform), true);
            sourceRig.leftHand.middle.intermediate = (Transform)EditorGUILayout.ObjectField("Middle Intermediate",
                sourceRig.leftHand.middle.intermediate, typeof(Transform), true);
            sourceRig.leftHand.middle.distal = (Transform)EditorGUILayout.ObjectField("Middle Distal",
                sourceRig.leftHand.middle.distal, typeof(Transform), true);
            sourceRig.leftHand.ring.proximal = (Transform)EditorGUILayout.ObjectField("Ring Proximal",
                sourceRig.leftHand.ring.proximal, typeof(Transform), true);
            sourceRig.leftHand.ring.intermediate = (Transform)EditorGUILayout.ObjectField("Ring Intermediate",
                sourceRig.leftHand.ring.intermediate, typeof(Transform), true);
            sourceRig.leftHand.ring.distal = (Transform)EditorGUILayout.ObjectField("Ring Distal",
                sourceRig.leftHand.ring.distal, typeof(Transform), true);
            sourceRig.leftHand.pinky.proximal = (Transform)EditorGUILayout.ObjectField("Pinky Proximal",
                sourceRig.leftHand.pinky.proximal, typeof(Transform), true);
            sourceRig.leftHand.pinky.intermediate = (Transform)EditorGUILayout.ObjectField("Pinky Intermediate",
                sourceRig.leftHand.pinky.intermediate, typeof(Transform), true);
            sourceRig.leftHand.pinky.distal = (Transform)EditorGUILayout.ObjectField("Pinky Distal",
                sourceRig.leftHand.pinky.distal, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Right Hand", EditorStyles.boldLabel);
            sourceRig.rightHand.thumb.proximal = (Transform)EditorGUILayout.ObjectField("Thumb Proximal",
                sourceRig.rightHand.thumb.proximal, typeof(Transform), true);
            sourceRig.rightHand.thumb.intermediate = (Transform)EditorGUILayout.ObjectField("Thumb Intermediate",
                sourceRig.rightHand.thumb.intermediate, typeof(Transform), true);
            sourceRig.rightHand.thumb.distal = (Transform)EditorGUILayout.ObjectField("Thumb Distal",
                sourceRig.rightHand.thumb.distal, typeof(Transform), true);
            sourceRig.rightHand.index.proximal = (Transform)EditorGUILayout.ObjectField("Index Proximal",
                sourceRig.rightHand.index.proximal, typeof(Transform), true);
            sourceRig.rightHand.index.intermediate = (Transform)EditorGUILayout.ObjectField("Index Intermediate",
                sourceRig.rightHand.index.intermediate, typeof(Transform), true);
            sourceRig.rightHand.index.distal = (Transform)EditorGUILayout.ObjectField("Index Distal",
                sourceRig.rightHand.index.distal, typeof(Transform), true);
            sourceRig.rightHand.middle.proximal = (Transform)EditorGUILayout.ObjectField("Middle Proximal",
                sourceRig.rightHand.middle.proximal, typeof(Transform), true);
            sourceRig.rightHand.middle.intermediate = (Transform)EditorGUILayout.ObjectField("Middle Intermediate",
                sourceRig.rightHand.middle.intermediate, typeof(Transform), true);
            sourceRig.rightHand.middle.distal = (Transform)EditorGUILayout.ObjectField("Middle Distal",
                sourceRig.rightHand.middle.distal, typeof(Transform), true);
            sourceRig.rightHand.ring.proximal = (Transform)EditorGUILayout.ObjectField("Ring Proximal",
                sourceRig.rightHand.ring.proximal, typeof(Transform), true);
            sourceRig.rightHand.ring.intermediate = (Transform)EditorGUILayout.ObjectField("Ring Intermediate",
                sourceRig.rightHand.ring.intermediate, typeof(Transform), true);
            sourceRig.rightHand.ring.distal = (Transform)EditorGUILayout.ObjectField("Ring Distal",
                sourceRig.rightHand.ring.distal, typeof(Transform), true);
            sourceRig.rightHand.pinky.proximal = (Transform)EditorGUILayout.ObjectField("Pinky Proximal",
                sourceRig.rightHand.pinky.proximal, typeof(Transform), true);
            sourceRig.rightHand.pinky.intermediate = (Transform)EditorGUILayout.ObjectField("Pinky Intermediate",
                sourceRig.rightHand.pinky.intermediate, typeof(Transform), true);
            sourceRig.rightHand.pinky.distal = (Transform)EditorGUILayout.ObjectField("Pinky Distal",
                sourceRig.rightHand.pinky.distal, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Left Leg", EditorStyles.boldLabel);
            sourceRig.leftLeg.thigh =
                (Transform)EditorGUILayout.ObjectField("Thigh", sourceRig.leftLeg.thigh, typeof(Transform), true);
            sourceRig.leftLeg.calf =
                (Transform)EditorGUILayout.ObjectField("Calf", sourceRig.leftLeg.calf, typeof(Transform), true);
            sourceRig.leftLeg.foot =
                (Transform)EditorGUILayout.ObjectField("Foot", sourceRig.leftLeg.foot, typeof(Transform), true);
            sourceRig.leftLeg.toes =
                (Transform)EditorGUILayout.ObjectField("Toes", sourceRig.leftLeg.toes, typeof(Transform), true);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Right Leg", EditorStyles.boldLabel);
            sourceRig.rightLeg.thigh =
                (Transform)EditorGUILayout.ObjectField("Thigh", sourceRig.rightLeg.thigh, typeof(Transform), true);
            sourceRig.rightLeg.calf =
                (Transform)EditorGUILayout.ObjectField("Calf", sourceRig.rightLeg.calf, typeof(Transform), true);
            sourceRig.rightLeg.foot =
                (Transform)EditorGUILayout.ObjectField("Foot", sourceRig.rightLeg.foot, typeof(Transform), true);
            sourceRig.rightLeg.toes =
                (Transform)EditorGUILayout.ObjectField("Toes", sourceRig.rightLeg.toes, typeof(Transform), true);
        }

        private void RetargetAnimations()
        {
            // Create a bone mapping
            boneMappings.Clear();
            CreateBoneMapping();

            // Ask for a place to save the animations
            animationsSavePath = EditorUtility.SaveFolderPanel("Save animations", "", "");
            if (string.IsNullOrEmpty(animationsSavePath))
            {
                return;
            }

            // Make the save path relative to the assets folder
            animationsSavePath = "Assets" + animationsSavePath.Substring(Application.dataPath.Length);

            Debug.Log("Saving animations to: " + animationsSavePath);


            // Get all clips from the source animator controller that we want to retarget
            AnimationClip[] sourceClips = sourceAnimatorController.runtimeAnimatorController.animationClips;
            AnimationClip[] targetClips = new AnimationClip[sourceClips.Length];

            // run through all clips and copy the position of all the bones from the source rig to the target rig
            for (var i = 0; i < sourceClips.Length; i++)
            {
                var clip = sourceClips[i];
                // Create a new animation clip
                AnimationClip newClip = new AnimationClip();
                newClip.name = clip.name;
                newClip.frameRate = clip.frameRate;

                // Get all the curves from the source clip
                EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
                foreach (EditorCurveBinding curveBinding in curveBindings)
                {
                    // Get the curve from the source clip
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, curveBinding);

                    // Get the bone name from the curve binding
                    string bonePath = curveBinding.path;

                    // Create a new curve binding for the target rig
                    EditorCurveBinding newCurveBinding = new EditorCurveBinding
                    {
                        path = TranslatePath(bonePath),
                        type = typeof(Transform),
                        propertyName = curveBinding.propertyName.Replace("m_LocalPosition", "localPosition")
                            .Replace("m_LocalRotation", "localRotation")
                            .Replace("m_LocalScale", "localScale")
                    };

                    // Add the curve to the new clip
                    Debug.Log("Adding curve: " + newCurveBinding.path + "/" + curveBinding.propertyName);
                    AnimationUtility.SetEditorCurve(newClip, newCurveBinding, curve);
                }

                // Save the new clip
                AssetDatabase.CreateAsset(newClip, animationsSavePath + "/" + newClip.name + ".anim");

                // Add the new clip to the target clips
                targetClips[i] =
                    AssetDatabase.LoadAssetAtPath<AnimationClip>(animationsSavePath + "/" + newClip.name + ".anim");
            }

            // Create a new animator controller
            AnimatorController targetAnimatorController = new AnimatorController();
            AssetDatabase.CreateAsset(targetAnimatorController, animationsSavePath + "/AnimatorController.controller");
            targetAnimatorController.AddLayer("Base Layer");
            AnimatorStateMachine stateMachine = targetAnimatorController.layers[0].stateMachine;

            // Add the clips to the animator controller
            foreach (AnimationClip clip in targetClips)
            {
                AnimatorState state = stateMachine.AddState(clip.name);
                state.motion = clip;
            }

            // Save the animator controller
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Set the new animator controller to the target animator
            targetAnimator.runtimeAnimatorController = targetAnimatorController;
        }

        private void CreateBoneMapping()
        {
            boneMappings.Clear();
            boneMappings.Add(sourceRig.hips.name, targetAnimator.GetBoneTransform(HumanBodyBones.Hips).name);
            boneMappings.Add(sourceRig.spine.name, targetAnimator.GetBoneTransform(HumanBodyBones.Spine).name);
            boneMappings.Add(sourceRig.lowerChest.name, targetAnimator.GetBoneTransform(HumanBodyBones.Chest).name);
            boneMappings.Add(sourceRig.upperChest.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.UpperChest).name);
            boneMappings.Add(sourceRig.neck.name, targetAnimator.GetBoneTransform(HumanBodyBones.Neck).name);
            boneMappings.Add(sourceRig.head.name, targetAnimator.GetBoneTransform(HumanBodyBones.Head).name);

            boneMappings.Add(sourceRig.leftArm.shoulder.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder).name);
            boneMappings.Add(sourceRig.leftArm.upper.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm).name);
            boneMappings.Add(sourceRig.leftArm.lower.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm).name);
            boneMappings.Add(sourceRig.leftArm.hand.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand).name);

            boneMappings.Add(sourceRig.rightArm.shoulder.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightShoulder).name);
            boneMappings.Add(sourceRig.rightArm.upper.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).name);
            boneMappings.Add(sourceRig.rightArm.lower.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm).name);
            boneMappings.Add(sourceRig.rightArm.hand.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightHand).name);

            boneMappings.Add(sourceRig.leftLeg.thigh.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).name);
            boneMappings.Add(sourceRig.leftLeg.calf.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).name);
            boneMappings.Add(sourceRig.leftLeg.foot.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftFoot).name);
            boneMappings.Add(sourceRig.leftLeg.toes.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.LeftToes).name);

            boneMappings.Add(sourceRig.rightLeg.thigh.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperLeg).name);
            boneMappings.Add(sourceRig.rightLeg.calf.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg).name);
            boneMappings.Add(sourceRig.rightLeg.foot.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightFoot).name);
            boneMappings.Add(sourceRig.rightLeg.toes.name,
                targetAnimator.GetBoneTransform(HumanBodyBones.RightToes).name);

            if (!boneMappings.ContainsKey(sourceRig.leftHand.thumb.proximal.name))
                boneMappings.Add(sourceRig.leftHand.thumb.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.thumb.intermediate.name))
                boneMappings.Add(sourceRig.leftHand.thumb.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.thumb.distal.name))
                boneMappings.Add(sourceRig.leftHand.thumb.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftThumbDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.index.proximal.name))
                boneMappings.Add(sourceRig.leftHand.index.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.index.intermediate.name))
                boneMappings.Add(sourceRig.leftHand.index.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.index.distal.name))
                boneMappings.Add(sourceRig.leftHand.index.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftIndexDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.middle.proximal.name))
                boneMappings.Add(sourceRig.leftHand.middle.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.middle.intermediate.name))
                boneMappings.Add(sourceRig.leftHand.middle.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.middle.distal.name))
                boneMappings.Add(sourceRig.leftHand.middle.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.ring.proximal.name))
                boneMappings.Add(sourceRig.leftHand.ring.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.ring.intermediate.name))
                boneMappings.Add(sourceRig.leftHand.ring.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.ring.distal.name))
                boneMappings.Add(sourceRig.leftHand.ring.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftRingDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.pinky.proximal.name))
                boneMappings.Add(sourceRig.leftHand.pinky.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftLittleProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.leftHand.pinky.intermediate.name))
                boneMappings.Add(sourceRig.leftHand.pinky.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.LeftLittleIntermediate).name);

            if (!boneMappings.ContainsKey(sourceRig.rightHand.thumb.proximal.name))
                boneMappings.Add(sourceRig.rightHand.thumb.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.thumb.intermediate.name))
                boneMappings.Add(sourceRig.rightHand.thumb.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.thumb.distal.name))
                boneMappings.Add(sourceRig.rightHand.thumb.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightThumbDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.index.proximal.name))
                boneMappings.Add(sourceRig.rightHand.index.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.index.intermediate.name))
                boneMappings.Add(sourceRig.rightHand.index.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.index.distal.name))
                boneMappings.Add(sourceRig.rightHand.index.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightIndexDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.middle.proximal.name))
                boneMappings.Add(sourceRig.rightHand.middle.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.middle.intermediate.name))
                boneMappings.Add(sourceRig.rightHand.middle.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.middle.distal.name))
                boneMappings.Add(sourceRig.rightHand.middle.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightMiddleDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.ring.proximal.name))
                boneMappings.Add(sourceRig.rightHand.ring.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightRingProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.ring.intermediate.name))
                boneMappings.Add(sourceRig.rightHand.ring.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightRingIntermediate).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.ring.distal.name))
                boneMappings.Add(sourceRig.rightHand.ring.distal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightRingDistal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.pinky.proximal.name))
                boneMappings.Add(sourceRig.rightHand.pinky.proximal.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightLittleProximal).name);
            if (!boneMappings.ContainsKey(sourceRig.rightHand.pinky.intermediate.name))
                boneMappings.Add(sourceRig.rightHand.pinky.intermediate.name,
                    targetAnimator.GetBoneTransform(HumanBodyBones.RightLittleIntermediate).name);
        }

        private string TranslatePath(string path)
        {
            string[] split = path.Split('/');

            string[] newPath = new string[split.Length];

            for (int i = 0; i < split.Length; i++)
            {
                if (boneMappings.ContainsKey(split[i]))
                {
                    newPath[i] = boneMappings[split[i]];
                }
                else
                {
                    newPath[i] = split[i];
                }
            }

            return string.Join("/", newPath);
        }
    }
}