using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(My.SignalRTest.Startup))]
namespace My.SignalRTest
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
