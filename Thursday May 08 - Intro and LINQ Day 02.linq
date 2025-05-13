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

// WHERE IN-CLASS EXERCISES

// Create a new DateOnly object the right side of the comparison to a DateOnly object when 
//		the database field is a (DateOnly?)
Employees
.Where(employee => employee.HireDate > new DateOnly(2022, 01, 01))
.Dump();

// BUT, create a new DateTime object on the right side of the equation and use the .Value.Date
//		of the left hand side when the database field is a (DateTime?)
Products
.Where(product => product.AvailableForSaleDate.Value.Date >= new DateTime(2019, 07, 01))
.Dump();


// Note that the underscore character used in the numbers for the comparison are 
//		effectively the same as the comma when writing larger numbers on paper.
Customers
.Where(customer => customer.YearlyIncome >= 60_000 && customer.YearlyIncome <= 61_000)
.Select(customer => customer.EmailAddress)
.Dump();

// When querying collections, you may use the extension methods including aggregations
//		More on this later.
Promotions
.Where(promotion => promotion.PromotionName.Contains("North America"))
.Dump();


// ORDERING IN-CLASS EXERCISES
// Note that the following queries are all starting with the queries from the WHERE section
//		above with ordering functionality added.
// Also, remember what we talked about in class about how usually you will want to perform
//		filtering and some other data manipulations before you order the data. There will of course
//		be some exceptions, but you should a really good reason for not filtering first.

Employees
.Where(employee => employee.HireDate > new DateOnly(2022, 01, 01))
.OrderBy(employee => employee.LastName)		// Order by employee first name ascending
.Dump();

Products
.Where(product => product.AvailableForSaleDate.Value.Date >= new DateTime(2019, 07, 01))
.OrderByDescending(product => product.ProductLabel)		// Order by ProductLabel descending
.Dump();

// Note:  	For this question the document mentioned that you must do the ordering before the 
//			Select.  That is not entirely true, but we will do as instructed first.
Customers
.Where(customer => customer.YearlyIncome >= 60_000 && customer.YearlyIncome <= 61_000)
.OrderBy(customer => customer.EmailAddress)		// Order by email address ascending
.Select(customer => customer.EmailAddress)
.Dump();
// Alternately, the results could have been ordered after the Select, but you must know what 
//		it is that you are dealing with in order to complete the ordering successfully.
Customers
.Where(customer => customer.YearlyIncome >= 60_000 && customer.YearlyIncome <= 61_000)
.Select(customer => customer.EmailAddress)
.OrderBy(emailAddress => emailAddress)     // Order by email address ascending, not an item inside a complex object
.Dump();
// The explanation for why I changed the lambda in the second query is that you must look at
//		what the return type is from the Select() to know what you are dealing with.  From the 
//		Where() we receive and IQueryable<Customer>, but from the Select() we receive back an
//		IQueryable<string>, so you can see that the fundamental data type that we are ordering 
// 		has changed to an atomic type (string) that does not require you to choose a Property 
//		within the type


// This has a two layer sort, functioning similar to SQL, but nothing more exotic than that.
Promotions
.Where(promotion => promotion.PromotionName.Contains("North America"))
.OrderBy(promotion => promotion.PromotionName)
.ThenByDescending(promotion => promotion.PromotionLabel)
.Dump();













