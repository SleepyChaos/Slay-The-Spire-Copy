public class BattleResultGA : GameAction
{
    public bool IsWin { get; private set; }
    public BattleResultGA(bool isWin) { IsWin = isWin; UnityEngine.Debug.Log(isWin ? "战斗结果：胜利" : "战斗结果：失败"); }
}
