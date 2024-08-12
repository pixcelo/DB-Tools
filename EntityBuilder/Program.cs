while (true)
{
    Console.WriteLine("モデルクラスを生成するテーブル名を入力してください (終了するには 'exit' と入力): ");
    var tableName = Console.ReadLine();

    if (tableName?.ToLower() == "exit")
    {
        break;
    }

    //var templateFolder = Path.Combine(Directory.GetCurrentDirectory());
    //var relativePath = Path.GetRelativePath(templateFolder, @"C:\Users\ModelTemplate.tt");

    var templatePath = "..\\..\\..\\Templates\\ModelTemplate.tt";

    if (File.Exists(templatePath))
    {
        // T4テンプレートファイルの内容を読み込む
        string templateContent = File.ReadAllText(templatePath);

        // テーブル名をテンプレート内のプレースホルダに置換
        templateContent = templateContent.Replace("YourTableName", tableName);

        // テンプレートファイルを書き換え
        File.WriteAllText(templatePath, templateContent);

        // T4テンプレートの実行（Visual Studio上での自動実行が一般的）
        var outputPath = "..\\..\\..\\Models\\" + tableName + ".cs";
        Console.WriteLine($"モデルクラスの生成が完了しました。{Path.GetFullPath(outputPath)} に保存されました。");
        Console.WriteLine();
        Console.WriteLine();
    }
    else
    {
        Console.WriteLine("T4テンプレートファイルが見つかりません。");
    }
}