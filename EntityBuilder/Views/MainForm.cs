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
        /// �R���X�g���N�^
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ��ʓǂݍ��ݎ��̏���
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
        /// �e���v���[�g�t�@�C���쐬�{�^��
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
                    throw new FileGenerationNotReadyException("�e�[�u���A�r���[��I�����Ă��������B");
                }

                // �e���v���[�g�t�@�C���ǂݍ��݁@���I���ł���悤�ɂ���ƑI�Ԃ̂��ʓ|�Ȃ̂ŌŒ�
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
                    $"�t�@�C���̐������ɃG���[���������܂���: {ex.Message}");
            }
        }

        /// <summary>
        /// �N���X����
        /// </summary>
        /// <param name="className"></param>
        /// <param name="hasAttribute">�����A�m�e�[�V�����t�^</param>
        /// <param name="hasComment">�R�����g�t�^</param>
        /// <returns></returns>
        private void GenerateClassTemplate(
            string className,
            bool hasAttribute,
            bool hasComment)
        {
            // �N���X��
            var contents = this.templateString.Replace("{className}", className);

            // ���O��ԁEusing�A�p��
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
                // V_ �� T_���p������ꍇ�����邪��U�AM_���p��������̂Ƃ��ČŒ�
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

            // �v���p�e�B
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
                $"�t�@�C��������ɐ�������܂���: {filePath}",
                "����",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// �e���v���[�g�t�@�C���I���{�^���Ńt�@�C���_�C�A���O���J��        
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void selectTemplateFolderButton_Click(object sender, EventArgs e)
        //{
        //    using (var fileDialog = new OpenFileDialog())
        //    {
        //        fileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        //        fileDialog.Title = "�e���v���[�g�t�@�C����I�����Ă�������";

        //        if (fileDialog.ShowDialog() == DialogResult.OK)
        //        {
        //            var templateFilePath = fileDialog.FileName;
        //            templateString = File.ReadAllText(templateFilePath);

        //            MessageBox.Show(
        //                "�e���v���[�g�t�@�C��������ɓǂݍ��܂�܂����B",
        //                "����",
        //                MessageBoxButtons.OK,
        //                MessageBoxIcon.Information);
        //        }
        //    }
        //}

        private void ShowErrorMessageBox(string message)
        {
            MessageBox.Show(
                message,
                "�G���[",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// �e�[�u���ꗗ���擾����
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
        /// �e�[�u���̃J�����ꗗ���擾����
        /// V_ �͍\����������Ȃ��̂Ńv���p�e�B�̒������K�v       
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

                        // BaseTableModel�ɂ���J�����̓X�L�b�v
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
        /// DB�̃f�[�^�^��C#�̃f�[�^�^�̃}�b�s���O
        /// </summary>
        /// <param name="sqlDataType"></param>
        /// <param name="isNullable">null���e�^��</param>
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
                // .NET Framework 4.8�ł́ustring?�v�̓R���p�C���G���[�ɂȂ邽�߁Astring�^�����O
                if (isNullable && csharpDataType != "string")
                {
                    return $"{csharpDataType}?";
                }
                return csharpDataType;
            }
            return defaultDataType;
        }

        /// <summary>
        /// �e�[�u�����̕ύX��
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
        /// �e�[�u���̃J���������擾����
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
        /// ��`��\������{�^��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDefinitionButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tableNameComboBox.Text))
            {
                return;
            }

            // MainForm�ő��삵�����̂Ń��[�h���X�_�C�A���O�ŕ\��
            var form = new DefinitionForm(this.tableNameComboBox.Text);
            form.Show();
        }
    }
}
