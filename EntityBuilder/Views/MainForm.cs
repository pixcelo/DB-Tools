using EntityBuilder.Exceptions;
using EntityBuilder.Models;
using EntityBuilder.Views;
using System.Data.SqlClient;

namespace EntityBuilder
{
    public partial class MainForm : Form
    {
        private string templateString = string.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画面読み込み時の処理
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            var tableNames = GetTableNames();
            this.tableNameComboBox.Items
                .AddRange(tableNames.OrderBy(x => x).ToArray());

            base.OnLoad(e);
        }

        /// <summary>
        /// テンプレートファイル作成ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generateButton_Click(object sender, EventArgs e)
        {
            try
            {
                var className = this.tableNameComboBox.Text;

                if (string.IsNullOrEmpty(className))
                {
                    throw new FileGenerationNotReadyException("テーブル、ビューを選択してください。");
                }

                // テンプレートファイル読み込み　※選択できるようにすると選ぶのが面倒なので固定
                this.templateString = File.ReadAllText("..\\..\\..\\Templates\\databaseModelClass.txt");

                this.GenerateClassTemplate(
                    className,
                    hasAttribute: this.attributeCheckBox.Checked,
                    hasComment: this.commentCheckBox.Checked);
            }
            catch (FileGenerationNotReadyException ex)
            {
                this.ShowErrorMessageBox(ex.Message);
            }
            catch (Exception ex)
            {
                this.ShowErrorMessageBox(
                    $"ファイルの生成中にエラーが発生しました: {ex.Message}");
            }
        }

        /// <summary>
        /// クラス生成
        /// </summary>
        /// <param name="className"></param>
        /// <param name="hasAttribute">属性アノテーション付与</param>
        /// <param name="hasComment">コメント付与</param>
        /// <returns></returns>
        private void GenerateClassTemplate(
            string className,
            bool hasAttribute,
            bool hasComment)
        {
            // クラス名
            var contents = this.templateString.Replace("{className}", className);

            // 名前空間・using、継承
            var usingString = string.Empty;
            var namespaceString = string.Empty;
            var inheritance = " : BaseModel";
            if (className.StartsWith("T_"))
            {
                namespaceString = "PMS.Lib.Models.Database.Transaction";
            }
            if (className.StartsWith("V_"))
            {
                namespaceString = "PMS.Lib.Models.Database.Views";
                // V_ は T_を継承する場合もあるが一旦、M_を継承するものとして固定
                usingString = "\nusing PMS.Lib.Models.Database.Master;";
                inheritance = $": {className.Replace("V_", "M_")}";
            }
            if (className.StartsWith("M_"))
            {
                namespaceString = "PMS.Lib.Models.Database.Master";
            }
            contents = contents.Replace("{using}", usingString);
            contents = contents.Replace("{namespace}", namespaceString);
            contents = contents.Replace("{inheritance}", inheritance);

            // プロパティ
            var columns = this.GetTableColumns(className);
            var properties = columns.Select(x =>
            {
                var comment = hasComment ? $"        /// <summary>\n        /// \n        /// </summary>\n" : "";
                var attribute = hasAttribute ? $"        [PropertyMapping(true, \"\")]\n" : "";
                return $"{comment}{attribute}        public {x.Item2} {x.Item1} {{ get; set; }}";
            });
            contents = contents.Replace("{properties}", string.Join("\n\n", properties));


            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"{className}.cs");
            File.WriteAllText(filePath, contents);

