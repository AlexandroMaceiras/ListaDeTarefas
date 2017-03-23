using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ListaDeTarefas.Startup))]
namespace ListaDeTarefas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
