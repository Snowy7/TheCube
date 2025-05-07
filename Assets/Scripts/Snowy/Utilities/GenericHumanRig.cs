using UnityEngine;

namespace Snowy.Utilities
{
    public struct Arm
    {
        public Transform shoulder;
        public Transform upper;
        public Transform lower;
        public Transform hand;
    }

    public struct Finger
    {
        public Transform proximal;
        public Transform intermediate;
        public Transform distal;
    }
    
    public struct Hand
    {
        public Finger thumb;
        public Finger index;
        public Finger middle;
        public Finger ring;
        public Finger pinky;
    }
    
    public struct Leg
    {
        public Transform thigh;
        public Transform calf;
        public Transform foot;
        public Transform toes;
    }

    public struct GenericHumanoidRig
    {
        public Transform hips;
        public Transform spine;
        public Transform lowerChest;
        public Transform upperChest;
        public Transform neck;
        public Transform head;
        
        public Arm leftArm;
        public Arm rightArm;
        public Hand leftHand;
        public Hand rightHand;
        
        public Leg leftLeg;
        public Leg rightLeg;
    }
}