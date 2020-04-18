This is the DocFx Documentation folder.

Setup instructions:
	-Install DocFx folder (anywhere you'd like) https://github.com/dotnet/docfx/releases (or via Chocolatey/Homebrew)
	-Add the DocFx folder path to the "Path" environment variable on your system (Skip if done via Chocolatey/Homebrew)
	
Build Documentation instructions (Requires setup):
	-Ensure your solution file has been created in the Project folder
	-Ensure your solution has been built via Visual Studio
	-Run the Build.bat batch file in the Documentation folder
	* This will update the documentation to your latest changes

Instructions to View Documentation (Requires setup):
	-Run the Build and Serve.bat batch file in the Documentation folder
	-Navigate to http://localhost:8080 in your browser of choice.
	-The batch file must be kept open when browsing the local documentation
	