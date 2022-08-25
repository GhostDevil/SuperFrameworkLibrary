using System.Collections.Generic;

namespace SuperFramework.SuperAlgorithm
{
    /// <summary>
    /// 分组算法
    /// </summary>
    public static class GroupHelper
    {
        #region 将一个数组进行按指定分组数分组

        /// <summary>
        /// 将一个数组进行按指定分组数分组
        /// </summary>
        /// <param name="strs">数组</param>
        /// <param name="groupCount">分组个数</param>
        /// <param name="isMerge">是否将于下的数组成员归为最后一组</param>
        /// <returns>返回分组后的泛型集合</returns>
        public static List<Group> GetGroup(string[] strs, int groupCount, bool isMerge = true)
        {
            List<Group> list = new List<Group>();
            if (strs.Length < groupCount)
            {
                Group gp = new Group() { GroupNo = "1" };
                for (int i = 0; i < strs.Length; i++)
                    gp.GroupStr += strs[i];
                list.Add(gp);
                return list;
            }
            else
            {
                int x = strs.Length % groupCount;
                int y = strs.Length / groupCount;
                int w = 0;
                for (int i = 1; i <= groupCount; i++)
                {
                    Group g = new Group() { GroupNo = (i).ToString() };
                    for (int j = 0; j < y; j++)
                    {
                        if (j > 0)
                            g.GroupStr = string.Format("{0},{1}", g.GroupStr, strs[w]);
                        else
                            g.GroupStr = strs[w];
                        w++;
                    }
                    list.Add(g);
                }
                if (x > 0)
                {

                    if (isMerge)
                    {
                        for (int i = x; i >= 1; i--)
                            list[list.Count - 1].GroupStr += strs[strs.Length - i];
                    }
                    else
                    {
                        Group gp = new Group() { GroupNo = (groupCount + 1).ToString() };
                        for (int i = x; i >= 1; i--)
                        {
                            if (i == x)
                                gp.GroupNo = strs[strs.Length - i];
                            else
                                gp.GroupStr = string.Format("{0},{1}", gp.GroupStr, strs[strs.Length - i]);
                        }
                        list.Add(gp);
                    }

                }

            }
            return list;
        }
        /// <summary>
        /// 分组对象类
        /// </summary>
        public class Group
        {
            /// <summary>
            /// 分组序号
            /// </summary>
            public string GroupNo { get; set; }
            /// <summary>
            /// 组内成员
            /// </summary>
            public string GroupStr { get; set; }
        }
        #endregion

    }
}
