using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Websecurity___Essay_2.Models
{
    public class UserUploadFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public DateTime TimeStamp { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }

        public long ConvertSizeToMegaByte()
        {
            long result = Size / 1000;
            return result;

        }

    }
}
