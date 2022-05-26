

namespace Transaction.Application.IContext
{
    public interface IIdentityContext
    {
        public int UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public int AccountNumber { get; }
    }
}
