public enum ReactionTiming 
{
    PRE,
    POST
}
public static class ReactionTimingLogger
{
    static ReactionTimingLogger(){
        UnityEngine.Debug.Log("ReactionTiming 枚举已加载");
    }
}
