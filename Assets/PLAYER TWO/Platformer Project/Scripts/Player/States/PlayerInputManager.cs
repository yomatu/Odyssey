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

    //初始化调用 唤醒方法    
    protected virtual void Awake() => CacheActions();

    protected virtual void Start()
    {
        //激活actions
        actions.Enable();
    }


    protected virtual void OnEnable() => actions?.Enable();


    protected virtual void OnDisable()=> actions?.Disable();


    protected virtual void CacheActions()
    {
        //拿到动作的名字
        m_movement = actions["Movement"];
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



}