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

// This is the solution to the second in-class question for nested LINQ queries

// The same process may be employed as with the first question.  This one has
// 		input data that must be tested.  Remember that if you use literal values
//		during the creation of the LINQ query, you must go back and replace
//		the literal values with the parameter variables when building the 
//		program template.
void Main()
{
	// Literal value is fine to use here with this call to the code behind as
	//		it is simulating a values being extracted from a UI control.
	// Note the string in the Dump() can be used as a header.
	// Also realize that more than one call could be used in main with a variety 
	//		of input information and you will see multiple result sets.  This
	//		will become useful when we want to test both legal and illegal
	//		values to make sure that errors are being handled gracefully.
	CodeBehind_GetInvoicesWithDetails("Torres").Dump("Nested Query Q02");	
}

public List<InvoiceView> CodeBehind_GetInvoicesWithDetails(string lastName)
{
	// Note how this call passes through the needed information to the system
	//		method.  The method is also still returning the information
	//		retrieved by the system method to main for display.
	return System_GetInvoicesWithDetails(lastName);
}

public List<InvoiceView> System_GetInvoicesWithDetails(string lastName)
{
	return Invoices
	// Remember to replace any literal values used in testing just the LINQ
	//		query by itself to the parameter variables passed into the method.
	.Where(invoice => invoice.Customer.LastName == lastName)
	.Select(invoice => new InvoiceView
	{
		InvoiceNo = invoice.InvoiceID,
		InvoiceDate = invoice.DateKey.ToShortDateString(),
		Customer = invoice.Customer.FirstName + " " + invoice.Customer.LastName,
		Amount = invoice.TotalAmount,
		Details = invoice
						.InvoiceLines
						.Select(line => new InvoiceLineView
						{
							LineReference = line.InvoiceLineID,
							ProductName = line.Product.ProductName,
							// Remember that when we checked the expected results for the 
							//		following we found that some were negative.  Also, we had
							//		to account for possible null values.
							
							// A quick subtraction allowed us to line up negative quantities 
							//		as expected.
							Qty = line.SalesQuantity - line.ReturnQuantity,
							
							// We used embedded ternary operators to both account for a null, 
							//		changing the value to 0 if a null was encountered, and 
							//		then if not, checking the quantity for negativity.  If that
							//		was negative, we set the unit price negative as well.
							Price = line.UnitPrice == null ? 0 :
									(line.SalesQuantity - line.ReturnQuantity) < 0 ?
									(decimal)-line.UnitPrice 
										: (decimal)line.UnitPrice,
							
							// The same process used for the unit price was also used for 
							//		determining negativity of the discount amount.
							Discount = line.DiscountAmount == null ? 0 :
										(line.SalesQuantity - line.ReturnQuantity) < 0 ?
										(decimal)-line.DiscountAmount 
											: (decimal)line.DiscountAmount,
							
							// The ExtPrice was slightly les complicated, but overall the
							//		same process was used.  Thankfully, the values were stored
							//		or else the calculations would have been rather complex
							// 		in comparison.
							ExtPrice = (line.SalesAmount - line.ReturnAmount)	
														== null ? 0 :
										(decimal)(line.SalesAmount - line.ReturnAmount)							
						})
						.ToList()
	})
	.ToList();
}	


public class InvoiceView
{
	public int InvoiceNo { get; set; }
	public string InvoiceDate { get; set; }
	public string Customer { get; set; }
	public decimal Amount { get; set; }
	public List<InvoiceLineView> Details { get; set; }
}

public class InvoiceLineView
{
	public int LineReference { get; set; }
	public string ProductName { get; set; }
	public int Qty { get; set; }
	public decimal Price { get; set; }
	public decimal Discount { get; set; }
	public decimal ExtPrice { get; set; }
}
