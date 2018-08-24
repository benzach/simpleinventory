using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Functional.Lib.Functional;
using SimpleInventory.DL.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Functional.Lib.Functional.F;
using static Functional.Lib.Functional.OptionExt;
using SimpleInventory.BL;
using System.Data.SqlClient;
using System.Net;

namespace SimpleInventory
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static string DockerHostMachineIpAddress => Dns.GetHostAddresses(new Uri("http://docker.for.win.localhost").Host)[0].ToString();
        // This method gets called by the runtime. Use this method to add services to the container.
        public Func<T, R> GetDbThing<T, R>(IServiceCollection services, Func<T, SimpleInventoryContext, R> f)
            => t =>
             {
                 var scopeFac = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
                 using (var scope = scopeFac.CreateScope())
                 {
                     var provider = scope.ServiceProvider;
                     using (var db = provider.GetRequiredService<SimpleInventoryContext>())
                     {
                         return f(t, db);
                     }
                 }
             };
        public Func<R> GetDbThing<R>(IServiceCollection services, Func<SimpleInventoryContext, R> f)
            =>()=>
            {
                var scopeFac = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
                using (var scope = scopeFac.CreateScope())
                {
                    var provicer = scope.ServiceProvider;
                    using (var db = provicer.GetRequiredService<SimpleInventoryContext>())
                    {
                        return f(db);
                    }
                }
            };

        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<SimpleInventorySettings>(Configuration);
            services.AddMvc();
            var server = Configuration["DatabaseServer"] ?? "mssqlserver";
            var database = Configuration["DatabaseName"] ?? "SimpleInventoryDb";
            var user = Configuration["DatabaseUser"] ?? "sa";
            var password = Configuration["DatabasePassword"] ?? "Password123!";
            var connectionstring = $"Server={server};Database={database};User Id={user};Password={password};MultipleActiveResultSets=true";

            //var connectionstring = @"Server=localhost,1445;Database=SimpleInventoryDb;User Id=sa;Password=Password123!;MultipleActiveResultSets=true";
            //var connectionstring = $"Server={DockerHostMachineIpAddress},1445;Database=SimpleInventoryDb;User Id=sa;Password=Password123!;MultipleActiveResultSets=true";
            services.AddDbContext<SimpleInventoryContext>(option => option.UseSqlServer(connectionstring));//Configuration["ConnectionString"]

            //var dboption = new DbContextOptionsBuilder<SimpleInventoryContext>();
            //dboption.UseSqlServer(connectionstring);
            //var simpleInventorydb = new SimpleInventoryContext(dboption.Options);

            //var scopefactory = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            //using (var scope = scopefactory.CreateScope())
            //{
            //    var provider = scope.ServiceProvider;
            //    using (var simpleInventorydb = provider.GetRequiredService<SimpleInventoryContext>())
            //    {
            //        services.AddTransient<Func<Option<IEnumerable<Item>>>>(cont => () => FromPossibleZeroLength<IEnumerable<Item>, Item>(simpleInventorydb.Items.AsEnumerable()));
            //        //services.AddTransient<Func<long, Option<Item>>>(cont => id => FromNullable(simpleInventorydb.Items.SingleOrDefaultAsync(x => x.Id == id).Result));
            //        var errorParam = new SqlParameter("@errorcode", System.Data.SqlDbType.Int);
            //        errorParam.Direction = System.Data.ParameterDirection.Output;
            //        var idParam = new SqlParameter("@id", System.Data.SqlDbType.BigInt);
            //        idParam.Direction = System.Data.ParameterDirection.Input;
            //        SqlParameter[] p = new[] { idParam, errorParam };
            //        services.AddTransient<Func<long, Option<Item>>>(cont => id => { p[0].Value = id; return FromNullable(simpleInventorydb.Items.FromSql($"GetItemByID @id, @errorcode", p).SingleOrDefault()); });
            //        services.AddTransient<Func<BusinessRepoIII<Item, long>>>(cont => () => simpleInventorydb.Items.ToList().Select(x => (val: x, key: x.Id)).ToBusinessRepoIII());

            //        services.AddTransient<Func<string, Option<IEnumerable<Item>>>>(prov => (NameSearch) => FromPossibleZeroLength<IEnumerable<Item>, Item>(simpleInventorydb.Items.Where(x => x.Name.Contains(NameSearch))));
            //        services.AddTransient<Func<int, Option<IEnumerable<Code_Value>>>>(prov => (codsetId) => FromPossibleZeroLength<IEnumerable<Code_Value>, Code_Value>(simpleInventorydb.Code_Values.Where(cv => cv.CodeSetId == codsetId)));
            //        services.AddTransient<Func<Option<IEnumerable<Supplier>>>>(prov => () => FromPossibleZeroLength<IEnumerable<Supplier>, Supplier>(simpleInventorydb.Suppliers.AsEnumerable()));

            //    }
            //}

            //services.AddTransient<InventoryBusinessRepo>();
            //--services.AddTransient<Func<Option<IEnumerable<Item>>>>(cont => GetDBContext(db => FromPossibleZeroLength<IEnumerable<Item>, Item>(db.Items.AsEnumerable()))(connectionstring));

            //services.AddTransient<Func<Option<IEnumerable<Item>>>>(cont => () => FromPossibleZeroLength<IEnumerable<Item>, Item>(simpleInventorydb.Items.AsEnumerable()));
            services.AddTransient<Func<Option<IEnumerable<Item>>>>(prov => GetDbThing<Option<IEnumerable<Item>>>(services, db => db.Items.ToList()));

            //services.AddTransient<Func<Option<IEnumerable<Item>>>>(
            //    prov
            //    =>()
            //    =>{
            //        var scopeFac = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>();
            //        using (var scope = scopeFac.CreateScope())
            //        {
            //            var provider = scope.ServiceProvider;
            //            using (var dbcontext = provider.GetRequiredService<SimpleInventoryContext>())
            //            {
            //                return FromPossibleZeroLength<IEnumerable<Item>,Item>(dbcontext.Items.AsEnumerable());
            //            }
            //        }
            //      }
            //    );

            //services.AddTransient<Func<long, Option<Item>>>(cont => id => FromNullable(simpleInventorydb.Items.SingleOrDefaultAsync(x => x.Id == id).Result));
            var errorParam = new SqlParameter("@errorcode", System.Data.SqlDbType.Int);
            errorParam.Direction = System.Data.ParameterDirection.Output;
            var idParam = new SqlParameter("@id", System.Data.SqlDbType.BigInt);
            idParam.Direction = System.Data.ParameterDirection.Input;
            SqlParameter[] p = new[] { idParam, errorParam };

            //services.AddTransient<Func<long, Option<Item>>>(cont => id => { p[0].Value = id; return FromNullable(simpleInventorydb.Items.FromSql($"GetItemByID @id, @errorcode", p).SingleOrDefault()); });
            services.AddTransient<Func<long, Option<Item>>>(cont => GetDbThing<long, Option<Item>>(services, (id,db)=> { p[0].Value = id; return FromNullable(db.Items.FromSql($"GetItemByID @id, @errorcode", p).SingleOrDefault()); }));
            //services.AddTransient<Func<BusinessRepoIII<Item, long>>>(cont => () => simpleInventorydb.Items.ToList().Select(x => (val: x, key: x.Id)).ToBusinessRepoIII());
            services.AddTransient<Func<BusinessRepoIII<Item, long>>>(cont => GetDbThing<BusinessRepoIII<Item, long>>(services, db => db.Items.ToList().Select(x => (val: x, key: x.Id)).ToBusinessRepoIII()));
            //services.AddTransient<Func<string, Option<IEnumerable<Item>>>>(prov => (NameSearch) => FromPossibleZeroLength<IEnumerable<Item>, Item>(simpleInventorydb.Items.Where(x => x.Name.Contains(NameSearch))));
            services.AddTransient<Func<string, Option<IEnumerable<Item>>>>(prov => GetDbThing<string, Option<IEnumerable<Item>>>(services, (namesearch, db) => FromPossibleZeroLength<IEnumerable<Item>,Item>(db.Items.Where(x => x.Name.Contains(namesearch)).ToList())));
            //services.AddTransient<Func<int, Option<IEnumerable<Code_Value>>>>(prov => (codsetId) => FromPossibleZeroLength<IEnumerable<Code_Value>, Code_Value>(simpleInventorydb.Code_Values.Where(cv => cv.CodeSetId == codsetId)));
            services.AddTransient<Func<int, Option<IEnumerable<Code_Value>>>>(prov => GetDbThing<int, Option<IEnumerable<Code_Value>>>(services, (codesetId, db) => FromPossibleZeroLength<IEnumerable<Code_Value>, Code_Value>(db.Code_Values.Where(cv => cv.CodeSetId == codesetId).ToList())));
            //services.AddTransient<Func<Option<IEnumerable<Supplier>>>>(prov => () => FromPossibleZeroLength<IEnumerable<Supplier>, Supplier>(simpleInventorydb.Suppliers.AsEnumerable()));
            services.AddTransient<Func<Option<IEnumerable<Supplier>>>>(prov => GetDbThing<Option<IEnumerable<Supplier>>>(services, db => db.Suppliers.ToList()));
            services.AddTransient<InventoryBusinessRepo>();
            //}
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
