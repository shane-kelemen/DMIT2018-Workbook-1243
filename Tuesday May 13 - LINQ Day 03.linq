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

// In these practice exercises, we continue with filtering and ordering,
//		but also add the introduction of anonymous types.  The biggest thing
// 		to remember is that after the anonymous type has been created, you
// 		no longer have access to the collection that you started with for 
//		that LINQ query.

// Get all the ProductLabel, ProductName, and UnitPrice from the Products 
//		table in the Contoso database, where the UnitPrice is less than $10.  
//		When you specify only the column name, those names become the result headers.  
//		Nothing else new in this query.
Products
.Where(product => product.UnitPrice < 10)
.Select(product => new 
				{
					product.ProductLabel,
					product.ProductName,
					product.UnitPrice
				})
.OrderBy(anon => anon.UnitPrice)	// Note "anon" in the lambda statement... new collection of anonymous type
.ThenBy(anon => anon.ProductName)
.Dump();


// Find all the Customers in the Contoso database that are located in British Columbia, Canada.
//		Return the Customer name formatted as {LastName, FirstName} and the City the customer lives
//		in.
Customers
					// Note the use of the Navigation Properties in the following filter.  Also, use of
					// 		the Equals method is often considered best practice for equality comparisons.
.Where(customer => customer.Geography.StateProvinceName.Equals("British Columbia")
					&& customer.Geography.RegionCountryName.Equals("Canada"))
.Select(customer => new 
					{
						// When a name is given and assigned to, the alias become the new 
						//		column name in the anonymous type.
						Name = customer.LastName + ", " + customer.FirstName,
						//First = customer.FirstName,		// Choose more atomic data if processing may be
						//Last = customer.LastName,			// 		needed in the client application.
						City = customer.Geography.CityName//,   
						//customer.Geography.StateProvinceName,	// Make sure to remove fields used for testing
						//customer.Geography.RegionCountryName	//		query results.
					})
.OrderBy(anon => anon.City)
.ThenBy(anon => anon.Name)
.Dump();


// In this query, we retrieve all of the Product categorization information for Pink products that are
//		Recording Pens or Bluetooth Headphones. The question also asked to get the products from the 
//		Audio product category.  Though this last condition could be assumed to be covered by the 
//		product subcategory filter, it does not hurt to make sure, just in case more thna one category
//		has the same subcategory.
Products
					// Note the use of ToLower() to create case-insensitive comparisons.  These
					// 		can be useful to account for possuble data entry errors.  However, in some 
					// 		cases, the extra precaution will not be necessary if the database has
					//		been set up to ignore case and/or kana during text searches.
					// Also note the brackets around the two sides of the OR (||) comparison.  
					//		Remember that AND (&&) comparisons occur first.  Remove the brackets to 
					//		see the difference in the results.
.Where(product => product.ColorName.ToLower().Contains("pink")
				&& (product.ProductSubcategory.ProductSubcategoryName.ToLower() == "recording pen"
				|| product.ProductSubcategory.ProductSubcategoryName.ToLower().Equals("bluetooth headphones"))
				&& product.ProductSubcategory.ProductCategory.ProductCategoryName.ToLower() == "Audio")
.Select(product => new 
				{
					CategoryName = product.ProductSubcategory.ProductCategory.ProductCategoryName,
					SubcategoryName = product.ProductSubcategory.ProductSubcategoryName,
					ProductName = product.ProductName
				})
.Dump();


// Finally, not too much special in this query to find invoices where the customer is in Europe, 
//		except to note that you may use Navigation Properties to go through more than one table.
Invoices
.Where(invoice => invoice.Customer.Geography.ContinentName.ToLower().Equals("Europe"))
.Select(invoice => new 
				{
					InvoiceNo = invoice.InvoiceID,
					InvoiceDate = invoice.DateKey.ToShortDateString(),	// Note that methods exist for date formatting
					CustomerName = invoice.Customer.FirstName + " " + invoice.Customer.LastName,
					City = invoice.Customer.Geography.CityName,
					Country = invoice.Customer.Geography.RegionCountryName//,
					//Continent = invoice.Customer.Geography.ContinentName
				})
.Dump();
























