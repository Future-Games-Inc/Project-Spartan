using UnityEngine;

[CreateAssetMenu(fileName = "Scaling COnfiguarion", menuName = "ScriptableObject/Scaling Configuration")]
public class ScalingScriptableObject : ScriptableObject
{
    public AnimationCurve healthCurve;
    public AnimationCurve damageCurve;
    public AnimationCurve speedCurve;
    public AnimationCurve SpawnRateCurve;
    public AnimationCurve spawnCountCurve;


}
