<Query Kind="Program">
  <Connection>
    <ID>8f5092f3-a57d-4c44-adf2-8d7dfe5a96d3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>OLTP-DMIT2018</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
  <NuGetReference>BYSResults</NuGetReference>
</Query>

using BYSResults;


void Main()
{
	// Create the CodeBehind Class instance that will be used for
	//		testing all of the interactions with the Service
	//		SystemLibrary methods.  
	// Note: You would only need one CodeBehind class for the group
	//			of methods intended to be used for the CodeBehind of
	//			a single Blazor component page.  You would set up
	//			an entirely new LINQ file corresponding to the methods
	//			needed on a different BLazor component page.
	// Remember: "this" is referring to the currently selected database
	//				connection at the top of the LINQ query file.  In the
	//				CodeBehind class it is the "context".
	CodeBehind codeBehind = new CodeBehind(this);
	
	// BEGIN GetCustomers() TESTS
	
	// Error Tests
	// Last name and phone number cannot both be null, empty, or whitespace
	codeBehind.GetCustomers(string.Empty, string.Empty);
	if (codeBehind.Customers == null || codeBehind.Customers.Count == 0)
	{
		codeBehind.ErrorDetails
				  .Dump("Should Fail - Last Name and Phone Both Null Test");
	}
	else
	{
		codeBehind.Customers
				  .Dump("No customers should be returned, but received:");
	}

	// No customers were found with the provided partial last name or phone
	//		number
	codeBehind.GetCustomers("zzz", "999999");
	if (codeBehind.Customers == null || codeBehind.Customers.Count == 0)
	{
		codeBehind.ErrorDetails
				  .Dump("Should Fail - No Customers for provided last name and phone");
	}
	else
	{
		codeBehind.Customers
				  .Dump("No customers should be returned, but received:");
	}

	// Pass Tests
	// Customers should be returned when provided a last name and/or phone
	//		number that partially matches in the Customers table
	codeBehind.GetCustomers("Fo", "432");
	if (codeBehind.Customers != null || codeBehind.Customers.Count > 0)
	{
		codeBehind.Customers
				  .Dump("Should Pass - Valid last name and/or phone");
	}
	else
	{
		codeBehind.ErrorDetails
				  .Dump("Failed! Customers should have been received!");
	}

	// Customers should be returned when provided a last name
	//		that partially matches in the Customers table
	codeBehind.GetCustomers("Fo", "");
	if (codeBehind.Customers != null || codeBehind.Customers.Count > 0)
	{
		codeBehind.Customers
				  .Dump("Should Pass - Valid last name");
	}
	else
	{
		codeBehind.ErrorDetails
				  .Dump("Failed! Customers should have been received!");
	}

	// Customers should be returned when provided a phone number
	//		that partially matches in the Customers table
	codeBehind.GetCustomers("", "432");
	if (codeBehind.Customers != null || codeBehind.Customers.Count > 0)
	{
		codeBehind.Customers
				  .Dump("Should Pass - Valid phone number");
	}
	else
	{
		codeBehind.ErrorDetails
				  .Dump("Failed! Customers should have been received!");
	}

	// END GetCustomers() TESTS

	// BEGIN GetCustomer() TESTS

	// Error Test
	// No customer should be returned when provided a customer ID
	//		less than or equal to zero.
	codeBehind.GetCustomer(0);
	if (codeBehind.customer.CustomerID == 0)
	{
		codeBehind.ErrorDetails
				  .Dump("Should Fail - Invalid customer ID");
	}
	else
	{
		codeBehind.customer
				  .Dump("No customer should be returned, but received:");
	}

	// No customer should be returned when provided a customer ID
	//		that does not exist.
	codeBehind.GetCustomer(100000000);
	if (codeBehind.customer.CustomerID == 0)
	{
		codeBehind.ErrorDetails
				  .Dump("Should Fail - No Customer for provided customer ID");
	}
	else
	{
		codeBehind.customer
				  .Dump("No customer should be returned, but received:");
	}


	// Pass Test
	// Customer should be returned when provided an existing customer ID
	codeBehind.GetCustomer(1);
	if (codeBehind.customer != null && codeBehind.customer.CustomerID > 0)
	{
		codeBehind.customer
				  .Dump("Should Pass - Customer exists for Customer ID");
	}
	else
	{
		codeBehind.ErrorDetails
				  .Dump("Failed! Customer should have been received!");
	}

}


