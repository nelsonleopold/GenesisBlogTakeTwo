using GenesisBlogTakeTwo.Data;

namespace GenesisBlogTakeTwo.Services
{
    public class DisplayService
    {
        private readonly ApplicationDbContext _context;

        public DisplayService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> DisplayModeratorName(string id)
        {
            var moderator = await _context.Users.FindAsync(id);
            return moderator?.FullName!;
        }
    }
}
