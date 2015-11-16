using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeutronDiffusion
{
    class NeutronThreadsWrapper
    {
        private List<List<Neutron>> _neutron_chunks;
        private int _tasksCount;
        private ManualResetEvent tasksDone = new ManualResetEvent(false);

        public NeutronThreadsWrapper(List<Neutron> neutrons)
        {
            this._neutron_chunks = neutrons.ChunkBy(100);
            this._tasksCount = _neutron_chunks.Count;
        }

        private void ThreadPoolCallback(Object threadContext)
        {
            var neutrons = (List<Neutron>)threadContext;
            neutrons.ForEach(neutron => neutron.Move());
            if (Interlocked.Decrement(ref _tasksCount) == 0)
            {
                tasksDone.Set();
            }
        }

        public void LaunchCalculations()
        {
            _neutron_chunks.ForEach(chunk => ThreadPool.QueueUserWorkItem(ThreadPoolCallback, chunk));
            tasksDone.WaitOne();
        }

    }

    public static class ListExtensions
    {
        public static List<List<T>> ChunkBy<T>(this List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
