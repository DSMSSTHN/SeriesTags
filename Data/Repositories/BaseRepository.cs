using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriesTags.Data.Repositories {
    public abstract class BaseRepository {
        protected MSSRDBContext context;

        public BaseRepository(string dbPath) {
            context = new MSSRDBContext(dbPath);
            try {
                context.Database.EnsureCreated();
            } catch (Exception) {

                throw;
            }
        }
    }
}
