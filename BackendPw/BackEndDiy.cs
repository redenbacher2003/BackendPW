using System;
using BackendPw.CustomObject;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendPw.Validation;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;

namespace BackendPw
{
    public class BackEndDiyManager
    {
        DiyProjectEntities entities = new DiyProjectEntities();
        /// <summary>
        /// Adds new Project
        /// </summary>
        /// <param name="projectName"></param>
        /// <param startDate="startDate"></param>
        /// <returns>Project Object</returns>
        /// <exception cref="DiyProjectFoundException"></exception>
        public Project addProject(string projectName, DateTime startDate) {

            Project newProject = new Project(); 
  
            if (projectName == null || startDate == null)
            {
                throw new DiyProjectAddException("Project Name and Start Date must both be supplied!");
            }

            else
            { 
                if (getDiyProjectByName(projectName) == null)
                {
                    //add it!
                    Project project = new Project() { 
                        Name = projectName,
                        StartDate = startDate,
                    };

                    entities.Projects.Add(project);
                    entities.SaveChanges();
                    newProject = project;
                }
                else
                {
                    throw new DiyProjectAddException($"Project : {projectName} already exist!");
                }
                return newProject;
            }
        }

        public async Task<string> getProjects_Async()
        {
            List<Project> Project = await Task.Run(() => getProjects());
            return JsonConvert.SerializeObject(Project);
        }
        public List<Project> getProjects()
        {
        
            return entities.Projects.ToList<Project>();

        }
        public Project getDiyProjectByName(string projectName)
        {
            Project diyProject = new Project();

            return entities.Projects.FirstOrDefault(p => p.Name == projectName);
            
        }
        /// <summary>
        /// Gets list of Project Materies by Project Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>List<ProjectMaterial></returns>
        public IEnumerable<DiyProjectIdDetail> getMaterialsByProjectId(int projectId) {

            IEnumerable<DiyProjectIdDetail> diyProjectIdDetails = new List<DiyProjectIdDetail>();

            diyProjectIdDetails = from project in entities.Projects
                                    join projectMaterials in entities.ProjectMaterials on project.id equals projectMaterials.ProjectId
                                    orderby (projectMaterials.Id)
                                    where projectMaterials.ProjectId == projectId
                                    select new DiyProjectIdDetail
                                    {
                                        Id = projectMaterials.Id,
                                        Name = project.Name,
                                        diyProjectId = project.id,
                                        materialName = projectMaterials.MaterialName,
                                        quantity = projectMaterials.Quantity,
                                        amount =  projectMaterials.Amount,
                                        StoreName = projectMaterials.StoreName,
                                        purchaseDate = projectMaterials.PurchaseDate,
                                        added = projectMaterials.Added,
                                        addedBy = projectMaterials.AddedBy
                                    };

            return diyProjectIdDetails;
        }

        public IQueryable<DiyProjectMaterial> getDiyProjectMaterialById(int materialId)
        {
            var diyProjectMaterial = from ProjectMaterial material in entities.ProjectMaterials
                                     where material.Id == materialId
                                     select new DiyProjectMaterial()
                                     {
                                         id = material.Id,
                                         diyProjectId = material.ProjectId,
                                         materialName = material.MaterialName,
                                         quantity = material.Quantity,
                                         amount = material.Amount,
                                         storeName = material.StoreName,
                                         purchaseDate = material.PurchaseDate,
                                         added = material.Added,
                                         addedBy = material.AddedBy
                                     };

            return diyProjectMaterial;
        }

        /// <summary>
        /// Gets Diy project material by storeName, materialName, amount, purchaseDate
        /// If a query finds a matching record, may potentially be used for user
        /// intervention before adding a new entry
        /// </summary>
        /// <param name="diyProjectId"></param>
        /// <param name="storeName"></param>
        /// <param name="materialName"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="purchaseDate"></param>
        /// <param name="user"></param>
        /// <returns>DiyProjectMaterial object</returns>
        public IQueryable<DiyProjectMaterial> getDiyProjectMaterialByInputs(int diyProjectId, string storeName, string materialName, int quantity, decimal amount, DateTime purchaseDate)
        {
            var diyProjectMaterial = from ProjectMaterial material in entities.ProjectMaterials
                                     where storeName == material.StoreName && materialName == material.MaterialName && amount == material.Amount && purchaseDate == material.PurchaseDate
                                     select new DiyProjectMaterial()
                                     {
                                         id = material.Id,
                                         diyProjectId = material.ProjectId,
                                         materialName = material.MaterialName,
                                         quantity = material.Quantity,
                                         amount = material.Amount,
                                         storeName = material.StoreName,
                                         purchaseDate = material.PurchaseDate,
                                         added = material.Added,
                                         addedBy = material.AddedBy
                                     };

            return diyProjectMaterial;
        }

        /// <summary>
        /// Adds Diy Project material by project id
        /// </summary>
        /// <param name="diyProjectId"></param>
        /// <param name="storeName"></param>
        /// <param name="materialName"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="purchaseDate"></param>
        /// <param name="user"></param>
        /// <returns>DiyProjectMaterial</returns>
        /// <exception cref="DiyProjectAddException"></exception>
        public DiyProjectMaterial addMaterialBydiyProjectId(int diyProjectId, string storeName, string materialName, int quantity, decimal amount, DateTime purchaseDate, string user)
        {
            DiyProjectMaterial diyProjectMaterial = new DiyProjectMaterial();

            if (diyProjectId == 0 || storeName == null || materialName == null || quantity == 0 || amount == 0)
            {
                throw new DiyProjectAddException("Project Id, StoreName, MaterialName, quantity, and amount are all required fields!");
            }

            else
            {

                using (DiyProjectEntities entities = new DiyProjectEntities())
                {

                    ProjectMaterial projectMaterial = new ProjectMaterial()
                    {
                        StoreName = storeName,
                        MaterialName= materialName,
                        Quantity = quantity,
                        Amount = amount,
                        PurchaseDate = purchaseDate,
                        Added = DateTime.Now,
                        AddedBy = user
                    };

                    entities.ProjectMaterials.Add(projectMaterial);
                    entities.SaveChanges();

                    diyProjectMaterial = getDiyProjectMaterialById(projectMaterial.Id).FirstOrDefault();
                        
                    
                }
            }

            return diyProjectMaterial;
            
        }

    }
}
