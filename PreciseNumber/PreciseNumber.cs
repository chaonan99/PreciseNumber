/*==========================
 * 类名：PreciseNumber
 * 描述：任意精度数基础类
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/8
 * 最后修改：2015/12/13
 ===========================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class PreciseNumber : IComparable<PreciseNumber>
    {
        #region 常量

        /// <summary>
        /// 一个List元素存储的整数位数
        /// 此数需<=9，若要避免乘法中使用long类型，应<=4
        /// </summary>
        public static readonly int OneCount = 9;
        /// <summary>
        /// 每个组元素最大数值
        /// </summary>
        public static readonly int MaxValue = (int)Math.Pow(10, OneCount);
        #endregion

        #region 构造方法
        public PreciseNumber()
        {
            //类对0的表示如下：
            intPart = new List<int>();
            intPart.Add(0);
            decPart = new List<int>();
            decPrecise = 0;
            IsPositive = true;
        }
        public PreciseNumber(List<int> intPart, List<int> decPart, bool isPositive)
        {
            if (intPart == null)
            {
                this.IntPart = new List<int>();
                IntPart.Add(0);
            }
            else
            {
                //这里需要深复制，下同
                this.IntPart = new List<int>(intPart);
            }
            if (decPart == null)
            {
                this.DecPart = new List<int>();
            }
            else
            {
                this.DecPart = new List<int>(decPart);
            }
            this.IsPositive = isPositive;
            this.DecPrecise = IdentifyNumber.DefaultPrecise(decPart);
        }
        public PreciseNumber(List<int> intPart, List<int> decPart, bool isPositive, int decPrecise)
        {
            if (intPart == null)
            {
                this.IntPart = new List<int>();
                IntPart.Add(0);
            }
            else
            {
                this.IntPart = new List<int>(intPart);
            }
            if (decPart == null)
            {
                this.DecPart = new List<int>();
            }
            else
            {
                this.DecPart = new List<int>(decPart);
            }
            this.IsPositive = isPositive;
            if (decPrecise < 0)
            {
                throw new Exception("精度不能为负数");
            }
            else
            {
                this.DecPrecise = decPrecise;
            }
        }
        public PreciseNumber(string text)
        {
            intPart = new List<int>();
            decPart = new List<int>();
            IsPositive = IdentifyNumber.GetNumberFromString(text, intPart, decPart,ref decPrecise);
            //如果整数部分为0，GetNumberFromString不会自动加0，需要给整数部分添加一个零元素
            if (intPart.Count == 0)
            {
                intPart.Add(0);
            }
        }

        public PreciseNumber(PreciseNumber nxtNumber)
        {
            intPart = nxtNumber.intPart;
            decPart = nxtNumber.decPart;
            decPrecise = nxtNumber.decPrecise;
            IsPositive = nxtNumber.IsPositive;
        }
        #endregion

        #region 属性
        private List<int> intPart; //整数部分
        private List<int> decPart; //小数部分
        private int decPrecise;     //小数部分精度

        /// <summary>
        /// 整数部分
        /// </summary>
        public List<int> IntPart
        {
            get { return intPart; }
            private set { intPart = new List<int>(value); }
        }
        /// <summary>
        /// 小数部分
        /// </summary>
        public List<int> DecPart
        {
            get { return decPart; }
            private set { decPart = new List<int>(value); }
        }
        /// <summary>
        /// 是否为正数
        /// </summary>
        public bool IsPositive { get; set; }
        /// <summary>
        /// 小数部分精度
        /// </summary>
        public int DecPrecise
        {
            get { return decPrecise; }
            private set { decPrecise = value; }
        }

        #endregion

        #region 公有方法
        /// <summary>
        /// 判断数字是否为0
        /// </summary>
        /// <returns>为0</returns>
        public bool IsZero()
        {
            return intPart.Count == 1 && intPart[0] == 0 && decPart.Count == 0;
        }
        /// <summary>
        /// 判断数字是否为整数
        /// </summary>
        /// <returns>为整数</returns>
        public bool IsIntegral()
        {
            return DecPrecise == 0;
        }
        /// <summary>
        /// 返回PreciseNumber整数部分的数字个数
        /// </summary>
        /// <returns></returns>
        public int IntFigureNumber()
        {
            StringBuilder sbInt = new StringBuilder();
            foreach (var i in IntPart)
            {
                sbInt.Append(i.ToString().PadLeft(PreciseNumber.OneCount, '0'));
            }
            string r = sbInt.ToString().TrimStart('0');
            return r.Length;
        }
        /// <summary>
        /// 返回实例的绝对值
        /// </summary>
        public PreciseNumber AbsoluteNumber()
        {
            return new PreciseNumber(IntPart, DecPart, true, DecPrecise);
        }
        /// <summary>
        /// 返回实例的相反数
        /// </summary>
        public PreciseNumber ReverseNumber()
        {
            return new PreciseNumber(IntPart, DecPart, !IsPositive, decPrecise);
        }

        #endregion

        #region 重写方法
        /// <summary>
        /// 重写Equals方法
        /// 参考 https://msdn.microsoft.com/zh-cn/library/ms173147(v=VS.90).aspx
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            PreciseNumber p = obj as PreciseNumber;
            if ((System.Object)p == null)
            {
                return false;
            }

            return CompareNumber.Compare(this, p) == 0;
        }
        public bool Equals(PreciseNumber p)
        {
            if ((object)p == null)
            {
                return false;
            }

            return CompareNumber.Compare(this, p) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 重写ToString方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sbInt = new StringBuilder();
            StringBuilder sbDec = new StringBuilder();
            string r;
            //整数部分
            foreach (int i in IntPart)
            {
                //左侧补0，将会在下面去掉最高位的0
                sbInt.Append(i.ToString().PadLeft(PreciseNumber.OneCount, '0'));
            }
            if (DecPrecise > 0)
            {
                sbDec.Append('.');
                if (DecPart.Count > 0)
                {
                    int i = 0;
                    if (DecPart.Count > 1)
                    {
                        for (; i < DecPart.Count - 1; i++)
                        {
                            //左侧补零
                            sbDec.Append(DecPart[i].ToString().PadLeft(PreciseNumber.OneCount, '0'));
                        }
                    }
                    //左侧补零，右侧补零至精度
                    sbDec.Append(DecPart[i].ToString().PadLeft(PreciseNumber.OneCount, '0').TrimEnd('0'));
                }
                sbDec.Append('0', decPrecise - sbDec.Length + 1);
            }
            r = sbInt.Append(sbDec).ToString();
            r = r.TrimStart('0');

            if (r == string.Empty)
                return "0";
            if (r[0] == '.')
                r = "0" + r;
            if (!IsPositive)
                r = "-" + r;
            return r;
        }
        #endregion

        #region 重载运算符
        public static PreciseNumber operator +(PreciseNumber a, PreciseNumber b)
        {
            return BasicCalculate.Add(a, b);
        }
        public static PreciseNumber operator -(PreciseNumber a,PreciseNumber b)
        {
            return BasicCalculate.Minus(a, b);
        }
        public static PreciseNumber operator *(PreciseNumber a, PreciseNumber b)
        {
            return BasicCalculate.Multiply(a, b);
        }
        public static PreciseNumber operator /(PreciseNumber a, PreciseNumber b)
        {
            return BasicCalculate.Division(a, b);
        }
        public static PreciseNumber operator ++(PreciseNumber a)
        {
            return BasicCalculate.Add(a, new PreciseNumber("1"));
        }
        public static implicit operator PreciseNumber(int a)
        {
            return new PreciseNumber(a.ToString());
        }
        public static implicit operator PreciseNumber(double a)
        {
            return new PreciseNumber(a.ToString());
        }
        public static bool operator >(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) == 1;
        }
        public static bool operator <(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) == -1;
        }
        public static bool operator >=(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) != -1;
        }
        public static bool operator <=(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) != 1;
        }
        public static bool operator ==(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) == 0;
        }
        public static bool operator !=(PreciseNumber a, PreciseNumber b)
        {
            return CompareNumber.Compare(a, b) != 0;
        }
        #endregion

        #region IComparable接口实现
        public int CompareTo(PreciseNumber other)
        {
            return CompareNumber.Compare(this, other);
        }
        #endregion

    }
}
