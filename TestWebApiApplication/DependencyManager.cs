using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Contracts;
using Facade;
using Repository;

namespace Api
{
    public class DependencyManager : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IUnityContainer _container;

        public IUnityContainer UnityContainer
        {
            get
            {
                return _container;
            }
        }

        public DependencyManager()
            : this(new UnityContainer())
        { }

        internal DependencyManager(IUnityContainer unityContainer)
        {
            _container = unityContainer;
            RegisterDependencies();
        }

        public void Dispose() { }

        public object GetService(Type serviceType)
        {
            try
            {
                return Resolve(serviceType);
            }
            catch (Exception)
            {
                //Web API will request dependencies that aren't registered.
                //When null is returned it will revert to the default depencency container.
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return UnityContainer.ResolveAll(serviceType);
            }
            catch (Exception)
            {
                //Web API will request dependencies that aren't registered.
                //When null is returned it will revert to the default depencency container.
                return null;
            }
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        public void Override<T>(T instance)
        {
            UnityContainer.RegisterInstance(instance);
        }

        public object Resolve(Type serviceType)
        {
            return UnityContainer.Resolve(serviceType);
        }

        private void RegisterDependencies()
        {
            _container
                .RegisterType<IEmployeeMapper, EmployeeMapper>()
                .RegisterType<IEmployeeRepository, EmployeeRepository>();

            _container
              .RegisterType<IStudentMapper, StudentMapper>()
              .RegisterType<IStudentRepository, StudentRepository>();


        }

    }
}