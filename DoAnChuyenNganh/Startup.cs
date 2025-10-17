using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DoAnChuyenNganh.Startup))]
namespace DoAnChuyenNganh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
