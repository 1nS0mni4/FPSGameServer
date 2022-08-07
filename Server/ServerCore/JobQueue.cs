using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore {
    public interface IJobQueue {
        public void Push(Action action);
    }
    public class JobQueue : IJobQueue {
        private Queue<Action> _jobQueue = new Queue<Action>();
        private object l_jobQueue = new object();
        private bool _flushing = false;

        public void Push(Action action) {
            if(action == null)
                return;
            bool needFlush = false;

            lock(l_jobQueue) {
                _jobQueue.Enqueue(action);
                if(_flushing == false)
                    _flushing = needFlush = true;
            }

            if(needFlush)
                Flush();
        }

        public Action Pop() {
            if(_jobQueue.Count == 0)
                return null;

            lock(l_jobQueue) {
                return _jobQueue.Dequeue();
            }
        }

        public void Flush() {
            Action action = null;

            while(true) {
                action = Pop();
                if(action == null)   break;
                action.Invoke();
            }

            _flushing = false;
        }

        public void Clear() {
            lock(l_jobQueue) {
                _jobQueue.Clear();
            }
        }
    }
}
