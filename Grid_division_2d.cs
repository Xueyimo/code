using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace arcball
{
    class Delaunay_2D
    {
        private float soliangle1, soliangle2;//用于选择角最大的点
        private LinkList<PO> pl;//表示当前所处理的点集
        private int loc = 0;//表示当前搜索到那个点
        protected OGL PAINT = null;//用于画图的对象
        private PO pkey;//表示当前正在处理的点
        private PO pg;//代表最后选择的点
        private LinkList<LINE> pre_l;//表示当前正在被处理的线链
        public LinkList<PO> work_pl;//表示当前工作的点链集
        public Delaunay_2D()
        {
            //初始化工作边集
            work_pl = new LinkList<PO>();
        }
        public void GetOGL(OGL pa)
        {
            //获取具备画图功能的对象
            PAINT = pa;
        }
        private void Set_direction(LINE l0, PO p0)
        {
            /*
             * 根据p0与s的相互关系，决定s的某个方向上是否有效
             * 设定为无效，则对应的标志为
             */
            util u1 = new util();
            float f;
            f = u1.Direct_2d(l0, p0);
            if (f > 0)
                l0.positive = false;
            else if (f < 0)
                l0.negtive = false;
        }
        private LINE AddLine(PO p1,PO p2,PO px){

		    //在p1和p2之间添加一条边，px用于确定p1-p2需要失效的方向,并返回新生成的边
		    //create the info for k1 and k2
            util u1 = new util();
		    LINE ln0=new LINE(p1,p2);
            PAINT.GetLL(ln0);
            Set_direction (ln0,px);
		    //put ln0 into k1
		    if (p1.non_merged){
			    if (p1.ll.Head==null){
				    p1.ll.Insert(ln0);
			    }
			    else
				    if (p1.ll.Last.Data.p1==px||p1.ll.Last.Data.p2==px)
                        p1.ll.Insert(ln0);
				    else
                        p1.ll.Head_Insert(ln0);
		    }
		    //put ln0 into k2
		    if (p2.non_merged)
                if (p2.ll.Head==null)
				    p2.ll.Insert(ln0);
			    else
                    if (p2.ll.Last.Data.p1 == px || p2.ll.Last.Data.p2 == px)
                        p2.ll.Insert(ln0);
                    else
                        p2.ll.Head_Insert(ln0);
            return ln0;
	    }
        private void PEBIGrid(PO p0){
            util u1 = new util();
		Node<LINE> la,lb,lc;
		PO pk1,pk2,pk3;
        PO p1, p2;
		la =p0.ll.Head;
		while (la.Next.Next!=null){
			lb = la.Next;
			lc = la.Next.Next;
			if (la.Data.p1==p0) pk1 = la.Data.p2;
            else pk1 = la.Data.p1;
            if (lb.Data.p1 == p0) pk2 = lb.Data.p2;
            else pk2 = lb.Data.p1;
            if (lc.Data.p1 == p0) pk3 = lc.Data.p2;
            else pk3 = lc.Data.p1;
			p1 = u1.SearchOutHeart(p0, pk1, pk2);
			p2 = u1.SearchOutHeart(p0, pk2, pk3);
            PAINT.GetLL(new LINE(p1,p2));
			la = la.Next;
		}
        lb = la.Next;
        lc = p0.ll.Head;
        if (la.Data.p1 == p0) pk1 = la.Data.p2;
        else pk1 = la.Data.p1;
        if (lb.Data.p1 == p0) pk2 = lb.Data.p2;
        else pk2 = lb.Data.p1;
        if (lc.Data.p1 == p0) pk3 = lc.Data.p2;
        else pk3 = lc.Data.p1;
        p1 = u1.SearchOutHeart(p0, pk1, pk2);
        p2 = u1.SearchOutHeart(p0, pk2, pk3);
        PAINT.GetLL(new LINE(p1, p2));
        la = la.Next;
	}
        public void Initial(model2d m)
        {
            //m从生成某个模型的点集中获取包含一个初始面的点集
            util u1 = new util();
            PO p0, p1;
            LINE l1;
            //获取初始面的三个顶点和一个初始面
            p0 = m.pi;
            p1 = m.pj;
            //加入初始的点，构成初始点集
            p0.selected = true;
            p1.selected = true;
            work_pl.Insert(p0);
            work_pl.Insert(p1);
            //产生初始三角形的一条边
            l1 = new LINE(p0, p1);
            //为每个点加入初始的边集
            p0.ll.Insert(l1);
            p1.ll.Insert(l1);
            pl = m.Get_p_l();        //获取模型的点数据，为一个点链      
            PAINT.GetLL(l1);
        }
        private LINE Is_OutSide(PO p0,PO ptest)
        {
            /*
             * 检测ptest是否在p0的边链的某个边的外端点处
             * 如果是，返回相应边,否则返回null
             */
            PO poutside;
            Node<LINE> lnt = p0.ll.Head;
            while (lnt != null)
            {
                if (p0 == lnt.Data.p1)
                    poutside = lnt.Data.p2;
                else
                    poutside = lnt.Data.p1;
                if (poutside == ptest)
                    return lnt.Data;
                lnt = lnt.Next;
            }
            if (p0.p_parent != null)
            {
                lnt = p0.p_parent.ll.Head;
                while (lnt != null)
                {
                    if (p0 == lnt.Data.p1)
                        poutside = lnt.Data.p2;
                    else
                        poutside = lnt.Data.p1;
                    if (poutside == ptest)
                        return lnt.Data;
                    lnt = lnt.Next;
                }
            }
            return null;
        }
        private void shadowpo(PO shap,PO shapflag,PO pap,PO papflag)
        {
            /*
             * 将影子点的边转移到父点中去
             * shap是影子点,shapflag是影子点的标记点
             * pap是父点，papflag是父点的标记点
             */

            if (pap.ll.Head.Data.Po_insideme(papflag))
            {//如果标记点在父点的首边上
                if (shap.ll.Head.Data.Po_insideme(shapflag))
                {
                    #region case1
                    Node<LINE> lnt = shap.ll.Head;
                    while (lnt != null)
                    {
                        //用父点换掉边中的影子点
                        lnt.Data.Po_insideme_and_changeit(pap);
                        pap.ll.Head_Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    #endregion case1
                }
                else
                {
                    #region case2
                    LinkList<LINE> lt = new LinkList<LINE>();
                    Node<LINE> lnt = shap.ll.Head;
                    while (lnt != null)//将shap的线链反序
                    {
                        lt.Head_Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    lnt = lt.Head;
                    while (lnt != null)
                    {
                        lnt.Data.Po_insideme_and_changeit(pap);
                        pap.ll.Head_Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    #endregion case2
                }
            }
            else
            {//如果标记点在父点的尾边上
                if (shap.ll.Head.Data.Po_insideme(shapflag))
                {
                    #region case3
                    Node<LINE> lnt = shap.ll.Head;
                    while (lnt != null)
                    {
                        lnt.Data.Po_insideme_and_changeit(pap);
                        pap.ll.Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    #endregion case3
                }
                else
                {
                    #region case4
                    LinkList<LINE> lt = new LinkList<LINE>();
                    Node<LINE> lnt = shap.ll.Head;
                    while (lnt != null)//将shap的线链反序
                    {
                        lt.Head_Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    lnt = lt.Head;
                    while (lnt != null)
                    {
                        lnt.Data.Po_insideme_and_changeit(pap);
                        pap.ll.Insert(lnt.Data);
                        lnt = lnt.Next;
                    }
                    #endregion case4
                }
            }
            work_pl.Delete(shap);
        }
        private void Insert(LINE l0)
        {
            util u1=new util();
            pg.ToString();
            /*获取线链的头边和尾边的外端点*/
            PO PLa, PFa;
            if (pkey == l0.p1)
                PLa = l0.p2;
            else
                PLa = l0.p1;
            if (pkey==pre_l.Head.Data.p1)
                PFa = pre_l.Head.Data.p2;
            else
                PFa = pre_l.Head.Data.p1;
            //PFa.ToString();
           // pg.ToString();
            if(u1.IsXl (pg,PLa,PFa,pkey)){
            //pg是当前边首边的外端点
                AddLine(PFa,PLa,pkey);
                Set_direction (pre_l.Head.Data,PLa);
                Set_direction (l0,pkey);
            }
            else{
                if (pg.selected)
                {           
                    //如果pg已经被选过
                    #region pg==PFa;
                    LINE lt;//获取相应边
                    if (pg == PFa)
                    {
                        lt= Is_OutSide(PLa, PFa);
                        lt = Is_OutSide(PLa, pg);
                        if (lt != null)
                        {
                            //如果PFa-PLa已存在
                            u1.InFile(u1.infopath, "PFa-PLa existed");
                            Set_direction(pre_l.Head.Data, PLa);
                            Set_direction(pre_l.Last.Data, PFa);
                            Set_direction(lt, pkey);
                            return;
                        }
                        else
                        {
                           // u1.InFile(u1.infopath, "PFa==pg");
                            AddLine(PFa, PLa, pkey);
                            Set_direction(pre_l.Head.Data, PLa);
                            Set_direction(l0, pkey);
                            return;
                        }
                    }
                    #endregion pg=PFa
                    #region last'soutside
                    //如果pg是last的边链的外端点
                    lt = Is_OutSide(PLa, pg);
                    if (lt!=null)
                    {
                        //u1.InFile(u1.infopath, "last's  outside");
                        AddLine(pkey, pg, PLa);
                        Set_direction(lt, pkey);
                        Set_direction(pre_l.Last.Data, pg);
                        if (PLa.p_parent != null)
                         //如果当前点是影子点，调整边链
                        {
                            //u1.InFile(u1.infopath, "in shadow");
                            shadowpo(PLa,pkey,PLa.p_parent,pg);
                        }
                        return;
                    }
                    #endregion last'soutside
                    #region first'soutside
                    //如果pg是first的边链的外端点
                    lt = Is_OutSide(PFa, pg);
                    if (lt != null)
                    {
                        LINE lt2 = AddLine(pkey, pg, PLa);
                        AddLine(PLa,pg,pkey);
                        Set_direction(lt, pkey);
                        Set_direction(pre_l.Head.Data,pg);
                        Set_direction(lt2,PFa);
                        Set_direction(pre_l.Last.Data, pg);
                        return;
                    }
                    #endregion first'soutside
                    #region arbitrary location
                    //如果pg是非特殊位置
                    PO pt= new PO(pg.x,pg.y,pg.z);
                    pt.p_parent = pg;
                    pg = pt;
                    //u1.InFile(u1.infopath, "here1");
                    AddLine(pkey, pg, PLa);
                   // u1.InFile(u1.infopath, "here2");
                    AddLine(PLa, pg, pkey);
                    //u1.InFile(u1.infopath, "here3");
                    Set_direction(l0, pg);
                   // u1.InFile(u1.infopath, "here4");
                    work_pl.Insert(pg);
                    //u1.InFile(u1.infopath,"here5");
                    pg.selected = true;
                    #endregion
                }
                else
                {
                    //pg作为普通点处理
                    AddLine(pkey, pg, PLa);
                    AddLine(PLa, pg, pkey);
                    Set_direction(l0, pg);
                    work_pl.Insert(pg);
                    //work_pl.Dispaly();
                    pg.selected = true;
                }
            }
        }
        private void SearchD(LINE l0)
        {
            /*
             * 针对某个面，在模型的点链中搜索某个满足条件的点
             */
            pg = null;
            Node<PO> pt = null;
            soliangle1 = 0;
            util u1 = new util();
            u1.InFile(u1.infopath, "--start SearchD--");
            pt = pl.Head;
            while (pt != null)
            {
                soliangle2 = u1.d2_Cal_Op_Angle(l0,pt.Data);
                if (soliangle1 < soliangle2&&u1.noSearched (l0,pt.Data))
                {
                    soliangle1 = soliangle2;
                    pg = pt.Data;
                }
                pt = pt.Next;
            }
            u1.InFile(u1.infopath, "--end SearchD--");
        }
        public void SearchLine()
        {
            //Delaunay算法的主函数，遍历边链的每一条边，并分别进行处理
            util u1 = new util();
            //pre_l.Dispaly();
            Node<LINE> lt = pre_l.Last;
            u1.InFile(u1.infopath, "--start SearchLine--");
            int num = 20;
            for (int i = 0; i < num; i++)
            {
                if (lt == null)
                    break;
                lt.Data.ToString();
                if (lt.Data.non_merged){
                    SearchD(lt.Data);
                    if (pg != null)
                        Insert(lt.Data);
                    else
                    {
                        u1.InFile(u1.infopath, "no point finded");
                        break;
                    }
                }
                else
                {
                    u1.InFile(u1.infopath, "-----has been merged-----");
                }
                lt = lt.Next;
            }
            u1.InFile(u1.infopath, "--SearchLine end--");
        }
        public void SearchPoint(int num)
        {
            //Delaunay算法的主函数，遍历边链的每一条边，并分别进行处理
            util u1 = new util();
            Node<PO> pt = work_pl.Head;
            u1.InFile(u1.infopath, "--start SearchPoint--");
            for (int i = 0; i < num; i++)
            {
                loc = i;
                if (pt == null)
                    break;
                u1.InFile(u1.infopath, i);
                pkey = pt.Data;
                pt.Data.ToString();//输出当前需要处理的点
                pre_l = pt.Data.ll;
                SearchLine();
                //PEBIGrid(pt.Data);
                pt = pt.Next;
                //pkey.ll.Dispaly();
                //u1.InFile(u1.infopath, i);
                //work_pl.Dispaly();
            }
            PAINT.SetPL(work_pl);
            u1.InFile(u1.infopath, "--SearchPoint end--");
        }
    }
}
