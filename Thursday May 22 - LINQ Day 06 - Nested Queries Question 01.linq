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

// This is the first in-class question dedicated to nested queries.
// Note that though these questions have asked for strongly typed subsets,
//		it is not actually necessary.  You can choose to build the entire
//		LINQ solution using anonymous types first and then introduct the
//		strong types and the template framework that we have been working 
//		with afterwards.
//
// Steps for starting with query first:
//		1) Open a new LINQ tab, choose the database, and set to C# Statement mode
//		2) Write the LINQ query needed using all anonymous types.  
//			a) ToList() is not required when using anonymous types, though
//				using them will make conversion faster.
//			b) It is suggested that you use explicit headers.  This will make
//				conversion faster.
//			c) You can even declare variables before your LINQ query to test
//				using input values instead of literal values (strings / numbers).
//			d) Don't forget your Dump() to see the results.
//		3) Once you have verified the returned data is correct, you should
//			make your ViewModels next.  Make sure the headers from above
//			match the automatic properties in the ViewModels.
//			a) You can still do this in C# Statement mode.  Generally the
//				ViewModels are placed below the LINQ queries.
//			b) Once you start using the strong types, if you only use one for
//				the nested query, you will not need to use ToList(), but as
//				soon as you also use a strong type for the parent part of the
//				query, you are likely including a collection (usually List)
//				to contain multiple results for the nested query, and so
//				use of ToList() will be required.
//		4) Now it is time to add main, and the system and code behind methods.
//			a) Make sure to check where the scoping braces are.  LINQPad usually 
//				puts everything in the window into the Main() method scope.
//			b) Build your system method around your LINQ query, and make sure
//				to use the input values in your LINQ method, and return either
//				a single instance or a collection of the required strong type.
//			c) Build your code behind method, making sure to call your system
//				method, passing in the parameters received from Main(), and
//				remember to return the same type to Main() as being returned
//				from the system method.
//			d) Make sure the only thing in Main is a call to your code behind 
//				method, and do not forget to use Dump() to display the results
//				returned from the code behind method.

// Here is a new comment so that the file will be seen as different by GitHub!

void Main()
{
	// This question is very basic as there is no data to pass in to the
	//		code behind method.
	CodeBehind_GetProductCategories().Dump();	// Remember to Dump()!
}

public List<ProductCategorySummaryView> CodeBehind_GetProductCategories()
{
	// Again, this is very straight-forward as there is no data to pass
	//		to the system method.  Use the return to pass the retrieved
	//		data from the system method straight back to Main().
	return System_GetProductCategories();
}

public List<ProductCategorySummaryView> System_GetProductCategories()
{
	// Here we can see the LINQ query retrieving the desired data.
	return ProductCategories
			.OrderBy(category => category.ProductCategoryName)
			.Select(category => new ProductCategorySummaryView 
				{
					ProductCategoryName = category.ProductCategoryName,
					Subcategories = category
									.ProductSubcategories
									.OrderBy(sub => sub.ProductSubcategoryName)
									.Select(sub => new ProductSubcategorySummaryView 
										{
											SubcategoryName = sub.ProductSubcategoryName,
											Description = sub.ProductSubcategoryDescription
										})
									.ToList()	// Needed to satisfy parent LINQ
												//	 strong type
				})
			.ToList();	// Needed to satisfy the return type specified by the 
						//		system method and expected by the code behind
						//		method.
}


public class ProductCategorySummaryView
{
	public string ProductCategoryName { get; set; }
	// This second property is what makes the use of ToList() required
	//		as part of the nested LINQ query.
	public List<ProductSubcategorySummaryView> Subcategories { get; set; }
}

public class ProductSubcategorySummaryView
{
	public string SubcategoryName { get; set; }
	public string Description { get; set; }
}