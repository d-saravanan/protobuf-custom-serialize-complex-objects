using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace ProtobufSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var obj = new MyData { Id = 1, Foo = new ComplexNonSerializableClass() { Name = "John.Smith" } };

            if (!RuntimeTypeModel.Default.IsDefined(typeof(ComplexNonSerializableClass)))
                RuntimeTypeModel.Default.Add(typeof(ComplexNonSerializableClass), false)
                    .SetSurrogate(typeof(SimplerThingThatLooksSimilar));
            var clone = Serializer.DeepClone(obj);

            Console.WriteLine(obj.Foo.Name);

            /* Proto Serializer Start */

            var ms = new MemoryStream();

            Serializer.Serialize(ms, obj); ms.Position = 0;

            var serializedData = new StreamReader(ms).ReadToEnd();

            var deserializableDataStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serializedData));

            MyData _dsV = Serializer.Deserialize<MyData>(deserializableDataStream);

            Console.WriteLine(_dsV.Foo.Name);

            /* Proto Serializer End */

            Console.ReadKey();
        }

        [ProtoContract]
        public class MyData
        {
            [ProtoMember(1)]
            public int Id { get; set; }
            [ProtoMember(2)]
            public ComplexNonSerializableClass Foo { get; set; }
        }

        public class ComplexNonSerializableClass
        {
            // ... real code here, obviously - although this probably isn't
            // your type in the first place - it is probably a BCL type
            [ProtoMember(1)] public string Name { get; set; }
        }

        [ProtoContract]
        public class SimplerThingThatLooksSimilar
        {
            public static implicit operator SimplerThingThatLooksSimilar(ComplexNonSerializableClass other)
            {   // ... real code here, obviously
                if (other != null)
                    return new SimplerThingThatLooksSimilar { Name = other.Name, Whatever = 123 };
                return new SimplerThingThatLooksSimilar();
            }
            public static implicit operator ComplexNonSerializableClass(SimplerThingThatLooksSimilar other)
            {
                return new ComplexNonSerializableClass()
                {
                    Name = other.Name
                }; // ... real code here, obviously
            }

            [ProtoMember(1)]
            public string Name { get; set; }
            [ProtoMember(2)]
            public int Whatever { get; set; }
        }

        private static void TestCustomSerializationInDotNet()
        {
            var b = new BaseScope();
            b.Name = System.IO.Path.GetRandomFileName();
            b.DisplayName = System.IO.Path.GetRandomFileName();
            b.Description = System.IO.Path.GetRandomFileName();
            b.Required = true;
            b.Emphasize = false;

            var data = Newtonsoft.Json.JsonConvert.SerializeObject(b);

            Console.WriteLine(data);

            b = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseScope>(data);

            Console.WriteLine(b.Name + " : " + b.Description + " : " + b.DisplayName + " : " + b.Required + " : " + b.Emphasize);
        }

        private static void TestProtoBuf()
        {
            var person = new Person
            {
                Id = 12345,
                Name = "Fred",
                Address = new Address
                {
                    Line1 = "Flat 1",
                    Line2 = "The Meadows"
                }
            };

            var proto = Serializer.GetProto<Person>();

            string data = string.Empty;

            using (var str = new MemoryStream())
            {
                Serializer.Serialize(str, person);
                var streamReader = new StreamReader(str);
                str.Position = 0;
                data = streamReader.ReadToEnd();
                Console.WriteLine(data);
            }

            var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));
            var objData = Serializer.Deserialize<Person>(ms);

            Console.WriteLine(objData.Address.Line1 + objData.Address.Line2);
        }
    }

    [Serializable]
    public class BaseScope : ISerializable
    {
        public BaseScope() { }
        protected BaseScope(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("bs_n");
            DisplayName = info.GetString("bs_dn");
            Description = info.GetString("bs_dsc");
            Required = info.GetBoolean("bs_req");
            Emphasize = info.GetBoolean("bs_emp");
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("bs_n", Name);
            info.AddValue("bs_dn", DisplayName);
            info.AddValue("bs_dsc", Description);
            info.AddValue("bs_req", Required);
            info.AddValue("bs_emp", Emphasize);
        }
    }

    [ProtoContract]
    class Person
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public Address Address { get; set; }
    }
    [ProtoContract]
    class Address
    {
        [ProtoMember(1)]
        public string Line1 { get; set; }
        [ProtoMember(2)]
        public string Line2 { get; set; }
    }
}
