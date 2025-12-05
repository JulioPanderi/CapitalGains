Introduction

	Capital Gains is a console application, to proccess json files with buy/sell operations.
	
Application Usage

	To use the application open a terminal console in the directory where the application is deployed, and run the application (CapitalGains.Console.exe) using the filename of the operation to process.

	Example:
	
	C:\CapitalGains> CapitalGains.Console.exe "DataFiles\Case1.txt"

Solution structure and architecure

The main layers are:

	* CapitalGains.Console: the main application (presentation)
	
	* CapitalGains.Domain: contains the DTO used to transfer data from the services to the presentation, and the classes necessary to format the json output
	
	* CapitalGains.Common: contains classes with utilities than are common for all layers
	
	* CapitalGains.Application: contains the services

- There is a "Test" folder with a single application, with the tests. This were writen using XUnit for assert testing, and Moq as mocking library. 

- The testing application is separated in different folders, wich corresponds to each layer application

- If there is an error, a message in screen is displayed with minimal information for the user; but also writes a log file in the log folder using Serilog, with more detailed information.

- There is a folder with the name "DataFiles" with all the cases described in the initial documentation, ready to use for testing.

- The application uses "appsettings.json" as configuration file. The file has two big sections:

	* Settings: the application general settings. It has two keys with values:
		- Tax: the percent value for the taxes
		- MinimumTaxed: the minimal value to consider for tax calculation
		
	  NOTE: if the keys with the values are missing, the application takes the default values specified in the initial documentation. 

	* Serilog: the configuration for Serilog library