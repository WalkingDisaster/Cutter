using System;
using System.Web.Mvc;
using Cutter.Domain.Data;
using Cutter.Domain.Service;

namespace Cutter.Demo.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var cutter = new CuttingService();
            var shape = Guid.NewGuid();
            var parameters = new CutParameters
                                 {
                                     StockItems = new[]
                                                      {
                                                          new StockItemParameter
                                                              {
                                                                  ShapeId = shape,
                                                                  Length = 350,
                                                                  CostPerUnit = 0,
                                                                  Quantity = 1000,
                                                                  Kerf = 0
                                                              }
                                                      },
                                     RequiredItems = new[]
                                                         {
                                                             new RequiredItemParameter
                                                                 {
                                                                     ShapeId = shape,
                                                                     Quantity = 5,
                                                                     Length = 162
                                                                 },
                                                             new RequiredItemParameter
                                                                 {
                                                                     ShapeId = shape,
                                                                     Quantity = 5,
                                                                     Length = 94
                                                                 },
                                                             new RequiredItemParameter
                                                                 {
                                                                     ShapeId = shape,
                                                                     Quantity = 10,
                                                                     Length = 48
                                                                 },
                                                         }
                                 };
            var result = cutter.Optimize(parameters);
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
