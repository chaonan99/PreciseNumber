/*===============================
 * 类名：CompareNumber
 * 描述：比较两个PreciseNumber的类
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
    class CompareNumber
    {

        internal static int Compare(PreciseNumber former, PreciseNumber latter)
        {
            if (former.IsPositive && latter.IsPositive)
                return ComparePositive(former, latter);
            else if (!former.IsPositive && latter.IsPositive)
                return -1;
            else if (former.IsPositive && !latter.IsPositive)
                return 1;
            else
                return (-1) * ComparePositive(former, latter);
        }

        private static int ComparePositive(PreciseNumber former, PreciseNumber latter)
        {
            //通过整数组数比较大小
            if (former.IntPart.Count > latter.IntPart.Count)
                return 1;
            else if (former.IntPart.Count < latter.IntPart.Count)
                return -1;
            else
            {
                //整数组数相等，比较每一组大小
                for (int i = 0; i < former.IntPart.Count; i++)
                {
                    if (former.IntPart[i] > latter.IntPart[i])
                        return 1;
                    else if (former.IntPart[i] < latter.IntPart[i])
                        return -1;
                }
                //整数每组均相等，比较是否有小数
                if (former.DecPart.Count > 0 && latter.DecPart.Count == 0)
                    return 1;
                else if (former.DecPart.Count == 0 && latter.DecPart.Count > 0)
                    return -1;
                else if (former.DecPart.Count == 0 && latter.DecPart.Count == 0)
                    return 0;
                //都存在大于0的小数部分，比较小数
                int minDecimalPartCount =
                    former.DecPart.Count < latter.DecPart.Count ? former.DecPart.Count : latter.DecPart.Count;
                for (int i = 0; i < minDecimalPartCount; i++)
                {
                    //比较每一组小数直到至少一组不再有大于0的小数部分
                    if (former.DecPart[i] > latter.DecPart[i])
                        return 1;
                    else if (former.DecPart[i] < latter.DecPart[i])
                        return -1;
                }
                //比较剩余小数组数，仍有剩余小数组的大，直到最终二者都不存在多余小数组，则相等
                if (former.DecPart.Count > latter.DecPart.Count)
                    return 1;
                else if (former.DecPart.Count < latter.DecPart.Count)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
