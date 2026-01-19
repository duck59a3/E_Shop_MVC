using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class VoucherUsageRepository : Repository<VoucherUsage>, IVoucherUsageRepository
    {

        private readonly ApplicationDbContext _db;
        public VoucherUsageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
