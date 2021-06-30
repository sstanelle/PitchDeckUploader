This simple tool allows a user to upload their pitch deck and display it on a web page.

The accepted file formats for the pitch deck are either PDF or PowerPoint.

To run on Ubuntu, follow these steps:

1. Download the PitchDeckUploader project from GitHub:
   https://github.com/sstanelle/PitchDeckUploader.git
      OR
   GitHub CLI: gh repo clone sstanelle/PitchDeckUploader
   
2. Open the Ubuntu machine terminal (CLI)

3. Go to the directory:
   cd PitchDeckUploader/PitchDeckUploader
   
4. Execute the following command:
   dotnet publish -c release -r ubuntu.20.04-x64 --self-contained

3. Go to the directory:
   cd bin/release/net5.0/ubuntu.20.04-x64/publish

4. Provide execute permissions:
   chmod 777 ./PitchDeckUploader

5. Execute the application
   ./PitchDeckUploader

6. Open a browser window and navigate to either http://localhost:5000 or https://localhost:5001

7. If you encounter the following error while uploading a pitch deck, you need to install the libgdiplus package.

   "The type initializer for 'Gdip' threw an exception"
   
   To install libgdiplus on Ubuntu server, execute the following commands:
   sudo apt-get update
   sudo apt-get install libgdiplus
