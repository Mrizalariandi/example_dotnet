using System;
using data_access;

namespace repositories
{
    public class CoreRepository
    {
        protected DBBakumContext Db;
        public CoreRepository(DBBakumContext _db)
        {
            this.Db = _db;
        }
    }
}
