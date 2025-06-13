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
  <Reference Relative="..\Downloads\BYSResults.dll">&lt;UserProfile&gt;\Downloads\BYSResults.dll</Reference>
</Query>

// Include the NuGet Package that will allow us to use the Result class
using BYSResults;

// Main will act as the replacement for your User Interface in the
//		Blazor Web Application.  The CodeBehind class will hold methods
//		that will become methods in the code behind page of your Blazor
//		components.
//
// Note that the way Main is structured allows us to test several
//		different "interactions" with the user that will allow us to
//		see the results of each permutation of inputs, legal and illegal,
//		for the method(s) we are testing.  Note that a new LINQPad
//		file will not be required for each method we are testing.
//
// Used the way described above, Main is considered to be the DRIVER
// 		part of this testing pattern.
void Main()
{
	// Create the instance of the class mimicing the CodeBehind of
	//		your Blazor component in the Web App.
	// Note: "this" is effectively passing in the database connection
	//			that is selected in the Connection dropdown above.
	CodeBehind codeBehind = new CodeBehind(this);
	
	// The following sections provide test data that will cause each
	//		of the possible paths through the database interaction to
	//		be explored.  This should include legal and illegal paths,
	//		and you should be able to predict what will happen so that
	//		you know whether your code is functioning as intended.
	
	// Both pieces of information missing:  Should result in an error
	//		message
	codeBehind.GetCustomers(string.Empty, string.Empty);
	codeBehind.ErrorDetails.Dump("Missing last name and phone number!");
	
	// Partial last name provided.  Should be a result set including
	//		only customers with a last name containing the provided
	//		string.
	codeBehind.GetCustomers("Smith", string.Empty);
	codeBehind.Customers.Dump("Testing last name only");

	// Partial phone number provided.  Should be a result set including
	//		only customers with a phone number containing the provided
	//		string.
	codeBehind.GetCustomers(string.Empty, "496");
	codeBehind.Customers.Dump("Testing phone only");

	// Both pieces of information provided.  Should be a result set 
	// 		including customers that contain either the provided last 
	//		name as part of the customer's last name, or the provided
	//		phone numbers as part of the customer's phone number.
	codeBehind.GetCustomers("Russell", "496");
	codeBehind.Customers.Dump("Testing both supplied");
}

// This class mimics the class you get when you add a codebehind page
//		to your Blazor component. Note the syntax that accepts the
//		TypedDataContext, which is basically the database connection.
//		This is shorthand syntax for creating a class data member and
//		initializing that data member to the item passed in as if there
//		was a matching constructor.
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
	private readonly Library YourService = new Library(context);
	#endregion
	
	// These data members are meant to mimic what you will actually 
	//		have in your code behind file.
	#region Local Data Members
	// A list for storing the error messages returned from the System
	//		Library method, if any, and used by the BLazor component.
	private List<string> errorDetails = new List<string>();
	
	// In the event that a result set is successfully retrieved, this
	//		List will store the results for use by the Blazor component.	
	public List<CustomerSearchView> Customers = 
							new List<CustomerSearchView>();
	#endregion
	
	// This method will be bound to a control in the Blazor component,
	//		and invoked when the user causes the appropriate action on
	//		the control.  For example, clicking a button.
	// The parameters here would become data members in the code behind
	//		class.
	public void GetCustomers(string lastName, string phone)
	{
		// Clear all code behind user messaging collections and 
		//		variables.
		errorDetails.Clear();
		
		// Though we are using the Result class for displaying results
		//		and for handling errors that we can predict, there is 
		//		still the possibility of system errors that cannot be 
		//		predicted.  Thus, to ensure all errors are gracefully 
		//		handled, we need a single try/catch to display the root 
		//		cause of the error.
		try
		{
			// Call the database interaction method in your Service 
			//		Library.  Result is a class object that contains a
			//		string collection for storing error messages, and
			//		a variable called Value that will store the
			//		List<CustomerServiceView> instances returned if the
			//		database action is able to retrieve records from
			//		the database.
			var result = YourService.GetCustomers(lastName, phone);
			
			// If the action was successful, then IsSuccess will be true
			if (result.IsSuccess)
			{
				// In that case, fill the collection in the code behind
				//		page for storing a List of CustomerServiceView
				//		instances with the items retrieved from the 
				//		database.  When the page is rebuilt for the user,
				//		the collection will be available to the Blazor
				//		component for extracting the data to the screen.
				Customers = result.Value;
			}
			else
			{
				// If the interaction was not successful for a predictable
				//		reason(s), then extract all of the Error messages
				//		to the string collection in the code behind page
				//		for display to the user.
				// Note: A support method has been created to eliminate
				//			repeated code for this purpose.
				errorDetails = GetErrorMessages(result.Errors.ToList());
			}
		}
		catch (Exception ex)
		{
			// If an exception is thrown due to unpredicted error,
			//		Extract the root cause of the error and display it 
			//		to the user.
			// Note: A support method has been created to eliminate 
			//		repeated code for this purpose.
			errorDetails.Add(GetInnerMostException(ex).Message);
		}
	}
	
}

#region Support Methods

