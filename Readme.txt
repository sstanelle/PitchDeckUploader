This simple tool allows a user to upload their pitch deck and display it on a web page.

The accepted file formats for the pitch deck are either PDF or PowerPoint.

To run on Ubuntu, follow these steps:

1. Download the PitchDeckUploader project from GitHub:
   https://github.com/sstanelle/PitchDeckUploader.git
      OR
   GitHub CLI: gh repo clone sstanelle/PitchDeckUploader
   
2. Execute the following command:
   dotnet publish -c release -r ubuntu.20.04-x64 --self-contained

3. Open the Ubuntu machine terminal (CLI) and go to the 'publish' directory

4. Provide execute permissions:
   chmod 777 ./PitchDeckUploader

5. Execute the application
   ./PitchDeckUploader

6. Open a browser window and navigate to either http://localhost:5000 or https://localhost:5001
