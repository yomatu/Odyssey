

public class Player : Entity<Player>
{                     //继承自通用的Entity<Player>基类 

     /// <summary>
     /// 玩家输入管理器实例
     /// </summary>
     public PlayerInputManager inputs { get; protected set; }
   
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
    }
    
     
     //初始化输入
     //问题出在这里.忘记写了 inputs = 导致的初始化失败
     protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();

}
