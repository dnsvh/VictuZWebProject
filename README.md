# Installation Guide
### 1. Make sure all nuget packages are installed
![afbeelding](https://github.com/user-attachments/assets/8f2cac03-71c0-4869-b949-2ce524ee07d0)

### 2. update the database
First update Main database in the nuget package manager console
```
update-database -Context Victuz_Lars_Db
```
Then update the User Accounts database
```
update-database -Context VictuzAccountDbContext
```

### 3. Start the project
![afbeelding](https://github.com/user-attachments/assets/7f4a8abf-7ed3-4d98-8eec-bedd81a692ad)

### 4. (Optional) Log in to the primary admin account
```
adminaccount@zuyd.nl
AdminAccount123!
```
You can use this to edit the roles of existing accounts, up to Admin, through the 'Accountgegevens' tab.
