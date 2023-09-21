# VideoGameGuy
A *Higher or Lower* style web game based on video game review scores. Built with ASP.NET Core and EF Core.

## Features
* Regulary updated data
* Session tracking
* Mobile friendly
* Multiple game modes (COMING SOON)
* Twitch integration (COMING SOON)

## To Run Locally
To build and run this application locally, the following are required:
* [.NET7 SDK](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks)
* [SqlServer Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* [Syncfusion License](https://www.syncfusion.com/sales/communitylicense) (Free version)
* [RAWG Api Key](https://rawg.io/apidocs)
* [IGDB Api Access](https://api-docs.igdb.com/#getting-started)

Open the VS Developer Console, perform the following:
1. dotnet user-secrets init --project VideoGameGuy
2. dotnet user-secrets set "SyncfusionProductKey" "PRODUCT-KEY-HERE"
3. dotnet user-secrets set "RawgApiKey" "API-KEY-HERE"
4. dotnet user-secrets set "IgdbClientId" "CLIENT-ID-HERE"
5. dotnet user-secrets set "IgdbClientSecret" "CLIENT-SECRET-HERE"

Open the Package Manager Console, perform the following:
1. Add-Migration "[MIGRATION_NAME]" -Context MainDbContext
2. Add-Migration "[MIGRATION_NAME]" -Context RawgDbContext
2. Add-Migration "[MIGRATION_NAME]" -Context IgdbDbContext
