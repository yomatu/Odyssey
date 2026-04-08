    using UnityEngine;

    
    //将该类添加到unity的组件菜单,方便在Inspector里面挂载
    [AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Brake Player State")]
    //brake拼错成break了
    public class BrakePlayerState : PlayerState
    {
        /// <summary>
        /// 进入刹车状态时调用(此处没有额外逻辑)
        /// </summary>
        /// <param name="player"></param>
        protected override void OnEnter(Player player)
        {
            
        }
        /// <summary>
        /// 离开刹车状态时调用(此处没有额外逻辑)
        /// </summary>
        /// <param name="player"></param>
        protected override void OnExit(Player player)
        {
            
        }
        /// <summary>
        /// 每帧更新时调用(刹车逻辑)
        /// </summary>
        /// <param name="player"></param>
        protected override void OnStep(Player player)
        {
            //执行减速逻辑(逐渐降低水平速度,直到停下)
            player.Decelerate();
            
            //如果玩家的水平速度为0(完全停下来了)
            if (player.lateralVelocity.sqrMagnitude==0)
            {
                
                //状态切换为Idle(待机状态)
                player.states.Change<IdlePlayerState>();
            }
            
        }

        /// <summary>
        /// 当处于刹车状态下发生碰撞时调用(此处没有逻辑)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="other"></param>
        public override void OnContact(Player player, Collider other)
        {
            
        }
    }
