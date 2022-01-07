using Autofac;
using Autofac.Extensions.DependencyInjection;
using BackProYsa.DataAccess.Context;
using BackProYsa.DataAccess.Core;
using BackProYsa.DataAccess.Repository;
using BackProYsa.lib;
using BackProYsa.lib.Layers;
using BackProYsa.lib.Propagation;
using BackProYsaML.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BackProYsa.UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Env = env;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DokumanHelper.Initialize(Env.WebRootPath);// DokumanHelper ilk basta initialize ediliyor. Dosya yolu için bilgisa
            services.AddControllersWithViews();
            services.AddPredictionEnginePool<ModelInput, ModelOutput>()
              .FromFile(modelName: HWRModel.Name, filePath: "MLModel.zip", watchForChanges: true);
            //services.AddPredictionEnginePool<TaxiFareModelInput, TaxiFareModelOutput>()
            //    .FromFile(modelName: TaxiFareModel.Name, filePath: "TaxiFare_MLModel.zip", watchForChanges: true);

            #region Debug modda iken Cshtml üzerinde degilik yapabilmek için eklendi. Canli için gerekmiyor. Bilgisa
            IMvcBuilder builderRazor = services.AddRazorPages();//Adds services related to Razor pages to the Dependency Injection container.
            services.AddControllersWithViews();//Adds services related to MVC controllers and views to the Dependency Injection container of the project.
#if DEBUG
            if (Env.IsDevelopment())
            {
                builderRazor.AddRazorRuntimeCompilation();
            }
#endif
            #endregion


            services.AddDbContext<DataContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.Load("BackProYsa.DataAccess"))
                 .Where(t => t.Name.EndsWith("Service"))
                 .AsImplementedInterfaces()
                 .InstancePerLifetimeScope();

            builder.RegisterType<DataContext>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<>)).InstancePerLifetimeScope().As(typeof(IRepository<>));

            //builder.RegisterType<BP1Layer<string>>().As<IBackPropagation<string>>().SingleInstance();
            //builder.RegisterType<EfProductDal>().As<IProductDal>().SingleInstance();
            builder.Populate(services);
            var appContainer = builder.Build();

            InfDependencyResolver.SetLifetimeScope(new InfLifetimeScope(appContainer));// Bi
            return new AutofacServiceProvider(appContainer);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseMigrationsEndPoint();

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
