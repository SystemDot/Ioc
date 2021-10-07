using System;
using System.Collections.Generic;

namespace SystemDot.Ioc
{
    public interface IIocContainer : IIocResolver
    {
        void RegisterInstance<TPlugin>(Func<TPlugin> instanceFactory) where TPlugin : class;

        void RegisterInstance<TPlugin, TConcrete>()
            where TPlugin : class
            where TConcrete : class, TPlugin;
        
        void RegisterInstance<TPlugin, TConcrete>(DependencyLifecycle lifecycle)
            where TPlugin : class
            where TConcrete : class, TPlugin;

        T Create<T>();

        object Create(Type type);

        void RegisterDecorator<TDecorator, TComponent>()
            where TDecorator: TComponent;

        void RegisterOpenTypeDecorator(Type openType, Type openDecoratorType);

        IEnumerable<RegisteredType> GetAllRegisteredTypes();

        void Verify();

        string Describe();
    }
}