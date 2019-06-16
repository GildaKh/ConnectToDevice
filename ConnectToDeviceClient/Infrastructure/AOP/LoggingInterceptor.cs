using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectToDevice.Infrastructure.AOP
{
    public class LoggingInterceptor : IInterceptor
    {
        LoggingInterceptor()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public void Intercept(IInvocation invocation)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(invocation.TargetType);
            var methodName = invocation.Method.Name;
            try
            {
                log.Info(string.Format("Start of Method:{0}, Arguments: {1}", methodName, string.Join(",", invocation.Arguments)));
                invocation.Proceed();
                log.Info(string.Format("Method sucessfully executed :{0}", methodName));
            }
            catch (Exception e)
            {
                log.Error(string.Format("Method {0} exception occured, Exception:{1}", methodName, e.Message));
                throw;
            }
            finally
            {
                log.Info(string.Format("End of Method:{0}", methodName));
            }
        }
    }
}
