

using System.Numerics;
using Vector3 = UnityEngine.Vector3;


public class Player : Entity<Player>
{                     //继承自通用的Entity<Player>基类 

     /// <summary>
     /// 玩家输入管理器实例
     /// </summary>
     public PlayerInputManager inputs { get; protected set; }
     
     /// <summary>
     /// 玩家数值管理实例
     /// </summary>
     public PlayerStatsManager stats { get; protected set; }
   
     // 这里是对的.说明问题出现在       InitializeInputs(); 方法里面
    //    protected override void Awake()
    // {
    //     base.Awake();
    //     inputs = GetComponent<PlayerInputManager>(); // 或者 GetComponentInChildren
    //     // 强烈建议添加检查
    //     if (inputs == null) {
    //         Debug.LogError("PlayerInputManager component is missing on the Player GameObject!", this);
    //     }
    // }

    
    protected override void Awake()
    {
         base.Awake();
         InitializeInputs();
         InitializeStats();
    }
    
     
     //初始化输入
     //问题出在这里.忘记写了 inputs = 导致的初始化失败
     protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
  
     //初始化数值
      protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();


      /// <summary>
      /// 在指定方向上平滑移动玩家(加速度控制)
      /// </summary>
      /// <param name="direction"></param>
      public virtual void Accelerate(Vector3 direction)
      {
          //根据是否按下Run键,是否在地面,决定不同的转向阻尼与加速度
          // var turningDrag = isGrounded && inputs.GetRun()
          //     ? stats.current.runningTurningDrag
          //     : stats.current.turningDrag;
          //
          // var acceleration = isGrounded && inputs.GetRun()
          //     ? stats.current.runningAcceleration
          //     : stats.current.acceleration;
          //
          // //空中与地面不同
          // var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
          //
          // var topSpeed = inputs.GetRun() ? stats.current.runningTopSpeed : stats.current.topSpeed;
          //
          
          var turningDrag = stats.current.turningDrag;
          
          var acceleration =  stats.current.acceleration;
          
          //空中与地面不同
          var finalAcceleration =  acceleration;
          
          var topSpeed = stats.current.topSpeed;
          
          
          //调用底层Accelerate(方向,转向阻尼,加速度,最大速度)
          Accelerate(direction, turningDrag , finalAcceleration, topSpeed);


          // //如果刚松开跑步键,限制最大速度,避免瞬间超速
          // if (inputs.GetRunUp())
          // {
          //     lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed);
          // }
          
          
          
      }
      
}
