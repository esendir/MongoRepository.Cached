![build status](https://dissperltd.visualstudio.com/_apis/public/build/definitions/6c214d4e-3676-4b7d-b7fb-9d7079975c67/8/badge)

## MongoRepository.Cached
Cache feature for [MongoRepository](https://github.com/esendir/MongoRepository)

### Definition

#### Model
You don't need to create a model, but if you are doing so you need to extend Entity
```csharp
	//if you are able to define your model
	public class User : Entity
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
```

#### Repository
There are multiple base constructors, read summaries of others
```csharp
	public class UserRepository : CachedRepository<User>
	{
		public UserRepository(string connectionString) : base(connectionString) {}

		//custom method
		public User FindbyUsername(string username)
		{
			return First(i => i.Username == username);
		}
		
		//custom method2
		public void UpdatePassword(User item, string newPassword)
		{
			repo.Update(item, i => i.Password, newPassword);
		}
	}
```