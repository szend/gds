namespace GenericDataStore.Models
{
    public class DatabaseTableRelations
    {
        public Guid DatabaseTableRelationsId { get; set; }
        public string FKName { get; set; }

        public string ParentTable { get; set; }

        public string ParentPropertyName { get; set; }

        public int ParentColumnId { get; set; }

        public string ChildTable { get; set; }

        public int ChildTableId { get; set; }

        public string ChildPropertyName { get; set; }

        public bool Virtual { get; set; }

        public Guid? ParentObjecttypeId { get; set; }
        public Guid? ChildObjecttypeId { get; set; }
    }
}
