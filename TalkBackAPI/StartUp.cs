using Microsoft.Owin;
using Owin;
using TalkBackAPI;

[assembly: OwinStartup(typeof(StartUp))]
namespace TalkBackAPI
{
    public class StartUp
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}