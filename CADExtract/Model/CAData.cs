using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace CADExtract.Model
{

    /// <summary>
    /// CAD Data
    /// </summary>
    [Serializable]
    [JsonObject]
    public class CAData
    {

        public FileInfo FileInfo { get; set; } = new FileInfo();
        public BlockTable Blocks { get; set; } = new BlockTable();
        public EntityTable Entities { get; set; } = new EntityTable();


        public CAData()
        {

        }


        public bool Save(string filepath)
        {

            try
            {
                var setting = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,


                };
                var s = JsonConvert.SerializeObject(this, Formatting.Indented, setting);
                File.WriteAllText(filepath, s);
                return true;
            }
            catch
            {
                return false;
            }

        }


        public static CAData Read(string filepath)
        {

            try
            {
                var setting = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All,
                };

                var t = File.ReadAllText(filepath);
                var s = JsonConvert.DeserializeObject<CAData>(t, setting);
                return s;
            }
            catch
            {
                return null;
            }
        }


    }
}