            MessageBox.Show(
                $"ファイルが正常に生成されました: {filePath}",
                "成功",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// テンプレートファイル選択ボタンでファイルダイアログを開く        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void selectTemplateFolderButton_Click(object sender, EventArgs e)
        //{
        //    using (var fileDialog = new OpenFileDialog())
        //    {
        //        fileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        //        fileDialog.Title = "テンプレートファイルを選択してください";

        //        if (fileDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            var templateFilePath = fileDialog.FileName;
        //            templateString = File.ReadAllText(templateFilePath);

        //            MessageBox.Show(
        //                "テンプレートファイルが正常に読み込まれました。",
        //                "成功",
        //                MessageBoxButtons.OK,
        //                MessageBoxIcon.Information);
        //        }
        //    }
        //}

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(
                message,
                "エラー",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// テーブル一覧を取得する
        /// </summary>
        /// <returns></returns>
        private List<string> GetTableNames()
        {
            string connectionString = "";
            var tableNames = new List<string>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT
                        TABLE_NAME
                    FROM
                        INFORMATION_SCHEMA.TABLES
                    WHERE
                        TABLE_TYPE = 'BASE TABLE' OR TABLE_TYPE = 'VIEW'
                    ";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }

            return tableNames;
        }

        /// <summary>
        /// テーブルのカラム一覧を取得する
        /// V_ は構成が分からないのでプロパティの調整が必要       
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private List<Tuple<string, string>> GetTableColumns(string tableName)
        {
            string connectionString = "";
            var columns = new List<Tuple<string, string>>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @$"
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE
                    FROM
                        INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = '{tableName}'";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();

                        // BaseTableModelにあるカラムはスキップ
                        if (columnName == "Id"
                         || columnName == "IsDeleted"
                         || columnName == "CreatedOn"
                         || columnName == "CreatedUserId"
                         || columnName == "UpdatedOn"
                         || columnName == "UpdatedUserId")
                        {
                            continue;
                        }

                        string dataType = reader["DATA_TYPE"].ToString();
                        bool isNullable = reader["IS_NULLABLE"].ToString() == "YES";
                        columns.Add(new Tuple<string, string>(columnName, MapDataType(dataType, isNullable)));
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// DBのデータ型とC#のデータ型のマッピング
        /// </summary>
        /// <param name="sqlDataType"></param>
        /// <param name="isNullable">null許容型か</param>
        /// <returns></returns>
        private string MapDataType(string sqlDataType, bool isNullable)
        {
            var DataTypeMap = new Dictionary<string, string>
            {
                { "int", "int" },
                { "bigint", "long" },
                { "decimal", "decimal" },
                { "nvarchar", "string" },
                { "varchar", "string" },
                { "char", "string" },
                { "bit", "bool" },
                { "datetime", "DateTime" },
            };

            const string defaultDataType = "string";

            if (DataTypeMap.TryGetValue(sqlDataType, out var csharpDataType))
            {
                // .NET Framework 4.8では「string?」はコンパイルエラーになるため、string型を除外
                if (isNullable && csharpDataType != "string")
                {
                    return $"{csharpDataType}?";
                }
                return csharpDataType;
            }
            return defaultDataType;
        }

        /// <summary>
        /// テーブル名の変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTableName = tableNameComboBox.SelectedItem?.ToString();
            var columns = this.GetTableColumnInfo(selectedTableName);
            this.tableDataGridView.DataSource = columns;
        }

        /// <summary>
        /// テーブルのカラム情報を取得する
        /// </summary>
        /// <param name="tableName"></param>
        /// <see href="https://learn.microsoft.com/ja-jp/sql/relational-databases/system-information-schema-views/columns-transact-sql?view=sql-server-ver16"/>
        /// <returns></returns>
        private List<ColumnInfo> GetTableColumnInfo(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return new List<ColumnInfo>();
            }

            string connectionString = "";
            var columns = new List<ColumnInfo>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @$"
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE,
                        CHARACTER_MAXIMUM_LENGTH
                    FROM
                        INFORMATION_SCHEMA.COLUMNS
                    WHERE
                        TABLE_NAME = '{tableName}'";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        bool isNullable = reader["IS_NULLABLE"].ToString() == "YES";
                        int? maxLength = reader["CHARACTER_MAXIMUM_LENGTH"] as int?;

                        columns.Add(new ColumnInfo
                        {
                            ColumnName = columnName,
                            DataType = dataType,
                            IsNullable = isNullable,
                            MaxLength = maxLength
                        });
                    }
                }
            }
            return columns;
        }

        /// <summary>
        /// 定義を表示するボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDefinitionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tableNameComboBox.Text))
            {
                return;
            }

            // MainFormで操作したいのでモードレスダイアログで表示
            var form = new DefinitionForm(this.tableNameComboBox.Text);
            form.Show();
        }
    }
}