public class CodeBehind(TypedDataContext context)
{
	// This section includes members and properties that we need
	//		in LINQPad but will not require in Blazor
	#region Supporting Members
	// This public property is not required in Blazor because the 
	//		Blazor component may directly access any data created 
	//		in the code behind page
	// The two following lines are identical in functionality.  The
	//		first line is a short version of the second line.
	public List<string> ErrorDetails => errorDetails;
	//public List<string> ErrorDetails { get { return errorDetails; } }

	// This line of code will be replaced by an [Inject] line in the
	//		Blazor app, used for allowing use of your Service classes
	//		in your BLL folder of your System Library.
	private readonly CustomerService CustomerService
									= new CustomerService(context);
	#endregion

	// These data members are meant to mimic what you will actually 
	//		have in your code behind file.
	#region Local Data Members
	// A list for storing the error messages returned from the System
	//		Library method, if any, and used by the BLazor component.
	private List<string> errorDetails = new List<string>();
	private string errorMessage = string.Empty;

	public CustomerEditView customer = new CustomerEditView();
	
	// In the event that a result set is successfully retrieved, this
	//		List will store the results for use by the Blazor component.	
	public List<CustomerSearchView> Customers =
							new List<CustomerSearchView>();
	#endregion
	
	// This method will provide information extracted from the controls
	//		of the UI to the CustomerService method that will attempt to
	//		save the new or edited details to the database.
	public void AddEditCustomer(CustomerEditView customer)
	{
		// Clear all error messages in the UI. 
		errorDetails.Clear();
		errorMessage = string.Empty;

		// Standard try/catch block for dealing with unanticipated
		//		exceptions encountered by the CustomerService method.
		try
		{
			// Catch the result object returned from the CustomerService
			//		class method
			var result = CustomerService.AddEditCustomer(customer);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if(result.IsSuccess)
			{
				customer = result.Value;
			}
			else
			{
				// Otherwise, bind the error messages returned to the 
				//		collection used by the UI.
				errorDetails = GetErrorMessages(result.Errors.ToList());
			}
		}
		catch(Exception ex)
		{
			// If an exception occurs, show the user the base cause for
			//		the exception.
			errorMessage = GetInnerMostException(ex).Message;
		}		
	}


	// This method is for retrieving a single customer by providing the 
	//		customer's ID.	
	public void GetCustomer(int customerID)
	{
		// Clear all error messages in the UI.
		errorDetails.Clear();
		errorMessage = string.Empty;

		// Standard try/catch block for dealing with unanticipated
		//		exceptions encountered by the CustomerService method.
		try
		{
			// Catch the result object returned from the CustomerService
			//		class method
			var result = CustomerService.GetCustomer(customerID);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind collection being read by the UI.
			if (result.IsSuccess)
			{
				customer = result.Value;
			}
			else
			{
				// Otherwise, bind the error messages returned to the 
				//		collection used by the UI.
				errorDetails = GetErrorMessages(result.Errors.ToList());
			}
		}
		catch (Exception ex)
		{
			// If an exception occurs, show the user the base cause for
			//		the exception.
			errorMessage = GetInnerMostException(ex).Message;
		}
	}


	// This method is for retrieving customers that at least partially 
	//		match a supplied last name or phone number
	public void GetCustomers (string lastName, string phone)
	{
		// Clear all error messages in the UI.
		errorDetails.Clear();
		errorMessage = string.Empty;
		
		// Standard try/catch block for dealing with unanticipated
		//		exceptions encountered by the CustomerService method.
		try
		{
			// Catch the result object returned from the CustomerService
			//		class method
			var result = CustomerService.GetCustomers(lastName, phone);
			
			// If the operation was successful, bind the returned information
			//		to the CodeBehind collection being read by the UI.
			if (result.IsSuccess)
			{
				Customers = result.Value;
			}
			else
			{
				// Otherwise, bind the error messages returned to the 
				//		collection used by the UI.
				errorDetails = GetErrorMessages(result.Errors.ToList());
			}
		}
		catch(Exception ex)
		{
			// If an exception occurs, show the user the base cause for
			//		the exception.
			errorMessage = GetInnerMostException(ex).Message;
		}
	}
}

