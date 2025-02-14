﻿using BCrypt.Net;
using Courses_app.Dto;
using Courses_app.Models;
using Courses_app.Repository;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;

namespace Courses_app.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<User> AddBasicUser(BasicUser user)
        {
            user.Password = EncodePassword(user.Password);
            return await _repository.AddBasicUser(user);
        }
        public async Task<User> AddAuthor(Author user)
        {
            user.Password = EncodePassword(user.Password);
            return await _repository.AddAuthor(user);
        }

        public async Task<User> GetByEmailAndPassword(string email, string password)
        {
            var user = await _repository.GetByEmail(email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password,user.Password))
            {
                return user;
            }

            return null;
        }

        public async Task<User> ChangePassword(long id, ChangePassordRequest request)
        {
            var user = await _repository.Get(id);
            if (user != null && BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password))
            {
                var updatedUser = await _repository.ChangePassword(id, EncodePassword(request.newPassword));
                return updatedUser;
            }

            return null;
        }

        public async Task<BasicUserDto> GetBasicUser(long id)
        {
            var user = await _repository.GetBasicUser(id);
            return new BasicUserDto(user);
        }



        public async Task<AuthorDetailsDto> GetAuthor(long id)
        {
            var author = await _repository.GetAuthorById(id);
            return new AuthorDetailsDto(author);
        }


        public async Task<User> UpdateUser(long userId, UpdateUserRequest request, UserRole role)
        {
            return await _repository.UpdateUser(userId, request.Name, request.Surname, request.Username, role);
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await _repository.GetAll();
            var userDtos = users.Select(u => new UserDto(u)).ToList();

            return userDtos;
        }

        public async Task<long> AddAdmin(User user)
        {
            user.Password = EncodePassword(user.Password);
            return await _repository.AddAdmin(user);
        }


        private string EncodePassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<long> BanUser(long userId)
        {
            return await _repository.BanUser(userId);
        }

        public async Task<long> UnbanUser(long userId)
        {
            return await _repository.UnbanUser(userId);
        }

        public async Task<List<UserDto>> Search(string query, SearchUserFlag flag)
        {
            var users = await _repository.Search(query, flag);
            return users.Select(u => new UserDto(u)).ToList();
        }
    }
}
