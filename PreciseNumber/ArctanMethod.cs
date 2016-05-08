/*=======================
 * 类名：ArctanMethod
 * 描述：基于PreciseNumber的反正切求值算法
 * 
 * 作者：chaonan99
 * 创建日期：2015/12/10
 * 最后修改：2015/12/20
 ========================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreciseNumber
{
    class ArctanMethod
    {
        /// <summary>
        /// 泰勒展开法求arctan
        /// </summary>
        /// <param name="src">源数据</param>
        /// <param name="decPrecise">精度</param>
        /// <returns>反正切结果</returns>
        public static PreciseNumber ArcTan1(PreciseNumber src, int decPrecise)
        {
            //1.均转换为0~1之间的运算
            //
            if (!src.IsPositive)
            {
                //负数转正数
                return ArcTan1(src.AbsoluteNumber(), decPrecise).ReverseNumber();
            }
            else if (src.IsZero())
            {
                //0返回0
                return new PreciseNumber();
            }
            else if (src < 0.45)
            {
                //0~0.45执行运算
                PreciseNumber srcSquare = src * src;
                PreciseNumber srcEvenPower = new PreciseNumber(srcSquare);
                string strZero = new string('0', decPrecise);
                //方法误差限定在0.4*10^(-decPrecise)范围内，为舍入误差留出裕量
                PreciseNumber demandPrecise = new PreciseNumber("0." + strZero + "4");

                int loopTimes = 1;
                for (; BasicCalculate.Division(srcEvenPower, (2 * loopTimes + 1), decPrecise) >= demandPrecise; loopTimes++)
                {
                    srcEvenPower = srcEvenPower * srcSquare;
                    if (loopTimes < 0)
                    {
                        throw new Exception("迭代超过次数限制！");
                    }
                }
                //至此，loopTimes为需要迭代的次数n

                PreciseNumber loopTimePre = new PreciseNumber((2 * loopTimes + 1).ToString());
                PreciseNumber itemN = BasicCalculate.Reciprocal(loopTimePre, decPrecise);
                for (int i = loopTimes; i > 0; i--)
                {
                    PreciseNumber iPre = new PreciseNumber((2 * i - 1).ToString());
                    itemN = BasicCalculate.Reciprocal(iPre, decPrecise + 1) - srcSquare * itemN;
                    itemN = IdentifyNumber.RoundPrecise(itemN, decPrecise + 1);
                }
                return IdentifyNumber.RoundPrecise(src * itemN, decPrecise);
            }
            else if (src <= 1)
            {
                //0.45~1运用差角公式
                PreciseNumber result = ArcTan1(0.4, decPrecise + 1) +
                    ArcTan1(BasicCalculate.Division(src - 0.4, src * 0.4 + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }
            else
            {
                //大于1转换到0~1
                PreciseNumber result = ArcTan1(1, decPrecise + 1) +
                    ArcTan1(BasicCalculate.Division(src - 1, src + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }

            throw new NotImplementedException();
        }
        /// <summary>
        /// 数值积分法，复化辛普森公式求arctan
        /// </summary>
        /// <param name="src">源数据</param>
        /// <param name="decPrecise">精度</param>
        /// <returns>反正切结果</returns>
        public static PreciseNumber ArcTan2(PreciseNumber src,int decPrecise)
        {
            if (!src.IsPositive)
            {
                //负数转正数
                return ArcTan2(src.AbsoluteNumber(), decPrecise).ReverseNumber();
            }
            else if (src.IsZero())
            {
                //0返回0
                return new PreciseNumber();
            }
            else if (src < 0.45)
            {
                //计算分段数
                PreciseNumber seed;
                if (decPrecise % 4 == 0)
                {
                    seed = new PreciseNumber("0.335");
                }
                else if (decPrecise % 4 == 1)
                {
                    //0.335*10^0.25
                    seed = new PreciseNumber("0.595");
                }
                else if (decPrecise % 4 == 2)
                {
                    //0.335*10^0.5
                    seed = new PreciseNumber("1.058");
                }
                else
                {
                    //0.335*10^0.75
                    seed = new PreciseNumber("1.881");
                }
                seed = new PreciseNumber(BasicCalculate.MovePoint(seed.ToString(), decPrecise / 4));
                int minSegment = ((src * seed).IntPart[0] + 1) * 2; //分段数的2倍
                //这一步会有舍入误差并且将线性累计
                PreciseNumber stepLength = BasicCalculate.Division(src, minSegment,
                    decPrecise + minSegment / 10 + 2);
                PreciseNumber presentPosition = new PreciseNumber();
                PreciseNumber result = new PreciseNumber();

                for (int i = 0; i <= minSegment; i++)
                {
                    int k;
                    if (i == 0 || i == minSegment)
                    {
                        k = 1;
                    }
                    else if (i % 2 == 1)
                    {
                        k = 4;
                    }
                    else
                    {
                        k = 2;
                    }
                    result = result + k * BasicCalculate.Reciprocal(1 + presentPosition * presentPosition,
                        decPrecise + 1);
                    presentPosition = presentPosition + stepLength;

                }
                return BasicCalculate.Division(stepLength * result, 3, decPrecise);
            }
            else if (src <= 1)
            {
                //0.45~1运用差角公式
                PreciseNumber result = ArcTan2(0.4, decPrecise + 1) +
                    ArcTan2(BasicCalculate.Division(src - 0.4, src * 0.4 + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }
            else
            {
                //大于1转换到0~1
                PreciseNumber result = ArcTan2(1, decPrecise + 1) +
                    ArcTan2(BasicCalculate.Division(src - 1, src + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }

            throw new NotImplementedException();
        }
        /// <summary>
        /// 数值积分法，龙贝格算法
        /// </summary>
        /// <param name="src">源数据</param>
        /// <param name="decPrecise">精度</param>
        /// <returns>反正切结果</returns>
        public static PreciseNumber ArcTan3(PreciseNumber src, int decPrecise)
        {
            if (!src.IsPositive)
            {
                //负数转正数
                return ArcTan3(src.AbsoluteNumber(), decPrecise).ReverseNumber();
            }
            else if (src.IsZero())
            {
                //0返回0
                return new PreciseNumber();
            }
            else if (src < 0.45)
            {
                string strZero = new string('0', decPrecise);
                PreciseNumber demandPrecise = new PreciseNumber("0." + strZero + "36");
                List<PreciseNumber> t = new List<PreciseNumber>();
                PreciseNumber previousT0 = BasicCalculate.DivideTwo(src *
                    (1 + BasicCalculate.Reciprocal(1 + src * src, decPrecise + 2)));    //上一行T0
                PreciseNumber previousStep;    //前一次步长
                PreciseNumber presentStep = new PreciseNumber(src);     //本次步长
                PreciseNumber result;
                t.Add(previousT0);
                for (int i = 1; ; i++)
                {
                    //向下计算新的T0
                    PreciseNumber presentT0 = new PreciseNumber(); //本行T0
                    previousStep = new PreciseNumber(presentStep);
                    presentStep = BasicCalculate.DivideTwo(previousStep);
                    PreciseNumber presentPosi = new PreciseNumber(presentStep);
                    while (presentPosi < src)
                    {

                        presentT0 = presentT0 +
                            BasicCalculate.Reciprocal(1 + presentPosi * presentPosi, decPrecise + 2);
                        presentPosi = presentPosi + previousStep;
                    }
                    presentT0 = BasicCalculate.DivideTwo(previousT0 + presentT0 * previousStep);
                    previousT0 = new PreciseNumber(presentT0);
                    t.Add(presentT0);   //算得本行T0

                    //外推加速
                    PreciseNumber powFour = new PreciseNumber("4");
                    int m;
                    for (m = 0; m < i - 1; m++)
                    {
                        t[i - m - 1] = BasicCalculate.Division(powFour, powFour - 1, decPrecise + 3 + m / 10) * t[i - m] -
                            BasicCalculate.Reciprocal(powFour - 1, decPrecise + 3 + m / 10) * t[i - m - 1];
                        powFour = powFour * 4;
                    }
                    result = BasicCalculate.Division(powFour, powFour - 1, decPrecise + 3 + m / 10) * t[1] -
                        BasicCalculate.Reciprocal(powFour - 1, decPrecise + 3 + m / 10) * t[0];
                    if ((result - t[0]).AbsoluteNumber() <= demandPrecise)
                    {
                        break;
                    }
                    t[0] = result;
                }
                result = IdentifyNumber.RoundPrecise(result, decPrecise);
                return result;
            }
            else if (src <= 1)
            {
                //0.45~1运用差角公式
                PreciseNumber result = ArcTan3(0.4, decPrecise + 1) +
                    ArcTan3(BasicCalculate.Division(src - 0.4, src * 0.4 + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }
            else
            {
                //大于1转换到0~1
                PreciseNumber result = ArcTan3(1, decPrecise + 1) +
                    ArcTan3(BasicCalculate.Division(src - 1, src + 1, decPrecise + 1), decPrecise + 1);
                return IdentifyNumber.RoundPrecise(result, decPrecise);
            }

            throw new NotImplementedException();
        }

    }
}
