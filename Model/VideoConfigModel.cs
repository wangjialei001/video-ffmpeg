using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoWebApi.Model
{
    public class VideoConfigModel
    {
        public string Url { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string Num { get; set; }
        public int Id { get; set; }
        public long EquipId { get; set; }
    }
}
