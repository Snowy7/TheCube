using UnityEngine;

namespace Actors.Player
{
    public enum AnimState
    {
        Idle,
        Walk,
        Sprint
    }
    
    [CreateAssetMenu(fileName = "BobCurves", menuName = "FPS/BobCurves", order = 0)]
    public class BobCurves : ScriptableObject
    {
        [Title("Vertical Bobbing Curves")] public AnimationCurve idleCurve;
        public AnimationCurve walkCurve, runCurve, aimCurve;

        [Title("Horizontal Bobbing Curves")] public AnimationCurve idleCurveH;
        public AnimationCurve walkCurveH, runCurveH, aimCurveH;
        
        public AnimationCurve GetCurveVertical(AnimState state)
        {
            switch (state)
            {
                case AnimState.Idle:
                default:
                    return idleCurve;
                
                case AnimState.Walk:
                    return walkCurve;
                
                case AnimState.Sprint:
                    return runCurve;
            }
        }
        
        public AnimationCurve GetCurveHorizontal(AnimState state)
        {
            switch (state)
            {
                case AnimState.Idle:
                default:
                    return idleCurveH;
                
                case AnimState.Walk:
                    return walkCurveH;
                
                case AnimState.Sprint:
                    return runCurveH;
            }
        }
    }
}