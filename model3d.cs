using System;
using System.Collections.Generic;
using System.Text;

namespace arcball
{
    public  abstract class model3d
    {
        protected LinkList<PO> ge_p_l;//某个对象生成的点链
        protected LinkList<SURFACE> contr_s_l;//某个对象的控制面链
        public PO pi, pj, pk;//用于确定某个模型三个点以便以后的四面体剖分
        protected OGL PAINT = null;//用于画图的对象
        public void GetOGL(OGL pa)
        {
            //获取具备画图功能的对象
            PAINT = pa;
        }
        public void Show_ge_p_l()
        {
            Node<PO> pt;
            pt = ge_p_l.Head;
            while (pt != null)
            {
                PAINT.GetPL(pt.Data);
                pt = pt.Next;
            }
        }
        public abstract void GenerateNetPo();
        public  LinkList<PO> Get_get_p_l()
        {
            return ge_p_l;
        }
        public  LinkList<SURFACE> Get_contr_s_l()
        {
            return contr_s_l;
        }
    }
    public class well:model3d
    {
        public well()
        {
            ge_p_l = new LinkList<PO>();
        }
        public override void GenerateNetPo()
        {
            int  sitan;
            float PI = 3.1415926f;
            util u1 = new util();
            sitan = 10;
            PO vec,vec2;//沿井身
            vec = new PO(0,0,1);
            vec2 = new PO();
            pi = new PO();
            pj = new PO();
            pk = new PO();
            for (int k = 0; k < 8;k=k+2 )
                for (int i = -10; i < 20; i = i + 5)
                    for (int j = 0; j < sitan; j++)
                    {
                        vec2.x = (float)System.Math.Cos(2 * PI * j / sitan);
                        vec2.y = (float)System.Math.Sin(2 * PI * j / sitan);
                        vec2.z = 0;
                        if(k==6&&i==-10&&j==1) pi=u1.Po_vec_f(u1.vec_f(vec, i), vec2, k);
                        if(k==6&&i==-10&&j==2) pj=u1.Po_vec_f(u1.vec_f(vec, i), vec2, k);
                        if (k ==6&&i==-5&&j == 2) pk = u1.Po_vec_f(u1.vec_f(vec, i), vec2, k);
                        ge_p_l.Insert(u1.Po_vec_f(u1.vec_f(vec, i), vec2, k));

                    }
        }
    }
}
