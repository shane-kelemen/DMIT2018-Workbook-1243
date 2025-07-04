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
	CodeBehind codeBehind = new CodeBehind(this);
	
	#region GetParts
	
	List<int> existingPartIDs = new List<int>();
	
	// Fail Tests
	// Rule: Part category ID and description cannot both be null / empty
	codeBehind.GetParts(0, string.Empty, existingPartIDs);
	codeBehind.ErrorDetails.Dump("Should Fail - Category ID or Description must be provided!");

	// Rule: No parts are found for the search criteria
	codeBehind.GetParts(0, "zzz", existingPartIDs);
	if (codeBehind.parts == null || codeBehind.parts.Count == 0)
	{
		codeBehind.ErrorDetails.Dump("Should Fail - No parts found for the existing criteria");
	}
	else
	{
		codeBehind.parts.Dump("No parts should be found, but we received:");
	}
	
	// Pass Tests
	// Rule: Provide a valid part category ID
	codeBehind.GetParts(23, string.Empty, existingPartIDs);
	codeBehind.parts.Dump("Should Pass - Valid Part Category ID");
	
	// Rule: Provide a valid partial description
	codeBehind.GetParts(0, "ra", existingPartIDs);
	codeBehind.parts.Dump("Should Pass - Valid partial description provided");

	// Rule: No parts that are supplied as existing should be returned
	existingPartIDs.Add(27);
	existingPartIDs.Add(33);
	existingPartIDs.Add(45);
	existingPartIDs.Add(44);
	codeBehind.GetParts(0, "ra", existingPartIDs);
	codeBehind.parts.Dump("Should Pass - Valid partial description provided with existing IDs");
	
	#endregion
	
	#region GetPart
	
	// Fail Tests
	// Rule: Provide an invalid Part ID
	codeBehind.GetPart(0);
	codeBehind.ErrorDetails.Dump("Should Fail - Invalid Part ID");

	// Rule: Provide a Part ID that does not exist
	codeBehind.GetPart(100000000);
	codeBehind.ErrorDetails.Dump("Should Fail - Part ID does not exist so no results");

	// Rule: Provide an invalid Part ID
	codeBehind.GetPart(52);
	codeBehind.part.Dump("Should Pass - Part does exist for provided part ID");
	#endregion
	
	#region GetInvoice for Invoice, Customer, and Employee IDs
	
	// Fail tests
	// Rule: Customer IDs must be greater than zero
	codeBehind.GetInvoice(0, 0, 1);
	codeBehind.ErrorDetails.Dump("Should Fail - Customer ID must be greater than zero");

	// Rule: Employee IDs must be greater than zero
	codeBehind.GetInvoice(0, 1, 0);
	codeBehind.ErrorDetails.Dump("Should Fail - Employee ID must be greater than zero");

	// Pass test
	// New Invoice
	codeBehind.GetInvoice(0, 1, 1);
	codeBehind.invoice.Dump("Should Pass - New Invoice");

	// Existing Invoice
	codeBehind.GetInvoice(1, 1, 1);
	codeBehind.invoice.Dump("Should Pass - Existing Invoice");

	#endregion

	#region GetInvoice for Invoice ID Only

	// Fail tests
	// Rule: Customer IDs must be greater than zero
	codeBehind.GetInvoice(0);
	codeBehind.ErrorDetails.Dump("Should Fail - Invoice ID must be greater than zero");

	// Rule: Employee IDs must be greater than zero
	codeBehind.GetInvoice(1000000);
	codeBehind.ErrorDetails.Dump("Should Fail - No invoices found for provided invoice ID");

	// Pass test
	// Invoice found
	codeBehind.GetInvoice(1);
	codeBehind.invoice.Dump("Should Pass - Invoice found");

	#endregion

	#region GetInvoices
	
	// Fail Tests
	// Rule: Customer ID is invalid
	codeBehind.GetCustomerInvoices(0);
	codeBehind.ErrorDetails.Dump("Should Fail - Customer ID provided is invalid");

	// Rule: Customer ID has no invoices
	codeBehind.GetCustomerInvoices(1000000);
	codeBehind.ErrorDetails.Dump("Should Fail - Customer ID provided has no invoices");

	// Pass test
	// Invoices found for customer ID
	codeBehind.GetCustomerInvoices(1);
	codeBehind.invoices.Dump("Should Pass - Invoices found for customer ID");

	#endregion
}

