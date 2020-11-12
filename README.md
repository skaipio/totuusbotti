# Set the bot's API token
The bot's API token should be set to the environment variable `TOTUUSBOTTI_API_TOKEN`.

## Setting via Visual Studio
1. Right-click TotuusBotti-project and open Properties.
2. Open the Debug tab.
3. Add the environment variable to the `Environment variables` section.

# Running the server
Make sure the Debug-profile is selected (to load the correct environment with the API token) in the toolbar for running the program. Hit the Start-button.

## Alternatively
In Package Manager Console
1. Navigate to the directory ./TotuusBotti
2. Run `dotnet run`

This seems to be more difficult to stop though. One might have to end the process in task manager if the Stop-button isn't enough.