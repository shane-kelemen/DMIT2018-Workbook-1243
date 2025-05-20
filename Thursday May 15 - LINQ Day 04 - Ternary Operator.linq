<Query Kind="Statements">
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

// In-class Exercises for Ternary Operator
// This operator is also called the conditional operator, and the terms are used synonymously.
// In C#, the ternary operator must resolve into a value for the true and false conditions.  The 
//		value can then be assigned to a variable or used wherever the value type is legal to use.
// Programming statements are not permitted.

// Display a list of all employees, their department, and indicate if a review is required, meaning
//		their base rate is less than $30 per hour.
Employees
//.Where(employee => employee.BaseRate < 30)  	// Careful with the question wording to make sure you do not
												// 		filter by mistake when a different comparison is needed
.OrderBy(employee => employee.LastName)			// Note that ordering before creating the new type, whether
												//		anonymouys or not, is in this case better because the 
												// 		first and last name are being combined in the new type.
												//		If you order after the new type creation, you would need 
												//		to split each name, adding more processing time.
.Select(employee => new 
					{
						FullName = employee.FirstName + " " + employee.LastName,
						Department = employee.DepartmentName,
						// This use of the ternary operator is basically a type conversion from decimal to string
						IncomeCategory = employee.BaseRate < 30 ? "Required Review" : "No review Required"
					})
.Dump();


// Display a list of all products in the "Music, Movies and Audio Books" category.  If the product is neither
//		black nor white, indicate that further colour processing is required.
Products
.Where(product => product.ProductSubcategory.ProductCategory.ProductCategoryName == "Music, Movies and Audio Books")
.OrderBy(product => product.StyleName)	// Again, the ordering is required before the new type is created
										//		because the ordering column will no longer exist in the
										//		new type. 
.Select(product => new 
				{
					ProductName = product.ProductName,
					Colour = product.ColorName,
					ColourProcessNeeded = product.ColorName == "Black" || product.ColorName == "White" ?
												"No" : "Yes"
				})
.Dump();













