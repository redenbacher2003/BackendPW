using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace BackendPw.CustomObject
{
    public class DiyProject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime startDate { get; set; }
        public DateTime finishDate { get; set; }
        public DateTime addedDate { get; set; }
        public string thumbNail { get; set; }
        public string addedBy { get; set; }

    }

    public class DiyProjectIdDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int diyProjectId { get; set; }
        public string materialName { get; set; }
        public int? quantity { get; set; }
        public decimal? amount { get; set; }
        public string StoreName { get; set; }
        public DateTime? purchaseDate { get; set; }
        public DateTime added { get; set; }
        public string addedBy { get; set; }
    }
    public class DiyProjectMaterial
    {
        public int id { get; set; }
        public int diyProjectId { get; set; }
        public string materialName { get; set; }
        public int? quantity { get; set; }
        public decimal? amount { get; set; }
        public string storeName { get; set; }
        public DateTime? purchaseDate { get; set; }
        public DateTime added { get; set; }
        public string addedBy { get; set; }
    }


}
