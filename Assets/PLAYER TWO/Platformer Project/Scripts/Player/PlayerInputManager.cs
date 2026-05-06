using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager :MonoBehaviour
{
    //输入动作资源(在 Input System 中配置的  InputActionAsset)
    public InputActionAsset actions;

    //用于锁定移动方向的时间戳(当前时间小于此时时,禁止移动输入)
    protected float m_movementDirectionUnlockTime;

    //输入动作缓存
    protected InputAction m_movement;
    protected InputAction m_look;
    protected InputAction m_jump;

    
    //主摄像机引用, 用于计算相对移动方向
    protected Camera m_camera;
    
    //常量:鼠标设备名称
    protected const string k_mouseDeviceName = "Mouse";
    
    //最近一次按下跳跃的时间.用于跳跃缓冲
    protected float? m_lastJumpTime;
    
    //常量,跳跃缓冲时常(单位:秒)
    protected const float k_jumpBuffer = 0.15f;
    
    

    //初始化调用 唤醒方法    
    protected virtual void Awake() => CacheActions();

    protected virtual void Start()
    {

        m_camera = Camera.main;
        //激活actions
        actions.Enable();
    }

    protected void Update()
    {
        //记录跳跃按下时间,用于实现跳跃缓冲
        if (m_jump.WasReleasedThisFrame())
        {
            m_lastJumpTime = Time.time;
        }
        
    }


    protected virtual void OnEnable() => actions?.Enable();


    protected virtual void OnDisable()=> actions?.Disable();


    protected virtual void CacheActions()
    {
        //拿到动作的名字
        m_movement = actions["Movement"];
        m_look = actions["Look"];
        m_jump = actions["Jump"];
    }

    /// <summary>
    /// 获取观察方向输入
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetLookDirection()
    {
        var value = m_look.ReadValue<Vector2>();

        if (IsLookingWithMouse())
        {
            return new Vector3(value.x, 0, value.y);
        }

        return GetAxisWithCrossDeadZone(value);
    }

    
    /// <summary>
    /// 判断是否通过鼠标进行观察输入
    /// </summary>
    /// <returns></returns>
    public virtual bool IsLookingWithMouse()
    {
        if (m_look.activeControl == null)
        {
            return false;
        }

        return m_look.activeControl.device.name.Equals(k_mouseDeviceName);
    }
    
    
    
    
    /// <summary>
    /// 获取移动方向输入(带十字型死区判定)
    /// 如果在锁定时间内,则返回Vector3.zero 
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetMovementDirection()
    {
        //死区判定
        if (Time.time < m_movementDirectionUnlockTime)
        {
            return Vector3.zero;
        }

        var value = m_movement.ReadValue<Vector2>();

        return GetAxisWithCrossDeadZone(value);

    }

    /// <summary>
    /// 根据十字形死区修正输入值(Input  System 默认是圆形死区)
    /// </summary>
    /// <param name="axis">输入轴</param>
    /// <returns></returns>

    public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis)
    {
        var deadzone = InputSystem.settings.defaultDeadzoneMin;

        axis.x = Mathf.Abs(axis.x) > deadzone ? RemapToDeadzone(axis.x, deadzone) : 0;
        axis.y = Mathf.Abs(axis.y) > deadzone ? RemapToDeadzone(axis.y, deadzone) : 0;

        return new Vector3(axis.x, 0, axis.y);
    }

    /// <summary>
    /// 将输入值按给定死区重新映射到 0-1
    /// </summary>
    ///  protected float RemapToDeadzone(float value, float deadzone) =>(value - deadzone) / (1 - deadzone);  
    /// <param name="value"></param>
    /// <param name="deadzone"></param>
    /// <returns></returns>

    protected float RemapToDeadzone(float value, float deadzone) =>
        (value - (value > 0 ? -deadzone : deadzone)) / (1 - deadzone);  
                //取了绝对值

    //

    /// <summary>
    /// 获取相机方向下的移动向量
    /// 将输入方向映射到相机朝向(Y轴旋转) 下
    /// </summary>
    /// <returns></returns>
    public virtual Vector3 GetMovementCameraDirection()
    {
        //1.获取移动方向(通常是玩家输入的水平/垂直方向, 比如 wasd 或摇杆)
        var direction = GetMovementDirection();

        //2. 如果有输入(不是0向量)
        if (direction.sqrMagnitude>0)
        {
            //3.构建一个旋转,根据摄像机的Y轴速度(水平朝向)
            //Quaternion.AngleAxis(angle, axis)表示绕某个轴旋转一个角度
            var rotation = Quaternion.AngleAxis(m_camera.transform.eulerAngles.y, Vector3.up);
            
            
            //4.把原始输入方向旋转到摄像机从朝向下

            direction = rotation * direction;
            
            //5.归一化,保持方向向量的长度为 1 (只保留方向)

            direction = direction.normalized;

        }

        // 6.返回最终的世界空间移动方向
        return direction;

    }


    public virtual bool GetJumpDown()
    {
        if (m_lastJumpTime != null&&
            Time.time - m_lastJumpTime < k_jumpBuffer)
        {
            m_lastJumpTime = null;
            return true;
        }

        return false;
    }

    public virtual bool GetJumpUp() => m_jump.WasReleasedThisFrame();
    
}