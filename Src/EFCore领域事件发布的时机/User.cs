namespace EFCore领域事件发布的时机
{
    public class User : BaseEntity
    {


        public Guid Id { get; init; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string? NickName { get; private set; }
        public int? Age { get; private set; }
        public DateTime CreateDateTime { get; private set; }


        private User()
        {

        }

        public User(string userName, string email)
        {
            this.Id = Guid.NewGuid();
            this.UserName = userName;
            this.Email = email;
            this.CreateDateTime = DateTime.Now;
            AddDomainEvent(new NewUserNotification(userName, this.CreateDateTime));
        }
        public void ChangeEmail(string value)
        {
            this.Email = value;

        }
        public void ChangeNickName(string? value)
        {
            this.NickName = value;

        }

        public void ChangeUserName(string userName)
        {
            string oldUserName = this.UserName;
            this.UserName = userName;
            AddDomainEvent(new UserNameChangeNotification(oldUserName, this.UserName));

        }


        public void ChangeAge(int value)
        {
            this.Age = value;

        }
    }
}