public class CustomerService 
{
	// We must store the "context", meaning database connection, for 
	//		your methods in this class to use to perform transactional
	//		operations against your database.  The CRUD operations...
	private readonly TypedDataContext _hogWildContext;

	// This constructor is required because we want to check for a 
	//		valid context as part of the successful creation of the 
	//		class instance.  The short-hand version used in the 
	//		CodeBehind class does not allow this check to be performed.
	public CustomerService(TypedDataContext context)
	{
		// If there is a valid context instance accepted, assign it to
		//		the ReadOnly class variable.
		_hogWildContext = context
					// If the passed in context is null, throw an 
					//		Exception.  You could also have accomplished
					//		this task with an if statement checking for
					//		a null context at the beginning of the 
					//		constructor.
					?? throw new ArgumentException(nameof(context));
	}
	
	// This method will attempt to store the new or edited information
	//		for a customer passed in using the CustomerEditView view model
	//		instance accepted from the caller.
	public Result<CustomerEditView> AddEditCustomer(CustomerEditView editCustomer)
	{
		var result = new Result<CustomerEditView>();
		
		#region Data Validation
		
		// Check that a view model instance was supplied 
		if (editCustomer == null)
		{
			result.AddError(new Error("Missing Customer", 
										"No customer object was supplied!"));
			
			// If we received no information, we may exit immediately
			return result;
		}
		
		// The following are all checks for missing information in the
		//		supplied view model.  Make sure to check all supplied
		//		pieces of data so you may return a full list of things
		//		that must be fixed to the user.
		// Note: You should make sure to match your checks to what is 
		//		 	required in the database fields. 
		if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
		{
			result.AddError(new Error("Missing information", 
											"First name required!"));
		}

		if (string.IsNullOrWhiteSpace(editCustomer.LastName))
		{
			result.AddError(new Error("Missing information",
											"Last name required!"));
		}

		if (string.IsNullOrWhiteSpace(editCustomer.Phone))
		{
			result.AddError(new Error("Missing information",
											"Phone number required!"));
		}

		if (string.IsNullOrWhiteSpace(editCustomer.Email))
		{
			result.AddError(new Error("Missing information",
											"Email is required!"));
		}
		
		// If after all of the above data checks have been completed, one
		//		or more errors have been encountered, then you may exit
		//		immediately.  There is no point to initiating database
		//		operations with invalid data.
		if(result.Errors.Count > 0)
		{
			return result;
		}
		#endregion

		#region Business Rules

		// After simple data validation, we must also enforce any business
		//		rules that further constrain the data.  
		
		// In this case, we wish to ensure that if a new customer is being 
		//		provided, that an existing customer with the same full name
		//		and phone number of an existing customer.
		if (editCustomer.CustomerID <= 0)
		{
			bool customerExist = _hogWildContext
								 .Customers.Any(customer =>
								 customer.FirstName.ToLower()
								 		== editCustomer.FirstName.ToLower()
								&& customer.LastName.ToLower()
										== editCustomer.LastName.ToLower()
								&& customer.Phone.ToLower()
										== editCustomer.Phone.ToLower());
			
			// Add an error message if a match is found.
			if (customerExist)
			{
				result.AddError(new Error("Existing Customer Data",
							"A customer with the same first name, last name," +
							" and phone number already exists in the database!"));
			}
		}		
		
		// A different way to check if any errors have been encountered
		//		is to check the IsFailure boolean in the result object.
		//		Effectively it checks to see if there are any Errors in the
		//		collection.
		if (result.IsFailure)
		{
			return result;
		}
		#endregion
		
		// If we get to this point, then we have valid data and no business
		//		rules have been violated.  Time to see if we have a customer
		//		matching the provided customer ID.
		Customer customer = _hogWildContext.Customers
							.Where(x => x.CustomerID == editCustomer.CustomerID)
							.FirstOrDefault();
		
		// If there is no matching customer, then we are dealing with a
		//		new customer and so we must create a new entity instance
		//		to populate.
		if (customer == null)
		{
			customer = new Customer();
		}
		
		// Transfer all customer information from the view model
		//		into the customer entity.
		customer.FirstName = editCustomer.FirstName;
		customer.LastName = editCustomer.LastName;
		customer.Address1 = editCustomer.Address1;
		customer.Address2 = editCustomer.Address2;
		customer.City = editCustomer.City;
		customer.ProvStateID = editCustomer.ProvStateID;
		customer.CountryID = editCustomer.CountryID;
		customer.PostalCode = editCustomer.PostalCode;
		customer.Email = editCustomer.Email;
		customer.Phone = editCustomer.Phone;
		customer.StatusID = editCustomer.StatusID;
		customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;
		
		// Save the customer information to the database
		if(customer.CustomerID == 0)
		{
			// If the ID was 0, we are performing an insert
			_hogWildContext.Customers.Add(customer);
		}
		else
		{
			// If we found an existing customer, the ID will not be 0,
			//		so we perform an update.
			_hogWildContext.Customers.Update(customer);
		}
		
		// Now we try to submit the information to the database.
		try
		{
			// If all is successful, the following will be executed and
			//		the catch will be skipped.
			_hogWildContext.SaveChanges();
		}
		catch(Exception ex)
		{
			// If there was an error communicating with the database,
			//		or a data constraint that we did not account for
			//		throws an error on the database side during the
			//		SaveChanges() operation, we will end up in the catch.
			
			// Clear the ChangeTracker so that future operations are not
			//		interfered with.
			_hogWildContext.ChangeTracker.Clear();
			
			// Populate an Error with the most inner exception message.
			// Alternately, you may rethrow the exception and get the 
			//		inner exception message in the code behind.  To do
			//		a rethrow, just use "throw ex;".  If rethrowing,
			//		you may skip the following couple lines of code.
			result.AddError(new Error("Error saving changes", 
				GetInnerMostException(ex).Message));
				
			return result;
		}
		
		// The only way we can get to this point is to have a successful
		//		operation.  So, retrieve the an updated CustomerEditView
		//		instance and return it through the Result instance.
		return GetCustomer(customer.CustomerID);
	}
	
	// This method will retrieve all information for a single customer
	//		that has the matching customer ID
	public Result<CustomerEditView> GetCustomer(int customerID)
	{
		var result = new Result<CustomerEditView>();

		#region Data Validation and BusinessRules - This time combined

		// Rule: Provided customer ID must be valid.  Greater then zero.
		if (customerID <= 0)
		{
			result.AddError(new Error("Missing Information!",
						"Customer ID must be greater than zero!"));

			// Only one check needed, so if this condition occurs, return
			//		immediately.  No point in carrying on.

			return result;
		}

		#endregion
		
		// In this case, we wish to retrieve all pieces of information
		//		for the customer matching the provided customer ID.
		var customer = _hogWildContext.Customers
					   .Where(c => c.CustomerID == customerID
					   			&& !c.RemoveFromViewFlag)
					   .Select(c => new CustomerEditView 
					   		{
								CustomerID = c.CustomerID,
								FirstName = c.FirstName,
								LastName = c.LastName,
								Address1 = c.Address1,
								Address2 = c.Address2,
								City = c.City,
								ProvStateID = c.ProvStateID,
								CountryID = c.CountryID,
								PostalCode = c.PostalCode,
								Phone = c.Phone,
								Email = c.Email,
								StatusID = c.StatusID,
								RemoveFromViewFlag = c.RemoveFromViewFlag
							})
					   .FirstOrDefault();

		// If no matching customer is found, return an error message
		if (customer == null)
		{
			result.AddError(new Error("No Customer!",
						"There is no customer with the provided customer ID!"));
			
			// No other errors possible at this point, so return immediately.
			return result;
		}

		// If we make it to here, we have customer information to return, so
		//		send back the populated CustomerEditView.
		return result.WithValue(customer);
	}
	
