<Query Kind="Program">
  <Connection>
    <ID>8fd53990-5b4f-4077-8c7e-322f9d5171ae</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>Contoso</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

// In-Class Exercises for creation of Strong Types
// The following is the beginning of the pattern that we will use when we get back to working with Blazor Web
//		Applications:
//			1) Main will be used to immitate the User Interface.  Normally you would have a button or some
//					other control that would be interacted with by the user, in turn causing a method in 
//					the C# code for the page to be executed.
//			2) C# Code Methods will be used to do a bit of local data validation, and assuming all data validation 
//					has passed, a call to the System Library methods in the BLL classes will occur.  Results
//					of the call to the BLL methods will also be handled in these C# Code Methods.
//			3) C# System Library methods will be responsible for server side data validation and application of 
//					business rules.  Assuming validation and rules are satisfied, and database interaction may
//					proceed from withing the System Library Methods.
//			4) ViewModels will be created to move data between the System Library and the Blazor Web Application.
//					It is not necessary for all fields from a particular Entity to be sent to the client.  Use of 
//					ViewModels will allow us to move only the data that is required in either direction.

void Main()
{
	//	Test your BLazor C# Code method by calling it from here in Main, simulating interaction with 
	//		a Blazor Web Control.
	CodeBehind_GetEmployeesByLastNameAndBaseRate("al", 30).Dump();
}

// For this first introduction to the pattern, we are not going to introduce a bunch of error handling,  
//		as the focus in this unit is LINQ.  We will flesh out the error handling portion later.
public List<EmployeeView> CodeBehind_GetEmployeesByLastNameAndBaseRate (string partialLastName, decimal baseRate)
{
	//	UI Variable Control and Collection Re-initialization 
	
	// 	Local Data Validation
	
	//	Calling the System Library method and dealing with its results.
	return System_GetEmployeesByLastNameAndBaseRate (partialLastName, baseRate);
}

//	The System Library methods will reside in your classes in the BLL folder, similar to CPSC1517.  The main
// 		difference will be in that ViewModels will often be accepted as the data input.  Never will an entity
//		be accepted or returned to the Blazor Web Application layer.  Again, data validation and business rule
//		enforcement are being left for a later unit.
public List<EmployeeView> System_GetEmployeesByLastNameAndBaseRate (string partialLastName, decimal baseRate)
{
	//	Data Validation
	//	Business Rule Enforcement
	
	//	Database Interaction - This could potentially have been a single return statement as in the C# Code
	//								method above.
	List<EmployeeView> employees = Employees
									//	Limit the results to employees matching the partial last name provided
									.Where(employee => employee.LastName.Contains(partialLastName))
									.OrderBy(employee => employee.LastName)
									.Select(employee => new EmployeeView 
													{
														FullName = employee.FirstName + " " + employee.LastName,
														Department = employee.DepartmentName,
														//	Indicate a review needed if rate is less than the 
														//		provided base rate.
														IncomeCategory = employee.BaseRate < baseRate ?
																			"Required Review" : "No Review Required"
													})
									.ToList();
		
	return employees;
}

//	Get used to seeing ViewModels like this.  In DMIT2018, all data will be moved between the System Library
//		and the Blazor Web Application using ViewModels.  The context in your DAL and all classes in the Entities 
//		folder will be marked with the internal access specifier so that they are only usable inside the 
// 		System Library.
public class EmployeeView
{
	public string FullName { get; set; }
	public string Department { get; set; }
	public string IncomeCategory { get; set; }
}