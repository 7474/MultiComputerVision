using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Data
{
    public class CosmosDbContext : IDisposable
    {
        private CosmosClient cosmosClient;
        private Database database;

        public Container UserContainer { get; private set; }

        public CosmosDbContext(string connectionString)
        {
            cosmosClient = new CosmosClient(connectionString);
        }
        public void Initialize(string databaseId)
        {
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            UserContainer = database.CreateContainerIfNotExistsAsync("ApplicationUser", "/UserName").Result;
        }

        public void Dispose()
        {
            cosmosClient.Dispose();
        }
    }
    public class ApplicationUser : IdentityUser
    {
        [JsonProperty(PropertyName = "id")]
        public override string Id { get; set; }
    }

    public class CosmosUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>
    {
        private CosmosDbContext context;

        public CosmosUserStore(CosmosDbContext context)
        {
            this.context = context;
        }

        public void Dispose() { }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.Id;

        public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.UserName;

        public async Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
            => user.UserName = userName;

        public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.NormalizedUserName;

        public async Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
            => user.NormalizedUserName = normalizedName;

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            await context.UserContainer.CreateItemAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            await context.UserContainer.UpsertItemAsync(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            await context.UserContainer.DeleteItemAsync<ApplicationUser>(user.Id, new PartitionKey(user.UserName));
            return IdentityResult.Success;
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            (await context.UserContainer.GetItemQueryIterator<ApplicationUser>(new QueryDefinition($"SELECT * FROM c WHERE c.id = '{userId}'")).ReadNextAsync()).FirstOrDefault();
            // XXX なんかLINQだとダメだった。プロパティ名の解決がイケていないのかも。
            //(await context.UserContainer.GetItemLinqQueryable<ApplicationUser>().Where(a => a.Id == userId).ToFeedIterator().ReadNextAsync()).FirstOrDefault();

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) =>
            (await context.UserContainer.GetItemLinqQueryable<ApplicationUser>().Where(a => a.NormalizedUserName == normalizedUserName).ToFeedIterator().ReadNextAsync()).FirstOrDefault();

        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
            => user.PasswordHash = passwordHash;

        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.PasswordHash;

        public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
            => !string.IsNullOrEmpty(user.PasswordHash);

        public async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
            (await context.UserContainer.GetItemLinqQueryable<ApplicationUser>().Where(a => a.NormalizedEmail == normalizedEmail).ToFeedIterator().ReadNextAsync()).FirstOrDefault();

        public async Task<string> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.Email;

        public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.EmailConfirmed;

        public async Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.NormalizedEmail;

        public async Task SetEmailAsync(ApplicationUser user, string email, CancellationToken cancellationToken)
            => user.Email = email;

        public async Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
            => user.EmailConfirmed = confirmed;

        public async Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken)
            => user.NormalizedEmail = normalizedEmail;

        public async Task<string> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.PhoneNumber;

        public async Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
            => user.PhoneNumberConfirmed;

        public async Task SetPhoneNumberAsync(ApplicationUser user, string phoneNumber, CancellationToken cancellationToken)
            => user.PhoneNumber = phoneNumber;

        public async Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
            => user.PhoneNumberConfirmed = confirmed;
    }

    public class MockRoleStore : IRoleStore<IdentityRole>
    {
        public void Dispose() { }

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
            => throw new NotImplementedException();

        public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
            => throw new NotImplementedException();
    }
}
