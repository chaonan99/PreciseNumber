/*===============================
 * 类名：IndentifyNumber
 * 描述：该类含有从字符串中提取PreciseNumber的方法，
 *       以及一些精度处理方法。
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/8
 * 最后修改：2015/12/17
 ================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace PreciseNumber
{
    class IdentifyNumber
    {
        #region 字符串中的PreciseNumber提取函数
        /// <summary>
        /// 从一个字符串中提取PreciseNumber
        /// </summary>
        /// <param name="text">字符串内容</param>
        /// <param name="intPart">整数部分，从左往右保存</param>
        /// <param name="decPart">小数部分，从左往右保存</param>
        /// <returns>精度和正负的信息</returns>
        internal static bool GetNumberFromString(string text, List<int> intPart, List<int> decPart, ref int decPrecise)
        {
            ValidateNumber(text);
            int haveSymbol = 0; //字符串是否具有符号位
            bool isPositive = true;
            decPrecise = 0;

            if (text[0] == '-')
            {
                haveSymbol = 1;
                isPositive = false;
            }
            else if (text[0] == '+')
            {
                haveSymbol = 1;
            }
            int pointIndex = text.IndexOf('.'); //小数点的位置
            //有小数点
            if (pointIndex == haveSymbol)
            {
                //小数点在开头
                text = text.Insert(haveSymbol, "0");
                pointIndex++;
            }
            else if (pointIndex == text.Length-1)
            {
                //小数点在末尾，去掉小数点
                text = text.Remove(text.Length - 1);
                pointIndex = -1;
            }
            if (pointIndex != -1)
            {
                //处理整数部分
                string integralString = text.Substring(haveSymbol, pointIndex - haveSymbol).TrimStart('0'); //将整数部分开头的0删除
                int headReminder = integralString.Length % PreciseNumber.OneCount;
                if (headReminder != 0)
                {
                    //保存非最大位数的开头部分
                    intPart.Add(Convert.ToInt32(integralString.Substring(
                        0, headReminder)));
                }
                if (integralString.Length >= PreciseNumber.OneCount)
                {
                    //剩余部分都以最大位数存储
                    for (int i = 0; i < integralString.Length / PreciseNumber.OneCount; i++)
                    {
                        intPart.Add(Convert.ToInt32(integralString.Substring(
                            i * PreciseNumber.OneCount + headReminder, PreciseNumber.OneCount)));
                    }
                }
                //处理小数部分
                string decimalString = text.Substring(pointIndex + 1);
                decPrecise = decimalString.Length;
                decimalString = decimalString.TrimEnd('0');
                int tailReminder = decimalString.Length % PreciseNumber.OneCount;
                if (decimalString.Length >= PreciseNumber.OneCount)
                {
                    for (int i = 0; i < decimalString.Length / PreciseNumber.OneCount; i++)
                    {
                        decPart.Add(Convert.ToInt32(decimalString.Substring(
                            i * PreciseNumber.OneCount, PreciseNumber.OneCount)));
                    }
                }
                if (tailReminder != 0)
                {

                    decPart.Add(Convert.ToInt32(decimalString.Substring(
                        decimalString.Length - tailReminder, tailReminder).PadRight(PreciseNumber.OneCount, '0')));
                }
            }
            //没有小数点
            else
            {
                //处理整数部分
                string integralString = text.Substring(haveSymbol).TrimStart('0'); //将整数部分开头的0删除
                int headReminder = integralString.Length % PreciseNumber.OneCount;
                if (headReminder != 0)
                {
                    //保存非最大位数的开头部分
                    intPart.Add(Convert.ToInt32(integralString.Substring(
                        0, headReminder)));
                }
                if (integralString.Length >= PreciseNumber.OneCount)
                {
                    //剩余部分都以最大位数存储
                    for (int i = 0; i < integralString.Length / PreciseNumber.OneCount; i++)
                    {
                        intPart.Add(Convert.ToInt32(integralString.Substring(
                            i * PreciseNumber.OneCount + headReminder, PreciseNumber.OneCount)));
                    }
                }
            }

            //返回值
            return isPositive;
        }
        /// <summary>
        /// 检查一个字符串是否为合法的PreciseNumber。不是则抛出异常。
        /// </summary>
        /// <param name="text">待检查的字符串</param>
        private static void ValidateNumber(string text)
        {
            Match m = Regex.Match(text, @"[^\d\.\+-]"); //
            if (m.Success)
                throw new NumberException("含有非法字符", m.Index);
            Match mNum = Regex.Match(text, @"\d");
            if (!mNum.Success)
                throw new NumberException("至少要含有一个数字", 0);
            Match mPlug = Regex.Match(text, @"[\+-]", RegexOptions.RightToLeft);
            if (mPlug.Success && mPlug.Index != 0)
                throw new NumberException("正负号只能在开头", mPlug.Index);
            MatchCollection ms = Regex.Matches(text, @"[\.]");
            if (ms.Count > 1)
                throw new NumberException("不能有一个以上的小数点", ms[1].Index);
            //else if (ms.Count == 1 && (ms[0].Index == 0 || ms[0].Index == text.Length - 1))
            //    throw new NumberException("小数点不能在开头或最后一位", ms[0].Index);
            //Match mPoints = Regex.Match(text, @"[\+-]\.");
            //if (mPoints.Success)
            //    throw new NumberException("正负号后不能直接有小数点", mPoints.Index);
        }

        #endregion

        #region 精度处理相关函数
        /// <summary>
        /// 返回给定小数部分的默认精度（尾部0全部去掉）
        /// </summary>
        /// <param name="decPart">小数部分</param>
        /// <returns>默认精度</returns>
        internal static int DefaultPrecise(List<int> decPart)
        {
            if ((object)decPart == null || decPart.Count == 0)
            {
                return 0;
            }
            for (int tail = decPart.Last(); tail == 0 && decPart.Count > 1; tail = decPart.Last())
            {
                decPart.RemoveAt(decPart.Count - 1);
            }
            if (decPart.Count == 1 && decPart[0] == 0)
            {
                return 0;
            }
            string tailNumber = decPart.Last().ToString().PadLeft(PreciseNumber.OneCount, '0').TrimEnd('0');
            return tailNumber.Length + (decPart.Count - 1) * PreciseNumber.OneCount;
        }

        #endregion

        #region 四舍五入相关函数
        /// <summary>
        /// 根据小数部分精度四舍五入
        /// </summary>
        /// <param name="src">源数据</param>
        /// <param name="decPrecise">小数部分精度</param>
        /// <returns>保留给定精度之后的数据</returns>
        public static PreciseNumber RoundPrecise(PreciseNumber src, int decPrecise)
        {
            #region 异常情况

            if (decPrecise < 0)
            {
                throw new NumberException("精度不能为负", 0);
            }

            #endregion

            #region 数字字符串化

            StringBuilder sbInt = new StringBuilder();
            StringBuilder sbDec = new StringBuilder();
            string strInt = "";
            string strDec = "";
            foreach (int i in src.IntPart)
            {
                sbInt.Append(i.ToString().PadLeft(PreciseNumber.OneCount, '0'));
            }
            strInt = sbInt.ToString().TrimStart('0');
            //至此，strInt以标准的字符串表示了src的整数部分
            //整数部分为0时，strInt为空字符串
            if (src.DecPrecise > 0)
            {
                if (src.DecPart.Count > 0)
                {
                    int j = 0;
                    if (src.DecPart.Count > 1)
                    {
                        for (; j < src.DecPart.Count - 1; j++)
                        {
                            //左侧补零
                            sbDec.Append(src.DecPart[j].ToString().PadLeft(PreciseNumber.OneCount, '0'));
                        }
                    }
                    //左侧补零，右侧补零至精度
                    sbDec.Append(src.DecPart[j].ToString().PadLeft(PreciseNumber.OneCount, '0').TrimEnd('0'));
                }
                sbDec.Append('0', src.DecPrecise - sbDec.Length);
                strDec = sbDec.ToString();
            }
            //至此，strDec以标准的字符串表示了src的小数部分。小数部分可能为空。

            #endregion

            #region 四舍五入

            if (decPrecise >= strDec.Length)
            {
                //改换精度并返回
                return new PreciseNumber(src.IntPart, src.DecPart, src.IsPositive, decPrecise);
            }

            string strRstNoPoint;
            if (strDec[decPrecise] >= '5')
            {
                PreciseNumber temp = new PreciseNumber(strInt + strDec.Substring(0, decPrecise)) + 1;
                strRstNoPoint = temp.ToString();
            }
            else
            {
                strRstNoPoint = strInt + strDec.Substring(0, decPrecise);
            }

            #endregion

            #region 加小数点

            if (decPrecise == 0)
            {
                //精度为0，不需要加小数点
                return new PreciseNumber(strRstNoPoint);
            }

            //需要加小数点
            if (strRstNoPoint.Length <= decPrecise)
            {
                //需要补零后再加小数点
                string strZero = new string('0', decPrecise - strRstNoPoint.Length);
                return new PreciseNumber("0." + strZero + strRstNoPoint);
            }
            else
            {
                //直接加小数点
                return new PreciseNumber(strRstNoPoint.Insert(strRstNoPoint.Length - decPrecise, "."));
            }

            #endregion
        }
        /// <summary>
        /// 根据有效数字位数四舍五入
        /// </summary>
        /// <param name="src">源数据</param>
        /// <param name="sig">有效数字位数</param>
        /// <returns>保留有效数字位之后的数据</returns>
        public static PreciseNumber RoundSigFigure(PreciseNumber src, int sig)
        {
            #region 排除特殊情况，接下来sig>=1
            if (sig < 0)
            {
                //有效数字位数为负，抛出异常
                throw new NumberException("有效数字位数不能小于零", 0);
            }
            else if (sig == 0)
            {
                //保留0位有效数字（这里也可以考虑抛出异常）
                return new PreciseNumber();
            }
            #endregion

            #region 数字的字符串化
            StringBuilder sbInt = new StringBuilder();
            StringBuilder sbDec = new StringBuilder();
            string strInt = "";
            string strDec = "";
            foreach (int i in src.IntPart)
            {
                sbInt.Append(i.ToString().PadLeft(PreciseNumber.OneCount, '0'));
            }
            strInt = sbInt.ToString().TrimStart('0');
            //至此，strInt以标准的字符串表示了src的整数部分
            //整数部分为0时，strInt为空字符串
            if (src.DecPrecise > 0)
            {
                if (src.DecPart.Count > 0)
                {
                    int j = 0;
                    if (src.DecPart.Count > 1)
                    {
                        for (; j < src.DecPart.Count - 1; j++)
                        {
                            //左侧补零
                            sbDec.Append(src.DecPart[j].ToString().PadLeft(PreciseNumber.OneCount, '0'));
                        }
                    }
                    //左侧补零，右侧补零至精度
                    sbDec.Append(src.DecPart[j].ToString().PadLeft(PreciseNumber.OneCount, '0').TrimEnd('0'));
                }
                sbDec.Append('0', src.DecPrecise - sbDec.Length);
                strDec = sbDec.ToString();
            }
            //至此，strDec以标准的字符串表示了src的小数部分。小数部分可能为空。

            //strAll将整数、小数部分连接
            string strAll = (strInt + strDec).TrimStart('0');

            #endregion

            #region 四舍五入运算与返回情况

            if (sig >= strAll.Length)
            {
                //有效数字位数大于总位数，直接返回原数
                //这里可以考虑抛出异常
                return src;
            }

            string strRstNoPoint;   //不带小数点的最终结果
            if (strAll[sig]>='5')
            {
                //五入
                PreciseNumber temp = new PreciseNumber(strAll.Remove(sig)) + 1;
                strRstNoPoint = temp.ToString();
            }
            else
            {
                //四舍
                strRstNoPoint = strAll.Remove(sig);
            }

            if (sig<=strInt.Length)
            {
                //不需要加小数点
                return new PreciseNumber(strRstNoPoint.Remove(sig));
            }
            else
            {
                //需要加小数点
                if (strAll.Length + strRstNoPoint.Length <= strDec.Length + sig)
                {
                    //需要向前补零再加小数点
                    string strZero = new string('0', strDec.Length - strAll.Length + sig - strRstNoPoint.Length);
                    return new PreciseNumber("0." + strZero + strRstNoPoint.Substring(0, sig));
                }
                else
                {
                    //不需要补零
                    return new PreciseNumber(strRstNoPoint.Substring(0, sig).Insert(
                        strAll.Length - strDec.Length + strRstNoPoint.Length - sig, "."));
                }
            }

            #endregion
        }

        #endregion

    }
}
