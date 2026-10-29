
using UnityEngine;

public class CrouchPlayerState : PlayerState
{
    /// <summary>
    /// 进入蹲伏状态时调用
    /// (此处留空.可以用于播放蹲伏动画.音效)
    /// 调整碰撞体高度为"蹲伏高度"
    /// </summary>
    /// <param name="player"></param>s
    protected override void OnEnter(Player player)
    {
        player.ResizeCollider(player.stats.current.crouchHeight);
    }

    /// <summary>
    /// 离开蹲伏状态时调用
    /// (此处留空.可以用于清理蹲伏状态效果)
    /// 恢复碰撞体高度为"原始高度"站立高度
    /// </summary>
    /// <param name="player"></param>
    protected override void OnExit(Player player)
    {
        player.ResizeCollider(player.originalHeight);
    }

    /// <summary>
    /// 每帧更新蹲伏状态逻辑
    /// </summary>
    /// <param name="player"></param>
    protected override void OnStep(Player player)
    {
        //基础物理更新
        //重力
        player.Gravity();
        //粘附地面
        player.SnapToGround();
        //下落
        player.Fall();
    
        // 应用下蹲的摩擦力,逐渐减速,直到停止移动
        player.Decelerate(player.stats.current.crouchFriction);
    
        // 处理移动输入
        // 获取玩家输入的移动方向(相对于世界,不考虑摄像机)
        var inputDirection = player.inputs.GetMovementDirection();

        //     处理蹲伏和爬行输入
        // 如果玩家按下蹲伏键,或者玩家被障碍阻挡不能站起来
        if(player.inputs.GetCrouchAndCraw()||!player.canStandUp())
        {
            //1.玩家有方向输入.并且角色手上没有拿东西
            if(inputDirection.sqrMagnitude>0 && !player.holding)
            {
                //如果速度为0 -> 进入爬行状态(从蹲姿转换成爬行移动)
                {
                   // player.states.Change<CrawlingPlayerState>();
                }
            }
            //2.玩家在下蹲状态按下"跳跃键" -> 执行后空翻
            else if(player.inputs.GetJumpDown())
            {
                //player.Backflip(player.stats.current.backflipBackwardForce);
            }
            else
            {
                //如果玩家松开下蹲键.且角色可以站起来 -> 切换为 Idle 状态
                player.states.Change<IdlePlayerState>();
            }


        }


       } 

    /// <summary>
    /// 碰撞检测逻辑(此状态下没有额外逻辑)
    /// 蹲伏状态下通常不需要额外碰撞处理
    /// </summary>
    /// <param name="player"></param>
    /// <param name="other"></param>
    public override void OnContact(Player player, Collider other)
    {
        
    }

}
