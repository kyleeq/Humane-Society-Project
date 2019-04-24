using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();

            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation) //ADD USER INPUT CHECK
        {
           switch (crudOperation)
            {
                case "update":
                    AddUsernameAndPassword(employee);
                    break;
                case "read":
                    Console.WriteLine($"First Name:{employee.FirstName}\n Last Name:{employee.LastName}\n" +
                        $"Username: {employee.UserName}\n Email Address: {employee.Email}\n" +
                        $"Employee ID: {employee.EmployeeId}\n Employee Number: {employee.EmployeeNumber}");
                    break;                    
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    db.SubmitChanges();
                    break;
                case "create":
                    CreateNewEmployee(employee.FirstName, employee.LastName, employee.UserName, employee.Password, employee.Email);
                    break;
            }
        }
        // DONE WITH CreateNewEmployee
        internal static void CreateNewEmployee(string firstName, string lastName, string username, string password, string email)
        {
            Employee employee = new Employee();

            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.UserName = username;
            employee.Password = password;
            employee.Email = email;
            db.Employees.InsertOnSubmit(employee);
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal) //DONE
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id) //DONE
        {
            var petWithId = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return petWithId;
        }       
        // FIGURE OUT HOW TO USE THE DICTIONARY PARAMETER
        internal static void UpdateAnimal(Animal animal, Dictionary<int, string> updates)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).Single();

            foreach (KeyValuePair<int, string> pet in updates)
                switch (pet.Key)
                {
                    case 1:
                        animal.Name = pet.Value;
                        break;
                    case 2:
                        animal.Weight = int.Parse(pet.Value);
                        break;
                    case 3:
                        animal.Age = int.Parse(pet.Value);
                        break;
                    case 4:
                        animal.Demeanor = pet.Value;
                        break;
                    case 5:
                        animal.KidFriendly = bool.Parse(pet.Value);
                        break;
                    case 6:
                        animal.PetFriendly = bool.Parse(pet.Value);
                        break;
                    case 7:
                        animal.Gender = pet.Value;
                        break;
                    case 8:
                        animal.AdoptionStatus = pet.Value;
                        break;
                }

            animalFromDb.Name = animal.Name;
            animalFromDb.Weight = animal.Weight;
            animalFromDb.Age = animal.Age;
            animalFromDb.Demeanor = animal.Demeanor;
            animalFromDb.KidFriendly = animal.KidFriendly;
            animalFromDb.PetFriendly = animal.PetFriendly;
            animalFromDb.Gender = animal.Gender;
            animalFromDb.AdoptionStatus = animal.AdoptionStatus;
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal) //DONE
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }

        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            throw new NotImplementedException();
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName) //DONE
        {
            var categories = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            return categories.CategoryId;
        }
        
        internal static Room GetRoom(int animalId) //DONE
        {
            var getRoom = db.Rooms.Where(r => r.AnimalId == animalId).FirstOrDefault();
            return getRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName) //DONE
        {
            var dietPlan = db.DietPlans.Where(d => d.Name == dietPlanName).FirstOrDefault();
            return dietPlan.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client) //FIX LOGIC
        {
            //client.ClientId = db.Adoptions.ClientId;
            //animal.AnimalId = db.Adoptions.AnimalId;
        }

        internal static IQueryable<Adoption> GetPendingAdoptions() //DONE
        {
            var petAdoptionsPending = db.Adoptions.Where(a => a.ApprovalStatus == null);
            return petAdoptionsPending;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption) //NEEDS FUNCTIONAL LOGIC
        {
            var updatedAdoption = db.Adoptions.Where(a => a.ApprovalStatus == null);
            db.SubmitChanges(); //?
        }

        internal static void RemoveAdoption(int animalId, int clientId) //DONE
        {
            var adoptionToRemove = db.Adoptions.Where(a => a.ClientId == clientId && a.AnimalId == animalId).Single();
            db.Adoptions.DeleteOnSubmit(adoptionToRemove);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal) //DONE
        {
            var animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId);
            return animalShot;
        }

        internal static void UpdateShot(string shotName, Animal animal) //ASK DEJA FOR LOGIC
        {
            //var shot = db.AnimalShots.Where(s => s.AnimalId == animal.AnimalId).Single();
            //shot.ShotId += db.AnimalShot.ShotId;

            //AnimalShot.DateReceived = animalWithUpdates.DateReceived;
            //db.SubmitChanges();
            throw new NotImplementedException();
        }
    }
}