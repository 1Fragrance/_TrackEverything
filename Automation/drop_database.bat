@ECHO OFF


sqlcmd -E -S . -i ../Sources/TrackEverything.Database/drop_database.sql 

set /p delExit=Press the ENTER key to exit...: