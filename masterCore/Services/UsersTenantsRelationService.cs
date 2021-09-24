using masterCore.Entities;
using masterCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class UsersTenantsRelationService : IUsersTenantsRelationService
    {
        private readonly IUsersTenantsRelationRepo _usersTenantsRelationRepo;

        public UsersTenantsRelationService(IUsersTenantsRelationRepo usersTenantsRelationRepo)
        {
            _usersTenantsRelationRepo = usersTenantsRelationRepo;
        }

        public async Task<responseData> GetUsersTenantsRelation(int id)
        {
            return await _usersTenantsRelationRepo.GetUsersTenantsRelation(id);
        }

        public async Task<responseData> GetUsersTenantsRelations(PaginatorData paginatorData)
        {
            var usersTenantsRelationResponse = await _usersTenantsRelationRepo.GetUsersTenantsRelations();

            if (usersTenantsRelationResponse.error)
            {
                return usersTenantsRelationResponse;
            }

            var usersTenantsRelations = (List<UsersTenantsRelation>)usersTenantsRelationResponse.data;

            usersTenantsRelationResponse.data = Paginator<UsersTenantsRelation>.Create(usersTenantsRelations, paginatorData);

            return usersTenantsRelationResponse;
        }

        public async Task<responseData> PostUsersTenantsRelation(UsersTenantsRelation usersTenantsRelation)
        {
            return await _usersTenantsRelationRepo.PostUsersTenantsRelation(usersTenantsRelation);
        }

        public async Task<responseData> PutUsersTenantsRelation(int id, UsersTenantsRelation usersTenantsRelation)
        {
            return await _usersTenantsRelationRepo.PutUsersTenantsRelation(id, usersTenantsRelation);
        }

        public async Task<responseData> DeleteUsersTenantsRelation(int id)
        {
            return await _usersTenantsRelationRepo.DeleteUsersTenantsRelation(id);
        }

        public async Task<responseData> UsersTenantsRelationExist(UsersTenantsRelation usersTenantsRelation)
        {
            return await _usersTenantsRelationRepo.UsersTenantsRelationExist(usersTenantsRelation);
        }

        public async Task<responseData> SyncUsersTenantsRelation(int customerId)
        {
            return await _usersTenantsRelationRepo.SyncUsersTenantsRelation(customerId);
        }
    }
}
