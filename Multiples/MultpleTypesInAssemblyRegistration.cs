using System;

namespace SystemDot.Ioc.Multiples
{
    public class MultpleTypesInAssemblyRegistration<TAssemblyOf>
    {
        readonly IIocContainer container;

        public MultpleTypesInAssemblyRegistration(IIocContainer container)
        {
            this.container = container;
        }

        public MultpleTypesInAssemblyFilteredRegistration AllTypes() =>
            new MultpleTypesInAssemblyFilteredRegistration(container, typeof(TAssemblyOf).GetTypesInAssembly().WhereNormalConcrete());
    }
}