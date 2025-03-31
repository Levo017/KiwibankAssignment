# Library Management System
Library Management System is a console app that use in-memory dictionary to store list of books with their ISBN number as unique identifier.

# Local Setup
1. Download and install the `.NET 8` SDK. Link [here](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Clone this repository.
3. Open a command prompt in admin mode.  Navigate to the folder where repository is downloaded. Then navigate to `/LibMgmt` folder.
4. Run the following command:
   ```bash
   dotnet run LibMgmt.csproj
   ```
5. Should on the command prompt the project is built, and console app start up.

# Code File Structure
* `LibMgmt.csproj`
   * `Modles` folder 
        * `Book.cs`
   * `Repository` folder 
        * `implementations` folder
        * `interfaces` folder
        * `RepositoryErrors.cs`
        * `RepositoryResult.cs`
   * `Services` folder 
        * `implementations` folder
        * `interfaces` folder
        * `ServiceResult.cs`
   * `UI` folder 
        * `UiHelpers.cs`
    * `Program.cs`

* `LibMgmt.Tests`
    * `BookRepoTests.cs`
    * `IsbnValidatorTests.cs`
    * `LibraryManagementServiceTests.cs`

# Design Decisions and Known Limitations
* Use of generic `RepositoryResult` and `ServiceResult` class to wrap repository layer and service layer operation result
    * Wrap actual result into these generic result allow repository / service layer to provide more info when operation is unsuccessful.  Although not shown in this assignment, the upstream caller can choose to take different error-handling action depending on the error info provided.
    * For `RepositoryResult`, all possible errors are categorised in `RepositoryErrors` enum.  This is because normally there is a finite number of failure reasons for repository error, use it in an enum allow its caller, often services, to easily decide what is wrong and how to proceed gracefully.
    * For `ServiceResult`, errors are captured and presented as error code with `long` data type. Because application logic services' error can be more complex, I prefer to use `long` to uniquely identify them.  Caller services / orchestrators can then use an errorcode parser / lookup service to understand it better and decide how to proceed.
* Console UI logic flow in `UIHelpers` class ( **Known Limitation** )
    * Ideally we can have a Model class to abstract away the user interaction logic, like when to show options, when to show errors, etc.  Then we can have some UnitTest done on the Model class.  Didn't implement this way as it's a little overkill for demo purpose.