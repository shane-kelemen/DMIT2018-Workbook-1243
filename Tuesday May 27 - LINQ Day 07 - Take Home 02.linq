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

void Main()
{
	CodeBehind_GetInventoryOnHandByCity("Seattle", 200)
										.Dump("Seattle @ 200");
	CodeBehind_GetInventoryOnHandByCity("Edmonton", 100)
										. Dump("Edmonton @ 100");
}

public List<StoreSummaryView> CodeBehind_GetInventoryOnHandByCity
								(string cityName, int threshold)
{	
	return System_GetInventoryOnHandByCity(cityName, threshold);
}

public List<StoreSummaryView> System_GetInventoryOnHandByCity
								(string cityName, int threshold)
{
	return Stores
		.Where(store => store.Geography.CityName == cityName)
		.OrderBy(store => store.StoreName)
		.Select(store => new StoreSummaryView
		{
			StoreName = store.StoreName,
			City = store.Geography.CityName,
			Inventory = store
						.Inventories
						.Where(inventory => inventory.OnHandQuantity >= threshold)
						.Select(inventory => new InventoryView
						{
							Name = inventory.Product.ProductName,
							Price = inventory.Product.UnitPrice == null ?
											0 : (decimal)inventory.Product.UnitPrice,
							Cost = inventory.Product.UnitCost == null ?
											0 : (decimal)inventory.Product.UnitCost,
							Margin = inventory.Product.UnitPrice
													- inventory.Product.UnitCost == null ?
											0 : (decimal)(inventory.Product.UnitPrice
													- inventory.Product.UnitCost),
							OnHand = inventory.OnHandQuantity
						})
						.OrderBy(anon => anon.Name)
						.ToList()
		})
		.ToList();
}

public class StoreSummaryView
{
	public string StoreName { get; set; }
	public string City { get; set; }
	public List<InventoryView> Inventory { get; set; }
}

public class InventoryView
{
	public string Name { get; set; }
	public decimal Price { get; set; }
	public decimal Cost { get; set; }
	public decimal Margin { get; set; }
	public int OnHand { get; set; }
}