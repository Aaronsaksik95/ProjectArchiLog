using Archi.API.Data;
using Archi.API.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using Archi.API.Controllers;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ArchiDbContext repository;

        public UnitTest1()
        {
            repository = MockDbContext.GetDbContext();
        }


        //[TestMethod]
        //public void GetAllTest()
        //{
        //    var controller = new PizzasController(repository);
        //    var getAllPizza = controller.GetAll().Result.Value;
        //}



        //[TestMethod]
        //public void GetIdTest()
        //{
        //    var controller = new PizzasController(repository);
        //    var getPizza = controller.GetId(1).Result.Value;
        //    var viewPizza = repository.Pizzas.SingleOrDefaultAsync(x => x.ID == 1).Result;

        //    Assert.IsNotNull(getPizza);
        //    Assert.IsNotNull(viewPizza);
        //    Assert.AreEqual(getPizza, viewPizza);
        //}


        [TestMethod]
        public void PostModelTest()
        {
            var controller = new PizzasController(repository);
            var postPizza = controller.PostModel(new Pizza { ID = 30, Active = true, Name = "Margarita", Topping = "jambon, tomate, emmental", Price = 18 }).Result;
            var viewPizza = repository.Pizzas.SingleOrDefaultAsync(x => x.ID == 30).Result;
            Assert.IsNotNull(viewPizza);
            Assert.AreEqual(viewPizza.Name, "Margarita");
            Assert.AreEqual(viewPizza.ID, 30);
            Assert.AreEqual(viewPizza.Price, 18);
            Assert.AreEqual(viewPizza.Topping, "jambon, tomate, emmental");
            Assert.AreEqual(viewPizza.Active, true);
        }


        //[TestMethod]
        //public void PutIdTest()
        //{
        //    var controller = new PizzasController(repository);
        //    var result = controller.PutId(3, new Pizza {Active = false, Name = "Olive", Price = 15, Topping = "" }).Result;
        //    var puutPizza = controller.GetId(3).Result.Value;
        //    var putPizza = repository.Pizzas.SingleOrDefaultAsync(x => x.ID == 3).Result;
        //    Assert.IsNotNull(putPizza);
        //    Assert.AreEqual(putPizza.Name, "Olive");
        //    Assert.AreEqual(putPizza.ID, 3);
        //    Assert.AreEqual(putPizza.Price, 15);
        //    Assert.AreEqual(putPizza.Topping, "");
        //    Assert.AreEqual(putPizza.Active, false);
        //}


        [TestMethod]
        public void DeleteModelTest()
        {
            var controller = new PizzasController(repository);
            var result = controller.DeleteModel(5).Result;
            var deletePizza = controller.GetId(5).Result.Value;
            Assert.AreEqual(deletePizza.Name, "Pizza5");
            Assert.AreEqual(deletePizza.ID, 5);
            Assert.AreEqual(deletePizza.Price, 15);
            Assert.AreEqual(deletePizza.Topping, null);
            Assert.AreEqual(deletePizza.Active, false);
        }

    }
}