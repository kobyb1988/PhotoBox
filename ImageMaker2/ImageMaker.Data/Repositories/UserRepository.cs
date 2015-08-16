using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ImageMaker.DataContext.Contexts;
using ImageMaker.Entities;

namespace ImageMaker.Data.Repositories
{
    public interface IUserRepository
    {
        void UpdateUser(User user);

        User GetUser(Expression<Func<User, bool>> selector);

        void Commit();

        User GetAdmin();
    }

    public class UserRepository : RepositoryBase<ImageContext>, IUserRepository
    {
        public UserRepository(ImageContext context) : base(context)
        {
        }

        public void UpdateUser(User user)
        {
            User dbUser = QueryAll<User>().FirstOrDefault(x => x.Id == user.Id);
            if (dbUser == null)
                throw new ObjectNotFoundException();

            dbUser.AppSettings = user.AppSettings;
            dbUser.CameraSettings = user.CameraSettings;
        }

        public User GetUser(Expression<Func<User, bool>> selector)
        {
            return GetSingle(selector);
        }

        public User GetAdmin()
        {
            //todo roles
            return GetUser(x => x.Id == 1);
        }
    }
}
