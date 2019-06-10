@ECHO OFF
	
sqlcmd -E -S . -i ../Sources/TrackEverything.Database/create_database.sql 
sqlcmd -E -S . -i ../Sources/TrackEverything.Database/create_projects.sql
sqlcmd -E -S . -i ../Sources/TrackEverything.Database/create_task_worker.sql 
sqlcmd -E -S . -i ../Sources/TrackEverything.Database/create_tasks.sql 
sqlcmd -E -S . -i ../Sources/TrackEverything.Database/create_workers.sql 

set /p delExit=Press the ENTER key to exit...: