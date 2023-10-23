This application consists of a dashboard, where one can save their favorite financial instruments to be followed.
The dashboard is accessible via first creating a useraccount, and then logging in. You can search among the available financial instruments and save them onto your dashboard.
Current supported functionality is Swedish stocks only as of 2023-10-23.

The backend part of this application is built with C# /.NET. It consists of a minimal API, serving a MYSQL database. A code first approach was used to design and build the database.

The frontend is built with React.

The data is retrieved from a service at RAPIDAPI found under X-RapidAPI-Host below. So you need to create an Account at RAPID API at register the service.
Please read the terms regarding API call rates and limits. This app uses a free version with limited usage.

TODO (2023-10-23)
To run this project: 

Place secret files...
Include an appSettings.Secret.json file as a member of appsettings.Json:

{
  "ConnectionStrings": {
    "FinDashDbContext": "REPLACEWITHYOURYOURCONNECTIONSTRINGHERE"
  },
  "JWTSettings": {
    "SecretKey": "REPLACEWITHATLEASTA16CHARACTERSTRING"
  },
  "YahooFinance": {
    "X-RapidAPI-Key": "REPLACEWITHTHEAPIKEYFROMTHEAPIPROVIDERATRAPIDAPI",
    "X-RapidAPI-Host": "apidojo-yahoo-finance-v1.p.rapidapi.com"
  }
}

Create a user in the database, log in to the database and change property IsAdmin to 1.
To Populate with reference data, run the following function once logged in to the admin page...TODO

The dashboard is available as a regular (non-admin) user.
