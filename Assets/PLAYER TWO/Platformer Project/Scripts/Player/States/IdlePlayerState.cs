
using System.Collections.ObjectModel;
using UnityEngine;

public class IdlePlayerState : PlayerState
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
        Debug.Log("Exited IdlePlayerState");
    }

    /// <summary>
    /// 每帧更新空闲状态逻辑
    /// </summary>
    /// <param name="player"></param>
    protected override void OnStep(Player player)
    {
        //应用重力
        player.Gravity();
        
        //这里打印了玩家是否正确的进入了该状态
        // Debug.Log("IdlePlayerState::OnStep");
     
        //根据输入要知道往哪边走
       // xxx.GetMovementDirection
       
       //获取玩家输入方向
       var inputDirection = player.inputs.GetMovementDirection();

       //walk,输入操作--配置数据--速度,方向----改变状态
       //这里监听了玩家的输入操作并打印在log里面
     //  Debug.Log("inputDirection = " + inputDirection);
     
     //如果有移动输入或水平速度>0 ->切换到 walk 状态
        //输入有效                              并且速度是存在值的
     if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude >0)
     {
         //则切换状态到walk状态 
         player.states.Change<WalkPlayerState>();
     }
     
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
