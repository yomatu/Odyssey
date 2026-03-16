
    using System.Collections.Generic;
    using UnityEngine;
    
    /// <summary>
    /// 测试.更新项目进度失败是为什么
    /// 好像是GitHub的登录token过期导致的无法获取仓库内容
    /// 现在好像又可以重新推送了是为什么??
    /// </summary>
    
    
    //要求当前对象必须挂载 Player 组件
    [RequireComponent(typeof(Player))]
    // 在Unity 的 "Add Component" 菜单里显示该脚本的路径
    [AddComponentMenu("PLAYER TWO/Platformer Project/Player/Player Animator")]
    public class PlayerAnimator : MonoBehaviour
    {
        [System.Serializable]
        public class ForcedTransition
        {
            [Tooltip("玩家状态机中 'fromStateId ' 的状态结束时,强制跳转到某个动画")]
            public int fromStateId;
            
            [Tooltip("目标动画所在的 Animator 层索引. 默认0表示 Base Layer ")]
            public int animationLayer;

            [Tooltip("要强制播放的动画状态名")]
            public string toAnimationState;

            
        }
        
        

        [Header("Parameters Names")] // Animator 参数的变量名(可在 Inspector 修改)
        //当前状态
        public string stateName = "State";
        
        //上一个状态
        public string lastStateName = "Last State";
        
        //横向速度
        public string lateralSpeedName = "Lateral Speed";

        //纵向速度
        public string verticalSpeedName = "Vertical Speed";
        
        //横向动画播放速度
        public string lateralAnimationSpeedName = "Lateral Animation Speed";

        //血量
        public string healthName = "Health";

        //跳跃计数
        public string jumpCounterName = "Jump Counter";

        //是否落地
        public string isGroundedName = "Is Grounded";

        //是否正在抓取物品
        public string isHoldingName = "Is Holding";

        //状态切换触发器
        public string onStateChangedName = "On State Changed";
        

        [Header("Settings")]
        public float minLateralAnimationSpeed = 0.5f;//横向动画播放的最小速度,防止太慢

        public List<ForcedTransition> forcedTransitions;// 强制过渡的列表
        
        


        // 角色 Animator 组件(动画控制器)
        public Animator animator;

        // Animator 参数的Hash值, 避免字符串查找开销
        
        protected int m_stateHash;
        protected int m_lastStateHash;
        protected int m_lateralSpeedHash;
        protected int m_verticalSpeedHash;
        protected int m_lateralAnimationSpeedHash;
        protected int m_healthHash;
        protected int m_jumpCounterHash;
        protected int m_isGroundedHash;
        protected int m_isHoldingHash;
        protected int m_onStateChangedHash;
        
        
        //强制过渡的映射表(通过状态ID快速查找)
        protected Dictionary<int, ForcedTransition> m_forcedTransition;
        
        //引用玩家对象
        protected Player m_player;

        /// <summary>
        /// 脚本启动时初始化所有逻辑 
        /// </summary>
        protected virtual void Start()
        {
            
            InitializePlayer();
            InitializeForcedTransitions();
            InitializeParametersHash();
            InitializeAnimatorTriggers();

        }
        
        /// <summary>
        /// 在 LateUpdate 中更新 Animator 参数.
        /// 保证动画在物理和输入计算完成后才同步
        /// </summary>
        protected virtual void LateUpdate() => HandleAnimatorParameters();

        protected virtual void HandleAnimatorParameters()
        {
            //横向速度
            var lateralSpeed = m_player.lateralVelocity.magnitude;
            //纵向速度
            var verticalSpeed = m_player.verticalVelocity.y;
           
            //横向动画播放速度 = 横向速度 / 最大速度,保证最小速度不低于minLateralAnimationSpeed
            var lateralAnimationSpeed =
                Mathf.Max(minLateralAnimationSpeed, lateralSpeed / m_player.stats.current.topSpeed);

            //设置 Animator 参数
            //可以让动画过渡更加丝滑
            
            animator.SetInteger(m_stateHash,m_player.states.index);
            animator.SetInteger(m_lastStateHash,m_player.states.lastIndex);
            animator.SetFloat(m_lastStateHash,lateralSpeed);
            animator.SetFloat(m_verticalSpeedHash,verticalSpeed);
            animator.SetFloat(m_lateralAnimationSpeedHash,lateralAnimationSpeed);
            animator.SetBool(m_isGroundedHash,m_player.isGrounded);
            
        }
        
        
        /// <summary>
        /// 初始化 Player 引用 , 并监听状态切换事件
        /// </summary>
        protected virtual void InitializePlayer()
        {
            m_player = GetComponent<Player>();

            //当玩家状态发生变化时,执行强制过渡逻辑       
            m_player.states.events.onChange.AddListener(HandleForcedTransitions);
            
            
        }

        /// <summary>
        /// 初始化强制过渡字典,避免重复key
        /// </summary>
        protected virtual void InitializeForcedTransitions()
        {
            m_forcedTransition = new Dictionary<int, ForcedTransition>();

            foreach (var transition in forcedTransitions)
            {
                if (!m_forcedTransition.ContainsKey(transition.fromStateId))
                {
                    m_forcedTransition.Add(transition.fromStateId,transition);
                }
            }
            
        }


        /// <summary>
        /// 初始化Animator的触发器 ,当前状态切换时触发动画事件
        /// </summary>
        
        protected virtual void InitializeAnimatorTriggers()
        {
            // 给 Animator 发送 trigger (触发器参数) , 用于过渡动画
            m_player.states.events.onChange.AddListener(()=> animator.SetTrigger(m_onStateChangedHash));
        }
        
        
        protected virtual void HandleForcedTransitions()
        {
            var lastStateIndex = m_player.states.lastIndex;

            if (m_forcedTransition.ContainsKey(lastStateIndex))
            {
                var layer = m_forcedTransition[lastStateIndex].animationLayer;

                animator.Play(m_forcedTransition[lastStateIndex].toAnimationState,layer);
                
            }
            
        }

         
        /// <summary>
        /// 把参数值转换为hash,提高性能
        /// </summary>
                               
        protected virtual void InitializeParametersHash()
        {
            m_stateHash = Animator.StringToHash(stateName);
            m_lastStateHash = Animator.StringToHash(lastStateName);
            m_lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
            m_verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
            m_lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeedName);
            m_healthHash = Animator.StringToHash(healthName);
            m_jumpCounterHash = Animator.StringToHash(jumpCounterName);
            m_isGroundedHash = Animator.StringToHash(isGroundedName);
            m_isHoldingHash = Animator.StringToHash(isHoldingName);
            m_onStateChangedHash = Animator.StringToHash(onStateChangedName);




        }
        

    }
