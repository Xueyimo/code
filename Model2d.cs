using System;
using System.Collections.Generic;
using System.Text;

namespace arcball
{
    public abstract class model2d
    {
        protected LinkList<PO> ge_p_l;//某个对象生成的点链
        protected LinkList<LINE> contr_l_l;//某个对象的控制面链
        public PO pi, pj;//用于确定某个模型三个点以便以后的三角剖分
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
        public LinkList<PO> Get_p_l()
        {
            return ge_p_l;
        }
        public LinkList<LINE> Get_contr_s_l()
        {
            return contr_l_l;
        }
    }
    public class well2d : model2d
    {
        public well2d()
        {
            ge_p_l = new LinkList<PO>();
        }
        public override void GenerateNetPo()
        {
            int sitan;
            float PI = 3.1415926f;
            util u1 = new util();
            sitan = 10;
            PO vec, vec2,pc;//沿井身
            vec = new PO(0, 0, 1);
            vec2 = new PO();
            pi = new PO();
            pj = new PO();
            for (int i = 5; i < 20; i = i + 4)
                for (int j = 0; j < sitan; j++)
                {
                    vec2.x = (float)System.Math.Cos(2 * PI * j / sitan);
                    vec2.y = (float)System.Math.Sin(2 * PI * j / sitan);
                    vec2.z = 0;
                    pc = u1.Po_vec_f(u1.vec_f(vec, 0), vec2, i);
                    if (i == 17 && j == 1) pi = pc;
                    if (i == 17 && j == 2) pj = pc;
                    PAINT.GetPL(pc);
                    ge_p_l.Insert(pc);
                }
            //PAINT.GetLL(new LINE(pi,pj));
        }
    }
}
