<Query Kind="Statements">
  <Connection>
    <ID>e68b4b85-a7a2-45dc-8ac0-c808d7c59bff</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <AllowDateOnlyTimeOnly>true</AllowDateOnlyTimeOnly>
    <Server>.</Server>
    <Database>StarTed-2024-09</Database>
    <DriverData>
      <EncryptSqlTraffic>True</EncryptSqlTraffic>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

// Question 01 Using a List for the Community choices.  
// Advantageous if the list may change over time

List<string> communities = new List<string> ()
					{"Forest Heights", "Oliver", "Westmount"};

Rentals
.Where(rental => rental.Vacancies > 0 && 
					communities.Contains(rental.Address.Community))
.Select(rental => new
{
	rental.RentalID,
	rental.MonthlyRent,
	rental.Vacancies,
	rental.Address.Community,
	Description = rental.RentalType.Description == null ?
						"U/K" : rental.RentalType.Description
})
.OrderBy(anon => anon.Community)
.ThenByDescending(anon => anon.MonthlyRent)
.Dump("Question 01 Results");

// Alternate Question 01 without the list
Rentals
.Where(rental => rental.Vacancies > 0 &&
				(rental.Address.Community.ToLower().Equals("oliver") ||
				rental.Address.Community.ToLower().Equals("westmount") ||
				rental.Address.Community.ToLower().Equals("forest heights")))
.Select(rental => new 
	{
		rental.RentalID,
		rental.MonthlyRent,
		rental.Vacancies,
		rental.Address.Community,
		Description = rental.RentalType.Description == null?
						"U/K" : rental.RentalType.Description
	})
.OrderBy(anon => anon.Community)
.ThenByDescending(anon => anon.MonthlyRent)
.Dump("Question 01 Results");

// Question 02, fairly straight-forward
ClubMembers
.Where(member => member.Role.Description.ToLower() != "member"
					&& member.Active)
.Select(member => new 
	{
		member.StudentNumber,
		Role = member.Role.Description,
		member.Student.FirstName,
		member.Student.LastName,
		member.Club.ClubName
	})
.OrderBy(anon => anon.ClubName)
.ThenBy(anon => anon.Role)
.ThenBy(anon => anon.LastName)
.Dump("Question 02 Results");


// Question 03 using a Dictionary.  Again useful if entries may change
Dictionary<decimal,string> categories = new Dictionary<decimal,string>();
categories.Add(15, "Coop");
categories.Add(6, "Indepth Courses");
categories.Add(3, "Introduction Course");

Console.Write(categories[15]);

ProgramCourses
.Where(pc => pc.Required)
.Select(pc => new
{
	pc.Program.ProgramName,
	pc.CourseID,
	pc.Course.CourseName,
	pc.Course.Credits,
	CourseCategory = categories.ContainsKey(pc.Course.Credits) ?							  
							 categories[pc.Course.Credits] :
							 "Unknown Course Type" 
})
.OrderBy(anon => anon.ProgramName)
.ThenBy(anon => anon.CourseName)
.Dump("Question 03 Results");


// Alternate Question 03
ProgramCourses
.Where(pc => pc.Required)
.Select(pc => new 
	{
		pc.Program.ProgramName,
		pc.CourseID,
		pc.Course.CourseName,
		pc.Course.Credits,
		CourseCategory = pc.Course.Credits == 15 ? 
							"Coop" : pc.Course.Credits == 6 ?
							"Indepth Courses" : pc.Course.Credits == 3 ?
							"Introduction Course" : "Unknown Course Type"
	})
.OrderBy(anon => anon.ProgramName)
.ThenBy(anon => anon.CourseName)
.Dump("Question 03 Results");

// Question 04
Employees
.Where(emp => !emp.Position.Description.ToLower().Equals("instructor"))
.Select(emp => new 
	{
		School = emp.Program.Schools.SchoolName,
		emp.EmployeeID,
		emp.FirstName,
		emp.LastName,
		PositionDescription = emp.Position.Description,
		emp.Program.ProgramName
	})
.OrderBy(anon => anon.ProgramName)
.ThenBy(anon => anon.PositionDescription)
.ThenBy(anon => anon.FirstName)
.Dump("Question 04 Results");


// Question 05 Attempt, but the final math is not quite working
ClassMembers
.OrderBy(member => member.OfferingSection.Offering
								.ProgramCourse.Course.CourseName)
.ThenBy(member => member.OfferingSection.SectionCode)
.GroupBy(member => member.OfferingSection, member => member.Mark)
.Select(group => new 
	{
		CourseName = group.Key.Offering.ProgramCourse.Course.CourseName,
		SectionCode = group.Key.SectionCode,
		Instructor = group.Key.Employee.FirstName + " " + 
						group.Key.Employee.LastName,
		TotalStudents = group.Count(),
		Average = Math.Round((decimal)group.Average(), 0),
		ReviewNeeded = group.Average() < 50 ? "Yes" : "No",
		MinMark = (ClassMembers
					.Where(member => member.OfferingSectionID
									== group.Key.ClassOfferingID)
					.Select(member => member.Mark))
				.Min(),
		MaxMark = (ClassMembers
					.Where(member => member.OfferingSectionID
									== group.Key.ClassOfferingID)
					.Select(member => member.Mark))
				.Max()
})
.Dump("Question 05 Results");



// Question 05 that is working as intended
ClassOfferings
.Where(co => co.OfferingSectionClassMembers.Count > 0)
.Select(co => new 
	{
		CourseName = co.Offering.ProgramCourse.Course.CourseName,
		SectionCode = co.SectionCode,
		Instructor = co.Employee.FirstName + " " +
						co.Employee.LastName,
		TotalStudents = co.OfferingSectionClassMembers.Count(),
		Average = (int)co.OfferingSectionClassMembers
					.Average(member => member.Mark),
		ReviewNeeded = (int)co.OfferingSectionClassMembers
					.Average(member => member.Mark) < 50 ? "Yes" : "No",
		MinMark = co.OfferingSectionClassMembers.Min(member => member.Mark),
		MaxMark = co.OfferingSectionClassMembers.Max(member => member.Mark),
		
})
.OrderBy(anon => anon.CourseName)
.ThenBy(anon => anon.SectionCode)
.Dump("Question 05 Results");



						
























