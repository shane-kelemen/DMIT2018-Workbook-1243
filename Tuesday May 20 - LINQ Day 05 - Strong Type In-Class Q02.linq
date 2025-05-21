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

// 	In-Class practice exercise #2, following the pattern introduced on Tuesday, May 20.

void Main()
{
	CodeBehind_GetProductsByCategory("Music").Dump();
}

public List<ProductColorProcessView> CodeBehind_GetProductsByCategory(string category)
{
	return System_GetProductsByCategory(category);
}


public List<ProductColorProcessView> System_GetProductsByCategory(string category)
{
	return Products
	.Where(product => product.ProductSubcategory.ProductCategory.ProductCategoryName.Contains("Music"))
	.OrderBy(product => product.StyleName)  // Again, the ordering is required before the new type is created
											//		because the ordering column will no longer exist in the
											//		new type. 
	.Select(product => new ProductColorProcessView
			{
				ProductName = product.ProductName,
				Colour = product.ColorName,
				ColourProcessNeeded = product.ColorName == "Black" || product.ColorName == "White" ?
															"No" : "Yes"
			})
	.ToList();
}


public class ProductColorProcessView
{
	public string ProductName { get; set; }
	public string Colour { get; set; }
	public string ColourProcessNeeded { get; set; }
}