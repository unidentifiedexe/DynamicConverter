using System;
using System.Dynamic;
using AutoTester.TestItems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static DynamicConverter.Converter;

namespace AutoTester
{
    [TestClass]
    public class AttributeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            dynamic x = new ExpandoObject();
            x.YVal = 10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass1 val);


            Assert.IsTrue(IsSucsses);
            Assert.IsNotNull(val);
            Assert.IsTrue(val.CtorUsed);
            Assert.AreEqual( 10, val.Y);
        }


        [TestMethod]
        public void TestMethod2()
        {
            dynamic x = new ExpandoObject();
            x.X = 10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass1 val);


            Assert.IsTrue(IsSucsses);
            Assert.IsNotNull(val);
            Assert.IsFalse(val.CtorUsed);
            Assert.AreEqual(10, val.X);
        }



        [TestMethod]
        public void TestMethod3()
        {
            dynamic x = new ExpandoObject();
            x.ZVal = (byte)10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass1 val);


            Assert.IsTrue(IsSucsses);
            Assert.IsNotNull(val);
            Assert.IsTrue(val.CtorUsed);
            Assert.AreEqual(10, val.Z);
        }

        [TestMethod]
        public void TestMethod4()
        {
            dynamic x = new ExpandoObject();
            x.Z = (byte)10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass1 val);


            Assert.IsTrue(IsSucsses);
            Assert.IsNotNull(val);
            Assert.IsFalse(val.CtorUsed);
            Assert.AreEqual(0, val.Z);
        }




        [TestMethod]
        public void TestMethod5()
        {
            dynamic x = new ExpandoObject();
            x.Z = (byte)10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass2 val);


            Assert.IsFalse(IsSucsses);
        }


        [TestMethod]
        public void TestMethod6()
        {
            dynamic x = new ExpandoObject();
            x.ZVal = (byte)10;

            var IsSucsses = TryConvert(x, out ConstructerTestClass2 val);


            Assert.IsTrue(IsSucsses);
            Assert.IsNotNull(val);
            Assert.IsTrue(val.CtorUsed);
            Assert.AreEqual(10, val.Z);
        }
    }
}
