﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PostHost.Startup))]
namespace PostHost
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}