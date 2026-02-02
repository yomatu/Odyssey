using System;
using Unity.Cinemachine;
using UnityEngine;

//要求该组件必须挂在CinemachineVirtualCamera上
[RequireComponent(typeof(CinemachineVirtualCamera))]
[AddComponentMenu("PLAYER TWO/Platformer Project/Player/PlayerCamera")]
public class PlayerCamera : MonoBehaviour
{
    //相机设置
    [Header("Camera Settings")]
    //跟随玩家的对象
    public Player player;
    
    //相机与目标的最大距离
    public float maxDistance = 15f;
    //初始俯仰角(相机上下角度)
    public float initialAngle = 20f;
    //相机相对玩家的垂直偏移量
    public float heightOffset = 1f;
    //相机目标位置(用于插值过渡)
    protected Vector3 m_cameraTargetPosition;
    
    //相机对象
    protected CinemachineVirtualCamera m_camera;
    // 3D跟随组件
    protected Cinemachine3rdPersonFollow m_cameraBody;

    //Cinemachine控制大脑
    protected CinemachineBrain m_brain;
    
    //内部变量
    protected float m_cameraDistance;
    protected float m_cameraTargetYaw;
    protected float m_cameraTargetPitch;
    
    //相机跟随的目标点(在玩家上方一点)
    protected Transform m_target;
    
    //临时目标对象的名称
    protected string k_targetName = "Player Follower Camera Target"; 
    
    /// <summary>
    /// unity生命周期,启动时初始化相机
    /// </summary>
    protected void Start()
    {
        InitializeComponents();
        InitializeFollower();
        InitializeCamera();
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected virtual void InitializeComponents()
    {
        if (!player)
        {
            //如果没有指定 player,则在场景中自动寻找
            player = FindObjectOfType<Player>();

        }

        m_camera = GetComponent<CinemachineVirtualCamera>();
        m_cameraBody = m_camera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
        m_brain = Camera.main.GetComponent<CinemachineBrain>();



    }

    /// <summary>
    /// 创建相机的跟随目标(跟随点)
    /// </summary>
    protected virtual void InitializeFollower()
    {
        m_target = new GameObject(k_targetName).transform;

        m_target.position = player.transform.position;

    }

    /// <summary>
    /// 初始化相机设置    
    /// </summary>
    protected virtual void InitializeCamera()
    {
        m_camera.Follow = m_target.transform; //相机跟随目标点
        m_camera.LookAt = player.transform;   //相机始终看向玩家


        Reset();


    }

    public virtual void Reset()
    {
        m_cameraDistance = maxDistance;
        //设定初始俯仰角度
        m_cameraTargetPitch = initialAngle;
        //根据玩家朝向设定相机水平角
        m_cameraTargetYaw = player.transform.rotation.eulerAngles.y;

        //初始位置
        //m_cameraTargetPosition = player.unsizedPosition + Vector3.up * heightOffset;
        m_cameraTargetPosition = player.transform.position + Vector3.up * heightOffset;

        MoveTarget();
        
        //强制刷新相机
        m_brain.ManualUpdate();

    }

    
    /// <summary>
    /// 移动相机跟随的目标点.使相机逐帧更新位置和角度
    /// </summary>
    protected virtual void MoveTarget()
    {
        m_target.position = m_cameraTargetPosition;

        m_target.rotation = Quaternion.Euler(m_cameraTargetPitch, m_cameraTargetYaw, 0.0f);

        m_cameraBody.CameraDistance = m_cameraDistance; 

    }
    
    
    
    /// <summary>
    /// unity生命周期,每帧在 LateUpdate 更新相机逻辑
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    protected void LateUpdate()
    {
        MoveTarget();
    }
}