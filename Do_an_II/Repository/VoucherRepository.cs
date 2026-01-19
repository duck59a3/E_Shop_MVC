using Do_an_II.Data;
using Do_an_II.Models;
using Do_an_II.Repository.IRepository;

namespace Do_an_II.Repository
{
    public class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        private readonly ApplicationDbContext _db;
        public VoucherRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
