<Query Kind="Statements">
  <Connection>
    <ID>3959f334-78de-495c-9cda-38251e7a1f69</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>ChinookSept2018</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

// LINQPad is very similar to the Query Window in SSMS as far as operation.
// You may execute all of the queries at once by making sure nothing is highlighted 
//		and then pressing the Green Play button.  The results will be displayed as 
//		one result set after another in them Results Window. 
// You may execute just a selection of code by highlighting the part you want to 
//		execute and hitting the Green Play button
// You may toggle the Results Window to be visible or hidden by pressing Ctrl-R

// There are three main modes that we may use: Expression, Statement, Program
//		Expression: You may just type a part of a query and execute, no semi-colon
//					or Dump() statement needed
// 		Statement:	This requires that C# syntax by more respected.  You will need 
//					to end statements with a semi-colon.  If a Dump() is not used
//					at the end of the query, no results will be displayed.  Consider
//					Dump() to be sort of like Console.Write().  The operation will
//					still occur but you will not see anything unless displayed
//					explicitly.
//		Program:	This mode comes complete with a Main() method.  You may write other
//					methods and call them, create classes, etc.  We will use this mode
//					in the next unit to create and test code that would eventually be 
//					meant for inclusion in a Blazor application. 

// The following is the equivalent of the SQL query:
//		select 	*
//		from 	Albums
//		where	ArtistId % 2 = 1
// Essectially, retrieve all of the Albums where the ArtistId of the album is an odd number
Albums.Where(album => album.ArtistId % 2 == 1);

// The following is quite a bit more complex
Customers.Select(customer => new 
			{ 
				customer.CustomerId,
				customer.Company,
				customer.SupportRep.FirstName,
				customer.SupportRep.LastName,
				customer.SupportRep.BirthDate
			})
			.Where(anon => anon.FirstName == "Margaret" || anon.FirstName == "Jane")
			.OrderByDescending(anon => anon.FirstName)
			.ThenBy(anon => anon.Company)
			.Dump();
// This retrieves the indicated columns in the braces { }, so CustomerId and Company
//		from the customers table, and the FirstName, LastName, and BirthDate of the 
//		associated support rep.  Once that has been returned, the results are filtered
//		to only include reps with a first name of Margaret or Jane, and then the results
// 		of the filtered set are ordered as indicated.
// The equivalent SQL to the above is:
//		select 	CustomerId,
//				Company,
//				FirstName,
//				LastName,
//				BirthDate
//		from		Customers	as C	
//			join	Employees	as E
//			on		C.SupportRepId = E.EmployeeId
//		where	FirstName in ("Margaret", "Jane")
//		order by FirstName desc, Company
			

//	This query employs a bit of grouping of data from a collection
Albums.Select(album => new 
			{
				Artist = album.Artist.Name,
				NumTracks = album.Tracks.Count()
			})
			.Where(anon => anon.NumTracks > 20)
			.OrderByDescending(anon => anon.NumTracks)
			.Dump();
// This retrieves the two columns indicated in the braces { }.  Note that one of the 
//		columns is an aggregate not just a single piece of information.  Also note that
//		a name followed by an assignment operator begins each line.  This name will be
// 		the Property in the new collection being created.
// The equivalent SQL to the above is:
//		select 	ar.[Name],
//				count(trackID)  as NumTracks
//		from        Albums  as a
//			join    Tracks  as t
//			on      a.AlbumId = t.AlbumId
//			join Artists as ar
//			on      a.ArtistId = ar.ArtistId
//		group by    a.albumID, ar.[Name]
//		having      count(trackID) > 20
//		order by	2 desc









