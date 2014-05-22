namespace PCLCache.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PCLTesting;

    [TestClass]
    public class UtilTest
    {
        public int TestProperty { get; set; }

        [TestMethod]
        public void GetPropertyName()
        {
            //Returns property as string
            Assert.AreEqual("TestProperty", Util.GetPropertyName(() => TestProperty));
        }
    }
}
