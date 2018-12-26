using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace XT.MVC.Core.ComponentModel
{
    /// <summary>
    /// 提供了一个方便的方法实现对资源的访问。
    /// </summary>
    /// <remarks>
    /// 作为基础类，提供线程锁
    /// </remarks>
    public class WriteLockDisposable : IDisposable
    {
        private readonly ReaderWriterLockSlim _rwLock;

        /// <summary>
        /// 初始化实体 <see cref="WriteLockDisposable"/>
        /// </summary>
        /// <param name="rwLock">The rw lock.</param>
        public WriteLockDisposable(ReaderWriterLockSlim rwLock)
        {
            _rwLock = rwLock;
            _rwLock.EnterWriteLock();
        }

        void IDisposable.Dispose()
        {
            _rwLock.ExitWriteLock();
        }
    }
}
