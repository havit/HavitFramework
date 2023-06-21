using Havit.GoogleAnalytics.ValueSerializers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havit.GoogleAnalytics.Tests.ValueSerializers;

    [TestClass]
    public class EnumValueSerializerTests
    {
        [TestMethod]
        public void IsEnum_CanSerialize()
        {
            EnumValueSerializer serializer = new EnumValueSerializer();

            Assert.IsTrue(serializer.CanSerialize(FakeEnum.SomeValue));
        }

        [TestMethod]
        public void IsEnum_SerializeFromParameterValueAttribute()
        {
            EnumValueSerializer serializer = new EnumValueSerializer();

            string value = serializer.Serialize(FakeEnum.SomeValue);

            Assert.AreEqual("sv", value);
        }

        private enum FakeEnum
        {
            [ParameterValue("sv")]
            SomeValue,
            [ParameterValue("ov")]
            OtherValue
        }
    }
