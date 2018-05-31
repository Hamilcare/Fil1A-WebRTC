using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServeurFusion.ReceptionUDP.Datas
{
    public class DataTransferer<T>
    {
        private static BlockingCollection<T> fileInfos = new BlockingCollection<T>();
        private int size = 0;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddData(T data)
        {
            size++;
            fileInfos.Add(data);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T ConsumeData()
        {
            size--;
            return fileInfos.Take();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BlockingCollection<T> ReadData()
        {
            return fileInfos;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }
    }
}
