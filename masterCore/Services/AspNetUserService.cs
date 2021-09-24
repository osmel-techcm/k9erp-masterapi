using masterCore.Entities;
using masterCore.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace masterCore.Services
{
    public class AspNetUserService : IAspNetUserService
    {
        private readonly IAspNetUserRepo _aspNetUserRepo;
        private readonly IUsersTenantsRelationService _usersTenantsRelationService;
        private readonly IAspNetUsersGroupsService _aspNetUsersGroupsService;

        public AspNetUserService(IAspNetUserRepo aspNetUserRepo, IUsersTenantsRelationService usersTenantsRelationService, IAspNetUsersGroupsService aspNetUsersGroupsService)
        {
            _aspNetUserRepo = aspNetUserRepo;
            _usersTenantsRelationService = usersTenantsRelationService;
            _aspNetUsersGroupsService = aspNetUsersGroupsService;
        }

        public async Task<responseData> GetUsersByCustomer(int customer)
        {
            return await _aspNetUserRepo.GetUsersByCustomer(customer);
        }

        public async Task<responseData> PostAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new responseData();

            if (applicationUser.masterDealer == null) {
                applicationUser.masterDealer = false;
            }                

            responseData = await _aspNetUserRepo.PostAspNetUserUser(applicationUser);
            if (responseData.error)
            {
                return responseData;
            }

            if (applicationUser.masterDealer == false)
            {
                responseData = await _usersTenantsRelationService.SyncUsersTenantsRelation(applicationUser.CustomerId);
                if (responseData.error)
                {
                    return responseData;
                }
            }

            return responseData;
        }

        public async Task<responseData> PutAspNetUserUser(AspNetUser applicationUser)
        {            
            return await _aspNetUserRepo.PutAspNetUserUser(applicationUser);
        }

        public async Task<responseData> GetUsersByTenant(PaginatorData paginatorData, string userId)
        {
            var userResponse = await _aspNetUserRepo.GetSiblingUsers(paginatorData, userId);
            if (userResponse.error)
            {
                return userResponse;
            }

            var aspNetUser = (List<AspNetUser>)userResponse.data;
            var paginatorResult = Paginator<AspNetUser>.Create(aspNetUser, paginatorData);

            addFields(paginatorResult);

            userResponse.data = paginatorResult;

            return userResponse;
        }

        public async Task<responseData> GetUserByTenant(string currentUserId, string userId)
        {
            return await _aspNetUserRepo.GetUser(currentUserId, userId);
        }

        public async Task<responseData> PutUserByTenant(string currentUserId, AspNetUser user)
        {
            var userValResponse = await validateUserData(currentUserId, user);
            if (userValResponse.error)
            {
                return userValResponse;
            }

            user = (AspNetUser)userValResponse.data;

            return await _aspNetUserRepo.PutAspNetUserUser(user);
        }

        public async Task<responseData> PostUserByTenant(string currentUserId, AspNetUser user)
        {
            var userValResponse = await validateUserData(currentUserId, user);
            if (userValResponse.error)
            {
                return userValResponse;
            }

            user = (AspNetUser)userValResponse.data;
            return await PostAspNetUserUser(user);
        }

        public async Task<responseData> GetUsers(PaginatorData paginatorData)
        {
            var userResponse = await _aspNetUserRepo.GetUsers(paginatorData);
            if (userResponse.error)
            {
                return userResponse;
            }

            var aspNetUser = (List<AspNetUser>)userResponse.data;
            var paginatorResult = Paginator<AspNetUser>.Create(aspNetUser, paginatorData);

            addFields(paginatorResult);

            userResponse.data = paginatorResult;

            return userResponse;
        }

        public async Task<responseData> GetUser(string id)
        {
            return await _aspNetUserRepo.GetUser(id);
        }

        private void addFields(Paginator<AspNetUser> paginatorResult)
        {
            var _fieldData = new fieldData
            {
                order = 1,
                name = "id",
                field = "id",
                type = "text",
                display = false,
                colSize = 1
            };

            paginatorResult.fields = new List<fieldData>
            {
                _fieldData
            };

            _fieldData = new fieldData
            {
                order = 2,
                name = "Username",
                field = "userName",
                type = "text",
                display = true,
                colSize = 20,
                sort = "asc"
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 3,
                name = "Email",
                field = "email",
                type = "text",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 4,
                name = "Phone",
                field = "phoneNumber",
                type = "text",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 5,
                name = "Name",
                field = "name",
                type = "text",
                display = true,
                colSize = 20
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 6,
                name = "Last Name",
                field = "lastName",
                type = "text",
                display = true,
                colSize = 20
            };

            paginatorResult.fields.Add(_fieldData);
        }

        private async Task<responseData> validateUserData(string currentUserId, AspNetUser user)
        {
            var responseData = new responseData();
            var userId = string.Empty;

            if (user.IdGroup == null)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "The group is required!";
                return responseData;
            }

            if (string.IsNullOrEmpty(user.Id))
            {
                userId = currentUserId;
            } else 
            {
                userId = user.Id;
            }

            var dbUserResponse = await _aspNetUserRepo.GetUser(currentUserId, userId);
            if (dbUserResponse.error)
            {
                return dbUserResponse;
            }

            var dbUser = (AspNetUser)dbUserResponse.data;
            var dbGroupResponse = await _aspNetUsersGroupsService.GetAspNetUsersGroup(dbUser.IdGroup ?? 0);
            if (dbGroupResponse.error)
            {
                return dbGroupResponse;
            }

            var dbGroup = (AspNetUsersGroup)dbGroupResponse.data;
            var siblingGroupsResponse = await _aspNetUsersGroupsService.GetAspNetUsersGroupByCustomer(dbGroup.idCustomer ?? 0);
            if (siblingGroupsResponse.error)
            {
                return siblingGroupsResponse;
            }

            var siblingGroups = (List<AspNetUsersGroup>)siblingGroupsResponse.data;
            if (!siblingGroups.Exists(x=>x.id == user.IdGroup))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "The group can't be found!";
                return responseData;
            }

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.Password = user.PasswordHash;
            }
            
            user.UserName = user.Email;
            user.masterDealer = false;
            user.CustomerId = dbGroup.idCustomer ?? 0;
            
            if (string.IsNullOrEmpty(user.Password))
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "The password is required!";
                return responseData;
            }

            responseData.data = user;

            return responseData;
        }

    }
}
