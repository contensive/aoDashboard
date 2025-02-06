
rem echo off

rem 
rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [versionNumber]
rem versionNumber is YY.MM.DD.build-number, like 20.7.7.1
rem

c:
cd \Git\aoDashboard\scripts

rem all paths are relative to the git scripts folder
rem
rem GIT folder
rem     -- aoSample
rem			-- collection
rem				-- Sample
rem					unzipped collection files, must include one .xml file describing the collection
rem			-- server 
rem 			(all files related to server code)
rem				-- aoSample (visual studio project folder)
rem			-- ui 
rem				(all files related to the ui
rem			-- etc 
rem				(all misc files)

rem -- release or debug
set DebugRelease=Debug

rem -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=Dashboard

rem -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\Dashboard\

rem -- name of the solution. SHOULD include ao prefix
set solutionName=aoDashboard.sln

rem -- name of the solution. SHOULD include ao prefix
set binPath=..\server\bin\%DebugRelease%\

rem -- name of the solution. SHOULD include ao prefix
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\

rem -- name of the solution. SHOULD include ao prefix
set deploymentFolderRoot=C:\Deployments\aoDashboard\Dev\

rem -- folder where nuget packages are copied
set NuGetLocalPackagesFolder=C:\NuGetLocalPackages\


rem Setup deployment folder
set year=%date:~12,4%
set month=%date:~4,2%
if %month% GEQ 10 goto monthOk
set month=%date:~5,1%
:monthOk
set day=%date:~7,2%
if %day% GEQ 10 goto dayOk
set day=%date:~8,1%
:dayOk
set versionMajor=%year%
set versionMinor=%month%
set versionBuild=%day%
set versionRevision=1
rem
rem if deployment folder exists, delete it and make directory
rem
:tryagain
set versionNumber=%versionMajor%.%versionMinor%.%versionBuild%.%versionRevision%
if not exist "%deploymentFolderRoot%%versionNumber%" goto :makefolder
set /a versionRevision=%versionRevision%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%versionNumber%"

rem pause

rem ==============================================================
rem
rem clean build folders
rem
rd /S /Q "..\server\bin"
rd /S /Q "..\server\obj"

rem pause

rem ==============================================================
rem
rem clean collection folder
rem

cd %collectionPath%
del *.zip
cd ..\..\scripts

rem pause

rem ==============================================================
rem
rem create helpfiles.zip file for install in private/helpfiles/
rem 
rem make a \help folder in the addon Git folder and store the collections markup files there. 
rem a comma in the filename represents a topic on the navigation, so to make an article "Shopping" in the "Ecommerce" topic, create a document "Ecommerce,Shopping.md"
rem help files are installed in the "privateFiles\helpfiles\(collectionname)" folder. The collectionname must match the addoon collections name exactly.
rem add a resource node to the collection xml file to install the helpfile zip to the site. For example
rem    <Resource name="HelpFiles.zip" type="privatefiles" path="helpfiles/(collectionname)" />
rem then if the first install, 
rem

cd ..\help
del %collectionPath%HelpFiles.zip

rem copy default article and articles for the  Help Pages collection
"c:\program files\7-zip\7z.exe" a "%collectionPath%HelpFiles.zip" 
cd ..\scripts

rem pause


rem ==============================================================
rem
rem install ui as zip file
rem layouts are developed in a folder with a subfolder for assets, named catalogassets, etc.
rem when deployed, they are saved in the root folder so the asset subfolder is off the root, to make the html src consistent

cd ..\ui\dashboard
"c:\program files\7-zip\7z.exe" a "..\..\collections\dashboard\uiDashboard.zip" 
cd ..\..\scripts

rem pause

rem ==============================================================
rem
echo build 
rem


cd ..\server
"%msbuildLocation%msbuild.exe" %solutionName% /p:Configuration=%DebugRelease%
if errorlevel 1 (
   echo failure building
   pause
   exit /b %errorlevel%
)
cd ..\scripts

rem pause

cd ..\scripts

rem ==============================================================
rem
echo Build addon collection
rem

c:
del "%collectionPath%%collectionName%.zip" /Q
del "%collectionPath%*.dll" /Q

copy "%binPath%*.dll" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%versionNumber%" /Y
cd ..\..\scripts

rem pause

rem ==============================================================
rem
echo cleanup collection folder to protect against lost edits if anyone works on them
rem

cd %collectionPath%

del *.dll
del *.dll.config

del "*.html"
del "*.css"
del "*.js"
del "*.jpg"
del "*.png"
del "helpFiles.zip"
del "uiDashboard.zip"

cd ..\..\scripts

rem pause