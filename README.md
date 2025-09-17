Step 1: Download and Extract
Download the ZIP file from GitHub

Extract the folder to your computer (e.g., Desktop)

Step 2: Open Command Prompt
Press Windows Key + R
Type cmd and press Enter 

Step 3: Navigate to the Project
cd Desktop\ContractMonthlyClaimSystem-main

Step 4: Run the Application
dotnet run

Step 5: Open Your Browser
Go to: https://localhost:7000 (or the URL shown in command prompt)

Step 6: Login
Email: Any email (e.g., test@email.com)
Password: Any password
Role: Choose Lecturer, Coordinator, or Manager
Click "Login"

If You Get Errors:
# First try:
dotnet restore
dotnet build
dotnet run

# If still doesn't work, install .NET SDK from:
# https://dotnet.microsoft.com/download/dotnet/8.0
