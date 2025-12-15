using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽象基类,用于管理实体状态机,带有事件支持.
/// </summary>

public abstract class EntityStateManager : MonoBehaviour
{

     /// <summary>
     /// 状态管理相关事件集合(进入状态,退出状态,状态切换)
     /// </summary>
  //   public EntityStateManagerEvents events;
}


/// <summary>
/// 泛型抽象类,继承自EntityStateManager,管理特定实体类型T的状态机
/// T 必须继承自 Entity<T>
/// </summary>
/// <typeparam name="T">实体类型,必须继承自Entity<T>,</typeparam>
public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
{
    /// <summary>
    /// 持有所有 状态 实例的列表,顺序定义状态管理器的状态顺序.
    /// </summary>
    protected List<EntityState<T>> m_list = new List<EntityState<T>>();

    /// <summary>
    /// 状态字典,键为状态类型,值为对应状态实例,方便快速查找.     
    /// </summary>
    protected Dictionary<Type, EntityState<T>> m_states = new Dictionary<Type, EntityState<T>>();
    
    
    /// <summary>
    /// 当前激活的状态实例. 
    /// </summary>
    public EntityState<T> current { get; protected set; }

    /// <summary>
    /// unity 生命周期start ,负重初始化实体和状态
    /// </summary>
    protected void Start()
    {
        InitializeStates();
    }

    /// <summary>
    /// 抽象方法,必须由子类实现,用于返回所有状态实例的列表.
    /// </summary>
    protected abstract List<EntityState<T>> GetStateList();

    /// <summary>
    /// 初始化状态列表和状态字典.
    /// 会调用GetStateList()获取状态列表,并将状态加入字典以便快速查找.
    /// 同时默认将current设为状态列表的第一状态(如果存在)
    /// </summary>
    protected virtual void InitializeStates()
    {
        
        m_list = GetStateList();
        
        foreach (var state in m_list)
        {
            var type = state.GetType();
            //避免重复添加相同类型的状态
            if (!m_states.ContainsKey(type))
            {
                m_states.Add(type, state);
            }
            
        }
        
        //如果状态列表不为空,默认激活第一个状态
        if(m_list.Count > 0)
        {
            current = m_list[0];
        }
        
    }
    
    
}