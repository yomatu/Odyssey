

using UnityEngine;

public class WalkPlayerState : PlayerState
{
    /// <summary>
    ///  进入空闲状态时调用
    /// (此处留空.可以用于播放空闲动画.音效)
    /// </summary>
    /// <param name="player"></param>
    protected override void OnEnter(Player player)
    {
        
    }
    
    
    /// <summary>
    ///  离开空闲状态时调用
    /// (此处留空.可以用于清理空闲状态效果)
    /// </summary>
    /// <param name="player"></param>
    protected override void OnExit(Player player)
    {
        
    }

    /// <summary>
    /// 每帧更新空闲状态逻辑
    /// </summary>
    /// <param name="player"></param>
    protected override void OnStep(Player player)
    {
        
    }

    /// <summary>
    /// 碰撞检测逻辑
    /// 空闲状态下通常不需要额外碰撞处理
    /// </summary>
    /// <param name="player"></param>
    /// <param name="other"></param>
    public override void OnContact(Player player, Collider other)
    {
        
    }

}
