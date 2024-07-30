using ConsoleDB.Classes;

while (true)
{
    Console.WriteLine("Enter a number: ");
    string input = Console.ReadLine();
    if (input == "quit")
    {
        break;
    }
    int number = Convert.ToInt32(input);
    Console.WriteLine($"You entered {number}.");

    switch (number)
    {
        case 1:
            // インサート
            string query = "INSERT INTO Users (Name, Age) VALUES ('John', 30)";
            DbExcuter.Execute(query);
            break;
        case 2:
            // CSVを一括インサート
            DbExcuter.BulkInsert();
            Console.WriteLine("Done");
            break;
        case 3:
            
            break;
        case 4:
            // 1000件ずつインサート
            DbExcuter.StepInsert();
            Console.WriteLine("Done");
            break;
        case 9:
            // CSVを作成
            await CsvMaker.CreateCsvAsync(1000000);
            Console.WriteLine($"Done, {nameof(CsvMaker.CreateCsvAsync)}");
            break;

        default:
            Console.WriteLine("Another number");
            break;
    }
}