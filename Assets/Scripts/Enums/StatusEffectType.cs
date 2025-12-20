public enum StatusEffectType 
{
    ARMOR,//护甲
    BURN,//燃烧
}
public static class StatusEffectTypeLogger
{
    static StatusEffectTypeLogger(){
        UnityEngine.Debug.Log("StatusEffectType 枚举已加载");
    }
}
