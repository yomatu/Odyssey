
using System.Collections.Generic;

/// <summary>
/// 泛型抽象类,代表某种实体(Entity)的状态机中的一个状态
/// T是继承自Entity<T>的实体类型
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EntityState<T> where T : Entity<T>
{

    /// <summary>
    /// 静态方法.通关类型名称字符串创建对应的状态实例
    /// 例如传入"PLAYERTWO.PlatformerProject.IdleState"返回类型实例.
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    
    public static EntityState<T> CreateFromString(string typeName)
    {
        return (EntityState<T>)System.Activator
            .CreateInstance(System.Type.GetType(typeName));
    }

    public static List<EntityState<T>> CreateListFromStringArray(string[] array)
    {
        var list = new List<EntityState<T>>();

        foreach (var typeName in array)
        {
            list.Add(CreateFromString(typeName));
        }

        return list;
    }
}
