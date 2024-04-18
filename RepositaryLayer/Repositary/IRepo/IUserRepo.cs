using BuisinessLayer.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IUserRepo
    {
       public int createUser(UserEntity entity);
       public UserEntity GetUserByEmail(string email);
       public int UpdatePassword(string mailid, string password);
    }
}
