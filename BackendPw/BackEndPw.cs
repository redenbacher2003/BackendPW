using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using BackendPw.CustomObject;
using static BackendPw.CustomObject.PwObject;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using Newtonsoft.Json;
using System.Xml.Linq;
using BackendPw.Validation;


namespace BackendPw
{
    public class PwManager
    {

        public PWType AddPW(string forWhatType, string forDescription, string user)
        {
            PWType newPwTye = new PWType();
            int id = -1;
            var errorMessage = "";

            newPwTye = PWExist(forWhatType);

            if (newPwTye != null)
            {
                return newPwTye;
            }
            else

            {
                try
                {
                    using (PWEntities pWEntities = new PWEntities())
                    {
                        PWType pWType = new PWType()
                        {
                            For = forWhatType,
                            Description = forDescription,
                            AddedBy = user,
                            Added = DateTime.Now

                        };
                        pWEntities.PWTypes.Add(pWType);
                        pWEntities.SaveChanges();
                        id = pWType.Id;
                        newPwTye = pWType;
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.InnerException.ToString();
                    newPwTye.Id = -2;
                }


            }

            return newPwTye;
        }

        //Get all Bill Payment Async
        public async Task<string> AddPW_Async(string forWhatType, string forDescription, string user)
        {
            var pwType = await Task.Run(() => this.AddPW(forWhatType, forDescription, user));
            if (pwType.Id <= 0)
            {
                return JsonConvert.SerializeObject(BackendPw.Validation.Validation.Create_Exception(pwType.Id, string.Format("Password for : {0} not created.", forWhatType)));
            }
            return JsonConvert.SerializeObject(pwType);
        }


        public List<PWType> PwGetAll()
        {
            PWEntities pWEntities = new PWEntities();

            return pWEntities.PWTypes.ToList<PWType>();

        }

        public async Task<string> PwGetAll_Async()
        {
            var pwTypeAll = await Task.Run(() => this.PwGetAll());

            return JsonConvert.SerializeObject(pwTypeAll);
        }

        public async Task<string> AddPwForType_Async(int Id, string pw, string user)
        {

            PwTypeDetail pwTypeDetail = new PwTypeDetail();


            var pwDetailId = await Task.Run(() => this.AddPwFor(Id, pw, user));
            if (pwDetailId > 0)
            {
                pwTypeDetail = PwTypeDetailGet(Id);
                return JsonConvert.SerializeObject(pwTypeDetail);
            }

            else
            {
                return JsonConvert.SerializeObject(Validation.Validation.Create_Exception(pwDetailId, string.Format("Password for : {0} does not exist.")));
            }

        }

        /* adding password by id */
        public int AddPwFor(int Id, string pw, string user)
        {
            // -1 error
            // > 0 success
            // -2 exception 
            int id = -1;
            var errorMessage = "";
            try
            {
                using (PWEntities pwEntities = new PWEntities())
                {
                    PWDetail pWDetail = new PWDetail()
                    {
                        PWTypeId = Id,
                        PW = pw,
                        Added = DateTime.Now,
                        AddedBy = user
                    };

                    pwEntities.PWDetails.Add(pWDetail);
                    pwEntities.SaveChanges();
                    id = pWDetail.Id;
                }
                if (id > 0)
                {
                    //delete any old password, just not the newly added id
                    PWDetailDelete(Id, id, user);
                }

            }

            catch (Exception ex)
            {
                errorMessage = ex.InnerException.ToString();
                id = -2;
            }

            return id;
        }

        public async Task<string> pwType_get_Async(int Id)
        {
            PwTypeDetail pwTypeDetail = new PwTypeDetail();

            var pwType = pwType_get(Id);

            if (pwType == null)
            {

                return JsonConvert.SerializeObject(Validation.Validation.Create_Exception(Id, string.Format("Password for : {0} does not exist.")));

            }



            pwType = await Task.Run(() => pwType_get(pwType.Id));
            if (pwType != null)
            {
                pwTypeDetail = PwTypeDetailGet(pwType.Id);
            }
            return JsonConvert.SerializeObject(pwTypeDetail);
        }
        public PWDetail getPwFor(int pwTypeId)
        {
            PWDetail pWDetail = new PWDetail();

            var errorMessage = "";
            try
            {
                using (PWEntities pwEntities = new PWEntities())
                {

                    pWDetail = pwEntities.PWDetails.FirstOrDefault(p => p.PWTypeId == pwTypeId && p.Deleted == null);

                }
            }

            catch (Exception ex)
            {
                errorMessage = ex.InnerException.ToString();
                pWDetail = null;
            }

            return pWDetail;
        }

        private PWType pwType_get(int Id)
        {
            PWType pWType = new PWType();

            var errorMessage = "";
            try
            {
                using (PWEntities pwEntities = new PWEntities())
                {

                    pWType = pwEntities.PWTypes.FirstOrDefault(p => p.Id == Id);

                }
            }

            catch (Exception ex)
            {
                pWType.Id = -1;
                errorMessage = ex.InnerException.ToString();
                pWType = null;
            }

            return pWType;
        }
        private PWType PWExist(string forWhatType)
        {

            PWType pWType = new PWType();

            using (PWEntities pWEntities = new PWEntities())
            {
                pWType = pWEntities.PWTypes.FirstOrDefault(f => f.For == forWhatType);
            }

            return pWType;
        }

        private bool PWDetailDelete(int pwTypeId, int newId, string user)
        {
            List<PWDetail> pWDetails = new List<PWDetail>();

            PWEntities pWEntities = new PWEntities();

            pWDetails = pWEntities.PWDetails.Where(p => p.PWTypeId == pwTypeId).ToList();
            foreach (PWDetail pwDetail in pWDetails)
            {
                if (pwDetail.Id != newId)
                {
                    pwDetail.Deleted = DateTime.Now;
                    pwDetail.DeletedBy = user;
                    pWEntities.SaveChanges();
                }
            }

            return true;
        }

        public PwTypeDetail PwTypeDetailGet(int Id)
        {
            PWEntities pWEntities = new PWEntities();

            PwTypeDetail pwTypeDetail = new PwTypeDetail();

            IEnumerable<PwTypeDetail> pWTypeDetails = new List<PwTypeDetail>();

            pWTypeDetails = from pWType in pWEntities.PWTypes
                            join pwDetail in pWEntities.PWDetails on pWType.Id equals pwDetail.PWTypeId
                            orderby (pWType.Id)
                            where pwDetail.Deleted == null
                            && pWType.Id == Id

                            select new PwTypeDetail
                            {
                                Id = pWType.Id,
                                PWDetailId = pwDetail.Id,
                                For = pWType.For,
                                Description = pWType.Description,
                                Pw = pwDetail.PW,
                                AddedBy = pwDetail.AddedBy,
                                Added = pwDetail.Added,
                                Deleted = pwDetail.Deleted
                            };

            pwTypeDetail = pWTypeDetails.FirstOrDefault();




            return pwTypeDetail;

        }
    }
}
