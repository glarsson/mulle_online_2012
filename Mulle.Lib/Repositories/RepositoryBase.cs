using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Mulle.Lib.Repositories
{
    public class RepositoryBase<C> : IDisposable where C : DbContext, new()
    {
        private C _Context;

        public virtual C Context
        {
            get 
            {
                if (_Context == null)
                    _Context = new C();

                return _Context;
            }    
        }

        public void Dispose()
        {
            _Context.Dispose();
        }
    }
}
