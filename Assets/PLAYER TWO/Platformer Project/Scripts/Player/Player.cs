

using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;



/// <summary>
/// 第二课存在一些问题,为什么在空中角色移动会悬空,为什么无法执行顺利的移动跳跃切换
/// 第二课跟了一遍没问题, 会不会是第一课的bug?0
///
/// 
/// </summary>
public class Player : Entity<Player>
{                     //继承自通用的Entity<Player>基类 

    //玩家事件(受伤,死亡,拾取物品等触发的事件)
    public PlayerEvents playerEvents;
    
     /// <summary>
     /// 玩家输入管理器实例
     /// </summary>
     public PlayerInputManager inputs { get; protected set; }
     
     /// <summary>
     /// 玩家数值管理实例
     /// </summary>
     public PlayerStatsManager stats { get; protected set; }
     
     
     public int jumpCounter { get; protected set; }
     

   
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
         
         //监听落地事件,重置跳跃/空中技能次数
         entityEvents.OnGroundEnter.AddListener(()=> {ResetJumps();});
         
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
      
      /// <summary>
      /// 平滑减速(使用decleration参数)
      /// </summary>
      /// <param name="deceleration"></param>

      public virtual void Decelerate() => Decelerate(stats.current.decleration);


      /// <summary>
      /// 平滑减速(使用摩擦力参数)
      /// </summary>
      public virtual void Friction()
      {
          if (OnSlopingGround())
          {
              Decelerate(stats.current.slideForce);//在斜坡上使用斜坡摩擦
          }
          else
          {
              Decelerate(stats.current.friction); //普通摩擦
          }
          
      }
              
      /// <summary>
      /// 根据相机方向来平滑移动玩家
      /// </summary>
      public virtual void AccelerateToInputDirection()
      {
            //输入相对于相机的方向
          var inputDirection = inputs.GetMovementCameraDirection();
          
          Accelerate(inputDirection);
      }
      
      
      /// <summary> 
      /// 平滑朝向某个方向旋转(陆地旋转速度)
      /// </summary>
      public virtual void FaceDirectionSmooth(Vector3 direction) =>
          FaceDirection(direction, stats.current.rotationSpeed);


      public virtual void Gravity()
      {

          //isGrounded = false;
          
          if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
          {
              var speed = verticalVelocity.y;
              //上升时使用普通重力,下落时用更强的下落重力
              var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;

              speed -= force * gravityMultiplier * Time.deltaTime;

              // 限制最大下落速度
              speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);

              verticalVelocity = new Vector3(0, speed, 0);

          }
      }

      /// <summary>
      /// 通过 snap 力量强制把玩家贴到地面上
      /// </summary>
      public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

      /// <summary>
      /// 重置跳跃计数(回到0,常用于落地时)
      /// 这里的reset拼写错误但是没有影响实际逻辑判断,问题在哪里呢
      /// </summary>
      public virtual void ResetJumps() => jumpCounter = 0;  
      
      
      /// <summary>
      /// 如果玩家不在地面上,切换到下落状态
      /// </summary>
      public virtual void Fall()
      {
          if (!isGrounded)
          {
              states.Change<FallPlayerState>();
          }
      }
      
      /// <summary>
      /// 执行跳跃逻辑(包括多段跳跃,土狼跳,持物判定)
      /// </summary>
      public virtual void Jump()
      {
          //是否可以进行二段/ 多段跳

          var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
          
          //土狼跳判定(离地面一小段时间后仍然可以进行跳跃操作)

          var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);

         // isGrounded = true;
          
          // //是否允许在持物状态下跳跃
          //
          // var holdJump = !holding || stats.current.canJumpWhileHolding;

          
          //地面  / 轨道 /多段跳 / 土狼跳条件满足时才允许跳跃
          if ((isGrounded || canMultiJump || canCoyoteJump))
          {
              if (inputs.GetJumpDown()) //按下跳跃键
              {
                    Jump(stats.current.maxJumpHeight);
              }
              
          }
          
          
          //松开跳跃键时,如果还在上升,限制为最小跳跃高度(实现"按的短跳得低"的效果),早松手就早限制
          if (inputs.GetJumpUp()&& (jumpCounter > 0)&& (verticalVelocity.y > stats.current.minJumpHeight))
          {
              verticalVelocity = Vector3.up* stats.current.minJumpHeight;
          }
          
          

      }

      /// <summary>
      ///执行一个标准的向上跳跃 
      /// </summary>
      /// <param name="height"></param>
      public virtual void Jump(float height)
      {
          jumpCounter++; //增加跳跃计数
          verticalVelocity = Vector3.up * height; //设置垂直速度
          // 切换为下落状态(跳起后最终会落下)
          states.Change<FallPlayerState>();

          //触发跳跃事件
          playerEvents.OnJump?.Invoke();

      }
      
      
}
