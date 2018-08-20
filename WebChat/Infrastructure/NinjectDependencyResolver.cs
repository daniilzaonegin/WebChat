using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebChat.Abstract;
using WebChat.Concrete;
using WebChat_Model.Abstract;
using WebChat_Model.Concrete;

namespace WebChat.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam) {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType) {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return kernel.GetAll(serviceType);
        }
        
        private void AddBindings() {
            kernel.Bind<IMessageRepository>().To<EFMessageRepository>();
            kernel.Bind<ISessionsRepository>().To<MemorySessionsRepository>();
        }

    }
}