/*===============================
 * 类名：NumberException
 * 描述：关于PreciseNumber的异常类
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/8
 * 最后修改：2015/12/8
 ================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class NumberException : Exception
    {
        public NumberException(string message, int index)
            : base(message)
        {
            _index = index;
        }
        int _index;
        /// <summary>发生错误的位置</summary>
        public int Index { get { return _index; } }

        ///// <summary>错误的类型</summary>
        //public override string Message
        //{
        //    get { return _message; }
        //}
    }
}
