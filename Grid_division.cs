using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace arcball
{
    class Delaunay
    {
       private float soliangle1,soliangle2;//用于选择立体角最大的点
       private PO p0, p1, p2, p3, pi, pj, pk;//p0,p1,p2,p3代表长方体的四个顶点
       private PO pg;//代表最后选择的点
       private float x1,x2,x3;//x1,x2,x3代表沿三个方向的细分数
       private LinkList<PO> pl;//表示当前所处理的点集
       private int loc=0;//表示当前搜索到那个点
        //new algorithm
       private LinkList<LINE> pre_l;//表示当前正在被处理的线链
       private LinkList<SURFACE> sl;//表示画图的面链
       private LinkList<PO> bad_pl;//表示当前的线不能选的点集
       private LinkList<LINE> ll;//表示用于画图的线链
       protected OGL PAINT = null;//用于画图的对象
       private PO pkey;//表示当前正在处理的点
      //
       public LinkList<PO> work_pl;//表示当前工作的链集
       public LinkList<String> info;//存放输出信息
       private int sid;//面的序号记录器
       private Node<SURFACE> sn_current;//当前正在处理的面
       public Delaunay()
       {      
           //初始化工作边集
           work_pl = new LinkList<PO>();
           sl = new LinkList<SURFACE>();
           pre_l = new LinkList<LINE>();
           bad_pl = new LinkList<PO>();
           ll = new LinkList<LINE>();
           info = new LinkList<string>();
           sid = 0;
       }
       public void GetOGL(OGL pa)
       {
           //获取具备画图功能的对象
           PAINT = pa;
       }
       public void Show_sl()
       {
           Node<SURFACE > sn;
           sn = sl.Head;
           while (sn != null)
           {
               PAINT.GetSL(sn.Data);
               sn = sn.Next;
           }
           sl.Clear();
       }
       public void Initial(model3d m)
       {
           //m从生成某个模型的点集中获取包含一个初始面的点集
           util u1 = new util();
           SURFACE s;
           PO p0,p1,p2;
           LINE l1, l2, l3;
           //获取初始面的三个顶点和一个初始面
           p0 = m.pi;
           p1 = m.pj;
           p2 = m.pk;
           s = New_SURFACE(p0, p1, p2);
           //加入初始的点，构成初始点集
           work_pl.Insert(p0);
           work_pl.Insert(p1);
           work_pl.Insert(p2);
           //产生初始面的三个边
           l1 = new LINE(p0,p1);
           l2 = new LINE(p0,p2);
           l3 = new LINE(p1,p2);
           //将三条边加到面中
           s.l1 = l1;
           s.l2 = l2;
           s.l3 = l3;
           //为每个边加入初始的面集
           l1.sl.Insert(s);
           l2.sl.Insert(s);
           l3.sl.Insert(s);
           //为每个点加入初始的边集
           p0.ll.Insert(l1);
           p0.ll.Insert(l2);
           p1.ll.Insert(l1);
           p1.ll.Insert(l3);
           p2.ll.Insert(l2);
           p2.ll.Insert(l3);
           pl = m.Get_get_p_l();        //获取模型的点数据，为一个点链      
           sl.Insert(s);
           PAINT.GetLL(l1);
           PAINT.GetLL(l2);
           PAINT.GetLL(l3);
       }
       private void Output(LINE l0)
       {
           info.Insert("------line-------");
           info.Insert("po:"+'('+l0.p1.x+','+l0.p1.y+','+l0.p1.z+')');
           info.Insert("po:" + '(' + l0.p2.x + ',' + l0.p2.y + ',' + l0.p2.z + ')');
           info.Insert("------line end-------");
       }
       private void Output(SURFACE  s0)
       {
           info.Insert("------surface-------");
           info.Insert("po:" + '(' + s0.p1.x + ',' + s0.p1.y + ',' + s0.p1.z + ')');
           info.Insert("po:" + '(' + s0.p2.x + ',' + s0.p2.y + ',' + s0.p2.z + ')');
           info.Insert("po:" + '(' + s0.p3.x + ',' + s0.p3.y + ',' + s0.p3.z + ')');
           info.Insert("------surface end-------");
       }
       private void Output(PO p0)
       {
           info.Insert("po:" + '(' + p0.x + ',' + p0.y + ',' + p0.z + ')');
       }
       private SURFACE New_SURFACE(PO p1,PO p2,PO p3)
       {
           SURFACE s0 = new SURFACE(p1,p2,p3);
           s0.id = sid++;
           return s0;
       }
       private void Set_direction(SURFACE s, PO p0)
       {
           /*
            * 根据p0与s的相互关系，决定s的某个方向上是否有效
            * 设定为无效，则对应的标志为
            */
           util u1 = new util();
           float f;
           f = u1.Direct(s, p0);
           if (f > 0)
               s.positive = false;
           else if (f < 0)
               s.negtive = false;
       }
       private void AddSurface(SURFACE s,LINE lk,SURFACE cur_s)
       {
           /*
            * 根据s与lk的面链的头尾的面的关系来确定s应该放在面链的头还是尾
            * 如果该边已经封闭则返回true,否则返回false
            */
           util u1=new util();
           SURFACE Last_s;
           if (lk.sl.Head == null)
               lk.sl.Insert(s);
           else
           {
               if (Surface_belong_line(lk, s)) return;
               Last_s = lk.sl.Last.Data;//获取面链尾部的面
               if (cur_s.id == Last_s.id)
                   lk.sl.Insert(s);
               else
                   lk.sl.Head_Insert(s);
           }            
         
       }
       private LINE OutSideofpo(PO pc, PO po)
       {
           /*
            * 判断po是否在pc边链的某个边的外端点
            * 是，返回该条边;否，返回null
            */
           Node<LINE> pn = pc.ll.Head;
           while (pn != null)
           {
               if (pn.Data.p1.IsEqualToMe(po) || pn.Data.p2.IsEqualToMe(po))
                   return pn.Data;
               pn = pn.Next;
           }
           return null;
       }
       private SURFACE Po_On_Surface(LINE l0,PO po)
       {
           /*
            * 判断po是否在l0的面链的某个面上
            * 是,返回这个面；否，返回null
            */
           Node<SURFACE> sn = l0.sl.Head;
           if(l0.shadow_ll!=null){
               Output(l0);
               Node<LINE> ln = l0.shadow_ll.Head;
               while (ln != null)
               {
                   sn = ln.Data.sl.Head;
                   while (sn != null)
                   {
                       if (sn.Data.Belongme(po))
                       {
                            return sn.Data;
                       }
                           
                       sn = sn.Next;
                   }
                   ln = ln.Next;
               }
           }
           else
           while (sn != null)
           {
               if (sn.Data.Belongme(po))
                   return sn.Data;
               sn = sn.Next;
           }
           return null;
       }
       private bool Surface_belong_line(LINE l0,SURFACE s0)
       {
           /*
            * 判断s0是否已经存在于l0的面链上
            * 是，返回true;否，返回false;
            */
           Node<SURFACE> sn = l0.sl.Head;
           if (l0.shadow_ll != null)
           {
               Node<LINE> ln = l0.shadow_ll.Head;
               while (ln != null)
               {
                   sn = ln.Data.sl.Head;
                   while (sn != null)
                   {
                       if (sn.Data.id == s0.id)
                           return true;
                       sn = sn.Next;
                   }
                   ln = ln.Next;
               }
           }
           else
               while (sn != null)
               {
                   if (sn.Data.id == s0.id)
                       return true;
                   sn = sn.Next;
               }
           return false;
       }
       private int Stateofsurface(SURFACE s0,PO po)
       {
           /*
            * 分析po与s0的关系，
            * 如果po与s0的一个点共边，返回1;两个点共边，返回2;三个点共边,返回3
            */
           int num_coin_po=0;
           if (OutSideofpo(s0.p1, po)!=null)
               num_coin_po++;
           if (OutSideofpo(s0.p2, po)!=null)
               num_coin_po++;
           if (OutSideofpo(s0.p3, po)!=null)
               num_coin_po++;
           return num_coin_po;
       }
       private LINE Get_LINE_From_One_Po(PO pc,PO po)
       {
           LINE l0;
           if ((l0 = OutSideofpo(pc, po)) != null)
           {
               if (l0.shadow_ll == null)
               {
                   l0.shadow_ll = new LinkList<LINE>();
                   l0.shadow_ll.Insert(l0);
               }
               LINE lt1 = new LINE(pc, po);
               pc.ll.Insert(lt1);
               po.ll.Insert(lt1);
               l0.shadow_ll.Insert(lt1);
               lt1.shadow_ll = l0.shadow_ll;
               l0 = lt1;
           }
           else
           {
               l0 = new LINE(pc, po);
               pc.ll.Insert(l0);
               po.ll.Insert(l0);
               PAINT.GetLL(l0);
           }
           return l0;
       }
       private LINE Get_LINE_From_One_Po(LINE l0, PO po)
       {
           Node<SURFACE> sn = l0.sl.Head;
           if (l0.shadow_ll != null)
           {
               Output(l0);
               Node<LINE> ln = l0.shadow_ll.Head;
               while (ln != null)
               {
                   sn = ln.Data.sl.Head;
                   while (sn != null)
                   {
                       if (sn.Data.Belongme(po))
                       {
                           info.Insert("find itxc");
                           return ln.Data;
                       }

                       sn = sn.Next;
                   }
                   ln = ln.Next;
               }
           }
           return null;
       }
       private LINE New_LINE(PO pc, PO po)
       {
           LINE l0;
           if ((l0 = OutSideofpo(pc, po)) != null)
               return l0;
           else
           {
               l0 = new LINE(pc, po);
               pc.ll.Insert(l0);
               po.ll.Insert(l0);
               PAINT.GetLL(l0);
           }
           return l0;
       }
       private SURFACE New_SURFACE(LINE lc,PO po)
       {
           SURFACE s0;
           if ((s0 = Po_On_Surface(lc, po)) != null)
               return s0;
           else
           {
               s0 = New_SURFACE(lc.p1,lc.p2,po);
               PAINT.GetSL(s0);
           }
           return s0;
       }
       private void Shadow_Delete(LINE lpa,PO lpa_pk,LINE lshadow,PO ls_pk)
       {
           /*
          * 将影子点的边转移到父点中去
          * shap是影子点,shapflag是影子点的标记点
          * pap是父点，papflag是父点的标记点
          */
           util u1 = new util();
           if (lpa.sl.Head.Data.Belongme(lpa_pk))
           {//如果标记点在父点的首边上
               if (lshadow.sl.Head.Data.Belongme(ls_pk))
               {
                   info.Insert("case1");
                   #region case1
                   Node<SURFACE> sn = lshadow.sl.Head;
                   while (sn != null)
                   {
                       //用父点换掉边中的影子点
                       lpa.sl.Head_Insert(sn.Data);
                       sn = sn.Next;
                   }
                   #endregion case1
               }
               else
               {
                   info.Insert("case2");
                   #region case2
                   LinkList<SURFACE> st = new LinkList<SURFACE>();
                   Node<SURFACE> sn = lshadow.sl.Head;
                   while (sn != null)//将shap的线链反序
                   {
                       st.Head_Insert(sn.Data);
                       sn = sn.Next;
                   }
                   sn = st.Head;
                   while (sn != null)
                   {
                       lpa.sl.Head_Insert(sn.Data);
                       sn = sn.Next;
                   }
                   #endregion case2
               }
           }
           else
           {//如果标记点在父点的尾边上
               if (lshadow.sl.Head.Data.Belongme(ls_pk))
               {
                   info.Insert("case3");
                   #region case3
                   Node<SURFACE> sn = lshadow.sl.Head;
                   while (sn != null)
                   {
                       //用父点换掉边中的影子点
                       lpa.sl.Insert(sn.Data);
                       sn = sn.Next;
                   }
                   #endregion 
               }
               else
               {
                   info.Insert("case4");
                   #region case4
                   LinkList<SURFACE> st = new LinkList<SURFACE>();
                   Node<SURFACE> sn = lshadow.sl.Head;
                   while (sn != null)//将shap的线链反序
                   {
                       st.Head_Insert(sn.Data);
                       sn = sn.Next;
                   }
                   sn = st.Head;
                   while (sn != null)
                   {
                       lpa.sl.Insert(sn.Data);
                       sn = sn.Next;
                   }
                   #endregion 
               }
               
           }
           lpa.p1.ll.Delete(lshadow);
           lpa.p2.ll.Delete(lshadow);
           sn_current = lpa.sl.Last;
       }
       private void Open_direction(LINE l0, PO pk)
       {
           //开启尾面的未搜索方向
           Node<SURFACE> stail, stailp, sn;
           sn = l0.sl.Head;
           stail = null;
           stailp = null;
           while(sn.Next!=null){
               stailp = sn;
               stail = sn.Next;
               sn = sn.Next;
           }
           PO pt;
           pt = stailp.Data.GetPd(l0);
           util u1 = new util();
           if (u1.Direct(stail.Data, pt) > 0)
               stail.Data.negtive = true;
           else
               stail.Data.positive = true;
       }
       private void Insert(LINE lk)
       {
          /*
           * lk：某个正在被操作的边，本函数的目的是将某个生成面加到与该
           * 边以工作面相连的边的面链合适的位置
           */
           util u1 = new util();
           SURFACE Last_s,First_s;//保存l0面链尾上的面对象;
           Last_s = lk.sl.Last.Data;
           First_s = lk.sl.Head.Data;
           PO pi, pj, pd,pfd;//当前所处理的面的三个顶点
           pi = lk.p1;
           pj = lk.p2;
           pd = Last_s.GetPd(lk);//获取面slast上除边l0外的剩余的点
           pfd = lk.sl.Head.Data.GetPd(lk);//获取面链上首面除边l0外的剩余的点
           LINE lk1, lk2;//取l0面链尾面除lk外的其他两条边
           lk1 = Last_s.GetLine(pi, pd);
           lk2 = Last_s.GetLine(pj, pd);
           LINE l1, l2, l3;//新生成的三条边
           SURFACE s, s1, s2;//新生成的三个面
           if (!u1.noSearched(First_s, pg) && u1.noSearched(First_s, pd))
           {
               pg = pfd;
           }
           if (pg.selected)
           {
               int State;
               State = Stateofsurface(Last_s,pg);
               if (State == 1||State==0)
               {
                   info.Insert("pg与Last_s的一个定点或零个顶点共边");
                   #region pg与Last_s的一个定点或零个顶点共边
                    l1=Get_LINE_From_One_Po(pi,pg);
                    l2=Get_LINE_From_One_Po(pd,pg);
                    l3=Get_LINE_From_One_Po(pj,pg);
                   //生成的三条边l1,l2,l3,加入工作链中                 
                   Set_direction(Last_s, pg);
                   s = New_SURFACE(pi, pj, pg);
                   s.l1 = l1;
                   s.l2 = l3;
                   s.l3 = lk;
                   PAINT.GetSL(s);
                   Set_direction(s, pd);
                   s1 = New_SURFACE(pi, pd, pg);
                   s1.l1 = l1;
                   s1.l2 = lk1;
                   s1.l3 = l2;
                   PAINT.GetSL(s1);
                   Set_direction(s1, pj);
                   s2 = New_SURFACE(pg, pj, pd);
                   s2.l1 = l2;
                   s2.l2 = l3;
                   s2.l3 = lk2;
                   PAINT.GetSL(s2);
                   Set_direction(s2, pi);
                   //将生成的面加到边的面链中
                   AddSurface(s, lk, Last_s);
                   AddSurface(s, l1, Last_s);//将面加到l1的面链中去
                   AddSurface(s, l3, Last_s);
                   AddSurface(s1, l1, Last_s);
                   AddSurface(s1, lk1, Last_s);
                   AddSurface(s1, l2, Last_s);
                   AddSurface(s2, l3, Last_s);
                   AddSurface(s2, lk2, Last_s);
                   AddSurface(s2, l2, Last_s);
                   #endregion
                   sn_current = sn_current.Next;
               }
               else
               {
                   info.Insert("pg与Last_s的两个顶点或三个顶点共边");
                   LINE l_shadow = Get_LINE_From_One_Po(lk, pg);
                   if (l_shadow != null && !First_s.Belongme(pg))
                   {
                       if (u1.Direct(l_shadow.sl.Head.Data, pg) * u1.Direct(l_shadow.sl.Head.Data, pd) < 0)
                           pg = l_shadow.sl.Head.Data.GetPd(lk);
                       if (u1.Direct(l_shadow.sl.Last.Data, pg) * u1.Direct(l_shadow.sl.Last.Data, pd) < 0)
                           pg = l_shadow.sl.Last.Data.GetPd(lk);
                       #region pg与Last_s的两个顶点或三个顶点共边
                       l1 = New_LINE(pi, pg);
                       l2 = New_LINE(pd, pg);
                       l3 = New_LINE(pj, pg);
                       Set_direction(Last_s, pg);
                       s = New_SURFACE(lk, pg);
                       s.l1 = l1;
                       s.l2 = l3;
                       s.l3 = lk;
                       Set_direction(s, pd);
                       s1 = New_SURFACE(lk1, pg);
                       s1.l1 = l1;
                       s1.l2 = lk1;
                       s1.l3 = l2;
                       Set_direction(s1, pj);
                       s2 = New_SURFACE(lk2, pg);
                       s2.l1 = l2;
                       s2.l2 = l3;
                       s2.l3 = lk2;
                       Set_direction(s2, pi);
                       //将生成的面加到边的面链中
                       AddSurface(s, lk, Last_s);
                       AddSurface(s, l1, Last_s);//将面加到l1的面链中去
                       AddSurface(s, l3, Last_s);
                       AddSurface(s1, l1, Last_s);
                       AddSurface(s1, lk1, Last_s);
                       AddSurface(s1, l2, Last_s);
                       AddSurface(s2, l3, Last_s);
                       AddSurface(s2, lk2, Last_s);
                       AddSurface(s2, l2, Last_s);
                       #endregion
                       Shadow_Delete(lk, pd, l_shadow, pg);
                   }
                   else
                   {
                        #region pg与Last_s的两个顶点或三个顶点共边
                        l1 = New_LINE(pi, pg);
                        l2 = New_LINE(pd, pg);
                        l3 = New_LINE(pj, pg);
                        Set_direction(Last_s, pg);
                        s = New_SURFACE(lk, pg);
                        s.l1 = l1;
                        s.l2 = l3;
                        s.l3 = lk;
                        Set_direction(s, pd);
                        s1 = New_SURFACE(lk1, pg);
                        s1.l1 = l1;
                        s1.l2 = lk1;
                        s1.l3 = l2;
                        Set_direction(s1, pj);
                        s2 = New_SURFACE(lk2, pg);
                        s2.l1 = l2;
                        s2.l2 = l3;
                        s2.l3 = lk2;
                        Set_direction(s2, pi);
                        //将生成的面加到边的面链中
                        AddSurface(s, lk, Last_s);
                        AddSurface(s, l1, Last_s);//将面加到l1的面链中去
                        AddSurface(s, l3, Last_s);
                        AddSurface(s1, l1, Last_s);
                        AddSurface(s1, lk1, Last_s);
                        AddSurface(s1, l2, Last_s);
                        AddSurface(s2, l3, Last_s);
                        AddSurface(s2, lk2, Last_s);
                        AddSurface(s2, l2, Last_s);
                        #endregion
                        sn_current = sn_current.Next;
                   }
                       
                   
               }
           }
           else
           {
               #region pg是一个普通点
               l1 = new LINE(pg, pi);
               l2 = new LINE(pg, pd);
               l3 = new LINE(pg, pj);
               PAINT.GetLL(l1);
               PAINT.GetLL(l2);
               PAINT.GetLL(l3);
               //生成的三条边l1,l2,l3,加入工作链中
               pg.ll.Insert(l1);
               pg.ll.Insert(l2);
               pg.ll.Insert(l3);
               pi.ll.Insert(l1);
               pd.ll.Insert(l2);
               pj.ll.Insert(l3);
               Set_direction(Last_s, pg);
               s = New_SURFACE(pi, pj, pg);
               s.l1 = l1;
               s.l2 = l3;
               s.l3 = lk;
               PAINT.GetSL(s);
               Set_direction(s, pd);
               s1 = New_SURFACE(pi, pd, pg);
               s1.l1 = l1;
               s1.l2 = lk1;
               s1.l3 = l2;
               PAINT.GetSL(s1);
               Set_direction(s1, pj);
               s2 = New_SURFACE(pg, pj, pd);
               s2.l1 = l2;
               s2.l2 = l3;
               s2.l3 = lk2;
               PAINT.GetSL(s2);
               Set_direction(s2, pi);        
               //将生成的面加到边的面链中
               AddSurface(s, lk, Last_s);
               AddSurface(s, l1, Last_s);//将面加到l1的面链中去
               AddSurface(s, l3, Last_s);
               AddSurface(s1, l1, Last_s);
               AddSurface(s1, lk1, Last_s);
               AddSurface(s1, l2, Last_s);
               AddSurface(s2, l3, Last_s);
               AddSurface(s2, lk2, Last_s);
               AddSurface(s2, l2, Last_s);
               pg.selected = true;
               work_pl.Insert(pg);
               #endregion
               sn_current = sn_current.Next;
           }           
       }
       private void SearchD(SURFACE s0)
       {
           /*
            * 针对某个面，在模型的点链中搜索某个满足条件的点
            */
            pi = s0.p1;
            pj = s0.p2;
            pk = s0.p3;
            pg = null;
            Node<PO> pt=null;
            soliangle1 = 0;
            util u1=new util();
            info.Insert("--start SearchD--");
            pt = pl.Head;
            while (pt!=null)
            {
                soliangle2 = u1.d3_Cal_Solid_Angle(pt.Data,pi,pj,pk);
                if (soliangle1 < soliangle2&&u1.noSearched(s0,pt.Data))
                {
                    soliangle1 = soliangle2;
                    pg = pt.Data;
                }
                pt = pt.Next;
            }
            info.Insert("--end SearchD--");
        }
       private void Search_In(LINE lk)
       {
           //对边链的某个边进行处理
           int i;
            util u1=new util();
            sn_current = lk.sl.Last;
            info.Insert("---Search_In start---");
            info.Insert("当前在处理的边");
            Output(lk);
            if (lk.shadow_ll!= null)
                Open_direction(lk,pg);
            for (i = 0; i <20;i++ )//当当前正在处理的边未封闭时
            {
                if (sn_current == null)
                    break;
                info.Insert("---当前的面---");
                Output(sn_current.Data);
                SearchD(sn_current.Data);
                if (pg == null)
                {
                    info.Insert("--no point finded--");
                    break;
                }
                else
                {
                    info.Insert("----Search Result----");
                    Output(pg);
                    Insert(lk);
                }
            }
            lk.shadow_ll = null;
            info.Insert("---Search_In end---");
       }
       public void SearchLine()
       {
           //Delaunay算法的主函数，遍历边链的每一条边，并分别进行处理
           util u1 = new util();
           Node<LINE> lt = pre_l.Head;
           info.Insert("--start SearchLine--");
           int num = 20;
           for (int i = 0; i <num;i++ )
           {
               if (lt == null)
                   break;
               if (loc == 20 && i == 10)
                   break;
               Search_In(lt.Data);
               lt = lt.Next;
           }
           info.Insert("--SearchLine end--");
       }
       public void SearchPoint(int num)
       {
           //Delaunay算法的主函数，遍历边链的每一条边，并分别进行处理
           util u1 = new util();
           Node<PO> pt = work_pl.Head;
           info.Insert( "--start SearchPoint--");
           for (int i = 0; i <10; i++)
           {
               loc = i;
               if (pt == null)
                   break;
               info.Insert("----step:"+i+"-----");
               pkey = pt.Data;
               Output(pkey);
               pre_l = pt.Data.ll;
               SearchLine();
               pt = pt.Next;
           }
           info.Insert("--SearchPoint end--");
       }
    }
}
