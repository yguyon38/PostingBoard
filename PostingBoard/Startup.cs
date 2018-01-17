using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PostingBoard.Startup))]
namespace PostingBoard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
