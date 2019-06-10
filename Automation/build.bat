@ECHO OFF

:inputpath
SET /p mspath="Enter path to msbuild:"
IF "%mspath%"=="" (
		goto inputpath)

cd ..\Sources
SET slnPath=%cd%\TutorialTask.sln
 
cd /D %mspath%

msbuild "%slnPath%" 
set /p delExit=Press the ENTER key to exit...: