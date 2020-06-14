using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookStoreTeam31.Startup))]
namespace BookStoreTeam31
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
