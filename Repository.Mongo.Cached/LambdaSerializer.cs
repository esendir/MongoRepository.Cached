using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Serialize.Linq.Nodes;
using System.IO;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Repository.Mongo.Cached
{
    public class LambdaSerializer : TextSerializer, IJsonSerializer
    {
        public new T Deserialize<T>(string text) where T : Node
        {
            return JsonConvert.DeserializeObject<T>(text);
        }

        public new string Serialize<T>(T obj) where T : Node
        {
            return JsonConvert.SerializeObject(obj);
        }

        protected override XmlObjectSerializer CreateXmlObjectSerializer(Type type)
        {
            return new DataContractJsonSerializer(type, this.GetKnownTypes());
        }

    }
}
