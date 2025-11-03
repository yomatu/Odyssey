
/// <summary>
/// 泛型抽象类,代表某种实体(Entity)的状态机中的一个状态
/// T是继承自Entity<T>的实体类型
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EntityState<T> where T : Entity<T>
{
    
}
