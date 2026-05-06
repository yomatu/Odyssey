using System;
using UnityEngine.Events;

[Serializable]
public class PlayerEvents
{
    /// <summary>
    /// 当玩家跳跃时调用
    /// </summary>
    /// <returns></returns>
    public UnityEvent OnJump;
}