/*=======================
 * 类名：BasicCalculate
 * 描述：PreciseNumber的四则运算实现
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/9
 * 最后修改：2015/12/19
 ========================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class BasicCalculate
    {
        #region 公有方法
        /// <summary>
        /// 两个PreciseNumber的加法
        /// </summary>
        /// <param name="leftNum">左操作数</param>
        /// <param name="rightNum">右操作数</param>
        /// <returns>和</returns>
        public static PreciseNumber Add(PreciseNumber leftNum, PreciseNumber rightNum)
        {
            PreciseNumber leftAbs = leftNum.AbsoluteNumber();
            PreciseNumber rightAbs = rightNum.AbsoluteNumber();

            if (leftNum.IsPositive && rightNum.IsPositive)
            {
                //正数加正数
                return PositiveAdd(leftAbs, rightAbs);
            }
            else if (leftNum.IsPositive && !rightNum.IsPositive)
            {
                //正数加负数
                return Minus(leftAbs, rightAbs);
            }
            else if (!leftNum.IsPositive && rightNum.IsPositive)
            {
                //负数加正数
                return Add(rightNum, leftNum);
            }
            else
            {
                //负数加负数
                PreciseNumber result = PositiveAdd(leftAbs, rightAbs);
                return new PreciseNumber(result.IntPart, result.DecPart, false, result.DecPrecise);
            }
        }
        /// <summary>
        /// 两个PreciseNumber的减法
        /// </summary>
        /// <param name="leftNum">被减数</param>
        /// <param name="rightNum">减数</param>
        /// <returns>差</returns>
        public static PreciseNumber Minus(PreciseNumber leftNum, PreciseNumber rightNum)
        {
            PreciseNumber leftAbs = leftNum.AbsoluteNumber();
            PreciseNumber rightAbs = rightNum.AbsoluteNumber();

            if (leftNum.IsPositive && rightNum.IsPositive)
            {
                //正数减正数
                if (leftAbs.CompareTo(rightAbs) == 1)
                {
                    return PositiveMinus(leftAbs, rightAbs);
                }
                else if (leftAbs.CompareTo(rightAbs) == -1)
                {
                    return PositiveMinus(rightAbs, leftAbs).ReverseNumber();
                }
                else
                {
                    return new PreciseNumber();
                }
            }
            else if (leftNum.IsPositive && !rightNum.IsPositive)
            {
                //正数减负数
                return PositiveAdd(leftAbs, rightAbs);
            }
            else if (!leftNum.IsPositive && rightNum.IsPositive)
            {
                //负数减正数
                return PositiveAdd(rightNum, leftNum).ReverseNumber();
            }
            else
            {
                //负数减负数
                return Minus(rightAbs, leftAbs);
            }
        }
        /// <summary>
        /// 两个PreciseNumber的乘法
        /// </summary>
        /// <param name="leftNum">左操作数</param>
        /// <param name="rightNum">右操作数</param>
        /// <returns>积</returns>
        public static PreciseNumber Multiply(PreciseNumber leftNum, PreciseNumber rightNum)
        {
            List<int> leftAll = new List<int>(leftNum.IntPart);
            List<int> rightAll = new List<int>(rightNum.IntPart);
            leftAll.AddRange(leftNum.DecPart);
            rightAll.AddRange(rightNum.DecPart);

            List<int> resultAll = IntegralMultiply(leftAll, rightAll);

            int decimalLength = leftNum.DecPart.Count + rightNum.DecPart.Count;
            List<int> integralPart = resultAll.GetRange(0, resultAll.Count - decimalLength);
            List<int> decimalPart = resultAll.GetRange(resultAll.Count - decimalLength, decimalLength);
            int decPrecise = IdentifyNumber.DefaultPrecise(decimalPart);
            return new PreciseNumber(
                integralPart, decimalPart, !(leftNum.IsPositive ^ rightNum.IsPositive), decPrecise);
        }
        /// <summary>
        /// PreciseNumber的除法，默认精度为除数、被除数中较大者
        /// </summary>
        /// <param name="leftNum">左操作数（被除数）</param>
        /// <param name="rightNum">右操作数（除数）</param>
        /// <returns>计算结果</returns>
        public static PreciseNumber Division(PreciseNumber leftNum, PreciseNumber rightNum)
        {
            return Division(leftNum, rightNum, 
                leftNum.DecPrecise > rightNum.DecPrecise ? leftNum.DecPrecise : rightNum.DecPrecise);
        }
        /// <summary>
        /// 带精度控制的除法
        /// </summary>
        /// <param name="leftNum">左操作数（被除数）</param>
        /// <param name="rightNum">右操作数（除数）</param>
        /// <param name="decPrecise">要求的小数精度</param>
        /// <returns>计算结果</returns>
        public static PreciseNumber Division(PreciseNumber leftNum, PreciseNumber rightNum, int decPrecise)
        {
            if (rightNum.IsZero())
            {
                //除零错误
                throw new DivideByZeroException();
            }
            if (leftNum.IsZero())
            {
                //被除数为0，直接返回0
                return new PreciseNumber();
            }
            if (leftNum.IsPositive^rightNum.IsPositive)
            {
                //除数、被除数异号
                return DivisionPositive(
                    leftNum.AbsoluteNumber(), rightNum.AbsoluteNumber(), decPrecise).ReverseNumber();
            }
            else
            {
                //除数、被除数同号
                return DivisionPositive(leftNum.AbsoluteNumber(), rightNum.AbsoluteNumber(), decPrecise);
            }
        }
        
        #endregion

        #region 求和的私有方法
        /// <summary>
        /// 两个非负PreciseNumber相加
        /// </summary>
        /// <param name="leftAbs">左操作数</param>
        /// <param name="rightAbs">右操作数</param>
        /// <returns>求和结果</returns>
        private static PreciseNumber PositiveAdd(PreciseNumber leftAbs, PreciseNumber rightAbs)
        {
            int carry = 0;
            List<int> decimalResult = new List<int>();
            List<int> integralResult = new List<int>();
            //精度取大者
            int decPrecise = leftAbs.DecPrecise > rightAbs.DecPrecise ? leftAbs.DecPrecise : rightAbs.DecPrecise;
            //小数部分相加
            if (leftAbs.DecPart.Count == 0)
            {
                //左操作数没有小数部分，整体小数部分即为右操作数小数部分（注意，右操作数可能也没有小数部分）
                decimalResult.AddRange(rightAbs.DecPart);
            }
            else if (rightAbs.DecPart.Count == 0)
            {
                //右操作数没有小数部分，整体小数部分即为左操作数小数部分
                decimalResult.AddRange(leftAbs.DecPart);
            }
            else if (leftAbs.DecPart.Count > rightAbs.DecPart.Count)
            {
                //左操作数长
                decimalResult = DecimalAddLongToShort(leftAbs.DecPart, rightAbs.DecPart, ref carry);
            }
            else
            {
                //左操作数和右操作数一样长或右比左长
                decimalResult = DecimalAddLongToShort(rightAbs.DecPart, leftAbs.DecPart, ref carry);
            }
            //整数部分相加
            //考虑到整数部分需要处理小数进位的1，没有采用小数部分的“加0”的加速方案
            if (leftAbs.IntPart.Count > rightAbs.IntPart.Count)
            {
                //左操作数长
                integralResult = IntegralAddLongToShort(leftAbs.IntPart, rightAbs.IntPart, ref carry);
            }
            else
            {
                //左操作数和右操作数一样长或右比左长
                integralResult = IntegralAddLongToShort(rightAbs.IntPart, leftAbs.IntPart, ref carry);
            }
            return new PreciseNumber(integralResult, decimalResult, true, decPrecise);
        }
        /// <summary>
        /// 整数列表相加
        /// </summary>
        /// <param name="list1">长列表</param>
        /// <param name="list2">短列表</param>
        /// <param name="carry">进位引用</param>
        /// <returns>结果列表</returns>
        private static List<int> IntegralAddLongToShort(List<int> list1, List<int> list2, ref int carry)
        {
            List<int> integralResult = new List<int>();
            int gap = list1.Count - list2.Count;    //由于以下插入过程中list2.Count还会变化，故需要预先计算好
            for (int i = 0; i < gap; i++)
            {
                //将list1与list2补齐
                list2.Insert(0, 0);
            }
            for (int i = list1.Count - 1; i >= 0; i--)
            {
                //倒序遍历，因为存储时是从高位往低位存的
                integralResult.Insert(0, AddTwoNumber(list1[i], list2[i], ref carry));
            }
            if (carry != 0)
            {
                //整数需要特别处理最后的进位，可能需要给整数部分添加一组
                integralResult.Insert(0, 1);
            }
            return integralResult;
        }
        /// <summary>
        /// 小数列表相加
        /// </summary>
        /// <param name="list1">长列表</param>
        /// <param name="list2">短列表</param>
        /// <param name="carry">进位引用</param>
        /// <returns>结果列表</returns>
        private static List<int> DecimalAddLongToShort(List<int> list1, List<int> list2, ref int carry)
        {
            List<int> decimalResult = new List<int>();
            int gap = list1.Count - list2.Count;
            for (int i = 0; i < gap; i++)
            {
                //将list1与list2补齐
                list2.Add(0);
            }
            for (int i = list1.Count - 1; i >= 0; i--)
            {
                //倒序遍历，因为存储时是从高位往低位存的
                decimalResult.Insert(0, AddTwoNumber(list1[i], list2[i], ref carry));
            }
            //小数部分不用特别处理进位
            return decimalResult;
        }
        /// <summary>
        /// 将两整数带进位相加，进位以引用形式返回
        /// </summary>
        /// <param name="p1">整数1</param>
        /// <param name="p2">整数2</param>
        /// <param name="carry">进位引用</param>
        /// <returns>计算结果</returns>
        private static int AddTwoNumber(int p1, int p2, ref int carry)
        {
            int result = p1 + p2 + carry;
            if (result>=PreciseNumber.MaxValue)
            {
                carry = 1;
                result = result - PreciseNumber.MaxValue;
            }
            else
            {
                carry = 0;
            }
            return result;
        }
        #endregion

        #region 求差的私有方法
        /// <summary>
        /// 两个非负数相减且左操作数大于右操作数
        /// </summary>
        /// <param name="leftAbs">左操作数</param>
        /// <param name="rightAbs">右操作数</param>
        /// <returns>求差结果</returns>
        private static PreciseNumber PositiveMinus(PreciseNumber leftAbs, PreciseNumber rightAbs)
        {
            int borrow = 0;
            List<int> integralResult = new List<int>();
            List<int> decimalResult = new List<int>();
            //
            int decPrecise = leftAbs.DecPrecise > rightAbs.DecPrecise ? leftAbs.DecPrecise : rightAbs.DecPrecise;
            //小数部分
            decimalResult = DecimalMinus(leftAbs.DecPart, rightAbs.DecPart, ref borrow);
            //整数部分
            integralResult = IntegralMinus(leftAbs.IntPart, rightAbs.IntPart, ref borrow);
            return new PreciseNumber(integralResult, decimalResult, true, decPrecise);
        }
        /// <summary>
        /// 整数列表相减
        /// </summary>
        /// <param name="list1">大列表</param>
        /// <param name="list2">小列表</param>
        /// <param name="borrow">借位引用</param>
        /// <returns>相减结果</returns>
        private static List<int> IntegralMinus(List<int> list1, List<int> list2, ref int borrow)
        {
            List<int> integralResult = new List<int>();
            int gap = list1.Count - list2.Count;    //由于以下插入过程中list2.Count还会变化，故需要预先计算好
            for (int i = 0; i < gap; i++)
            {
                //将list1与list2补齐
                list2.Insert(0, 0);
            }

            for (int i = list1.Count - 1; i >= 0; i--)
            {
                //倒序遍历，因为存储时是从高位往低位存的
                integralResult.Insert(0, MinusTwoNumber(list1[i], list2[i], ref borrow));
            }
            while (integralResult[0] == 0 && integralResult.Count > 1)
            {
                integralResult.RemoveAt(0);
            }
            return integralResult;
        }
        /// <summary>
        /// 小数列表相减，对于大小没有要求，结果一定为正，两列表大小关系反映在借位上
        /// </summary>
        /// <param name="list1">列表1</param>
        /// <param name="list2">列表2</param>
        /// <param name="borrow">借位引用</param>
        /// <returns>相减结果</returns>
        private static List<int> DecimalMinus(List<int> list1, List<int> list2, ref int borrow)
        {
            List<int> decimalResult = new List<int>();
            int gap = list1.Count - list2.Count;
            if (gap > 0)
            {
                for (int i = 0; i < gap; i++)
                {
                    //将list1与list2补齐
                    list2.Add(0);
                }
            }
            else if (gap < 0)
            {
                for (int i = 0; i < -gap; i++)
                {
                    list1.Add(0);
                }
            }
            for (int i = list1.Count - 1; i >= 0; i--)
            {
                //倒序遍历，因为存储时是从高位往低位存的
                decimalResult.Insert(0, MinusTwoNumber(list1[i], list2[i], ref borrow));
            }
            return decimalResult;
        }
        /// <summary>
        /// 两整数带借位相减，借位以引用形式返回
        /// </summary>
        /// <param name="p1">整数1，被减数</param>
        /// <param name="p2">整数2，减数</param>
        /// <param name="borrow">借位引用</param>
        /// <returns>相减结果</returns>
        private static int MinusTwoNumber(int p1, int p2, ref int borrow)
        {
            int result = p1 - p2 - borrow;
            if (result < 0)
            {
                result = result + PreciseNumber.MaxValue;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }
            return result;
        }

        #endregion

        #region 求积的私有方法
        /// <summary>
        /// 两个整数相乘
        /// </summary>
        /// <param name="leftAll">左操作数</param>
        /// <param name="rightAll">右操作数</param>
        /// <returns>返回相乘结果</returns>
        private static List<int> IntegralMultiply(List<int> leftAll, List<int> rightAll)
        {
            if (PreciseNumber.OneCount >= 5)
            {
                //此时需要临时乘积用long，否则运算溢出
                List<int> resultAll = new List<int>();
                long tempResult = 0;
                int carry = 0;
                for (int i = 0; i < leftAll.Count; i++)
                {
                    for (int j = 0; j < rightAll.Count; j++)
                    {
                        tempResult = (long)leftAll[leftAll.Count - 1 - i] * rightAll[rightAll.Count - 1 - j] + carry;
                        carry = (int)(tempResult / PreciseNumber.MaxValue);
                        if (resultAll.Count == i + j)
                        {
                            resultAll.Add((int)(tempResult % PreciseNumber.MaxValue));
                        }
                        else
                        {
                            resultAll[i + j] += (int)(tempResult % PreciseNumber.MaxValue);
                            if (resultAll[i + j] >= PreciseNumber.MaxValue)
                            {
                                resultAll[i + j] = resultAll[i + j] - PreciseNumber.MaxValue;
                                carry++;
                            }
                        }
                        if (j == rightAll.Count - 1 && carry != 0)
                        {
                            resultAll.Add(carry);
                            carry = 0;
                        }
                    }
                }
                resultAll.Reverse();
                return resultAll;
            }
            else
            {
                //每组小于5位
                List<int> resultAll = new List<int>();
                int tempResult = 0;
                int carry = 0;
                for (int i = 0; i < leftAll.Count; i++)
                {
                    for (int j = 0; j < rightAll.Count; j++)
                    {
                        tempResult = leftAll[leftAll.Count - 1 - i] * rightAll[rightAll.Count - 1 - j] + carry;
                        carry = tempResult / PreciseNumber.MaxValue;
                        if (resultAll.Count == i + j)
                        {
                            resultAll.Add(tempResult % PreciseNumber.MaxValue);
                        }
                        else
                        {
                            resultAll[i + j] += tempResult % PreciseNumber.MaxValue;
                            if (resultAll[i + j] >= PreciseNumber.MaxValue)
                            {
                                resultAll[i + j] = resultAll[i + j] - PreciseNumber.MaxValue;
                                carry++;
                            }
                        }
                        if (j == rightAll.Count - 1 && carry != 0)
                        {
                            resultAll.Add(carry);
                            carry = 0;
                        }
                    }
                }
                resultAll.Reverse();
                return resultAll;
            }
        }

        #endregion

        #region 求商的私有方法

        /// <summary>
        /// 两个正数的除法
        /// </summary>
        /// <param name="leftNum">左操作数（被除数）</param>
        /// <param name="rightNum">右操作数（除数）</param>
        /// <param name="decPrecise">要求的小数精度</param>
        /// <returns>计算结果</returns>
        private static PreciseNumber DivisionPositive(PreciseNumber leftNumber, PreciseNumber rightNum, int decPrecise)
        {
            //拟采用牛顿迭代，初值用2^(1 - 二进制位数)

            PreciseNumber rightInt = new PreciseNumber(rightNum.IntPart, new List<int>(), true, 0);
            PreciseNumber rightReciprocal = Reciprocal(rightNum, decPrecise + leftNumber.IntPart.Count * PreciseNumber.OneCount);

            PreciseNumber result = rightReciprocal * leftNumber;
            result = IdentifyNumber.RoundPrecise(result, decPrecise); //控制结果精度
            if (result.DecPrecise < decPrecise)
            {
                result = new PreciseNumber(result.IntPart, result.DecPart, true, decPrecise);
            }
            return result;
        }
        /// <summary>
        /// 求PreciseNumber的倒数
        /// </summary>
        /// <param name="src">待计算数字</param>
        /// <param name="decPrecise">小数部分要求精度</param>
        /// <returns>倒数结果</returns>
        public static PreciseNumber Reciprocal(PreciseNumber src, int decPrecise)
        {
            //
            if (src.IsZero())
            {
                throw new NumberException("零不存在倒数！", 0);
            }
            else if (src < 0)
            {
                return Reciprocal(src.AbsoluteNumber(), decPrecise).ReverseNumber();
            }
            else if (src < 1)
            {
                //乘以10^m
                //计算m
                string strSrc = src.ToString();   //strSrc形如0.000xxxxx
                int moveLength = 2;
                for (; strSrc[moveLength] == '0'; moveLength++) ;
                moveLength--;
                //此步之后10的指数m = moveLength

                string strSrcMerge = MovePoint(strSrc, moveLength);
                PreciseNumber srcMerge = new PreciseNumber(strSrcMerge);
                //srcMerge为乘以10^m之后的数字

                //递归调用，注意要保证精度在乘以10^m之后仍有decPrecise位
                PreciseNumber srcMergeReciprocal = Reciprocal(srcMerge, decPrecise + moveLength);

                //再乘以10^m
                return new PreciseNumber(MovePoint(srcMergeReciprocal.ToString(), moveLength));
            }
            else if (src == 1)
            {
                return new PreciseNumber("1");
            }
            else if (src >= 10)
            {
                //除以10^m
                //m=src的整数位数-1
                int moveLength = -src.IntFigureNumber() + 1;
                PreciseNumber srcMerge = new PreciseNumber(MovePoint(src.ToString(), moveLength));
                //在对1000000000等很大的数求倒数时，
                PreciseNumber srcMergeReciprocal = Reciprocal(srcMerge, decPrecise + moveLength);
                return new PreciseNumber(MovePoint(srcMergeReciprocal.ToString(), moveLength));
            }
            else
            {
                //选取初值
                PreciseNumber starter;
                PreciseNumber error;
                if (src < 2)
                {
                    starter = new PreciseNumber("0.75");
                    error = new PreciseNumber("0.25");
                }
                else if (src < 4)
                {
                    starter = new PreciseNumber("0.375");
                    error = new PreciseNumber("0.125");
                }
                else if (src < 8)
                {
                    starter = new PreciseNumber("0.1875");
                    error = new PreciseNumber("0.0625");
                }
                else
                {
                    starter = new PreciseNumber("0.1");
                    error = new PreciseNumber("0.025");
                }

                //确定迭代次数
                int loopTimes = 0;
                string strZero = new string('0', decPrecise);
                PreciseNumber pnPrecise = new PreciseNumber("0." + strZero + "5");
                for (; pnPrecise < error; loopTimes++)
                {
                    error = error * error * src;
                    //此处可以加入精度控制，也可以将下一行注释掉，
                    //节省精度控制的时间而使得乘法运算位数增多
                    error = IdentifyNumber.RoundPrecise(error, decPrecise + 1);
                }

                //迭代
                for (int i = 0; i < loopTimes; i++)
                {
                    starter = starter * (2 - src * starter);
                    //此处也可以取消精度控制。
                    //实际检验发现，此处和上面的精度保留也可以不加1，尤其是在decPrecise较大时，
                    //可以证明不加1是可以保证误差不累积的
                    starter = IdentifyNumber.RoundPrecise(starter, decPrecise + 1);
                }
                //此处的精度控制是必要的
                starter = IdentifyNumber.RoundPrecise(starter, decPrecise);
                return starter;
            }
        }

        public static string MovePoint(string strSrc, int m)
        {
            int k = strSrc.IndexOf('.');
            if (k == -1)
            {
                k = strSrc.Length;
                strSrc = strSrc.Insert(k, ".");
            }
            int l = strSrc.Length;

            if (k + m < 0)
            {
                //需要向前补零
                string strZero = new string('0', -k - m);
                return "0." + strZero + strSrc.Remove(k, 1);
            }
            else if (k + m >= l - 1)
            {
                //需要向后补零
                string strZero = new string('0', k + m - l + 1);
                return strSrc.Remove(k, 1) + strZero;
            }
            else
            {
                //直接移动小数点
                return strSrc.Remove(k, 1).Insert(k + m, ".");
            }
        }
        /// <summary>
        /// 对PreciseNumber除以2
        /// </summary>
        /// <param name="src">源数据</param>
        /// <returns>计算结果</returns>
        public static PreciseNumber DivideTwo(PreciseNumber src)
        {
            if (src.IsZero())
            {
                //待计算数为0
                return new PreciseNumber();
            }
            else
            {
                //待计算数既有整数部分又有小数部分
                //将整数与小数拼接
                List<int> srcAll = new List<int>(src.IntPart);
                srcAll.AddRange(src.DecPart);

                //整体除以2
                List<int> rstAll = DivideListTwo(srcAll);

                //整数部分的组个数在调用DivideListTwo时不会改变
                List<int> rstIntPart = rstAll.GetRange(0, src.IntPart.Count);
                rstAll.RemoveRange(0, src.IntPart.Count);

                //去掉整数开头的零（如果整数部分不为0）
                if (rstIntPart.Count > 1 && rstIntPart[0] == 0)
                {
                    rstIntPart.RemoveAt(0);
                }

                //返回值。如果不写精度，则会调用默认精度计算函数，可能会降低效率
                return new PreciseNumber(rstIntPart, rstAll, true, rstAll.Count * PreciseNumber.OneCount);
            }
        }
        /// <summary>
        /// 对一列整数除以2
        /// </summary>
        /// <param name="list">待运算整数列</param>
        /// <returns>运算结果整数列</returns>
        private static List<int> DivideListTwo(List<int> list)
        {
            List<int> result = new List<int>(list);
            if (list.Count == 0)
            {
                throw new Exception("请避免在DivideListTwo函数中输入空列表");
            }
            int carry = 0;
            //对每组数除以2，使用位运算加快速度
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = (carry + list[i]) >> 1; //除以2
                if ((list[i] & 1) == 1)
                {
                    //list[i]为奇数
                    carry = PreciseNumber.MaxValue;
                }
                else
                {
                    carry = 0;
                }
            }
            if (carry != 0)
            {
                result.Add(PreciseNumber.MaxValue / 2);
            }
            return result;
        }
        #endregion

    }
}
