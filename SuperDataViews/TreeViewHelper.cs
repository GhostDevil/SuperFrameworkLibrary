using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SuperFramework.SuperDataViews
{
    /// <summary>
    /// treeview常用方法集合
    /// </summary>
    public class TreeViewHelper
    {
        #region  改变节点的选中状态，此处为所有子节点不选中时才取消父节点选中(TreeView_AfterCheck事件) 
        /// <summary>
        /// 系列节点 Checked 属性控制
        /// </summary>
        /// <param name="e">TreeViewEventArgs对象</param>
        /// <param name="trueColor">选中文字颜色</param>
        /// <param name="falseColor">未选中文字颜色</param>
        public static void CheckNodeState(TreeViewEventArgs e, Color trueColor, Color falseColor)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node != null && !Convert.IsDBNull(e.Node))
                {
                    CheckParentNode(e.Node, trueColor, falseColor);
                    if (e.Node.Nodes.Count > 0)
                        CheckAllChildNodes(e.Node, e.Node.Checked, trueColor, falseColor);
                }
            }
        }
        #endregion

        #region  改变所有子节点的状态 
        /// <summary>
        /// 改变所有子节点的状态
        /// </summary>
        /// <param name="pn">父节点</param>
        /// <param name="IsChecked">状态</param>
        /// <param name="trueColor">选中文字颜色</param>
        /// <param name="falseColor">未选中文字颜色</param>
        public static void CheckAllChildNodes(TreeNode pn, bool IsChecked, Color trueColor, Color falseColor)
        {
            foreach (TreeNode tn in pn.Nodes)
            {
                tn.Checked = IsChecked;
                if (IsChecked)
                    tn.ForeColor = trueColor;
                else
                    tn.ForeColor = falseColor;
                if (tn.Nodes.Count > 0)
                {
                    CheckAllChildNodes(tn, IsChecked, trueColor, falseColor);
                }
            }
        }
        #endregion

        #region  改变父节点的选中状态，此处为所有子节点不选中时才取消父节点选中 
        /// <summary>
        /// 改变父节点的选中状态，此处为所有子节点不选中时才取消父节点选中
        /// </summary>
        /// <param name="curNode">节点</param>
        /// <param name="trueColor">选中文字颜色</param>
        /// <param name="falseColor">未选中文字颜色</param>
        public static void CheckParentNode(TreeNode curNode, Color trueColor, Color falseColor)
        {
            bool bChecked = false;
            if (curNode.Parent != null)
            {
                foreach (TreeNode node in curNode.Parent.Nodes)
                {
                    if (node.Checked)
                    {
                        bChecked = true;
                        break;
                    }
                }
                if (bChecked)
                {
                    curNode.Parent.Checked = true;
                    curNode.ForeColor = trueColor;
                    CheckParentNode(curNode.Parent, trueColor, falseColor);
                }
                else
                {
                    curNode.Parent.Checked = false;
                    curNode.ForeColor = falseColor;
                    CheckParentNode(curNode.Parent, trueColor, falseColor);
                }
            }
        }
        #endregion

        #region  选中节点，选中相应的全部子节(TreeView_AfterCheck事件) 
        /// <summary>
        /// 选中节点，选中相应的全部子节(TreeView_AfterCheck事件)
        /// </summary>
        /// <param name="e">TreeViewEventArgs</param>
        /// <param name="ChkFreeColor">选中文字颜色</param>
        /// <param name="UnChkColor">未选中文字颜色</param>
        public static void TreeView_AfterCheck(TreeViewEventArgs e, Color trueColor, Color falseColor)
        {

            if (e.Action == TreeViewAction.ByMouse)
            {
                if (e.Node.Checked)
                {
                    //节点选中状态之后，所有父节点选中
                    setChildNodeCheckedState(e.Node, true, trueColor);
                    if (e.Node.Parent != null)
                        setParentNodeCheckedState(e.Node, true, trueColor);
                }
                else
                {
                    //取消节点选中状态之后，取消所有父节点选中
                    setChildNodeCheckedState(e.Node, false, falseColor);
                    if (e.Node.Parent != null)
                        setParentNodeCheckedState(e.Node, false, falseColor);
                }
            }
        }
        #endregion

        #region  取消节点选中状态之后，取消所有父节点的选中状态 
        /// <summary>
        ///取消节点选中状态之后，取消所有父节点的选中状态
        /// </summary>
        /// <param name="currNode">节点</param>
        /// <param name="state">状态</param>
        /// <param name="foreColor">文字颜色</param>
        private static void setParentNodeCheckedState(TreeNode currNode, bool state, Color foreColor)
        {
            TreeNode parentNode = currNode.Parent;
            parentNode.Checked = state;
            parentNode.ForeColor = foreColor;
            if (currNode.Parent.Parent != null)
                setParentNodeCheckedState(currNode.Parent, state, foreColor);
        }
        #endregion

        #region  选中节点之后，选中节点的所有子节点 
        /// <summary>
        ///选中节点之后，选中节点的所有子节点
        /// </summary>
        /// <param name="currNode">节点</param>
        /// <param name="state">状态</param>
        /// <param name="foreColor">文字颜色</param>

        public static void setChildNodeCheckedState(TreeNode currNode, bool state, Color foreColor)
        {
            TreeNodeCollection nodes = currNode.Nodes;
            if (nodes.Count > 0)
            {
                foreach (TreeNode tn in nodes)
                {
                    tn.Checked = state;
                    tn.ForeColor = foreColor;
                    setChildNodeCheckedState(tn, state, foreColor);
                }
            }
        }
        #endregion

        #region  查找treeview中的node节点（递归算法-效率低于非递归算法） 
        /// <summary>
        /// 查找treeview中的node节点（递归算法-效率低于非递归算法）
        /// </summary>
        /// <param name="tnParent">父节点</param>
        /// <param name="strValue">查找节点</param>
        /// <returns>TreeNode</returns>
        public static TreeNode FindNode(TreeNode tnParent, string strValue)
        {
            if (tnParent == null) return null;
            if (tnParent.Text == strValue) return tnParent;
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                tnRet = FindNode(tn, strValue);
                if (tnRet != null) break;
            }
            return tnRet;
        }
        #endregion

        #region  查找treeview中的node节点（非递归算法-效率高于递归算法） 
        /// <summary>
        /// 查找treeview中的node节点（非递归算法-效率高于递归算法）
        /// </summary>
        /// <param name="tnParent">父节点</param>
        /// <param name="strValue">查找节点</param>
        /// <returns>TreeNode</returns>
        public static TreeNode FastFindNode(TreeNode tnParent, string strValue)
        {
            if (tnParent == null) return null;
            if (tnParent.Text == strValue) return tnParent;
            else if (tnParent.Nodes.Count == 0) return null;
            TreeNode tnCurrent, tnCurrentPar;
            //Init node
            tnCurrentPar = tnParent;
            tnCurrent = tnCurrentPar.FirstNode;
            while (tnCurrent != null && tnCurrent != tnParent)
            {
                while (tnCurrent != null)
                {
                    if (tnCurrent.Text == strValue) return tnCurrent;
                    else if (tnCurrent.Nodes.Count > 0)
                    {
                        //Go into the deepest node in current sub-path
                        tnCurrentPar = tnCurrent;
                        tnCurrent = tnCurrent.FirstNode;
                    }
                    else if (tnCurrent != tnCurrentPar.LastNode)
                    {
                        //Goto next sible node 
                        tnCurrent = tnCurrent.NextNode;
                    }
                    else
                        break;
                }
                //Go back to parent node till its has next sible node
                while (tnCurrent != tnParent && tnCurrent == tnCurrentPar.LastNode)
                {
                    tnCurrent = tnCurrentPar;
                    tnCurrentPar = tnCurrentPar.Parent;
                }
                //Goto next sible node
                if (tnCurrent != tnParent)
                    tnCurrent = tnCurrent.NextNode;
            }
            return null;
        }
        #endregion

        #region  遍历TreeView节点，并将二级节点及其子节点存入集合中 
        /// <summary>  
        /// 遍历TreeView节点，并将二级节点及其子节点存入集合中  
        /// </summary>  
        /// <param name="node">TreeView节点</param>   
        public static List<TreeViewNodes> GetAllNodes(TreeNode node)
        {
            List<TreeViewNodes> nodes = new List<TreeViewNodes>();
            List<TreeViewItems> items = new List<TreeViewItems>();
            foreach (TreeNode itemNode in node.Nodes)
            {
                TreeViewNodes no = new TreeViewNodes();
                no.ParentNode = itemNode;
                foreach (TreeNode item in itemNode.Nodes)
                {
                    TreeViewItems ti = new TreeViewItems();
                    ti.SecondParentNode = item;
                    if (item.Nodes.Count > 0)
                        GetChildNodes(item, ti.ChildNodes);

                    else
                        ti.ChildNodes.Add(item);
                    items.Add(ti);
                }
                nodes.Add(no);
            }
            return nodes;
        }
        #endregion

        #region  遍历TreeView节点，并将子节点存入集合中 
        /// <summary>  
        /// 遍历TreeView节点，并将子节点存入集合中  
        /// </summary>  
        /// <param name="TreeNode">TreeView节点</param>
        /// <param name="nodeList">List对象</param>
        public static List<TreeNode> GetChildNodes(TreeNode node, List<TreeNode> nodeList)
        {
            foreach (TreeNode itemNode in node.Nodes)
            {
                nodeList.Add(itemNode);
                GetChildNodes(itemNode, nodeList);
            }
            return nodeList;
        }
        #endregion

        #region  获得Treeview的所有Check（true或false）项（递归算法） 
        /// <summary>
        /// 获得Treeview的所有Check（true或false）项（递归算法）
        /// </summary>
        /// <param name="nodes">节点</param>
        /// <param name="selectedNodes">接收返回结果List对象</param>
        /// <param name="check">节点状态</param>
        /// <param name="isParent">是否包含父节点</param> 
        public static void GetSelectedTreeNode(TreeNode nodes, List<TreeNode> selectedNodes, bool check = true, bool isParent = false)
        {
            if (nodes.Checked == check && isParent)
                selectedNodes.Add(nodes);
            foreach (TreeNode node in nodes.Nodes)
            {
                if (node.Checked == check)
                    selectedNodes.Add(node);
                if (node.Nodes.Count > 0)
                    GetSelectedTreeNode(node, selectedNodes, check, isParent);
            }
        }
        #endregion

        #region  选中值为value的节点（及其子节点） 
        /// <summary>
        /// 选中值为value的节点（及其子节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="value">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void CheckChildLeaves(TreeNode tnode, string value, Color foreColor)
        {
            try
            {
                //找到节点，把节点及其子节点都Check
                if (tnode.Text.Trim() == value.Trim())
                {
                    tnode.Checked = true;
                    tnode.ForeColor = foreColor;
                    foreach (TreeNode tnchild in tnode.Nodes)
                    {
                        CheckChildLeaves(tnchild, tnchild.Text, foreColor);
                    }
                    return;
                }
                //找不到节点，向子节点找
                foreach (TreeNode tnchild in tnode.Nodes)
                {
                    CheckChildLeaves(tnchild, value.Trim(), foreColor);
                }
            }
            catch { throw; }
        }
        #endregion

        #region  取消选中值为value的节点（及其子节点） 
        /// <summary>
        /// 取消选中值为value的节点（及其子节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="value">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void UnCheckChildLeaves(TreeNode tnode, string value, Color foreColor)
        {
            try
            {
                //找到节点，把节点及其子节点都UnCheck
                if (tnode.Text.Trim() == value.Trim())
                {
                    tnode.Checked = false;
                    tnode.ForeColor = foreColor;
                    foreach (TreeNode tnchild in tnode.Nodes)
                    {
                        UnCheckChildLeaves(tnchild, tnode.Text.Trim(), foreColor);
                    }
                    return;
                }
                //找不到节点，向子节点找
                foreach (TreeNode tnchild in tnode.Nodes)
                {
                    UnCheckChildLeaves(tnchild, value.Trim(), foreColor);
                }
            }
            catch { throw; }
        }
        #endregion

        #region  取消选中值为value的节点（及其父节点） 
        /// <summary>
        /// 取消选中值为value的节点（及其父节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="value">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void UnCheckParentLeaves(TreeNode tnode, string value, Color foreColor)
        {
            try
            {
                TreeNode tnparent = tnode.Parent;
                if (tnode.Text == "") { return; }
                //找到节点，把节点及其子节点都UnCheck
                if (tnode.Text.Trim() == value.Trim())
                {
                    tnode.Checked = false;
                    tnode.ForeColor = foreColor;
                    UnCheckParentLeaves(tnparent, tnode.Text.Trim(), foreColor);
                    return;
                }
                //找不到节点，向子节点找
                UnCheckParentLeaves(tnparent, value.Trim(), foreColor);
            }
            catch { throw; }
        }
        #endregion

        #region  选中Tag为tag的节点（及其子节点） 
        /// <summary>
        /// 选中Tag为tag的节点（及其子节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="tag">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void CheckChildLeaves(TreeNode tnode, object tag, Color foreColor)
        {
            try
            {
                //找到节点，把节点及其子节点都Check
                if (tnode.Tag == tag)
                {
                    tnode.Checked = true;
                    tnode.ForeColor = foreColor;
                    foreach (TreeNode tnchild in tnode.Nodes)
                    {
                        CheckChildLeaves(tnchild, tnode.Tag, foreColor);
                    }
                    return;
                }
                //找不到节点，向子节点找
                foreach (TreeNode tnchild in tnode.Nodes)
                {
                    CheckChildLeaves(tnchild, tag, foreColor);
                }
            }
            catch { throw; }
        }
        #endregion

        #region  取消选中Tag为tag的节点（及其子节点） 
        /// <summary>
        /// 取消选中Tag为tag的节点（及其子节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="tag">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void UnCheckChildLeaves(TreeNode tnode, object tag, Color foreColor)
        {
            try
            {
                //找到节点，把节点及其子节点都UnCheck
                if (tnode.Tag == tag)
                {
                    tnode.Checked = false;
                    tnode.ForeColor = foreColor;
                    foreach (TreeNode tnchild in tnode.Nodes)
                    {
                        UnCheckChildLeaves(tnchild, tnode.Tag, foreColor);
                    }
                    return;
                }
                //找不到节点，向子节点找
                foreach (TreeNode tnchild in tnode.Nodes)
                {
                    UnCheckChildLeaves(tnchild, tag, foreColor);
                }
            }
            catch { throw; }
        }
        #endregion

        #region  取消选中Tag为tag的节点（及其父节点） 
        /// <summary>
        /// 取消选中Tag为tag的节点（及其父节点）
        /// </summary>
        /// <param name="tnode">被判断节点</param>
        /// <param name="tag">判断值</param>
        /// <param name="foreColor">文字颜色</param>
        public static void UnCheckParentLeaves(TreeNode tnode, object tag, Color foreColor)
        {
            try
            {
                TreeNode tnparent = tnode.Parent;
                if (tnode.Tag == null) { return; }
                //找到节点，把节点及其子节点都UnCheck
                if (tnode.Tag == tag)
                {
                    tnode.Checked = false;
                    tnode.ForeColor = foreColor;
                    UnCheckParentLeaves(tnparent, tnode.Tag, foreColor);
                    return;
                }
                //找不到节点，向子节点找
                UnCheckParentLeaves(tnparent, tag, foreColor);
            }
            catch { throw; }
        }
        #endregion

        #region  设置TreeView鼠标拖动TreeView节点(自动注册事件：ItemDrag，DragEnter，DragDrop) 
        public static TreeView tView = null;
        /// <summary>
        /// 设置TreeView鼠标拖动TreeView节点(自动注册事件：ItemDrag，DragEnter，DragDrop)
        /// </summary>
        /// <param name="tv">TreeView</param>
        public static void SetTreeViewItemDrag(TreeView tv)
        {
            tView = tv;
            tv.AllowDrop = true;
            tv.ItemDrag += new ItemDragEventHandler(tv_ItemDrag);
            tv.DragEnter += new DragEventHandler(tv_DragEnter);
            tv.DragDrop += new DragEventHandler(tv_DragDrop);
        }
        #endregion

        #region  取消TreeView鼠标拖动TreeView节点(自动注销事件：ItemDrag，DragEnter，DragDrop) 
        /// <summary>
        /// 取消TreeView鼠标拖动TreeView节点(自动注销事件：ItemDrag，DragEnter，DragDrop)
        /// </summary>
        /// <param name="tv">TreeView</param>
        public static void CancelTreeViewItemDrag(TreeView tv)
        {
            tView = null;
            tv.AllowDrop = true;
            tv.ItemDrag -= new ItemDragEventHandler(tv_ItemDrag);
            tv.DragEnter -= new DragEventHandler(tv_DragEnter);
            tv.DragDrop -= new DragEventHandler(tv_DragDrop);
        }
        #endregion

        #region  TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop) 
        /// <summary>
        /// TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop)
        /// </summary>
        /// <param name="sender">object对象</param>
        /// <param name="e">DragEventArgs对象</param>
        public static void tv_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode moveNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode");
            //根据鼠标坐标确定要移动到的目标节点
            Point pt;
            TreeNode targeNode;
            pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            targeNode = tView.GetNodeAt(pt);
            if (targeNode == null)
            {
                // moveNode.Remove();
                return;
            }
            //如果目标节点无子节点则添加为同级节点,反之添加到下级节点的未端
            TreeNode NewMoveNode = (TreeNode)moveNode.Clone();
            //if (targeNode.Nodes.Count == 0)
            //    targeNode.Parent.Nodes.Insert(targeNode.Index, NewMoveNode);
            //else
            targeNode.Nodes.Insert(targeNode.Nodes.Count, NewMoveNode);
            //更新当前拖动的节点选择
            tView.SelectedNode = NewMoveNode;
            //展开目标节点,便于显示拖放效果
            targeNode.Expand();
            //移除拖放的节点
            moveNode.Remove();
        }
        #endregion

        #region  TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop) 
        /// <summary>
        /// TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop)
        /// </summary>
        /// <param name="sender">object对象</param>
        /// <param name="e">DragEventArgs对象</param>
        public static void tv_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }
        #endregion

        #region  TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop) 
        /// <summary>
        /// TreeView鼠标拖动TreeView节点(配合事件：ItemDrag，DragEnter，DragDrop)
        /// </summary>
        /// <param name="sender">object对象</param>
        /// <param name="e">DragEventArgs对象</param>
        static void tv_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                tView.DoDragDrop(e.Item, DragDropEffects.Move);
        }
        #endregion

        #region   反选子节点 
        /// <summary>
        /// 反选子节点
        /// </summary>
        /// <param name="treeNode">节点</param>
        /// <param name="nodeChecked">节点状态</param>
        public static void ReverseAllChildNodes(TreeNode treeNode, Color trueColor, Color falseColor)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                if (node.Checked)
                {
                    node.ForeColor = falseColor;
                    node.Checked = false;
                    node.Parent.Checked = false;
                    node.Parent.ForeColor = falseColor;
                }
                else
                {
                    node.ForeColor = trueColor;
                    node.Checked = true;
                    node.Parent.Checked = true;
                    node.Parent.ForeColor = trueColor;
                }
                if (node.Nodes.Count > 0)
                    ReverseAllChildNodes(node, trueColor, falseColor);
            }
        }
        #endregion

        #region  删除check(true 或 false)节点 
        /// <summary>
        /// 删除check(true 或 false)节点
        /// </summary>
        /// <param name="tv">TreeView对象</param>
        /// <returns></returns>
        public static void DeleteCheckNode(TreeView tv)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode item in tv.Nodes)
                GetSelectedTreeNode(item, nodes, true, true);
            foreach (TreeNode item in nodes)
            {
                try
                {
                    item.Remove();
                }
                catch { throw; }
            }
        }
        #endregion

        #region  更新节点 
        /// <summary>
        /// 更新节点
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="text">节点名称</param>
        /// <param name="tag">节点tag</param>
        /// <param name="imgindex">图片索引</param>
        public static void UpdateSelectNode(TreeNode node, string text = "", object tag = null, int imgindex = -1)
        {
            if (!string.IsNullOrWhiteSpace(text))
                node.Text = text;
            if (tag != null)
                node.Tag = tag;
            if (imgindex != -1)
                node.ImageIndex = imgindex;
        }
        #endregion

        #region  添加节点到指定位置 
        /// <summary>
        /// 添加节点到指定位置
        /// </summary>
        /// <param name="parentNode">父节点</param>
        /// <param name="text">节点名称</param>
        /// <param name="tag">节点tag</param>
        /// <param name="imgindex">图片索引</param>
        public static void AddNode(TreeNode parentNode, string text, object tag = null, int imgindex = -1)
        {
            TreeNode node = new TreeNode() { Text = text };
            if (tag != null)
                node.Tag = tag;
            if (imgindex != -1)
                node.ImageIndex = imgindex;
            parentNode.Nodes.Add(node);
            parentNode.Expand();
        }
        #endregion

        #region  获得所有磁盘目录和文件-使用栈遍历磁盘文件和目录，非传统递归方法（速度比递归算法高）。 
        /// <summary>
        /// 获得所有磁盘目录和文件-使用栈遍历磁盘文件和目录，非传统递归方法（速度比递归算法高）。 
        /// </summary>
        public static void GetDirectoryAllNodes(TreeView tv)
        {
            //暂停重绘
            tv.BeginUpdate();
            //存放树节点的栈
            Stack<TreeNode> skNode = new Stack<TreeNode>();
            //添加磁盘列表
            string[] drives = Directory.GetLogicalDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                //每个节点的Text存放目录名，Name存放全路径
                TreeNode node = new TreeNode(drives[i], 0, 0) { Name = drives[i] };
                tv.Nodes.Add(node);
                skNode.Push(node);
            }
            while (skNode.Count > 0)
            {
                //弹出栈顶目录，并获取路径
                TreeNode curNode = skNode.Pop();
                string path = curNode.Name;
                FileInfo fInfo = new FileInfo(path);
                try
                {
                    if ((fInfo.Attributes & FileAttributes.Directory) != 0)
                    {
                        string[] subDirs = null;
                        string[] subFiles = null;
                        try
                        {
                            //获取当前目录下的所有子目录和文件
                            subDirs = Directory.GetDirectories(path);
                            subFiles = Directory.GetFiles(path);
                        }
                        catch
                        { }
                        if (subDirs != null && subFiles != null)
                        {
                            //目录入栈
                            for (int i = 0; i < subDirs.Length; i++)
                            {
                                string dirName = Path.GetFileName(subDirs[i]);
                                TreeNode dirNode = new TreeNode(dirName, 1, 1) { Name = subDirs[i] };
                                curNode.Nodes.Add(dirNode);
                                skNode.Push(dirNode);
                            }
                            //文件无需入栈
                            for (int i = 0; i < subFiles.Length; i++)
                            {
                                string fileName = Path.GetFileName(subFiles[i]);
                                curNode.Nodes.Add(subFiles[i], fileName, 2);
                            }
                        }
                    }
                }
                catch { }
            }
            tv.EndUpdate();
        }
        #endregion

        #region  获得所有磁盘目录和文件--递归算法 
         /// <summary>
         /// 定义一个队列，用于记录用户创建的线程
         /// 以便在窗体关闭的时候关闭所有用于创建的线程
         /// </summary>
        public static List<Thread> ChaosThreadList=new List<Thread>();

        static TreeNode node = null;
        static List<DriveInfo> drive = new List<DriveInfo>();
        delegate void MyMethod();　　//声明一个委托，以使其它线程访问 
        static TreeView tvw = null;
        /// <summary>
        /// 获得所有磁盘目录和文件
        /// </summary>
        /// <param name="form">窗体对象</param>
        /// <param name="tv">TreeView对象</param>
        /// <param name="firstName">一级根节点名称</param>
        public static TreeView GetDirectoryAllNodes(TreeView tv, string firstName = "我的电脑")
        {
            x = 0;
            tvw = null;
            node = null;
            fileIndex = childIndex = 0;
            filePaths.Clear();
            FileNode.Clear();
            drive.Clear();
            childNode.Clear();
            childPaths.Clear();
            Control.CheckForIllegalCrossThreadCalls = false;
            ChaosThreadList.Clear();
            TreeNode nd = new TreeNode(firstName);
            nd.Expand();
            node = nd;
            tvw = tv;
            Thread doc = new Thread(new ThreadStart(Document));
            doc.Start();
            DriveInfo[] drives = DriveInfo.GetDrives();
            Thread th;
            foreach (DriveInfo dr in drives)
            {
                if (dr.DriveType != DriveType.CDRom && dr.DriveType != DriveType.Unknown)
                {
                    drive.Add(dr);
                    //GetDirectoryNoods(node, dr);
                    th = new Thread(new ThreadStart(AllNodes));
                    th.IsBackground = true;
                    ChaosThreadList.Add(th);
                    th.Start();
                }
            }
            Thread close = new Thread(new ParameterizedThreadStart(CloseThread));
            close.IsBackground = true;
            close.Start();
            HDSpaceInfo space= GetHDSpace();
            nd.Text = string.Format("我的电脑 ({0}G/{1}G)", space.HdSurplusSpace, space.HdTotalSpace);
            tv.Nodes.Add(nd);
            return tv;
        }
       
        /// <summary>
        /// 关闭线程组
        /// </summary>
        /// <param name="obj"></param>
        public static void CloseThread(object obj)
        {
            int count = 0;
            while (true)
            {
                if(ChaosThreadList.Count==0)
                    break;
                
                    try
                    {
                        if (ChaosThreadList[0].ThreadState == ThreadState.Aborted)
                        {
                            ChaosThreadList[0].Abort();
                            ChaosThreadList.RemoveAt(0);
                            count++;
                        }
                    }
                    catch { }
            }
            //ChaosThreadList.Clear();
        }
        /// <summary>
        /// 获取电脑文件目录和文件跨线程方法
        /// </summary>

        static void AllNodes()
        {
            MyMethod mt = new MyMethod(GetAllNodes);
            tvw.Invoke(mt);
            //在当前线程，调用mt        
        }
        /// <summary>
        /// 获取我的文档跨线程方法   
        /// </summary>

        static void Document()
        {
            MyMethod mt = new MyMethod(DocumenNodes);
            tvw.Invoke(mt);
            //在当前线程，调用mt        
        }
        static int x = 0;
        /// <summary>
        /// 获取计算机目录和文件
        /// </summary>
        static void GetAllNodes()
        {
            GetDirectoryNoods(node, drive[x]);
            x++;
        }
        /// <summary>
        /// 获取我的文档目录和文件
        /// </summary>
        static void DocumenNodes()
        {
            GetDocumentNodes(node);
        }
        #endregion

        #region  获取我的文档目录个文件 
        /// <summary>
        /// 获取我的文档文件目录和文件
        /// </summary>
        /// <param name="node">添加的父节点</param>
        /// <returns>返回TreeNode节点</returns>
        public static TreeNode GetDocumentNodes(TreeNode node)
        {
            //获取计算机我的文档文件夹
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TreeNode nd = new TreeNode();
            nd.Text = "我的文档";
            nd.Tag = path;
            node.Nodes.Add(GetChildNoodNodes(path, nd));
            return node;
        }
        #endregion

        #region  获得磁盘目录和文件--递归算法 
        /// <summary>
        /// 获得磁盘目录和文件--递归算法
        /// </summary>
        /// <param name="node">添加的父节点</param>
        /// <param name="dr">驱动器盘符</param>
        /// <returns>返回TreeNode节点</returns>
        public static TreeNode GetDirectoryNoods(TreeNode node, DriveInfo dr)
        {


            DirectoryInfo[] fs = new DirectoryInfo(dr.RootDirectory.Root.Name).GetDirectories();
            TreeNode tn = new TreeNode();

            double x = dr.TotalFreeSpace / 1024 / 1024 / 1024;
            double y = dr.TotalSize / 1024 / 1024 / 1024;
            tn.Text = string.Format("{0} ({1}/{2})", dr.VolumeLabel, x.ToString("#G"), y.ToString("#G"));
            foreach (DirectoryInfo item in fs)
            {
                TreeNode t = new TreeNode();
                t.Text = item.Name;
                t.Tag = item.FullName;
                tn.Nodes.Add(t);
                //childNode.Add(tn);
                //childPaths.Add(item.FullName);
                //Thread doc = new Thread(new ThreadStart(Mychild));
                //ChaosThreadList.Add(doc);
                //doc.Start();
                GetChildNoodNodes(item.FullName, t);
            }
            GetFileNodes(dr.RootDirectory.Root.Name, tn);
            node.Nodes.Add(tn);

            return node;
        }
        static List<string> childPaths = new List<string>();
        static List<TreeNode> childNode = new List<TreeNode>();
        static void Mychild()
        {
            MyMethod mt = new MyMethod(GetchildFile);
            tvw.Invoke(mt);
            //在当前线程，调用mt        
        }
        static int childIndex = 0;
        /// <summary>
        /// 获取文件
        /// </summary>
        static void GetchildFile()
        {
            GetChildNoodNodes(childPaths[childIndex], childNode[childIndex]);
            childIndex++;

        }
        static List<string> filePaths = new List<string>();
        static List<TreeNode> FileNode = new List<TreeNode>();
        /// <summary>
        /// 获得目录下的子目录--递归算法
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="node">添加的父节点</param>
        /// <returns>返回TreeNode节点</returns>
        private static TreeNode GetChildNoodNodes(string path, TreeNode node)
        {
            try
            {
                DirectoryInfo[] f = new DirectoryInfo(path).GetDirectories();
                if (f.Length != 0)
                {
                    foreach (DirectoryInfo item in f)
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = item.Name;
                        tn.Tag = item.FullName;
                        FileNode.Add(tn);
                        filePaths.Add(item.FullName);
                        Thread doc = new Thread(new ThreadStart(MyFile));
                        doc.IsBackground = true;
                        ChaosThreadList.Add(doc);
                        doc.Start();
                        //GetFileNodes(item.FullName, tn);
                        //childNode.Add(tn);
                        //childPaths.Add(item.FullName);
                        //Thread doc2 = new Thread(new ThreadStart(Mychild));
                        //ChaosThreadList.Add(doc2);
                        //doc2.Start();
                        GetChildNoodNodes(item.FullName, tn);
                        node.Nodes.Add(tn);
                    }
                }
            }
            catch { }
            return node;
        }

        static void MyFile()
        {
            MyMethod mt = new MyMethod(GetMyFile);
            tvw.Invoke(mt);
            //在当前线程，调用mt        
        }
        static int fileIndex=0;
        /// <summary>
        /// 获取文件
        /// </summary>
        static void GetMyFile()
        {
            GetFileNodes(filePaths[fileIndex], FileNode[fileIndex]);
            fileIndex++;

        }
        #endregion

        #region  获得目录下的所有文件 
        /// <summary>
        /// 获得目录下的所有文件
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="node">添加的父节点</param>
        /// <returns>返回TreeNode节点</returns>
        private static TreeNode GetFileNodes(string path, TreeNode node)
        {
            try
            {
                FileInfo[] f = new DirectoryInfo(path).GetFiles();
                foreach (FileInfo fi in f)
                {
                    TreeNode t2 = new TreeNode() { Text = fi.Name, Tag = fi.FullName };
                    node.Nodes.Add(t2);
                }
            }
            catch { }
            return node;
        }
        #endregion

        #region  TreeView对象 
        /// <summary>
        /// treeview 节点对象
        /// </summary>
        public class TreeViewNodes
        {
            /// <summary>
            /// treeview一级父节点
            /// </summary>
            public TreeNode ParentNode { get; set; }
            /// <summary>
            /// treeview二级子节点集合
            /// </summary>
            public List<TreeViewItems> Items { get; set; }
        }
        /// <summary>
        /// treeview 二级节点对象
        /// </summary>
        public class TreeViewItems
        {
            /// <summary>
            /// treeview二级父节点
            /// </summary>
            public TreeNode SecondParentNode { get; set; }
            /// <summary>
            /// treeview二级节点的子节点集合
            /// </summary>
            public List<TreeNode> ChildNodes { get; set; }
        }
        #endregion

        #region  获得电脑存储容量 
        /// <summary>
        /// 获得电脑存储容量
        /// </summary>
        /// <returns>返回电脑存储容量对象</returns>
        private static HDSpaceInfo GetHDSpace()
        {
            DriveInfo[] drivers = DriveInfo.GetDrives();
            double freeSpace = 0, Space = 0;
            foreach (DriveInfo temp in drivers)
            {
                //驱动器类型非光驱和移动设备
                if (temp.DriveType != DriveType.CDRom && temp.DriveType != DriveType.Removable)
                {
                    //得到硬盘总空间
                    freeSpace += double.Parse(temp.TotalSize.ToString()) / 1024 / 1024 / 1024;
                    //得到空用空间
                    Space += double.Parse(temp.TotalFreeSpace.ToString()) / 1024 / 1024 / 1024;
                }
            }
            return new HDSpaceInfo() { HdSurplusSpace = Space.ToString("#0.00"), HdTotalSpace = freeSpace.ToString("#0.00") };
        }
        #endregion

        #region  磁盘空间对象 
        /// <summary>
        /// 硬盘空间对象
        /// </summary>
        private struct HDSpaceInfo
        {
            /// <summary>
            /// 硬盘总空间（GB）
            /// </summary>
            public string HdTotalSpace { get; set; }
            /// <summary>
            /// 硬盘剩余空间（GB）
            /// </summary>
            public string HdSurplusSpace { get; set; }
        }
        #endregion
    }

}
