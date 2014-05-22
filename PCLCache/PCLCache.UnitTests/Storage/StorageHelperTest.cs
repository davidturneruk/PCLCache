namespace PCLCache.UnitTests.Storage
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PCLTesting;
    using System.Threading.Tasks;

    [TestClass]
    public class StorageHelperTest
    {
        public class MyModel
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        [TestMethod]
        public async Task StorageHelperJsonTest()
        {
            await StorageHelperSaveTest(StorageSerializer.JSON);
            await StorageHelperSaveOverwriteTest(StorageSerializer.JSON);
            await StorageHelperDeleteTest(StorageSerializer.JSON);
            await StorageHelperDeleteNotExistingTest(StorageSerializer.JSON);
            await StorageHelperNotExistingTest(StorageSerializer.JSON);
        }

        [TestMethod]
        public async Task StorageHelperXmlTest()
        {
            await StorageHelperSaveTest(StorageSerializer.XML);
            await StorageHelperSaveOverwriteTest(StorageSerializer.XML);
            await StorageHelperDeleteTest(StorageSerializer.XML);
            await StorageHelperDeleteNotExistingTest(StorageSerializer.XML);
            await StorageHelperNotExistingTest(StorageSerializer.XML);
        }

        [TestMethod]
        public async Task StorageHelperDifferentSerializerTest()
        {
            var myObject = new MyModel() { Name = "Michiel", Age = 29 };

            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local);

            await sh.SaveAsync(myObject, "myfile");

            IStorageHelper<MyModel> shXml = new StorageHelper<MyModel>(StorageType.Local, serializerType: StorageSerializer.XML);

            var loadedObject = await shXml.LoadAsync("myfile");

            Assert.IsNull(loadedObject);

            await sh.DeleteAsync("myfile");

        }

        public async Task StorageHelperSaveTest(StorageSerializer serializerType)
        {
            var myObject = new MyModel() { Name = "Michiel", Age = 29 };

            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local, serializerType: serializerType);

            await sh.SaveAsync(myObject, "myfile");

            var loadedObject = await sh.LoadAsync("myfile");

            Assert.AreEqual(myObject.Name, loadedObject.Name);
            Assert.AreEqual(myObject.Age, loadedObject.Age);

            await sh.DeleteAsync("myfile");

        }

        public async Task StorageHelperSaveOverwriteTest(StorageSerializer serializerType)
        {
            var myObject = new MyModel() { Name = "Michiel", Age = 29 };

            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local, serializerType: serializerType);

            await sh.SaveAsync(myObject, "myfile");

            var newObject = new MyModel() { Name = "Simon", Age = 0 };

            //Save new object
            await sh.SaveAsync(newObject, "myfile");
            var loadedObject = await sh.LoadAsync("myfile");

            Assert.AreEqual(newObject.Name, loadedObject.Name);
            Assert.AreEqual(newObject.Age, loadedObject.Age);

            await sh.DeleteAsync("myfile");

        }

        public async Task StorageHelperDeleteTest(StorageSerializer serializerType)
        {
            var myObject = new MyModel() { Name = "Michiel", Age = 29 };

            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local, serializerType: serializerType);

            await sh.SaveAsync(myObject, "myfile");

            //Delete saved object
            await sh.DeleteAsync("myfile");

            var loadedObject = await sh.LoadAsync("myfile");
            Assert.IsNull(loadedObject);


        }

        public async Task StorageHelperDeleteNotExistingTest(StorageSerializer serializerType)
        {
            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local, serializerType: serializerType);

            //Delete non existing object
            await sh.DeleteAsync("myfile6526161651651");

            var loadedObject = await sh.LoadAsync("myfile6526161651651");
            Assert.IsNull(loadedObject);


        }


        public async Task StorageHelperNotExistingTest(StorageSerializer serializerType)
        {
            IStorageHelper<MyModel> sh = new StorageHelper<MyModel>(StorageType.Local, serializerType: serializerType);

            var loadedObject = await sh.LoadAsync("myfile561616516");

            Assert.IsNull(loadedObject);

        }
    }
}
