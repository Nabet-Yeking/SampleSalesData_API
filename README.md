## Architecture - Over View
There are two endpoints:
`api: /upload`
`api: /download/{fileID}`

One is to handle the incoming text/csv file from the client, and then immediately after recieving the file
it will be stored in a local directory with a unique name `{Guid}.csv` and the another method is called to 
extract total number of sales per department, it iterates in each line in the temporarily stored sales data 
and extracting the department and the number of sales and insert in a Dictionary object, and continues iterating 
until reach end of line. 

To handle rows with the same department name, it first checks if there is a department name as a key in the dictionary
object and then if it is, only the value(number of sales) be incremented. After extracting total number of sales per department
then a csv file wiil be created and the the extracted data will be written to the file. And theb finally it returns an endpoint
that let the client to download the file `api: /download/{Unique_File_Name}`.