#region Code Behind Classes

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
	private readonly PartService PartService
									= new PartService(context);

	private readonly InvoiceService InvoiceService
									= new InvoiceService(context);
	#endregion

	// These data members are meant to mimic what you will actually 
	//		have in your code behind file.
	#region Local Data Members
	// A list for storing the error messages returned from the System
	//		Library method, if any, and used by the BLazor component.
	private List<string> errorDetails = new List<string>();
	private string errorMessage = string.Empty;

	public List<PartView> parts = new List<PartView>();
	public PartView part = new PartView();
	public InvoiceView invoice = new InvoiceView();
	public List<InvoiceView> invoices = new List<InvoiceView>();

	#endregion

	// This method will call the system method intended to retrieve a list of parts that
	//		are either in the provided category, partially match the provided description,
	//		or both, all while not being included in the list of provided existing part IDs.
	public void GetParts(int partCategoryID, string description, List<int> existingPartIDs)
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
			var result = PartService.GetParts(partCategoryID, description, existingPartIDs);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if (result.IsSuccess)
			{
				parts = result.Value;
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

	// This method will call the system method intended to retrieve a part that matches the 
	//		provided part ID.
	public void GetPart(int partID)
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
			var result = PartService.GetPart(partID);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if (result.IsSuccess)
			{
				part = result.Value;
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
	
	// This method will call the system method intended to retrieve an invoice that matches the 
	//		provided invoice, customer, and employee IDs.
	public void GetInvoice(int invoiceID, int customerID, int employeeID)
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
			var result = InvoiceService.GetInvoice(invoiceID, customerID, employeeID);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if (result.IsSuccess)
			{
				invoice = result.Value;
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

	// This method will call the system method intended to retrieve an invoice that matches the 
	//		provided invoice ID.
	public void GetInvoice(int invoiceID)
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
			var result = InvoiceService.GetInvoice(invoiceID);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if (result.IsSuccess)
			{
				invoice = result.Value;
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
	
	// This method will call the system method intended to retrieve all invoices that
	//		are linked to a provided customer ID.
	public void GetCustomerInvoices(int customerID)
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
			var result = InvoiceService.GetCustomerInvoices(customerID);

			// If the operation was successful, bind the returned information
			//		to the CodeBehind variable being read by the UI.
			if (result.IsSuccess)
			{
				invoices = result.Value;
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
}

#endregion

#region System Library Classes

public class PartService
{
	#region Connection Setup
	// We must store the "context", meaning database connection, for 
	//		your methods in this class to use to perform transactional
	//		operations against your database.  The CRUD operations...
	private readonly TypedDataContext _hogWildContext;

	// This constructor is required because we want to check for a 
	//		valid context as part of the successful creation of the 
	//		class instance.  The short-hand version used in the 
	//		CodeBehind class does not allow this check to be performed.
	public PartService(TypedDataContext context)
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
	
	#endregion
	
	// This system method will retrieve a list of parts that
	//		are either in the provided category, partially match the provided description,
	//		or both, all while not being included in the list of provided existing part IDs.
	public Result<List<PartView>> GetParts (int partCategoryID, string description,
												List<int> existingPartIDs)
	{
		// Set up the object used for returning data to the calling scope.
		var result = new Result<List<PartView>>();
		
		// In this case only a single test is required for the incoming data, so the 
		//		data validation and business rules may be combined.
		#region Data Validation and Business Rules
		
		// If both the provided part category ID and the provided description are invalid,
		//		immediately return with the following error message for the user.
		if (partCategoryID <= 0 && string.IsNullOrWhiteSpace(description))
		{
			result.AddError(new Error("Missing Information!",
						"Please provide either a category ID and/or description!"));
			
			return result;
		}
		
		#endregion
		
		// Making it to this point means we have a valid piece of information to use 
		//		as a filter for the part list.
		// If the description passed in is not useable information, replace it with
		//		a piece of information guaranteed not to have a match in the database.
		Guid tempGuid = new Guid();
		if (string.IsNullOrWhiteSpace(description))
		{
			description = tempGuid.ToString();
		}
		
		// Retrieve the parts matching the provided information
		var parts = _hogWildContext.Parts
								// First make sure the part is not in the exclusion list
					.Where(p => !existingPartIDs.Contains(p.PartID) 
										// If we have a proper partial description
									&& (description.Length > 0
										// That is not equal to the guaranteed "no match" from above
									&& description != tempGuid.ToString()
										// If the part category is valid
									&& partCategoryID > 0
										// Include all parts matching the provided information
										? (p.Description.ToUpper().Contains(description.ToUpper())
											&& p.PartCategoryID == partCategoryID)
										: (p.Description.ToUpper().Contains(description.ToUpper())
											|| p.PartCategoryID == partCategoryID)
									// Don't forget to exclude those parts that have been discontinued
									&& !p.RemoveFromViewFlag)) 
					// Polulate the View Model instances for information transport
					.Select(p => new PartView 
						{
							PartID = p.PartID,
							PartCategoryID = p.PartCategoryID,
							// You may need to use the navigation properties to get some information
							CategoryName = p.PartCategory.Name,
							Description = p.Description,
							Cost = p.Cost,
							Price = p.Price,
							ROL = p.ROL,
							QOH = p.QOH,
							Taxable = (bool)p.Taxable,
							RemoveFromViewFlag = p.RemoveFromViewFlag
						})
						.OrderBy(p => p.Description)
						.ToList();
		
		// If no parts are found for the search criteria, send an error message back to the
		//		calling scope.
		if (parts == null || parts.Count == 0)
		{
			result.AddError(new Error("No Parts Found!", 
					"No parts were found that match the provided criteria"));
					
			return result;
		}
		
		// return the results retrieved to the calling scope if no errors encountered
		return result.WithValue(parts);
	}

	// This system method intended to retrieve a part that matches the 
	//		provided part ID.
	public Result<PartView> GetPart(int partID)
	{
		// Set up the object used for returning data to the calling scope.
		var result = new Result<PartView>();

		// In this case only a single test is required for the incoming data, so the 
		//		data validation and business rules may be combined.
		#region Business Rules
		
		// If the provided part ID is invalid
		//		immediately return with the following error message for the user.
		if (partID <= 0)
		{
			result.AddError(new Error("Missing Information!", 
									"Part ID must be greater than zero!")); 
			
			return result;
		}
		
		#endregion
		
		// Retrieve the information requested for the provided part ID
		var part = _hogWildContext.Parts
					.Where(p => p.PartID == partID && !p.RemoveFromViewFlag)
					.Select(p => new PartView
					{
						PartID = p.PartID,
						PartCategoryID = p.PartCategoryID,
						CategoryName = p.PartCategory.Name,
						Description = p.Description,
						Cost = p.Cost,
						Price = p.Price,
						ROL = p.ROL,
						QOH = p.QOH,
						Taxable = (bool)p.Taxable,
						RemoveFromViewFlag = p.RemoveFromViewFlag
					})
					.FirstOrDefault();  // Use FirstOrDefault() instead of ToList() when
										//		retrieve a single item.
		
		// If no part found, populate and return an appropriate error message to the
		//		calling scope.
		if (part == null)
		{
			result.AddError(new Error("No Part Found!", 
						"No part was found for the provided part ID"));
			
			return result;
		}
		
		// return the results retrieved to the calling scope if no errors encountered
		return result.WithValue(part);
	}	
}

public class InvoiceService
{
	#region Connection Setup
	// We must store the "context", meaning database connection, for 
	//		your methods in this class to use to perform transactional
	//		operations against your database.  The CRUD operations...
	private readonly TypedDataContext _hogWildContext;

	// This constructor is required because we want to check for a 
	//		valid context as part of the successful creation of the 
	//		class instance.  The short-hand version used in the 
	//		CodeBehind class does not allow this check to be performed.
	public InvoiceService(TypedDataContext context)
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

	#endregion
	
	// This method will be used to handle both existing invoices and new invoices.
	public Result<InvoiceView> GetInvoice(int invoiceID, int customerID, int employeeID)
	{
		var result = new Result<InvoiceView>();
		
		// Just a couple checks here so as in previous methods they can all be combined under 
		//		one section.
		#region Data Validation and Business Rules
		
		// Rule:  Both customer ID and emplyee ID must be provided
		
		if (customerID == 0)
		{
			result.AddError(new Error("Missing Information!",
							"A customer ID must be provided!"));
		}

		if (employeeID == 0)
		{
			result.AddError(new Error("Missing Information!",
							"An employee ID must be provided!"));
		}
		
		if (result.IsFailure)
		{
			return result;
		}
		#endregion
		
		InvoiceView invoice;
		
		// If creating a new invoice
		if (invoiceID <= 0)
		{
			// Use initializer syntax for creating a new class instance of InvoiceView
			invoice = new InvoiceView()
			{
				CustomerID = customerID,
				EmployeeID = employeeID,
				InvoiceDate = DateOnly.FromDateTime(DateTime.Now)
			};
		}
		else // If retrieving information for a current invoice
		{
			invoice = _hogWildContext.Invoices
						.Where(x => x.InvoiceID == invoiceID && !x.RemoveFromViewFlag)
						.Select(x => new InvoiceView 
							{
								InvoiceID = x.InvoiceID,
								InvoiceDate = x.InvoiceDate,
								CustomerID = x.CustomerID,
								EmployeeID = x.EmployeeID,
								Subtotal = x.SubTotal,
								Tax = x.Tax,
								InvoiceLines = _hogWildContext.InvoiceLines
												.Where(line => line.InvoiceID == invoiceID)
												.Select(line => new InvoiceLineView 
													{
														InvoiceLineID = line.InvoiceLineID,
														InvoiceID = line.InvoiceID,
														PartID = line.PartID,
														Quantity = line.Quantity,
														Description = line.Part.Description,
														Price = line.Price,
														Taxable = line.Part.Taxable,
														RemoveFromViewFlag = line.RemoveFromViewFlag														
													})
												.ToList()
							})
						.FirstOrDefault();
						
						customerID = invoice.CustomerID;
		}
		
		// If there was no invoice for a provided invoice ID.  No point in continuing,
		//		so return an error message to the calling scope.
		if (invoice == null)
		{
			result.AddError(new Error("No Invoice Found!",
					"There was no invoice found for the provided invoice ID!"));
			
			return result;
		}
			
		// Whether a new or existing invoice, retrieve the full names for the customer and employee
		//		using their respective services.
		CustomerService cust = new CustomerService(_hogWildContext);
		invoice.CustomerName = cust.GetCustomerFullName(invoice.CustomerID);
		EmployeeService emp = new EmployeeService(_hogWildContext);
		invoice.EmployeeName = emp.GetEmployeeFullName(invoice.EmployeeID);
		
		// Getting to this point means we have a valid invoice
		return result.WithValue(invoice);
	}
	
	// This method will be used to retrieve a single existing invoice independent of the
	//		customer and employee IDs
	public Result<InvoiceView> GetInvoice(int invoiceID)
	{
		var result = new Result<InvoiceView>();

		// Just a couple checks here so as in previous methods they can all be combined under 
		//		one section.
		#region Data Validation and Business Rules

		// Rule:  Invoice ID must be provided

		if (invoiceID <= 0)
		{
			result.AddError(new Error("Missing Information!",
							"An invoice ID must be provided!"));
			
			return result;
		}

		
		#endregion

			
		var	invoice = _hogWildContext.Invoices
						.Where(x => x.InvoiceID == invoiceID && !x.RemoveFromViewFlag)
						.Select(x => new InvoiceView
						{
							InvoiceID = x.InvoiceID,
							InvoiceDate = x.InvoiceDate,
							CustomerID = x.CustomerID,
							EmployeeID = x.EmployeeID,
							Subtotal = x.SubTotal,
							Tax = x.Tax,
							InvoiceLines = _hogWildContext.InvoiceLines
												.Where(line => line.InvoiceID == invoiceID)
												.Select(line => new InvoiceLineView
												{
													InvoiceLineID = line.InvoiceLineID,
													InvoiceID = line.InvoiceID,
													PartID = line.PartID,
													Quantity = line.Quantity,
													Description = line.Part.Description,
													Price = line.Price,
													Taxable = line.Part.Taxable,
													RemoveFromViewFlag = line.RemoveFromViewFlag
												})
												.ToList()
						})
						.FirstOrDefault();
		

		// If there was no invoice for a provided invoice ID.  No point in continuing,
		//		so return an error message to the calling scope.
		if (invoice == null)
		{
			result.AddError(new Error("No Invoice Found!",
					"There was no invoice found for the provided invoice ID!"));

			return result;
		}

		// Whether a new or existing invoice, retrieve the full names for the customer and employee
		//		using their respective services.
		CustomerService cust = new CustomerService(_hogWildContext);
		invoice.CustomerName = cust.GetCustomerFullName(invoice.CustomerID);
		EmployeeService emp = new EmployeeService(_hogWildContext);
		invoice.EmployeeName = emp.GetEmployeeFullName(invoice.EmployeeID);

		// Getting to this point means we have a valid invoice
		return result.WithValue(invoice);
	}

	// This method will retrieve all existing invoices fro a provided customer ID
	public Result<List<InvoiceView>> GetCustomerInvoices(int customerID)
	{
		var result = new Result<List<InvoiceView>>();

		// In this case only a single test is required for the incoming data, so the 
		//		data validation and business rules may be combined.
		#region Data Validation and Business Rules

		// If the provided customer ID is invalid,
		//		immediately return with the following error message for the user.
		if (customerID <= 0)
		{
			result.AddError(new Error("Missing Information!",
						"Please provide a valid customer ID!"));

			return result;
		}

		#endregion

		var customerInvoices = _hogWildContext.Invoices
								.Where(x => x.CustomerID == customerID && !x.RemoveFromViewFlag)
								.Select(x => new InvoiceView 
									{
										InvoiceID = x.InvoiceID,
										InvoiceDate = x.InvoiceDate,
										CustomerID = x.CustomerID,
										Subtotal = x.SubTotal,
										Tax = x.Tax
									})
								.ToList();
		
		// If no invoices were found.  Send back an error message to the calling scope.
		if (customerInvoices == null || customerInvoices.Count == 0)
		{
			result.AddError(new Error("No Invoice Found!", 
								"No invoices found for the provided customer ID!"));
								
			return result;
		}
		
		// return the retrieved invoices
		return result.WithValue(customerInvoices);
	}
	
	// This method will save invoice details to the database for either new or existing
	//		invoices, including the associated invoice line details
	public Result<InvoiceView> AddEditInvoice(InvoiceView invoiceView)
	{
		var result = new Result<InvoiceView>();
		
		#region Data Validation
		
		if (invoiceView == null)
		{
			result.AddError(new Error("Missing Invoice!", "No invoice information was provided!"));
			
			return result;
		}
		
		if (invoiceView.CustomerID <= 0)
		{
			result.AddError(new Error("Missing information!", "Invalid customer ID provided!"));
		}

		if (invoiceView.EmployeeID <= 0)
		{
			result.AddError(new Error("Missing information!", "Invalid employee ID provided!"));
		}

		if (invoiceView.InvoiceLines.Count == 0)
		{
			result.AddError(new Error("Missing information!", "Invoice line details are required!"));
		}
		
		foreach (var line in invoiceView.InvoiceLines)
		{
			if (line.PartID <= 0)
			{
				result.AddError(new Error("Missing information!",
						$"No part ID provided for invoice line {line.InvoiceLineID}"));
			}
			
			if (line.Price < 0)
			{
				string partName = _hogWildContext.Parts
									.Where(x => x.PartID == line.PartID)
									.Select(x => x.Description)
									.FirstOrDefault();

				result.AddError(new Error("Invalid Price!",
						$"Price provided for {partName} is less than zero!"));
			}
			
			if (line.Quantity < 1)
			{
				string partName = _hogWildContext.Parts
									.Where(x => x.PartID == line.PartID)
									.Select(x => x.Description)
									.FirstOrDefault();

				result.AddError(new Error("Invalid Quantity!",
						$"Quantity provided for {partName} is less than one!"));
			}
			
			List<string> duplicatedParts = invoiceView.InvoiceLines
											.GroupBy(x => new { x.PartID })
											.Where(group => group.Count() > 1)
											.OrderBy(group => group.Key.PartID)
											.Select(group => _hogWildContext.Parts
															.Where(p => p.PartID == group.Key.PartID)
															.Select(p => p.Description)
															.FirstOrDefault()
													)
											.ToList();
											
			if (duplicatedParts.Count > 0)
			{
				foreach(var partName in duplicatedParts)
				{
					result.AddError(new Error("Duplicate Part!",
						$"{partName} can only be added to invoice lines once!"));
				}
			}
		}
		
		// If any errors, exit
		if (result.IsFailure)
		{
			return result;
		}

		#endregion
		
		Invoice invoice = _hogWildContext.Invoices
							.Where(x => x.InvoiceID == invoiceView.InvoiceID)
							.FirstOrDefault();
		
		if (invoice == null)
		{
			invoice = new Invoice();
			invoice.InvoiceDate = DateOnly.FromDateTime(DateTime.Now);
		}
		
		invoice.CustomerID = invoiceView.CustomerID;
		invoice.EmployeeID = invoiceView.EmployeeID;
		invoice.RemoveFromViewFlag = invoiceView.RemoveFromViewFlag;
		invoice.SubTotal = 0;
		invoice.Tax = 0;
		
		foreach (var line in invoiceView.InvoiceLines)
		{
			InvoiceLine invoiceLine = _hogWildContext.InvoiceLines
										.Where(x => x.InvoiceLineID == line.InvoiceLineID
													&& !x.RemoveFromViewFlag)
										.FirstOrDefault();
			
			if (invoiceLine == null)
			{
				invoiceLine = new InvoiceLine();
				invoiceLine.PartID = line.PartID;
			}
			
			invoiceLine.Quantity = line.Quantity;
			invoiceLine.Price = line.Price;
			invoiceLine.RemoveFromViewFlag = line.RemoveFromViewFlag;
			
			if (invoiceLine.InvoiceLineID <= 0)
			{
				invoice.InvoiceLines.Add(invoiceLine);
			}
			else
			{
				_hogWildContext.InvoiceLines.Update(invoiceLine);
			}
			
			if (!invoiceLine.RemoveFromViewFlag)
			{
				invoice.SubTotal += invoiceLine.Quantity * invoiceLine.Price;
				bool isTaxable = _hogWildContext.Parts
									.Where(x => x.PartID == invoiceLine.PartID)
									.Select(x => x.Taxable)
									.FirstOrDefault();
				invoice.Tax += isTaxable ? invoiceLine.Quantity * invoiceLine.Price * 0.05m : 0;
			}
		}
		
		if (invoice.InvoiceID <= 0)
		{
			_hogWildContext.Invoices.Add(invoice);
		}
		else
		{
			_hogWildContext.Invoices.Update(invoice);
		}
		
		try
		{
			_hogWildContext.SaveChanges();
		}
		catch (Exception ex)
		{
			_hogWildContext.ChangeTracker.Clear();
			
			result.AddError(new Error("Error Saving Changes!", GetInnerMostException(ex).Message));
			
			return result;
		}
		
		return GetInvoice(invoice.InvoiceID);
	}
}

public class CustomerService
{
	#region Connection Setup
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

	#endregion
	
	public string GetCustomerFullName(int customerID)
	{
		return _hogWildContext.Customers
							.Where(x => x.CustomerID == customerID
									&& !x.RemoveFromViewFlag)
							.Select(x => $"{x.FirstName} {x.LastName}")
							. FirstOrDefault() 
									?? string.Empty;
	}
}

public class EmployeeService
{
	#region Connection Setup
	// We must store the "context", meaning database connection, for 
	//		your methods in this class to use to perform transactional
	//		operations against your database.  The CRUD operations...
	private readonly TypedDataContext _hogWildContext;

	// This constructor is required because we want to check for a 
	//		valid context as part of the successful creation of the 
	//		class instance.  The short-hand version used in the 
	//		CodeBehind class does not allow this check to be performed.
	public EmployeeService(TypedDataContext context)
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

	#endregion

	public string GetEmployeeFullName(int employeeID)
	{
		return _hogWildContext.Employees
							.Where(x => x.EmployeeID == employeeID
									&& !x.RemoveFromViewFlag)
							.Select(x => $"{x.FirstName} {x.LastName}")
							.FirstOrDefault()
									?? string.Empty;
	}
}

#endregion


#region View Models

// Remember that you may reuse ViewModels for more than one system method
public class PartView
{
	public int PartID { get; set; }
	public int PartCategoryID { get; set; }
	public string CategoryName { get; set; }
	public string Description { get; set; }
	public decimal Cost { get; set; }
	public decimal Price { get; set; }
	public int ROL { get; set; }
	public int QOH { get; set; }
	public bool Taxable { get; set; }
	public bool RemoveFromViewFlag { get; set; }
}

public class InvoiceView
{
	public int InvoiceID { get; set; }
	public DateOnly InvoiceDate { get; set; }
	public int CustomerID { get; set; }
	public string CustomerName { get; set; }
	public int EmployeeID { get; set; }
	public string EmployeeName { get; set; }
	public decimal Subtotal { get; set; }
	public decimal Tax { get; set; }
	public decimal Total => Subtotal + Tax;
	public List<InvoiceLineView> InvoiceLines { get; set; } = new List<InvoiceLineView>();
	public bool RemoveFromViewFlag { get; set; }
}

public class InvoiceLineView
{
	public int InvoiceLineID { get; set; }
	public int InvoiceID { get; set; }
	public int PartID { get; set; }
	public string Description { get; set; }
	public int Quantity { get; set; }
	public decimal Price { get; set; }
	public bool Taxable { get; set; }
	public decimal ExtentPrice => Price * Quantity;
	public bool RemoveFromViewFlag { get; set; }
}



#endregion


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










