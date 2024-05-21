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
        /// 
        public async Task<string> addProject_Async(string projectName, DateTime startDate, DateTime finishDate, string thumbNail, string addedBy, DateTime added)
        {
            Project project = await Task.Run(() => addProject(projectName, startDate, finishDate, thumbNail, addedBy, added));
            return JsonConvert.SerializeObject(project);
        }
        public Project addProject(string projectName, DateTime startDate, DateTime finishDate, string thumbNail, string addedBy, DateTime added)
        {

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
                    Project project = new Project()
                    {
                        Name = projectName,
                        StartDate = startDate,
                        FinishDate = finishDate,
                        thumbnail = thumbNail,
                        AddedBy = addedBy,  
                        Added = added 
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
        public async Task<string> UpdateProject_Async(int projectId, string projectName, DateTime startDate, DateTime finishDate, string thumbNail)
        {
            Project project = await Task.Run(() => UpdateProject(projectId, projectName, startDate, finishDate, thumbNail));
            return JsonConvert.SerializeObject(project);
        }
        public Project UpdateProject(int projectId, string projectName, DateTime startDate, DateTime finishDate, string thumbNail)
        {
            Project project = new Project();
            if (projectId <= 0)
            {
                throw new DiyProjectAddException("Project Id must be supplied!");
            }
            else
            {
                project = entities.Projects.FirstOrDefault(p => p.id == projectId);
                project.Name = projectName;
                project.StartDate = startDate;
                project.FinishDate = finishDate;
                project.thumbnail = thumbNail;
                entities.SaveChanges();
                return project;
            }
        }

        public async Task<string> UpdateProjectMaterial_Async(int projectMaterialId, string materialName, int quantity, decimal amount, string storeName, DateTime purchaseDate)
        {
            ProjectMaterial projectMaterial = await Task.Run(() => UpdateProjectMaterial(projectMaterialId, materialName, quantity, amount, storeName, purchaseDate));
            DiyProjectMaterial diyProjectMaterial = new DiyProjectMaterial()
            {
                materialName = projectMaterial.MaterialName,
                quantity = projectMaterial.Quantity,
                amount = projectMaterial.Amount,
                purchaseDate = projectMaterial.PurchaseDate,
                id = projectMaterial.Id,
                diyProjectId = projectMaterial.ProjectId,
                storeName = projectMaterial.StoreName
            };
            return JsonConvert.SerializeObject(diyProjectMaterial);
        }

        public ProjectMaterial UpdateProjectMaterial(int projectMaterialId, string materialName, int quantity, decimal amount, string storeName, DateTime purchaseDate)
        {
            ProjectMaterial material = new ProjectMaterial();

            if (projectMaterialId <= 0)
            {
                throw new DiyProjectMaterialAddException("projectMaterialId must be supplied.");
            }
            else
            {
                material = entities.ProjectMaterials.FirstOrDefault(p => p.Id == projectMaterialId);
                material.MaterialName = materialName;
                material.Quantity    = quantity;
                material.Amount = amount;
                material.StoreName = storeName;
                material.PurchaseDate = purchaseDate;
                entities.SaveChanges();
                return material;


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
        public async Task<string> getMaterialsByProjectId_Async(int projectId)
        {
            IEnumerable<DiyProjectIdDetail> ProjectDetails = await Task.Run(() => getMaterialsByProjectId(projectId));
            return JsonConvert.SerializeObject(ProjectDetails);
        }
        /// <summary>
        /// Gets list of Project Materies by Project Id
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>List<ProjectMaterial></returns>
        public IEnumerable<DiyProjectIdDetail> getMaterialsByProjectId(int projectId)
        {

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
                                      amount = projectMaterials.Amount,
                                      storeName = projectMaterials.StoreName,
                                      purchaseDate = projectMaterials.PurchaseDate,
                                      added = projectMaterials.Added,
                                      addedBy = projectMaterials.AddedBy
                                  };

            return diyProjectIdDetails;
        }
        public async Task<string> getDiyProjectMaterialById_Async(int materialId)
        {
            IEnumerable<DiyProjectMaterial> diyProjectMaterial = await Task.Run(() => getDiyProjectMaterialById(materialId));
            return JsonConvert.SerializeObject(diyProjectMaterial);
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

        public async Task<string> addMaterialBydiyProjectId_Async(int diyProjectId, string storeName, string materialName, int quantity, decimal amount, DateTime purchaseDate, string user)
        {
            DiyProjectMaterial projectMaterial = await Task.Run(() => addMaterialBydiyProjectId(diyProjectId, storeName, materialName, quantity, amount, purchaseDate, user));
            return JsonConvert.SerializeObject(projectMaterial);
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
            DiyProjectMaterial newProjectMaterial = new DiyProjectMaterial();

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
                        ProjectId = diyProjectId,
                        StoreName = storeName,
                        MaterialName = materialName,
                        Quantity = quantity,
                        Amount = amount,
                        PurchaseDate = purchaseDate,
                        Added = DateTime.Now,
                        AddedBy = user
                    };

                    entities.ProjectMaterials.Add(projectMaterial);
                    entities.SaveChanges();

                    newProjectMaterial = getDiyProjectMaterialById(projectMaterial.Id).FirstOrDefault();


                }
            }

            return newProjectMaterial;

        }

    }
}
