using System;
using Microsoft.Maui.Hosting;

namespace Maui.Plugins.PageResolver
{
    internal class Initializer : IMauiInitializeService
    {
#region Implementation of IMauiInitializeService

        /// <inheritdoc />
        public void Initialize( IServiceProvider services )
        {
            Resolver.RegisterServiceProvider( services );
        }

#endregion
    }
}