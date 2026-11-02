using UnityEngine;

public class CrawlingPlayerState : PlayerState
{

/// <summary>
/// 进入爬行状态时调用
/// -- 调整碰撞体高度为下蹲高度
/// </summary>
/// <param name="player"></param>
    protected override void OnEnter(Player player)
    {
       player.ResizeCollider(player.stats.current.crouchHeight);
    }
    
/// <summary>
/// 离开爬行状态时调用
/// -- 调整碰撞体高度为正常高度原始战立高度
/// </summary>
/// <param name="player"></param>
    protected override void OnExit(Player player)
    {
       player.ResizeCollider(player.originalHeight);
    }

/// <summary>
/// 爬行状态下每帧更新时调用(爬行逻辑)
/// </summary>
/// <param name="player"></param>

protected override void OnStep(Player player)
{
// 基础运动逻辑
    //应用重力
    player.Gravity();
    //吸附地面,防止角色悬空
    player.SnapToGround();
    //检查是否执行跳跃
    player.Jump();
    //检查是否进入下落状态
    player.Fall();

    //获取输入方向(相对于摄像机的方向)
    var inputDirection = player.inputs.GetMovementCameraDirection();

    //判断是否保持爬行状态
    //1.玩家仍然按着爬行键
    //2.玩家不能起立(头顶有障碍物)
    if (player.inputs.GetCrouchAndCraw() || !player.canStandUp())
    {
        //如果有移动输入,则执行爬行移动
        if(inputDirection.sqrMagnitude > 0)
        {
            
            //执行爬行加速移动
            player.CrawlingAccelerate(inputDirection);

            //平滑转向,使得角色面向移动方向
            player.FaceDirectionSmooth(player.lateralVelocity);
        }
        else
        {
            //如果没有移动输入,则逐渐减少速度
            player.Decelerate(player.stats.current.crawlingFriction);
        }

    }
    else
    {
        //如果松开了爬行按键,并且角色可以站起来,则切换到空闲Idle状态
        player.states.Change<IdlePlayerState>();
    }

}


/// <summary>
/// 
/// 爬行状态下发生碰撞时调用(此处无逻辑)
/// </summary>
/// <param name="player"></param>
/// <param name="other"></param>
public override void OnContact(Player player, Collider other)
{
   
}   

}