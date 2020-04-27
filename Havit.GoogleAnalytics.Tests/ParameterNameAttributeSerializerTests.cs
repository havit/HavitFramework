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

            Assert.AreEqual(FakeValueSerializer.SerializedValue, serializedModel.Single().Value);
        }

        internal class FakeModel
        {
            [ParameterName("xs")]
            public string Value { get; set; } = String.Empty;
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
