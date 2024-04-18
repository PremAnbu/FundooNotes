using BuisinessLayer.CustomException;
using BuisinessLayer.Entity;
using BuisinessLayer.MailSender;
using BuisinessLayer.service.Iservice;
using Microsoft.AspNetCore.Mvc;
using RepositaryLayer.DTO.RequestDto;
using RepositaryLayer.Repositary.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisinessLayer.service.serviceImpl
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepo UserRepo;
        private static string otp;
        private static string mailid;
        private static UserEntity entity;
        public UserServiceImpl(IUserRepo UserRepo)
        {
            this.UserRepo = UserRepo;
        }
        private UserEntity MapToEntity(UserRequest request)
        {
            return new UserEntity {UserFirstName=request.FirstName,
                                   UserLastName=request.LastName,
                                   UserEmail=request.Email,
                                   UserPassword=Encrypt(request.Password)
            };
        }
        private String Encrypt(String password)
        {
           
            byte[] passByte = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(passByte);

        }
        private String Decrypt(String encryptedPass)
        {          
            byte[] passbyte=Convert.FromBase64String(encryptedPass);
            string s= Encoding.UTF8.GetString(passbyte);  
            return s;
        }
        private UserResponce MapToResponce(UserEntity responce)
        {
            return new UserResponce
            {
                UserId=responce.UserId,
               FirstName= responce.UserFirstName,
               LastName= responce.UserLastName ,
               Email = responce.UserEmail,
               
            };
        }

        public int createUser(UserRequest request)
        {
          return UserRepo.createUser(MapToEntity(request));
        }

        public UserResponce Login(string Email, string password)
        {
            UserEntity entity ;
            try
            {
                 entity = UserRepo.GetUserByEmail(Email);
            }
            catch(AggregateException e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId");
            }
            if(password.Equals(Decrypt(entity.UserPassword)))
            {
                return MapToResponce(entity);
            }
            else
            {
                throw new PasswordMissmatchException("Incorrect Password");
            }

        }
        public String ChangePasswordRequest(string Email)
        {
            try
            {
                 entity = UserRepo.GetUserByEmail(Email);
            }
            catch (Exception e)
            {
                throw new UserNotFoundException("UserNotFoundByEmailId" + e.Message);
            }

            string generatedotp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                generatedotp += r.Next(0, 10);
            }
            otp = generatedotp;
            mailid = Email;
            Console.WriteLine(otp+" ,"+Email);
            MailSenderClass.sendMail(Email, generatedotp);
            //Console.WriteLine(otp);
           return "MailSended Successfully ✔️";
            
        }
        public string ChangePassword(string otp,string password)
        {
            if (otp.Equals(null))
            {
                return "Generate Otp First";
            }
            if (Decrypt(entity.UserPassword).Equals(password))
            {
                throw new PasswordMissmatchException("Dont give the existing password");
            }
           
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[a-zA-Z\d!@#$%^&*]{8,16}$"))
            {
                if (UserServiceImpl.otp.Equals(otp))
                {
                   if( UserRepo.UpdatePassword(mailid,Encrypt(password))==1)
                    {
                        entity = null;otp = null;mailid = null;
                        return "password changed successfully";
                    }
                }
                else
                {
                    return "otp miss matching";
                }
            }
            else
            {
                return "regex is mismatching";
            }
            return "password not changed";
            
        }
    }
}
