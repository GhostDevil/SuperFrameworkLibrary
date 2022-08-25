using System;

namespace SuperFramework.SuperThread
{
    /// <summary>
    /// 实现了对数学表达式的解析和计算(加减乘除、正弦余弦、正切余切、平方根、求幂。)
    /// </summary>
    public static class FormulaDeal
    {

        /// <summary>
        /// 分级计算
        /// </summary>
        /// <param name="strExpression"></param>
        /// <returns></returns>
        public static string SpiltExpression(string strExpression)
        {
            while (strExpression.IndexOf("(") != -1)
            {
                string strTemp = strExpression.Substring(strExpression.LastIndexOf("(") + 1, strExpression.Length - strExpression.LastIndexOf("(") - 1);
                string strExp = strTemp.Substring(0, strTemp.IndexOf(")"));
                strExpression = strExpression.Replace("(" + strExp + ")", CalculateExpress(strExp).ToString());
            }
            if (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                strExpression = CalculateExpress(strExpression).ToString();
            }
            return strExpression;
        }
        

        /// <summary>
        /// 运算方式
        /// </summary>
        public enum EnumFormula
        {
            /// <summary>
            /// 加号
            /// </summary>
            Add,//加号
            /// <summary>
            /// 减号
            /// </summary>
            Dec,//减号
            /// <summary>
            /// 乘号
            /// </summary>
            Mul,//乘号
            /// <summary>
            /// 除号
            /// </summary>
            Div,//除号
            /// <summary>
            /// 正玄
            /// </summary>
            Sin,//正玄
            /// <summary>
            /// 余玄
            /// </summary>
            Cos,//余玄
            /// <summary>
            /// 正切
            /// </summary>
            Tan,//正切
            /// <summary>
            /// 余切
            /// </summary>
            ATan,//余切
            /// <summary>
            /// 平方根
            /// </summary>
            Sqrt,//平方根
            /// <summary>
            /// 求幂
            /// </summary>
            Pow,//求幂
            /// <summary>
            /// 无
            /// </summary>
            None,//无
        }
        /// <summary>
        /// 计算表达式的结果
        /// </summary>
        /// <param name="strExpression">运算程式</param>
        /// <returns>返回运算结果</returns>
        public static double CalculateExpress(string strExpression)
        {
            while (strExpression.IndexOf("+") != -1 || strExpression.IndexOf("-") != -1
            || strExpression.IndexOf("*") != -1 || strExpression.IndexOf("/") != -1)
            {
                string strTemp;
                string strTempB;
                string strOne;
                string strTwo;
                double ReplaceValue;
                if (strExpression.IndexOf("*") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("*") + 1, strExpression.Length - strExpression.IndexOf("*") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("*"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDouble(GetExpType(strOne)) * Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "*" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("/") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("/") + 1, strExpression.Length - strExpression.IndexOf("/") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("/"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDouble(GetExpType(strOne)) / Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "/" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("+") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("+") + 1, strExpression.Length - strExpression.IndexOf("+") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("+"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);

                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDouble(GetExpType(strOne)) + Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "+" + strTwo, ReplaceValue.ToString());
                }
                else if (strExpression.IndexOf("-") != -1)
                {
                    strTemp = strExpression.Substring(strExpression.IndexOf("-") + 1, strExpression.Length - strExpression.IndexOf("-") - 1);
                    strTempB = strExpression.Substring(0, strExpression.IndexOf("-"));
                    strOne = strTempB.Substring(GetPrivorPos(strTempB) + 1, strTempB.Length - GetPrivorPos(strTempB) - 1);


                    strTwo = strTemp.Substring(0, GetNextPos(strTemp));
                    ReplaceValue = Convert.ToDouble(GetExpType(strOne)) - Convert.ToDouble(GetExpType(strTwo));
                    strExpression = strExpression.Replace(strOne + "-" + strTwo, ReplaceValue.ToString());
                }
            }
            return Convert.ToDouble(strExpression);
        }
        /// <summary>
        /// 计算三角函数
        /// </summary>
        /// <param name="strExpression">运算程式</param>
        /// <returns>返回结果</returns>
        private static string GetExpType(string strExpression)
        {
            strExpression = strExpression.ToUpper();
            if (strExpression.IndexOf("SIN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Sin).ToString();
            }
            if (strExpression.IndexOf("COS") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("S") + 1, strExpression.Length - 1 - strExpression.IndexOf("S")), EnumFormula.Cos).ToString();
            }
            if (strExpression.IndexOf("TAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.Tan).ToString();
            }
            if (strExpression.IndexOf("ATAN") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("N") + 1, strExpression.Length - 1 - strExpression.IndexOf("N")), EnumFormula.ATan).ToString();
            }
            if (strExpression.IndexOf("SQRT") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("T") + 1, strExpression.Length - 1 - strExpression.IndexOf("T")), EnumFormula.Sqrt).ToString();
            }
            if (strExpression.IndexOf("POW") != -1)
            {
                return CalculateExExpress(strExpression.Substring(strExpression.IndexOf("W") + 1, strExpression.Length - 1 - strExpression.IndexOf("W")), EnumFormula.Pow).ToString();
            }
            return strExpression;
        }
        /// <summary>
        /// 计算三角函数
        /// </summary>
        /// <param name="strExpression"></param>
        /// <param name="ExpressType"></param>
        /// <returns></returns>
        private static double CalculateExExpress(string strExpression, EnumFormula ExpressType)
        {
            double retValue = 0;
            switch (ExpressType)
            {
                case EnumFormula.Sin:
                    retValue = Math.Sin(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Cos:
                    retValue = Math.Cos(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Tan:
                    retValue = Math.Tan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.ATan:
                    retValue = Math.Atan(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Sqrt:
                    retValue = Math.Sqrt(Convert.ToDouble(strExpression));
                    break;
                case EnumFormula.Pow:
                    retValue = Math.Pow(Convert.ToDouble(strExpression), 2);
                    break;
            }
            if (retValue == 0) return Convert.ToDouble(strExpression);
            return retValue;
        }


        private static int GetNextPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.IndexOf("+");
            ExpPos[1] = strExpression.IndexOf("-");
            ExpPos[2] = strExpression.IndexOf("*");
            ExpPos[3] = strExpression.IndexOf("/");
            int tmpMin = strExpression.Length;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMin > ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMin = ExpPos[count - 1];
                }
            }
            return tmpMin;
        }

        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <param name="strExpression">运算程式</param>
        /// <returns>返回优先级</returns>
        private static int GetPrivorPos(string strExpression)
        {
            int[] ExpPos = new int[4];
            ExpPos[0] = strExpression.LastIndexOf("+");
            ExpPos[1] = strExpression.LastIndexOf("-");
            ExpPos[2] = strExpression.LastIndexOf("*");
            ExpPos[3] = strExpression.LastIndexOf("/");
            int tmpMax = -1;
            for (int count = 1; count <= ExpPos.Length; count++)
            {
                if (tmpMax < ExpPos[count - 1] && ExpPos[count - 1] != -1)
                {
                    tmpMax = ExpPos[count - 1];
                }
            }
            return tmpMax;

        }
        

    }
}

