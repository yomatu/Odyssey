    using UnityEngine;

    public class FallPlayerState : PlayerState
    {
        /// <summary>
        /// 进入下落状态时可以调用
        /// (此处没有额外逻辑,可以拓展播放下落动画/音效)
        /// </summary>
        /// <param name="player"></param>
        protected override void OnEnter(Player player)
        {
            
        }

        /// <summary>
        /// 离开下落状态时调用
        /// (此处没有额外逻辑)
        /// </summary>
        /// <param name="player"></param>
        protected override void OnExit(Player player)
        {
      
        }


        /// <summary>
        /// 每帧更新下落逻辑
        /// </summary>
        /// <param name="player"></param>
        protected override void OnStep(Player player)
        {
            player.Gravity();
            //平滑转向,使角色朝向移动方向
            player.FaceDirectionSmooth(player.lateralVelocity);
            
            //空中可跳跃
            player.Jump();

            //如果落地 -> 切换到 Idle 状态
            if (player.isGrounded)
            {
                player.states.Change<IdlePlayerState>();
            }
            
        }
        
        
        /// <summary>
        /// 碰撞检测逻辑
        ///  - 下落状态与物体接触时:
        ///     1.推动物体刚体
        ///     2.墙面阻力处理
        ///     3. 抓杆逻辑
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        public override void OnContact(Player player, Collider other)
        {
            
        }
    }
