using Havit.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.GoogleAnalytics.Tests
{
    [TestClass]
    public class ParameterNameAttributeSerializerTests
    {
        [TestMethod]
        public void Serializers_UseCustomSerializers()
        {
            IValueSerializer[] customSerializers = new IValueSerializer[]
            {
                new FakeValueSerializer()
            };
            PropertyNameAttributeSerializer serializer = new PropertyNameAttributeSerializer(customSerializers);
            var serializedModel = serializer.SerializeModel(new FakeModel());

            Assert.AreEqual(FakeValueSerializer.SerializedValue, serializedModel.Single(x => x.Key == "xs").Value);
        }

        [TestMethod]
        public void Values_IsNullableType_HasValue_CanSerialize()
        {
            PropertyNameAttributeSerializer serializer = new PropertyNameAttributeSerializer();

            int value = 100;
            FakeModel model = new FakeModel
            {
                NullableValue = value
            };
            
            var serializedModel = serializer.SerializeModel(model);

            Assert.AreEqual(value.ToString(), serializedModel.Single(x => x.Key == "nv").Value);
        }

        [TestMethod]
        public void Values_IsNullableType_IsNull_ShouldNotSerialize()
        {
            PropertyNameAttributeSerializer serializer = new PropertyNameAttributeSerializer();

            int? value = null;
            FakeModel model = new FakeModel
            {
                NullableValue = value
            };

            var serializedModel = serializer.SerializeModel(model);

            Assert.IsTrue(!serializedModel.Any(x => x.Key == "nv"));
        }

        internal class FakeModel
        {
            [ParameterName("xs")]
            public string Value { get; set; } = String.Empty;
            [ParameterName("nv")]
            public int? NullableValue { get; set; }
        }

        internal class FakeValueSerializer : IValueSerializer
        {
            public const string SerializedValue = "fake value";

            public bool CanSerialize(object value)
            {
                return true;
            }

            public string Serialize(object value)
            {
                return SerializedValue;
            }
        }
    }
}
