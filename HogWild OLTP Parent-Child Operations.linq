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
	if (codeBehind.Parts == null || codeBehind.Parts.Count == 0)
	{
		codeBehind.ErrorDetails.Dump("Should Fail - No parts found for the existing criteria");
	}
	else
	{
		codeBehind.Parts.Dump("No parts should be found, but we received:");
	}
	
	// Pass Tests
	// Rule: Provide a valid part category ID
	codeBehind.GetParts(23, string.Empty, existingPartIDs);
	codeBehind.Parts.Dump("Should Pass - Valid Part Categort ID");
	
	// Rule: Provide a valid partial description
	codeBehind.GetParts(0, "ra", existingPartIDs);
	codeBehind.Parts.Dump("Should Pass - Valid partial description provided");

	// Rule: No parts that are supplied as existing should be returned
	existingPartIDs.Add(27);
	existingPartIDs.Add(33);
	existingPartIDs.Add(45);
	existingPartIDs.Add(44);
	codeBehind.GetParts(0, "ra", existingPartIDs);
	codeBehind.Parts.Dump("Should Pass - Valid partial description provided with existing IDs");
	
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
	#endregion

	// These data members are meant to mimic what you will actually 
	//		have in your code behind file.
	#region Local Data Members
	// A list for storing the error messages returned from the System
	//		Library method, if any, and used by the BLazor component.
	private List<string> errorDetails = new List<string>();
	private string errorMessage = string.Empty;

	public List<PartView> Parts = new List<PartView>();
	public PartView part = new PartView();

	#endregion

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
				Parts = result.Value;
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
	
	public Result<List<PartView>> GetParts (int partCategoryID, string description,
												List<int> existingPartIDs)
	{
		var result = new Result<List<PartView>>();
		
		#region Data Validation and Business Rules
		
		if (partCategoryID <= 0 && string.IsNullOrWhiteSpace(description))
		{
			result.AddError(new Error("Missing Information!",
						"Please provide either a category ID and/or description!"));
			
			return result;
		}
		
		#endregion
		
		Guid tempGuid = new Guid();
		if (string.IsNullOrWhiteSpace(description))
		{
			description = tempGuid.ToString();
		}
		
		var parts = _hogWildContext.Parts
					.Where(p => !existingPartIDs.Contains(p.PartID)
									&& (description.Length > 0
									&& description != tempGuid.ToString()
									&& partCategoryID > 0
										? (p.Description.ToUpper().Contains(description.ToUpper())
											&& p.PartCategoryID == partCategoryID)
										: (p.Description.ToUpper().Contains(description.ToUpper())
											|| p.PartCategoryID == partCategoryID)
									&& !p.RemoveFromViewFlag))
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
						.OrderBy(p => p.Description)
						.ToList();
		
		if (parts == null || parts.Count == 0)
		{
			result.AddError(new Error("No Parts Found!", 
					"No parts were found that match the provided criteria"));
					
			return result;
		}
		
		return result.WithValue(parts);
	}
	
	public Result<PartView> GetPart(int partID)
	{
		var result = new Result<PartView>();
		
		#region Business Rules
		
		if (partID <= 0)
		{
			result.AddError(new Error("Missing Information!", 
									"Part ID must be greater than zero!")); 
			
			return result;
		}
		
		#endregion
		
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
					.FirstOrDefault();
		
		if (part == null)
		{
			result.AddError(new Error("No Part Found!", 
						"No part was found for the provided part ID"));
			
			return result;
		}
		
		return result.WithValue(part);
	}
}

#endregion


#region View Models

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










