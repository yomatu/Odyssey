using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
//强制要求该组件所在的物体必须要有Player 组件
public class PlayerStateManager : EntityStateManager<Player>
{
    
    /// <summary>
    /// 玩家状态类的字符串数组.
    /// 使用 ClassTypeName特性, 让 unity inspector面板可以通过
    /// 下拉/输入选择继承自 PlayerState 的类
    ///
    /// 示例:
    ///  states = {"IdlePlayerState","RunPlayerState",
    /// "JumpPlayerState","SpinPlayerState"}
    /// </summary>
    [ClassTypeName(typeof(PlayerState))] 
    public string[] states;

    /// <summary>
    /// 重写基类方法,获取玩家的状态列表
    /// 会将states中的字符串类名数组转换为真正的状态对象列表.
    /// 
    /// </summary>
    /// <returns>返回一个包含所有状态的List<EntityState<Player>></returns>
    protected override List<EntityState<Player>> GetStateList()
    {
        //调用PlayerState的辅助方法,把字符串数组转换为对象集合
        //例如: "RunPlayerState" → new RunPlayerState()
        return PlayerState.CreateListFromStringArray(states);
    }

}
