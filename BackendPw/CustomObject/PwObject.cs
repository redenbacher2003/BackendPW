using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendPw.CustomObject
{
    public class PwObject
    {
        public class PwTypeDetail
        {
            //Constructor
            public PwTypeDetail()
            {

            }

            public int Id  {get;set;}
            public int PWDetailId { get;set;}
            public string For { get; set; }
            public string Description { get; set; }
            public string Pw { get; set; }

            public string AddedBy { get; set; }
            public DateTime Added { get; set; }
            public DateTime? Deleted { get; set; }

            
        }

        public class PwException
        {

            public PwException(int Id, string Message)
            {
                this.Id = Id;
                this.Message = Message;
            }
            public int Id { get; set; }
            public string Message { get; set; }
        }

    }
}
