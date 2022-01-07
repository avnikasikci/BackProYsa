using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.DataAccess.Core
{
    public class InfDependencyResolver
    {
        private static InfDependencyResolver _Current;

        public static InfDependencyResolver Current
        {
            get
            {
                return _Current;
            }
        }


        private readonly IInfLifetimeScope _RootLifetimeScope;

        public static void SetLifetimeScope(IInfLifetimeScope RootLifetimeScope)
        {
            _Current = new InfDependencyResolver(RootLifetimeScope);
        }

        public InfDependencyResolver(IInfLifetimeScope RootLifetimeScope)
        {
            _RootLifetimeScope = RootLifetimeScope;
        }

        public IInfLifetimeScope BeginLifetimeScope()
        {
            return _RootLifetimeScope.BeginLifetimeScope();
        }
        public IInfLifetimeScope BeginLifetimeScope(object Tag)
        {
            return _RootLifetimeScope.BeginLifetimeScope(Tag);
        }
    }

    public class InfLifetimeScope : IInfLifetimeScope
    {
        private readonly ILifetimeScope _LifetimeScope;

        public InfLifetimeScope(ILifetimeScope LifetimeScope)
        {
            _LifetimeScope = LifetimeScope;
        }

        public IInfLifetimeScope BeginLifetimeScope()
        {
            return new InfLifetimeScope(_LifetimeScope.BeginLifetimeScope());
        }
        public IInfLifetimeScope BeginLifetimeScope(object Tag)
        {
            return new InfLifetimeScope(_LifetimeScope.BeginLifetimeScope(Tag));
        }

        public object Resolve(Type type)
        {
            return _LifetimeScope.Resolve(type);
        }

        public T Resolve<T>()
        {
            return _LifetimeScope.Resolve<T>();
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            Type enumerableOfType = typeof(IEnumerable<>).MakeGenericType(type);
            return (object[])_LifetimeScope.ResolveService(new TypedService(enumerableOfType));
        }
        public IEnumerable<T> ResolveAll<T>()
        {
            return _LifetimeScope.Resolve<IEnumerable<T>>();
        }

        public void Dispose()
        {
            _LifetimeScope.Dispose();
        }
    }

    public interface IInfLifetimeScope : IDisposable
    {
        IInfLifetimeScope BeginLifetimeScope();
        IInfLifetimeScope BeginLifetimeScope(object Tag);
        object Resolve(Type type);
        T Resolve<T>();
        IEnumerable<object> ResolveAll(Type type);
        IEnumerable<T> ResolveAll<T>();
    }
}
