using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleInventory.Functional;
using SimpleInventory.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SimpleInventory.Functional.F;
using static Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions;
namespace SimpleInventory.Controllers
{
    public class ControllerActivator : IControllerActivator
    {
        internal ILoggerFactory LoggerFactory { get; set; }
        DefaultControllerActivator DefaultActivator;
        TypeActivatorCache TypeActivatorCache;
        IConfigurationRoot Configuration;
        public ControllerActivator(IConfigurationRoot config)
        {
            Configuration = config;
            TypeActivatorCache = new TypeActivatorCache();
            DefaultActivator = new DefaultControllerActivator(TypeActivatorCache);
        }
        public object Create(ControllerContext context)
        {
            //throw new NotImplementedException();
            var type = context.ActionDescriptor.ControllerTypeInfo;
            var option = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<SimpleInventoryContext>();
            option.UseSqlServer(Configuration["ConnectionString"]);
            if (type.AsType().Equals(typeof(InventoryController)))
                return ConfigureDependency(context.HttpContext.RequestServices, new SimpleInventoryContext(option.Options));
            return DefaultActivator.Create(context);
        }

        public void Release(ControllerContext context, object controller)
        {
            //throw new NotImplementedException();
            var disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();

        }
        InventoryController ConfigureDependency(IServiceProvider serviceProvider,SimpleInventoryContext simpleInvetoryContext)
        {
            //Func<Option<IEnumerable<Item>>> getall=>simpleInvetoryContext.Items.Any()?Some<Option<IEnumerable<Item>>>(simpleInvetoryContext.Items.AsEnumerable()):None;
            Func<Option<IEnumerable<Item>>> getAllItems = () => simpleInvetoryContext.Items; //simpleInvetoryContext.Items.Any() ? Some(simpleInvetoryContext.Items.AsEnumerable()) : None;
            Func<int, Option<Item>> getItemByID = id => simpleInvetoryContext.Items.SingleOrDefault(x => x.Id == id);
            return new InventoryController(getAllItems, getItemByID);
        }
    }
}
