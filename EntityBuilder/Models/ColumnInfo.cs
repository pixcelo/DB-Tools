namespace EntityBuilder.Models
{
    /// <summary>
    /// テーブルのカラム情報
    /// </summary>
    internal sealed class ColumnInfo
    {
        public string? ColumnName { get; set; }
        public string? DataType { get; set; }
        public bool IsNullable { get; set; }
        public int? MaxLength { get; set; }
    }
}