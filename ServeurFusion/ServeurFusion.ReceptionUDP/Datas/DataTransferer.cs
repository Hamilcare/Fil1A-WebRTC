using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.Datas
{
    public class DataTransferer<T>
    {
        private static Queue fileInfos = new Queue();
        private int size = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddData(T data)
        {
            size++;
            fileInfos.Enqueue(data);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T ConsumeData()
        {
            size--;
            return (T)fileInfos.Dequeue();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Queue ReadData()
        {
            return fileInfos;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }
    }
}
