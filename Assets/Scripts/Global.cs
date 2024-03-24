using QFramework;
using UnityEngine;

namespace Jiang.Games
{
    public class Global : Architecture<Global>
    {
        /// <summary>
        /// 注册模块
        /// </summary>
        protected override void Init()
        {
            // RegisterSystem(new XXXSystem());
            RegisterSystem(new SaveSystem());
        }

        [RuntimeInitializeOnLoadMethod]
        public static void AutoInit()
        {
            _ = Interface; // 初始化所有的 Model 和 System
        }
    }
}