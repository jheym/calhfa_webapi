This package is for CalHFA from Sac State as part of a joint collaboration between students in Professor Salem's Fall 2021 CSC130-02 class.
Our team consists of 5 members. Justin Heyman, Team Lead; David Enzler, Lead Software Developer; Isaac Williams, Developer; Johnny Velazquez, Developer; and Jamal Stanackzai. Our team name is "In Over Our Heads." 

In Over Our Heads gives CalHFA this project freely, to use however they see fit.

The package contents include:
1. Source code for the project. We have stripped the source code of any unncessary files.
2. Our first and final build Release V1.0 for CalHFA to deploy and test.
3. Documentation for project clarity and maintenance
4. IIS Setup instructions for deploying the V1.0 release.
5. This readme.txt


We assume CalHFA will be using IIS, but if that is not the case, below we have included a very general set up guide with just the essentials they need to know for setting up our API.

GENERAL HOSTING INSTRUCTIONS

1. Install latest ASP.NET Core Runtime 5.0.x (https://dotnet.microsoft.com/download/dotnet/5.0)

2. Move the application folder to a desired location to be used as a webroot for your webserver.
	- Note: The application functions as a top level directory.

3. Set the connection string in the appSettings.json file located in the application folder

4. Once hosted, if you navigate to the domain root you have set, the app should redirect you to the swagger UI.
    - Note: The endpoints will only function properly on https. You should configure all http to redirect to https.

5. To stop the app from redirecting home page to Swagger, refer to the maintenance docs.


