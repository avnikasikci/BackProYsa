using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackProYsa.Infrastructure.General
{
    public static class UtilityJson
    {
        //Colum şeklinde kullanımı. Obje tarafında yazılmalı.
        //public bool SorDaireEkle { get => (JsonDeserialize(JsonSitePerData)).SorDaireEkle; set { var obj = JsonDeserialize(JsonSitePerData); obj.SorDaireEkle = value; JsonSitePerData = JsonSerialize(obj); } }
        //public bool SorIsinmaGor { get => (JsonDeserialize(JsonSitePerData)).SorIsinmaGor; set { var obj = JsonDeserialize(JsonSitePerData); obj.SorIsinmaGor = value; JsonSitePerData = JsonSerialize(obj); } }

        static JsonSerializerSettings _Setting = new JsonSerializerSettings()
        {
            DateFormatString = "dd/MM/yyyy",
            //Converters
        };

        // json datası çok büyük bir data olduğu zaman JavaScriptSerializer default değerleri ile bu datayı Serialize edemiyor. Bunun için JavaScriptSerializer'ın MaxJsonLength değerini el ile maximum yaparak gelen datasını Serialize ediyoruz. Bu fonksiyon bu nedenle yazılmıştır.
        public static string JsonSerialize<T>(T Data)
        {
            //JsonSerializerSettings _Setting = new JsonSerializerSettings()
            //{
            //    //Converters
            //};
            return JsonConvert.SerializeObject(Data, Formatting.Indented, _Setting);


            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.RegisterConverters(new JavaScriptConverter[] { new NullPropertiesConverter() });
            //serializer.MaxJsonLength = Int32.MaxValue;
            //return serializer.Serialize(Data);
        }

        // json datası çok büyük bir data olduğu zaman JavaScriptSerializer default değerleri ile bu datayı Deserialize edemiyor. Bunun için JavaScriptSerializer'ın MaxJsonLength değerini el ile maximum yaparak gelen datasını Deserialize ediyoruz. Bu fonksiyon bu nedenle yazılmıştır.
        public static T JsonDeserialize<T>(string Data)
        {
            //JsonSerializerSettings _Setting = new JsonSerializerSettings()
            //{
            //    //Converters
            //};
            return !string.IsNullOrEmpty(Data) ? JsonConvert.DeserializeObject<T>(Data, _Setting) : default(T);

            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //serializer.MaxJsonLength = Int32.MaxValue;
            //return !string.IsNullOrEmpty(Data) ? serializer.Deserialize<T>(Data) : default(T);
        }

        //// \~turkish  Json serialize işleminde null ve zero olan değerleri gereksiz yere almamak için. NOT: Nesne içinde 01.01.0001 şeklinde Tarih değeri varsa bu fonsiyon ignore ediliyor, 
        //private class NullPropertiesConverter : JavaScriptConverter
        //{
        //    public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        //    {
        //        var jsonExample = new Dictionary<string, object>();
        //        foreach (var prop in obj.GetType().GetProperties())
        //        {
        //            //Gelen nesne nullable bir nesne mi diye kontrol ediliyor.
        //            var nullableobj = prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        //            //check if decorated with ScriptIgnore attribute
        //            bool ignoreProp = prop.IsDefined(typeof(ScriptIgnoreAttribute), true);
        //            // nesneye atalı değer alınıyor
        //            var value = prop.GetValue(obj, System.Reflection.BindingFlags.Public, null, null, null);
        //            //Gelen nesne sayısal bir nesne mi float,double,decimal veya int 'mi
        //            bool TypeNumber = prop.PropertyType == typeof(float) || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(int) || prop.PropertyType == typeof(decimal);
        //            int i;
        //            //Gelen nesne nullable değilse, sayısal bir değerse ve 0 atanmış ise json içine kayıt edilmesine gerek yok nesne yeni oluştuğunda default değeri zaten 0
        //            //Gelen nesne değeri null ise, json içine kayıt edilmesine gerek yok nesne yeni oluştuğunda default değeri zaten null
        //            //Gelen nesne üzerine Attribute olarak ScriptIgnoreAttribute ise json içine kayıt edilmesine gerek yok.
        //            if (!(nullableobj == false && value != null && TypeNumber && (int.TryParse(value.ToString(), out i) ? i : 1) == 0) && value != null && !ignoreProp)
        //                jsonExample.Add(prop.Name, value);
        //        }

        //        return jsonExample;
        //    }

        //    public override IEnumerable<Type> SupportedTypes
        //    {
        //        get { return GetType().Assembly.GetTypes(); }
        //    }
        //}
    }

}
