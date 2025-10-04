using System;
using Microsoft.Maui.Hosting;

namespace Plugin.Maui.SmartNavigation
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