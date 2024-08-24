using System.Data.SqlClient;

namespace EntityBuilder.Views
{
    public partial class DefinitionForm : Form
    {
        private readonly string tableName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DefinitionForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tableName"></param>
        public DefinitionForm(string tableName) : this()
        {            
            this.tableName = tableName;

            this.ShowDefinition(this.tableName);
        }

        /// <summary>
        /// 定義を表示する
        /// </summary>
        private void ShowDefinition(string tableName)
        {
            var sql = GetDefinitionScript(tableName);
            this.definitionTextBox.Text = sql;
        }

        /// <summary>
        /// 新規作成スクリプトを取得する
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        private string GetDefinitionScript(string objectName)
        {
            if (objectName.StartsWith("V_"))
            {
                return this.GetViewDefinition(objectName);
            }
            
            return this.GetTableDefinition(objectName);            
        }

        /// <summary>
        /// ビューの定義を取得する
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        private string GetViewDefinition(string viewName)
        {
            var script = string.Empty;
            string connectionString = "";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        OBJECT_SCHEMA_NAME(v.object_id) AS schema_name,
                        v.name AS view_name,
                        m.definition AS view_definition
                    FROM 
                        sys.views v
                    INNER JOIN 
                        sys.sql_modules m ON v.object_id = m.object_id
                    WHERE 
                        v.name = @ObjectName
                        AND OBJECT_SCHEMA_NAME(v.object_id) = 'dbo';";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ObjectName", viewName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            script = reader["view_definition"].ToString();
                        }
                    }
                }
            }

            return script;
        }

        /// <summary>
        /// テーブルの定義を取得する
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetTableDefinition(string tableName)
        {
            var script = string.Empty;
            string connectionString = "";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        'CREATE TABLE ' + TABLE_SCHEMA + '.' + TABLE_NAME + ' (' + 
                        STUFF((
                            SELECT 
                                ', ' + COLUMN_NAME + ' ' + DATA_TYPE + 
                                CASE 
                                    WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                                    ELSE ''
                                END + 
                                CASE 
                                    WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL'
                                    ELSE ' NULL'
                                END
                            FROM 
                                INFORMATION_SCHEMA.COLUMNS
                            WHERE 
                                TABLE_NAME = @ObjectName
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') + 
                        ')' AS table_definition
                    FROM 
                        INFORMATION_SCHEMA.COLUMNS
                    WHERE 
                        TABLE_NAME = @ObjectName
                    GROUP BY 
                        TABLE_SCHEMA, TABLE_NAME;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ObjectName", tableName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            script = reader["table_definition"].ToString();
                        }
                    }
                }
            }

            return script;
        }

    }
}
