## How to start using Tutorial Task:

### 1. Requirements

* .NET Core SDK 2.1
* MS SQL Server 2014 (at least 12.0).
* MSBuild 15.0

### 2. How to run project:

* Open Automation folder.

* Execute "build.bat" to build project.

  * At first, you will have to input full path to the folder with installed msbuild.exe.

  * Your path should be like:

    ```
    C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\bin
    ```

* Execute create_database.bat  to generate database.

  * This file require to input name of the MS SQL Server you host.

* Execute start.bin to run server. 

### 3. Additional features:

* You can execute "drop_database.bat" to delete database from the project. 
  After that, you should execute "create_database.bat" to generate new database.

* You can choose between ADO.NET and Entity Framework in WebConfig file.
  Just set ADO.NET property to false to use Entity Framework.