	public Result<List<CustomerSearchView>> 
							GetCustomers(string lastName, string phone)
	{
		var result = new Result<List<CustomerSearchView>>();
		
		#region Data Validation and BusinessRules - This time combined
		
		// Rule: lastName and phone may not both be empty or white space
		if (string.IsNullOrWhiteSpace(lastName) && 
								string.IsNullOrWhiteSpace(phone))
		{
			result.AddError(new Error("Missing Information!",
						"Must provide either last name or phone number!"));
			
			// Only one check needed, so if this condition occurs, return
			//		immediately.  No point in carrying on.
			return result;
		}
		
		#endregion
		
		// In this case, we wish to retrieve all customers where at
		//		there is a partial match to the lastName or phone
		//		provided.  RemoveFromViewFlag must be false.
		var customers = _hogWildContext.Customers
						.Where(c => (string.IsNullOrWhiteSpace(lastName)
										|| c.LastName.ToLower()
											.Contains(lastName.ToLower()))
								 && (string.IsNullOrWhiteSpace(phone)
								 		|| c.Phone.Contains(phone))
								 && !c.RemoveFromViewFlag)
						.Select(c => new CustomerSearchView 
							{
								CustomerID = c.CustomerID,
								FirstName = c.FirstName,
								LastName = c.LastName,
								City = c.City,
								Phone = c.Phone,
								Email = c.Email,
								StatusID = c.StatusID,
								TotalSales = c.Invoices.Sum(i => 
											((decimal?)(i.SubTotal + i.Tax) ?? 0))
							})
						.OrderBy(anon => anon.LastName)
						.ToList();
		
		// If no customers were found, return an error message
		if (customers == null || customers.Count == 0)
		{
			result.AddError(new Error("No Customers!", 
					"No customers were found matching the provided search values!"));
		
			// No other errors possible at this point, so return immediately.
			return result;
		}
		
		// If we make it to here, we have customer information to return, so
		//		send back the List<CustomerSearchView>.
		return result.WithValue(customers);
	}
}


#region View Models - For moving data between CodeBehind and System Library

// This VM will be used to retrieve complete 
//		information for a particular customer 
//		from the DB for the purposes of editing 
//		the info for that customer.  It will also 
//		be used to collect information for new
//		customers from the UI so that an insert
//		or update may be performed against the DB
//		Customers table.
public class CustomerEditView
{
	public int CustomerID { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Address1 { get; set; }
	public string Address2 { get; set; }
	public string City { get; set; }
	public int ProvStateID { get; set; }
	public int CountryID { get; set; }
	public string PostalCode { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }
	public int StatusID { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

// This VM will be used soley for retrieving information
//		about customers in order to populate the search
//		page in the Blazor application.  Consider that the
//		target page will have a couple of textboxes for
//		finding the customers that will be included in
//		the list.  Once the information is retrieved, we
// 		will populate a tabular view with the summarized
//		customer information retrieved.
public class CustomerSearchView
{
	public int CustomerID { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string City { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }
	public int StatusID { get; set; }
	public decimal TotalSales { get; set; }
}

#endregion - 

#region Helper Methods - Most Likely Used in the CodeBehind

// This method will accept a list of Error instances, likely
//		retrieved from a Result instance returned from a DB
//		System operation.  The List of Error will be turned
//		into a List of String so that they can be read into the
//		Error block on the Blazor page.
public static List<string> GetErrorMessages(List<Error> errorList)
{
	List<string> errorMessages = new List<string>();

	foreach (Error error in errorList)
	{
		errorMessages.Add(error.ToString());
	}

	return errorMessages;
}

// Though we have eliminated most of the error handling (try/catch)
// 		from our CodeBehind and SystemLibrary methods, it is 
//		impossible to eliminate them 100% as there will always
//		be some exceptions that cannot be predicted.  This method
//		will help us dig down to the most core Exception message,
//		instead of having to try to figure out what is happening
// 		from the higher level, more generic message.
public static Exception GetInnerMostException(Exception ex)
{
	// While the current exception has an InnerException
	while (ex.InnerException != null)
	{
		// Make the InnerException the current Exception
		ex = ex.InnerException;
	}

	// When we reach this point we have the InnerMostException.  Return
	//		it to the calling scope.
	return ex;
}

#endregion




