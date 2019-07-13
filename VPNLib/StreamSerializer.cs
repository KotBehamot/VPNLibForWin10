using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NetworkLib
{
    public static class StreamSerialize
    {
        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            try
            {
                string jsonString = Encoding.ASCII.GetString(stream.ToArray());
                IFormatter formatter = new BinaryFormatter();
                //      stream.Seek(01, SeekOrigin.Begin);
                object o = formatter.Deserialize(stream);
                return o;
            }
            //    try
            //{

            //    IFormatter formatter = new BinaryFormatter();
            //    return (JObject)formatter.Deserialize(stream);
            //}
            catch (Exception ex)
            {
                throw new Exception("ERROR" + Environment.NewLine + ex.Message);
            }


        }
        public static string DeserializeFromStreamToString(MemoryStream stream)
        {
            try
            {
                return Encoding.ASCII.GetString(stream.ToArray());

            }

            catch (Exception ex)
            {
                throw new Exception("ERROR" + Environment.NewLine + ex.Message);
            }
        }
    }

    internal sealed class VersionConfigToNamespaceAssemblyObjectBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;
            try
            {
                string ToAssemblyName = assemblyName.Split(',')[0];
                Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly ass in Assemblies)
                {
                    if (ass.FullName.Split(',')[0] == ToAssemblyName)
                    {
                        typeToDeserialize = ass.GetType(typeName);
                        break;
                    }
                }
            }
            catch (System.Exception exception)
            {
                throw exception;
            }
            return typeToDeserialize;
        }
    }

}
