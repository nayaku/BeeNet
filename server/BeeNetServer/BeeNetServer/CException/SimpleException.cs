using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeNetServer.CException
{
    /// <summary>
    /// 简单封装异常
    /// </summary>
    public class SimpleException :Exception
    {
        public SimpleException(string message) : base(message)
        {
        }

        public SimpleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
