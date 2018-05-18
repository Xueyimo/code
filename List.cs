using System;
using System.Collections.Generic;
using System.Text;

namespace arcball
{
    public class Node<T>
    {
        public T Data { set; get; }          //数据域,当前结点数据
        public Node<T> Next { set; get; }    //位置域,下一个结点地址

        public Node(T item)
        {
            this.Data = item;
            this.Next = null;
        }

        public Node()
        {
            this.Data = default(T);
            this.Next = null;
        }
    }
    public class LinkList<T>
    {
        public Node<T> Head { set; get; } //单链表头
        public Node<T> Last { set; get; } //单链尾
        public Node<T> Current { set;get;}//当前节点

        public int num;

        //构造
        public LinkList()
        {
            num = 0;
            Clear();
        }
        /// <summary>
        /// 判断单键表是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (Head == null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// 清空单链表
        /// </summary>
        public void Clear()
        {
            Head = null;
            Last = null;
        }
        /// <summary>
        /// 增加单链表插入的位置
        /// </summary>
        /// <param name="item">结点内容</param>
        /// <param name="n">结点插入的位置</param>
        public void Insert(T item)
        {
            //增加到头部
            util u1 = new util();
            Node<T> H = new Node<T>(item);
            if (Head == null)
            {
                Last = H;
                Head = H;
            }
            else
            {
                //Last.Data.ToString();
                Last.Next = H;
                Last = H;
                //Last.Data.ToString();
                //u1.InFile(u1.infopath,"add ok");
            }
          
            num++;
            return;
        }
        ///<summary>
        ///插入到单链表的头部
        ///</summary>
        ///<param name="item">节点内容</param>
        ///<param name="n">节点插入的位置</param>
        public void Head_Insert(T item)
        {
            Node<T> H = new Node<T>(item);
            H.Next = Head;
            Head = H;
        }
        /// <summary>
        /// 显示单链表
        /// </summary>
        public void Dispaly()
        {
            Node<T> A = new Node<T>();
            A = Head;
            while (A != null)
            {
                A.Data.ToString();
                A = A.Next;
            }
        }
        /// <summary>
        /// 删除链中某个元素
        /// </summary>
        public void Delete(T item)
        {
            Node<T> A = new Node<T>();
            Node<T> At = new Node<T>();
            A = Head;
            At = Head;
            if (A.Data.Equals(item))
                Head = Head.Next;
            A=A.Next;
            while (A!= null)
            {
                if (A.Data.Equals(item))
                {
                    if (A == Last)
                        Last = At;
                    At.Next = A.Next;
                    break;
                }
                At = A;
                A = A.Next;
            }
        }
    }
}
