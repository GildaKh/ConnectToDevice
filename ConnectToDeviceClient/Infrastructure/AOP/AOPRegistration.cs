using Castle.MicroKernel.Registration;
using Castle.MicroKernel;
using ConnectToDevice.Model.Interfaces;
using ConnectToDevice.Model;
using Castle.Core;

namespace ConnectToDevice.Infrastructure.AOP
{
    public class AOPRegistration : IRegistration
    {
        public void Register(IKernelInternal kernel)
        {
            kernel.Register(Component.For<LoggingInterceptor>().ImplementedBy<LoggingInterceptor>());
            kernel.Register(Component.For<IPersonnel>().ImplementedBy<SendData>()
                .Interceptors(InterceptorReference.ForType<LoggingInterceptor>()).Anywhere);
            kernel.Register(Component.For<IChannel>().ImplementedBy<SendDataManagment>()
              .Interceptors(InterceptorReference.ForType<LoggingInterceptor>()).Anywhere);
            kernel.Register(Component.For<IReceivedData>().ImplementedBy<ReceivedData>()
              .Interceptors(InterceptorReference.ForType<LoggingInterceptor>()).Anywhere);
        }
    }
}
