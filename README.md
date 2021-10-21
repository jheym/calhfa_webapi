# Overview
## Main Goal
Given some database schema provided by the calHFA team, our API should be able to update the figures listed under "File Review Status" on this webpage: https://www.calhfa.ca.gov/homeownership/index.htm

## Other Goals
* Make it as easy for them to deploy as possible
	* Provide clear documentation for how to set it up on their end
* Add authentication 
* Make it scalable so that they may implement new API functionalities in the future
* Document how they can adapt it to accomplish other tasks with their database.

# Program Functions

### Function 1: Get the Loans in Line that are in Compliance

### Function 2: Get the loans in line that are in suspense

### Function 3: Get the post-closing MyHome/ZIP/School loans that are in line 
Need more info from client. Do we only include the MyHome/ZIP/School LoanTypes in this count? 
#### According to the database and their website:
* There are two types of loans: First (1), Subordinate(2)
* The website lists only 3 of the subordinate loans in post-closing count: myHome, ZIP, and School. Each of these loans has an ID in the database.
	* Zero Interest Program (ZIP): 11
	* ZIP extra: 12 (Do we include this?)
	* MyHome: 32
	* School Program: 40

### Function 4: Get the post-closing loans that are in suspense
Again, we need more info from the client here. Are these in-suspense loans only the ones that are MyHome/ZIP/School loans?


## Function 5: Generalize these so that they can pick which loans to get a count of.






# Resources

[Software Requirement Specification Document](https://docs.google.com/document/d/1GOlYUiFW_JA_SkW9TGgr0nLoAtgv1Vz_/edit?usp=sharing&ouid=106228635187920286644&rtpof=true&sd=true)

[Video Series - ASP.NET Core Web API with Entity Framework](https://youtube.com/playlist?list=PL4WEkbdagHIQVbiTwos0E38VghMJA06OT)

[MS Docs - Data Access in ASP.NET](https://docs.microsoft.com/en-us/aspnet/whitepapers/aspnet-data-access-content-map)

[All extension methods for querying our DB](https://docs.microsoft.com/en-us/dotnet/api/system.linq.iqueryable-1?view=net-5.0)
