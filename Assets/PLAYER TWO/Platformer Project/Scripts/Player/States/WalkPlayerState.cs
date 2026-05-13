

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
        //重力处理
        player.Gravity();
        
        //保持贴地
        player.SnapToGround();
        
        //跳跃处理
        player.Jump();

        //下落处理
        player.Fall();
        
        //获取玩家输入方向(相机方向)
        var inputDirection = player.inputs.GetMovementCameraDirection();

        if (inputDirection.sqrMagnitude > 0)
        {
            //输入方向与当前水平速度的点乘.用于判定刹车阈值
            var dot = Vector3.Dot(inputDirection, player.lateralVelocity);
        
            if (dot >= player.stats.current.brakeThreshold)
            {
                //超过刹车阈值 -> 正常加速与面向方向
                //加速函数
                player.Accelerate(inputDirection);
                
                //朝向变化速度方向走
                player.FaceDirectionSmooth(player.lateralVelocity);
            }
            else
            {
                //低于刹车阈值 -> 进入刹车状态`
                player.states.Change<BrakePlayerState>();
            }
        }
        else
        {
            //没有输入 ->使用摩擦力减速
            player.Friction();
            
            //当水平速度为零 -> 切换到闲置状态
            if (player.lateralVelocity.sqrMagnitude <= 0)
            {
                player.states.Change<IdlePlayerState>();
                
                
            }

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
