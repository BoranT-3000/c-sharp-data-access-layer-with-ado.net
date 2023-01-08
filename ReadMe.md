# Data Access Layer using ADO.net C#
I did this because i was in need something like this for my homework
with the help of this methods you will be able to do CRUD operations

## Methods
1. command_ExecuteReader: This is for Select statement (Read part).
2. command_ExecuteNonQuery: This method is for inserting, deleting, etc.
3. command_open_Connection: This is for opening a connection to mssql. 
4. command_close_Connection: This is for closing a connection to mssql. 
5. Is_there_a_connection: This is a stupid function just for is there a connection.
6. command_ExecuteScalar: Returns number of rows inside database.
7. command_read_sp: for reading sp from database.
8. ExecuteStoredProcedureReturnDataSet: this is for inserting or updating sp.
9. command_multiple_ExecuteNonQuery_with_transaction: silly method for looking everything is ok while sending multiple queries.


## Usefull links:
1. https://dotnettutorials.net/lesson/what-is-ado-net/ 
2. https://www.tutorialsteacher.com/mvc/