// This method has been created to extract all messages received in
//		a Result Error list and add them as strings to a collection
//		to be located in the code behind page.
public static List<string> GetErrorMessages(List<Error> errorList)
{
	List<string> errorMessages = new List<string>();
	
	foreach (Error error in errorList)
	{
		errorMessages.Add(error.ToString());
	}
	
	return errorMessages;
}

// This method has been created to extract the root cause of a received
// 		exception. 
public static Exception GetInnerMostException(Exception ex)
{
	// While the current exception has an InnerException
	while(ex.InnerException != null)
	{
		// Make the InnerException the current Exception
		ex = ex.InnerException;
	}
	
	// When we reach this point we have the InnerMostException.  Return
	//		it to the calling scope.
	return ex;
}
#endregion


// This class has been created to mimic a Service class that you would 
//		place in the BLL folder of your System Library.
public class Library
{
	// We must store the "context", meaning database connection, for 
	//		your methods in this class to use to perform transactional
	//		operations against your database.  The CRUD operations...
	private readonly TypedDataContext _templateContext;
	
	// This constructor is required because we want to check for a 
	//		valid context as part of the successful creation of the 
	//		class instance.  The short-hand version used in the 
	//		CodeBehind class does not allow this check to be performed.
	public Library(TypedDataContext context)
	{
		// If there is a valid context instance accepted, assign it to
		//		the ReadOnly class variable.
		_templateContext = context 
				// If the passed in context is null, throw an 
				//		Exception.  You could also have accomplished
				//		this task with an if statement checking for
				//		a null context at the beginning of the 
				//		constructor.
					?? throw new ArgumentException(nameof(context));
	}
	
	
	// This method will retrieve a List of CustomerSearchView instances
	// 		if there are matches to the passed in data in the database.
	public Result<List<CustomerSearchView>> GetCustomers(string lastName,
															string phone)
	{
		// Create the result instance.
		var result = new Result<List<CustomerSearchView>>();
		
		// Data validation is ensuring that data is present.
		// Business rules enforcement is making sure that the received
		//		data is being used in a valid way.
		// Example:  If playing chess, data validation is checking that
		//				in-range start and end squares have been received.
		//				Business rule enforcement would check that the 
		//				requested move for a particular game piece is
		//				valid within the rules of the game.  Rooks
		//				cannot move diagonally for instance.
		#region Data Validation and Business Rules
		// It is not allowed for both the last name and the phone to 
		//		be missing.  This check plays the roles of both data
		// 		validation and business rule application.  Sometimes
		//		we get lucky like this, but sometimes the checks are
		//		separate.
		if (string.IsNullOrWhiteSpace(lastName)
					&& string.IsNullOrWhiteSpace(phone))
		{
			// Add an error message to the Error collection in the Result
			//		instance if the validation / rule is violated.
			result.AddError(new Error("Must provide a last name and/or phone!"));
		
			// Seeing as we have only one possible error for this method,
			//		if it happens you may return immediately.  If there 
			//		are multiple checks to perform, only add the error,
			//		and once all validations and rules have been processed,
			//		check if there are any items in the Error collection
			//		and return if an Error message has been added.
			return result;
		}
		#endregion
		
		// Once past the validation and rules enforcement stage, 
		//		prepare the data you have for interaction with the 
		//		database.  In this case, the next two blocks will
		//		assign a randomly generated very long string to 
		//		any of the input parameters that were missing.
		//		This will ensure no matches in the database for that
		//		parameter.
		if (string.IsNullOrWhiteSpace(lastName))
		{
			lastName = Guid.NewGuid().ToString();
		}

		if (string.IsNullOrWhiteSpace(phone))
		{
			phone = Guid.NewGuid().ToString();
		}
		
		// Now that the data has been prepared, create the retrieval 
		//		operation for customers matching the input values.
		//		Include customers that have a partial match to either
		//		the customer's last name, phone number, or both, but
		//		only include customers that have not been
		//		RemovedFromView, meaning only active customers.
		var customers = _templateContext
						.Customers
						.Where(customer => (customer.LastName
											.Contains(lastName)
										|| customer.Phone
											.Contains(phone))
										&& !customer.RemoveFromViewFlag)
						.Select(customer => new CustomerSearchView
						{
							CustomerID = customer.CustomerID,
							FirstName = customer.FirstName,
							LastName = customer.LastName,
							City = customer.City,
							ContactPhone = customer.Phone,
							EmailAddress = customer.Email
						})
						.OrderBy(anon => anon.LastName)
						.ToList();
		
		// If no customers have been found, add a message to display
		//		to the user.  Arguably this could have been checked
		//		in the code behind page as well.
		if (customers == null)
		{
			result.AddError(new Error("No matching customers"));
		}
		
		// The operation has been successful, meaning no exceptions
		// 		thrown, so return the Result object, and put the 
		//		customers into the object as part of the operation.
		return result.WithValue(customers);
	}
}




#region View Models

// ViewModels are how information will always move between the System
//		Library and the Blazor web app.  Only include the fields you
//		need, not everything.
public class CustomerSearchView
{
	public int CustomerID { get; set; }
	public string LastName { get; set; }
	public string FirstName { get; set; }
	public string City { get; set; }
	public string ContactPhone { get; set; }
	public string EmailAddress { get; set; }
}

#endregion

