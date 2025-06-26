## Over View
There are two endpoints:
- `api: /upload`
- `api: /download/{fileID}`

## Algorithm Explanation
`/upload` endpoint handles the incoming text/csv file from the client, and then immediately after recieving the file it stored in a local directory with a unique name `{Guid}.csv` and another method is called to extract total number of sales per department, it iterates in each line in the temporarily stored sales data and extracting the department and the number of sales and store it in a `Dictionary` object, and continues iterating until reach end of line. 

To handle rows with the same department name, it first checks if there is a department name as a key in the dictionary object and then if there is, only the value(number of sales) will be updated. After extracting total number of sales per department then a csv file will be created and the extracted data will be written to that file. And finally an endpoint that let the client to download the file `api: /download/{Unique_File_Name}` will be returned.

## Estimated time Complexity
At first to extract the total number of sales per department data from the temporarily stored sales data, there is n(number of rows of the sales data) th iteration on the sales data and then after, to persistantly store the extracted data to a csv file there will be again n(number of rows of the sales data) th iteration. Generally, it has O(2n) time complexity.